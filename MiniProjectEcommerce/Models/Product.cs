using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniProjectEcommerce.Models
{

    public class Category
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "category Name")]
        public string categoryName { get; set; }


    }
    public class Product
    {
        public int Id { get; set; }

        public string productDescription { get; set; }

        public string ProductImageUrl { get; set; }
        [NotMapped]
        public IFormFile ProductImageFile { get; set; }
        public string ProductName { get; set; }
        [ForeignKey("category")]

        public int CategoryId { get; set; }

        public Category category { get; set; }

        public int ProductPrice { get; set; }

        public bool Availability { get; set; }

    }
    public class Inventory
    {
        public int Id { get; set; }
        

        [ForeignKey("product")]
        public int ProductId { get; set; }

        public Product product{ get; set; }

        public int quantity { get; set; }

        public int reOrderLevel { get; set; }



    }

    public class Cart
    {
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }
        public float totalcartPrice { get; set; }
        public int productQuantity { get; set; }
        [ForeignKey("product")]

        public int ProductId { get; set; }

        public Product product { get; set; }

        

        public string userName { get; set; }




    }

    public class Sale
    {

        public int Id { get; set; }

        public int totalSales { get; set; }
        public bool Flag { get; set; }

        public DateTime Timestamp { get; set; }

        public string userName { get; set; }

    }

    public class ProductSold
    {
        public int Id { get; set; }

        [ForeignKey("sale")]
        public int SaleId { get; set; }

        public Sale sale { get; set; }

        [ForeignKey("product")]

        public int Productid { get; set; }

        public Product product { get; set; }

        public int Quantity { get; set; }

        public int price { get; set; }

        public string userName { get; set; }

    }


    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {

        }

        public DbSet<Product> products { get; set; }

        public DbSet<Category> categories  { get; set; }

        public DbSet<Inventory> inventories  { get; set; }

        public DbSet<Cart> carts { get; set; }


        public DbSet<Sale> sales { get; set; }

        public DbSet<ProductSold> productsSold { get; set; }

    }

}
