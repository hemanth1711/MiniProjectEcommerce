using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniProjectEcommerce.Models;
using Razorpay.Api;
using System.Diagnostics;

namespace MiniProjectEcommerce.Controllers
{
   
    public class HomeController : Controller
    {
        public const string raz_key = "rzp_test_cfElqRcKNLh6aA";
        public const string raz_secret = "3QaoZiwflNJbQftFonT55elT";
        public ProductContext _pdContext;
        public HomeController(ProductContext pdContext)
        {
            _pdContext = pdContext;
        }

        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public IActionResult Index()
        {
            ViewData["savecat"] = new SelectList(_pdContext.categories, "Id", "categoryName");
            return View(_pdContext.products.Include(c=>c.category).ToList());
        }
        [HttpPost]
        public IActionResult Index(int? id)
        {
            return RedirectToAction(nameof(Details));
        }

        //public JsonResult GetProducts(int sid)
        //{
        //    var products = _pdContext.products.Where(e => e.CategoryId == sid).Select(e => new
        //    {
        //        id = e.Id,
        //        name = e.ProductName,
        //        desc = e.productDescription,
        //        imgsrc = e.ProductImageUrl,
        //        price = e.ProductPrice,
        //    });
        //    return Json(products);
        //}

        public IActionResult Details(int? id)
        {
            var products = _pdContext.products.Include(c => c.category).FirstOrDefault(p => p.Id == id);
            return View(products);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Details(Product products)
        {
            Cart cart = new Cart();
            cart.productQuantity = products.CategoryId;
            cart.totalcartPrice = products.ProductPrice * products.CategoryId;
            cart.Timestamp = DateTime.UtcNow;
            cart.ProductId = products.Id;
            DateTime d = cart.Timestamp;
            int pid = cart.ProductId;
            int totalp = (int)cart.totalcartPrice;
            int quantity = cart.productQuantity;
            string us = HttpContext.User.Identity.Name;
            cart.userName = us;
            ViewData["pIm"] = us;
            _pdContext.carts.Add(cart);
            _pdContext.SaveChanges();
            return RedirectToAction(nameof(Cart));
        }
        [Authorize]
        public IActionResult Cart()
        {
            string us = HttpContext.User.Identity.Name.ToString();
            var cati = _pdContext.carts.Include(c=>c.product).Where(p => p.userName == us).ToList();
            ViewData["hello"] = _pdContext.carts.Where(p => p.userName == us).Sum(c => c.totalcartPrice).ToString();
            return View(cati);
        }

        public IActionResult Remove_cart(int? id)
        {
            var cartss = _pdContext.carts.FirstOrDefault(c=>c.Id==id);
            _pdContext.Remove(cartss);
            _pdContext.SaveChanges();
            return RedirectToAction(nameof(Cart));
        }
        [Authorize]
        public IActionResult payment(int? id)
        {
            var products = _pdContext.products.Include(c => c.category).FirstOrDefault(p => p.Id == id);
            var oid = Createorder(products);
            payOptions pay = new payOptions()
            {
                key = raz_key,
                Amountinsub = products.ProductPrice * 100,
                currency = "INR",
                name = "Global Logic MiniMall",
                Description = "A Good",
                ImageUrl = "",
                Orderid = oid,
                productid = products.Id

            };
            return View(pay);

        }

        public IActionResult Success(payOptions pay)
        {
            Sale s = new Sale();
            ProductSold ps = new ProductSold();
            var products = _pdContext.products.FirstOrDefault(p => p.Id == pay.productid);
            s.totalSales = 1;
            s.Timestamp = DateTime.Now;
            s.Flag = true;
            s.userName = HttpContext.User.Identity.Name;
            ps.userName = HttpContext.User.Identity.Name;
            ps.Productid = pay.productid;
            ps.Quantity = 1;
            ps.price = products.ProductPrice;
            _pdContext.sales.Add(s);
            _pdContext.SaveChanges();
            var SaleId = _pdContext.sales.FirstOrDefault(c=>c.Timestamp == s.Timestamp);
            ps.SaleId = SaleId.Id;
            saveps(ps);

            return View();
        }

        public void saveps (ProductSold ps)
        {
            _pdContext.productsSold.Add(ps);
            _pdContext.SaveChanges();
        }
        public String Createorder(Product products)
        {
            try
            {
                RazorpayClient client = new RazorpayClient(raz_key, raz_secret);
                Dictionary<string, object> input = new Dictionary<string, object>();
                input.Add("amount", products.ProductPrice * 100);
                input.Add("currency", "INR");
                input.Add("receipt", "12121");

                Order ord_Res = client.Order.Create(input);
                var oid = ord_Res.Attributes["id"].ToString();
                return oid;

            }
            catch
            {
                return null;
            }
        }
        [Authorize]
        public IActionResult MyOrders()
        {
            string us = HttpContext.User.Identity.Name.ToString();
            var myorder = _pdContext.productsSold.Include(c => c.product).Include(c=>c.sale).Where(p => p.userName == us).ToList();
            return View(myorder);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}