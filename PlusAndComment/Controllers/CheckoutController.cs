using AutoMapper;
using Microsoft.AspNet.Identity;
using PlusAndComment.Models;
using PlusAndComment.Models.Entities;
using PlusAndComment.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PlusAndComment.Controllers
{
    public class CheckoutController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        const string PromoCode = "FREE";

        // GET: Checkout
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Checkout/AddressAndPayment
        public ActionResult AddressAndPayment()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            OrderVM vm = new OrderVM();
            vm.Total = cart.GetTotal();

            return View(vm);
        }

        //
        // POST: /Checkout/AddressAndPayment
        [HttpPost]
        public ActionResult AddressAndPayment(FormCollection values)
        {
            var order = new OrderVM();
            TryUpdateModel(order);

            if (!ModelState.IsValid)
            {
                return View(order);
            }


            try
            {
                //if (string.Equals(values["PromoCode"], PromoCode,
                //    StringComparison.OrdinalIgnoreCase) == false)
                //{
                //    return View(order);
                //}
                //else
                //{
                    order.Username = User.Identity.Name;
                    order.OrderDate = DateTime.Now;

                    var orderEntity = Mapper.Map<OrderEntity>(order);

                    //Save Order
                    db.Orders.Add(orderEntity);
                    db.SaveChanges();
                    //Process the order
                    var cart = ShoppingCart.GetCart(this.HttpContext);
                    order.OrderId =cart.CreateOrder(orderEntity);

                    return RedirectToAction("CheckoutComplete",
                        new { id = order.OrderId });
                //}
            }
            catch(Exception e)
            {
                //Invalid - redisplay with errors
                return View(order);
            }
        }

        //
        // GET: /Checkout/Complete
        public async Task<ActionResult> CheckoutComplete(int id)
        {
            // Validate customer owns this order
            bool isValid = db.Orders.Any(
                o => o.OrderId == id &&
                o.Username == User.Identity.Name);

            var order = db.Orders.First(
                o => o.OrderId == id &&
                o.Username == User.Identity.Name);

            var user = db.Users.FirstOrDefault(x => x.UserName ==  User.Identity.Name);

            if (isValid)
            {
                await SendMessageOrderToCustomerAndTotheCompany(order, user);
                return View(id);
            }
            else
            {
                return View("Error");
            }
        }

        private async Task SendMessageOrderToCustomerAndTotheCompany(OrderEntity order, ApplicationUser user)
        {
            //var products = db.Products.Join(db.OrderDetails, product => product.ProductId, ord => ord.ProductId, (product, orderEnt) => new { ProductEntity = product, OrderDetailEntity = orderEnt })
            //    .Where(o => o.OrderDetailEntity.OrderId == order.OrderId);

            var products = db.OrderDetails.Where(x => x.OrderId == order.OrderId).Select(y => y.Product);
            StringBuilder messageContent = new StringBuilder();

            messageContent.Append("<div style:' margin: 50px'>");
                messageContent.Append("Thank you for buying the products: ");
                messageContent.Append("<br>");
                messageContent.Append("<table border='1' cellpadding='10'>");
                messageContent.Append("<th>Name</th>");
                messageContent.Append("<th>Model</th>");
                messageContent.Append("<th>Price</th>");

                double summary = 0;
                foreach (var product in products)
                {
                    messageContent.Append("<tr>");
                        messageContent.Append("<td>");
                            messageContent.Append(product.Name + " ");
                        messageContent.Append("</td>");
                        messageContent.Append("<td>");
                            messageContent.Append("Model ????");
                        messageContent.Append("</td>");

                        messageContent.Append("<td>");
                            messageContent.Append(product.Price);
                        messageContent.Append("</td>");
                    messageContent.Append("</tr>");

                    summary += product.Price;
                }

                var cart = ShoppingCart.GetCart(this.HttpContext);
                double sum = cart.GetTotal();

                messageContent.Append("<td colspan='2'></td>");
                messageContent.Append("<td>");
                    messageContent.Append("Sum: " + summary);
                messageContent.Append("</td>");

                messageContent.Append("</table>");
            messageContent.Append("</div>");



            var message = new IdentityMessage
            {
                Destination = order.Email,
                Body = messageContent.ToString()

            };

            EmailService service = new EmailService();
            await service.SendAsync(message);
        }
    }
}