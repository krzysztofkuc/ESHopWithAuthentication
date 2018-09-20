using AutoMapper;
using PlusAndComment.Models;
using PlusAndComment.Models.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PlusAndComment.Controllers
{
    public class ShoppingCartController : Controller
    {
        ApplicationDbContext storeDB = new ApplicationDbContext();

        public double CartTotal { get; private set; }

        //
        // GET: /ShoppingCart/
        public ActionResult Index()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Set up our ViewModel
            var viewModel = new ShoppingCartVM()
            {
                CartItems = Mapper.Map<ICollection<CartVM>>(cart.GetCartItems()),
                CartTotal = cart.GetTotal() 
            };

            // Return the view
            return View(viewModel);
        }

        public PartialViewResult BasketThumb()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Set up our ViewModel
            var viewModel = new ShoppingCartVM()
            {
                CartItems = Mapper.Map<ICollection<CartVM>>(cart.GetCartItems()),
                CartTotal = cart.GetTotal()
            };
            // Return the view

            //here have to return partial view and show it byu ajax
            return PartialView("_BasketPopupPreview", viewModel);
        }

        //
        // GET: /Store/AddToCart/5
        public async Task<int> AddToCart(int id)
        {
            // Retrieve the album from the database
            var product = await storeDB.Products
                .FindAsync(id);

            // Add it to the shopping cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            await cart.AddToCart(product);

            // Set up our ViewModel

            var CartItems = Mapper.Map<ICollection<CartVM>>(cart.GetCartItems());

            var numberOfAllItems = (int)CartItems.Sum(m => m.Number);

            this.HttpContext.Session["NumberOfAllItems"] = numberOfAllItems;

            // Go back to the main store page for more shopping
            return numberOfAllItems;
        }

        //
        // AJAX: /ShoppingCart/RemoveFromCart/5
        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            // Remove the item from the cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Get the name of the album to display confirmation
            string albumName = storeDB.Carts
                .Single(item => item.RecordId == id).Product.Title;

            // Remove from cart
            int itemCount = cart.RemoveFromCart(id);

            // Display the confirmation message
            var results = new ShoppingCartRemoveVM
            {
                Message = Server.HtmlEncode(albumName) +
                    " has been removed from your shopping cart.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = id
            };

            HttpContext.Session["NumberOfAllItems"] = cart.GetCount();

            return Json(results);
        }
        //
        // GET: /ShoppingCart/CartSummary
        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            ViewData["CartCount"] = cart.GetCount();
            return PartialView("CartSummary");
        }
    }
}