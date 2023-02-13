using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProjectEcommerce.Models;

namespace MiniProjectEcommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private ProductContext _pdContext;
        public CategoryController(ProductContext pdContext)
        {
            _pdContext = pdContext;
        }

        public IActionResult Index()
        {
            
            return View(_pdContext.categories.ToList());
        }
        // Get method for create

        public ActionResult Create()
        {
            return View();
        }

        // Post method

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            _pdContext.categories.Add(category);
            _pdContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // Get method for Edit

        public ActionResult Edit(int? id)
        {
            var categoryEdit = _pdContext.categories.Find(id);
            return View(categoryEdit);
        }
        [HttpPost]
        public ActionResult Edit(Category category)
        {
            _pdContext.categories.Update(category);
            _pdContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // get Method for details

        public ActionResult Details(int? id)
        {
            var categoryEdit = _pdContext.categories.Find(id);
            return View(categoryEdit);
        }

        // Post Method for Details

        [HttpPost]
        public ActionResult Details(Category category)
        {
            return RedirectToAction(nameof(Index));
        }
        // get Method for Delete

        public ActionResult Delete(int? id)
        {
            var categoryEdit = _pdContext.categories.Find(id);
            return View(categoryEdit);
        }

        // Post Method for Details

        [HttpPost]
        public ActionResult Delete(Category category)
        {
            _pdContext.Remove(category);
            _pdContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
