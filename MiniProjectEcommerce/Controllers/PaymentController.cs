using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniProjectEcommerce.Models;
using Razorpay.Api;

namespace MiniProjectEcommerce.Controllers
{
    public class PaymentController : Controller
    {
        private ProductContext _pdContext;
        public PaymentController(ProductContext pdContext)
        {
            _pdContext = pdContext;
        }
        private const string raz_key = "rzp_test_cfElqRcKNLh6aA";
        private const string raz_secret = "3QaoZiwflNJbQftFonT55elT";
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult payment(int? id)
        {
            var products = _pdContext.products.Include(c => c.category).FirstOrDefault(p => p.Id == id);
            var oid = Createorder(products);
            payOptions pay = new payOptions()
            {
                key = raz_key,
                Amountinsub = products.ProductPrice*100,
                currency = "INR",
                name = "Global Logic MiniMall",
                Description = "A Good",
                ImageUrl="",
                Orderid=oid,

            };
            return View(pay);

        }
        public String Createorder(Product products)
        {
            try
            {
                RazorpayClient client = new RazorpayClient(raz_key, raz_secret);
                Dictionary<string, object> input = new Dictionary<string, object>();
                input.Add("amount", products.ProductPrice*100);
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
    }
}
