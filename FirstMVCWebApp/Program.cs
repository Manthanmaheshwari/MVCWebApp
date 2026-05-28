using FirstMVCWebApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Data.Common;

var builder = WebApplication.CreateBuilder(args);
var testConn = builder.Configuration.GetConnectionString("DefaultConn");
Console.WriteLine($"--- TEST --- Connection is: {testConn}");

if (string.IsNullOrEmpty(testConn))
{
    Console.WriteLine("--- CRITICAL --- The file is NOT being read!");
}

// Default connection string configuration.
var hardcodedConn = "Server=.;Database=FirstMVC;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(hardcodedConn)
           .ConfigureWarnings(w => w.Log(RelationalEventId.PendingModelChangesWarning)));

// Add MVC controllers with views support.
builder.Services.AddControllersWithViews();

// Configure session state management services.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

// Configure cookie-based authentication options.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

var app = builder.Build();

// Run database migrations and seed default data on application startup.
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        logger.LogInformation("Migration startup: Initializing database migration pipeline.");

        // Validate database connectivity before applying migrations.
        var canConnect = db.Database.CanConnect();
        logger.LogInformation("Migration startup: CanConnect = {CanConnect}", canConnect);

        if (canConnect)
        {
            // Verify existing tables to reconcile EF migrations and prevent duplication errors.
            try
            {
                var migrationId = "20260523051502_dbinit";
                var productVersion = "10.0.8";

                using (var conn = db.Database.GetDbConnection())
                {
                    // Ensure the connection string is correctly assigned to the underlying DbConnection.
                    if (string.IsNullOrEmpty(conn.ConnectionString)) { conn.ConnectionString = hardcodedConn; }

                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Users'";
                        var usersExists = Convert.ToInt32(cmd.ExecuteScalar() ?? 0) > 0;

                        bool migrationRecorded = false;
                        try
                        {
                            cmd.CommandText = "SELECT COUNT(*) FROM dbo.__EFMigrationsHistory WHERE MigrationId = @mid";
                            var p = cmd.CreateParameter();
                            p.ParameterName = "@mid";
                            p.Value = migrationId;
                            cmd.Parameters.Add(p);
                            migrationRecorded = Convert.ToInt32(cmd.ExecuteScalar() ?? 0) > 0;
                        }
                        catch { /* Handle scenarios where the migration history table is not yet created */ }

                        if (usersExists && !migrationRecorded)
                        {
                            cmd.CommandText = "INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion) VALUES (@mid, @pv)";
                            cmd.Parameters.Clear();
                            var p1 = cmd.CreateParameter(); p1.ParameterName = "@mid"; p1.Value = migrationId; cmd.Parameters.Add(p1);
                            var p2 = cmd.CreateParameter(); p2.ParameterName = "@pv"; p2.Value = productVersion; cmd.Parameters.Add(p2);
                            cmd.ExecuteNonQuery();
                            logger.LogInformation("Migration startup: Manually marked migration as applied.");
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex) { logger.LogWarning("Pre-migration check skipped: {msg}", ex.Message); }

            // Set connection string dynamically and apply pending migrations.
            db.Database.SetConnectionString(hardcodedConn);
            db.Database.Migrate();
            logger.LogInformation("Migration startup: Database schema is up to date.");
        }

        // Seed catalog with default products if none exist.
        if (!db.Products.Any())
        {
            db.Products.AddRange(new[] {
                new FirstMVCWebApp.Models.Product { Name = "Sample Coffee", Description = "Brute Force Success!", Price = 4.99m },
                new FirstMVCWebApp.Models.Product { Name = "Sample Cake", Description = "Database Connected", Price = 2.50m }
            });
            db.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "CRITICAL: Database migration failed!");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();