using FirstMVCWebApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace FirstMVCWebApp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardController"/> class.
        /// </summary>
        /// <param name="db">The application database context.</param>
        public DashboardController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            // Retrieve all products from the database asynchronously.
            var products = await _db.Products.ToListAsync();

            // Render the dashboard view with the populated product list.
            return View(products);
        }
    }
}