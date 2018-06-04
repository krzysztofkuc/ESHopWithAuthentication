using PlusAndComment.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PlusAndComment.Models
{
    public partial class ShoppingCart
    {
        ApplicationDbContext storeDB = new ApplicationDbContext();
        string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";

        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }

        // Helper method to simplify shopping cart calls
        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }

        public async Task<int> AddToCart(ProductEntity product)
        {
            // Get the matching cart and album instances
            var cartItem = await Task.Run(() => 
                storeDB.Carts.SingleOrDefault(
                c => c.CartId == ShoppingCartId
                && c.CartProductId == product.ProductId));

            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new CartEntity
                {
                    CartProductId = product.ProductId,
                    CartId = ShoppingCartId,
                    Number = 1,
                    DateCreated = DateTime.Now
                };
                storeDB.Carts.Add(cartItem);
                storeDB.Entry(cartItem).State = EntityState.Added;
            }
            else
            {
                // If the item does exist in the cart, 
                // then add one to the quantity
                cartItem.Number++;
            }
            // Save changes
             return await storeDB.SaveChangesAsync();
        }

        public int RemoveFromCart(int id)
        {
            // Get the cart
            var cartItem = storeDB.Carts.Single(
                cart => cart.CartId == ShoppingCartId
                && cart.RecordId == id);

            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Number > 1)
                {
                    cartItem.Number--;
                    itemCount = cartItem.Number;
                }
                else
                {
                    storeDB.Carts.Remove(cartItem);
                }
                // Save changes
                storeDB.SaveChanges();
            }
            return itemCount;
        }

        public void EmptyCart()
        {
            var cartItems = storeDB.Carts.Where(
                cart => cart.CartId == ShoppingCartId);

            foreach (var cartItem in cartItems)
            {
                storeDB.Carts.Remove(cartItem);
            }
            // Save changes
            storeDB.SaveChanges();
            //Session["NumberOfAllItems"] = 0;
        }

        public ICollection<CartEntity> GetCartItems()
        {
            return storeDB.Carts.Where(
                cart => cart.CartId == ShoppingCartId).ToList();
        }

        public int GetCount()
        {
            // Get the count of each item in the cart and sum them up
            int? count = (from cartItems in storeDB.Carts
                          where cartItems.CartId == ShoppingCartId
                          select (int?)cartItems.Number).Sum();
            // Return 0 if all entries are null
            return count ?? 0;
        }

        public double GetTotal()
        {
            // Multiply album price by count of that album to get 
            // the current price for each of those albums in the cart
            // sum all album price totals to get the cart total
            double total = (from cartItems in storeDB.Carts
                              where cartItems.CartId == ShoppingCartId
                              select (int?)cartItems.Number *
                              cartItems.Product.Price).Sum() ?? 0;

            return total;
        }

        //public double GetTotalProductsNumber()
        //{
        //    // Multiply album price by count of that album to get 
        //    // the current price for each of those albums in the cart
        //    // sum all album price totals to get the cart total
        //    double total = (from cartItems in storeDB.Carts
        //                    where cartItems.CartId == ShoppingCartId
        //                    select (int?)cartItems.Number *
        //                    cartItems.Product.Price).Sum() ?? 0;

        //    return total;
        //}

        public int CreateOrder(OrderEntity order)
        {
            double orderTotal = 0;

            var cartItems = GetCartItems();

            // Iterate over the items in the cart, 
            // adding the order details for each
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetailEntity
                {
                    ProductId = item.CartProductId,
                    OrderId = order.OrderId,
                    UnitPrice = item.Product.Price,
                    Quantity = item.Number
                };
                // Set the order total of the shopping cart
                orderTotal += (item.Number * item.Product.Price);

                storeDB.OrderDetails.Add(orderDetail);

            }
            // Set the order's total to the orderTotal count
            order.Total = orderTotal;

            // Save the order
            storeDB.SaveChanges();
            // Empty the shopping cart
            EmptyCart();
            // Return the OrderId as the confirmation number
            return order.OrderId;
        }

        // We're using HttpContextBase to allow access to cookies.
        public string GetCartId(HttpContextBase context)
        {
            if (context.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session[CartSessionKey] =
                        context.User.Identity.Name;
                }
                else
                {
                    // Generate a new random GUID using System.Guid class
                    Guid tempCartId = Guid.NewGuid();
                    // Send tempCartId back to client as a cookie
                    context.Session[CartSessionKey] = tempCartId.ToString();
                }
            }
            return context.Session[CartSessionKey].ToString();
        }
        // When a user has logged in, migrate their shopping cart to
        // be associated with their username
        public void MigrateCart(string userName)
        {
            IQueryable<CartEntity> shoppingCart = storeDB.Carts.Where(
                c => c.CartId == ShoppingCartId);

            foreach (CartEntity item in shoppingCart)
            {
                item.CartId = userName;
            }
            storeDB.SaveChanges();
        }
    }
}