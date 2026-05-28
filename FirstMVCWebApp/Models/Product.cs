using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstMVCWebApp.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [NotMapped]
        public int Id
        {
            get => ProductId;
            set => ProductId = value;
        }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the price of the product.
        /// </summary>
        public decimal Price { get; set; } = 0.0m;

        public int Stock { get; set; } = 0;
    }
}
