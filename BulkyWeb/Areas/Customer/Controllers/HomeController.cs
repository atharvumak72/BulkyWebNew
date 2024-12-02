using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Utility;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            //To get user id we have some default funcations
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //To handle count of cart when your not login
            if (claims != null)
            {
                //Adding session
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claims.Value).Count());
            }

            IEnumerable<Product> productsList = _unitOfWork.Product.GetAll(includeProperties:"Category,ProductImages");
            return View(productsList);
        }
        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.Get(u=>u.Id== productId, includeProperties: "Category,ProductImages"),
                Count=1,
                ProductId=productId
            };
            
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            //To get user id we have some default funcations
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            //We are comparing Id
            shoppingCart.ApplicationUserId = userId;

            //To Handle for duplicate entry 
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);

            if (cartFromDb!=null)
            {
                //Shopping cart exists
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();
                //Note : if we commented out above line then also count will get update without above line because of enitity core framework tracked funcationality
                //to stop it we need to add bool tracked=false in irepository.
            }
            else
            {
                //add new recoards
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();

                //Adding session
                HttpContext.Session.SetInt32(SD.SessionCart,_unitOfWork.ShoppingCart.GetAll(u=>u.ApplicationUserId == userId).Count());
            }

            TempData["Success"] = "Cart updated successfully";
            


            return RedirectToAction(nameof(Index));
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
