using FirstMVCWebApp.Data;
using FirstMVCWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirstMVCWebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductController"/> class.
        /// </summary>
        /// <param name="db">The application database context.</param>
        public ProductController(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// GET: Displays the list of all products.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var products = await _db.Products.ToListAsync();
            return View(products);
        }

        /// <summary>
        /// GET: Displays the form to create a new product.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST: Receives form data, validates, and saves a new product to the database.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _db.Products.Add(product);
                await _db.SaveChangesAsync();

                // Redirect to the product listing page upon successful creation.
                return RedirectToAction(nameof(Index));
            }

            // Return the form view with validation errors if the submitted model is invalid.
            return View(product);
        }
    }
}