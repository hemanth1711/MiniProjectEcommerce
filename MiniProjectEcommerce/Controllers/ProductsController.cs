using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniProjectEcommerce.Models;
using System.Dynamic;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace MiniProjectEcommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        
        public ProductContext _pdContext;
        public IHostingEnvironment _env;
        public ProductsController(ProductContext pdContext, IHostingEnvironment env )
        {
            _pdContext = pdContext;
            _env = env;
        }

        public IActionResult Index()
        {
            return View(_pdContext.products.Include(c=>c.category).ToList());
        }

        public IActionResult Dashbord()
        {
            List<Product> products = _pdContext.products.ToList();
            List<Category> categories = _pdContext.categories.ToList();
            dynamic obj = new ExpandoObject();
            obj.Category = categories;
            obj.Products = products;
            return View(obj);
        }
        // Get Method for Create
        public IActionResult Create()
        {
            ViewData["categoryId"] = new SelectList(_pdContext.categories, "Id", "categoryName");
            return View();
        }
        // post method for create
        [HttpPost]
        public IActionResult Create(Product product)
        {
            //string wwwroot = _env.WebRootPath;
            //string FileName = Path.GetFileNameWithoutExtension(product.ProductImageFile.FileName);
            var nam = Path.Combine(_env.WebRootPath + "/Images", Path.GetFileName(product.ProductImageFile.FileName));
            product.ProductImageFile.CopyTo(new FileStream(nam, FileMode.Create));
            product.ProductImageUrl = "Images/" + product.ProductImageFile.FileName;
            _pdContext.products.Add(product);
            _pdContext.SaveChanges();
            Inventory inventory = new Inventory();
            inventory.reOrderLevel = 21;
            inventory.quantity = int.Parse(Request.Form["qty"].ToString());
            inventory.ProductId = product.Id;
            _pdContext.inventories.Add(inventory);
            _pdContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int? id)
        {            var products = _pdContext.products.Include(c=>c.category).FirstOrDefault(p=>p.Id == id);
            ViewData["savecat"] = new SelectList(_pdContext.categories, "Id", "categoryName");
            return View(products);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (product.ProductImageFile != null)
            { 
            var nam = Path.Combine(_env.WebRootPath + "/Images", Path.GetFileName(product.ProductImageFile.FileName));
            product.ProductImageFile.CopyTo(new FileStream(nam, FileMode.Create));
            product.ProductImageUrl = "Images/" + product.ProductImageFile.FileName;
        }
            _pdContext.products.Update(product);
            _pdContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int? id)
        {
            var products = _pdContext.products.Include(c => c.category).FirstOrDefault(p => p.Id == id);
            ViewData["savecat"] = new SelectList(_pdContext.categories, "Id", "categoryName");
            return View(products);
        }
        // Get Method for delete
        public IActionResult Delete(int? id)
        {
            var products = _pdContext.products.Include(c => c.category).FirstOrDefault(p => p.Id == id);
            ViewData["savecat"] = new SelectList(_pdContext.categories, "Id", "categoryName");
            return View(products);
        }
        [HttpPost]
        public IActionResult Delete(Product products)
        {
            _pdContext.Remove(products);
            _pdContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult GetCategorys()
        {
            return View(_pdContext.categories.ToList());
        }

        public IActionResult AssignProducts(int? id)
        {
            return View(_pdContext.products.Include(e=>e.category).Where(e=>e.CategoryId==id).ToList());
        }
        [HttpPost]
        public IActionResult AssignProducts(List<Product> products)
        {
            foreach(Product product in products)
            {
                if(product.Availability)
                {
                    Product p = new Product();
                    p = _pdContext.products.FirstOrDefault(e=>e.Id==product.Id);
                    Cart cart = new Cart()
                    {
                        ProductId = p.Id,
                        totalcartPrice = p.ProductPrice

                    };
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
