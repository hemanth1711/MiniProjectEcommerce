using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniProjectEcommerce.Models;

namespace MiniProjectEcommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InventroysController : Controller
    {
        public ProductContext _pdContext;
        public InventroysController(ProductContext pdContext)
        {
            _pdContext = pdContext;
        }

        public IActionResult Index()
        {
            return View(_pdContext.inventories.Include(c => c.product).ToList());
        }

        public IActionResult Create()
        {
            ViewData["prodId"] = new SelectList(_pdContext.products, "Id", "ProductName");
            return View();
        }
        [HttpPost]
        public IActionResult Create(Inventory inventory)
        {
            //string wwwroot = _env.WebRootPath;
            //string FileName = Path.GetFileNameWithoutExtension(product.ProductImageFile.FileName);
            //inventory.Id = inventory.ProductId;
            _pdContext.inventories.Add(inventory);
            _pdContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            var inve = _pdContext.inventories.Include(c => c.product).FirstOrDefault(p => p.Id == id);
            ViewData["prodId"] = new SelectList(_pdContext.products, "Id", "ProductName");
            return View(inve);
        }

        [HttpPost]
        public IActionResult Edit(Inventory inve)
        {
            _pdContext.inventories.Update(inve);
            _pdContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            var inve = _pdContext.inventories.Include(c => c.product).FirstOrDefault(p => p.Id == id);
            //ViewData["savecat"] = new SelectList(_pdContext.categories, "Id", "categoryName");
            _pdContext.inventories.Remove(inve);
            _pdContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
