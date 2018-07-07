

using AutoMapper;
using PlusAndComment.Models;
using PlusAndComment.Models.Entities;
using PlusAndComment.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PlusAndComment.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private List<ProductAttributesVM> _currentAllCategoryFilters = new List<ProductAttributesVM>();

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index(int? categoryId)
        {
            var products = GetProductsByCategory(db.Categories.Find(categoryId))?? new List<ProductEntity>();

            var categories = db.Categories.ToList();

            FillCurrentAllCategoryChildsFilters(categoryId);

            HomeVM homeVm = new HomeVM()
            {
                Prducts = Mapper.Map<ICollection<ProductVM>>(products),
                Categories = Mapper.Map<ICollection<CategoryVM>>(categories),
                CurrentAttributes = _currentAllCategoryFilters

            };

            //shopping cart
            var cart = ShoppingCart.GetCart(this.HttpContext);
            var CartItems = Mapper.Map<ICollection<CartVM>>(cart.GetCartItems());
            int numberOfItems = (int)CartItems.Sum(m => m.Number);

            ViewData["TrolleyItemsCount"] = numberOfItems;

            return View(homeVm);
        }

        [HttpPost]
        public ActionResult FilterProducts(List<ProductAttributesVM> attrs)
        {
            //db.Products.Where(m => m. );
            // All model properties are null here????

            return Json("Success");
        }

        private void FillCurrentAllCategoryChildsFilters(int? id)
        {
            if (id == null) return;
            var category = db.Categories.Find(id);

            foreach (var attribute in category.Attributes)
            {
                _currentAllCategoryFilters.Add(Mapper.Map<ProductAttributesVM>(attribute));
            }

            FillCurrentAllCategoryChildsFilters(category);
        }

        private void FillCurrentAllCategoryChildsFilters(CategoryEntity category)
        {
            if (category.ParentId == null) return;

            CategoryEntity parent = db.Categories.Find(category.ParentId);

            HelperFillRecurencyFilters(parent);
        }

        private void HelperFillRecurencyFilters(CategoryEntity cat)
        {
            foreach (var attr in cat.Attributes)
            {
                _currentAllCategoryFilters.Add(Mapper.Map<ProductAttributesVM>(attr));
            }

            var parent = db.Categories.Find(cat.ParentId);

            if (parent == null) return;

            HelperFillRecurencyFilters(parent);
        }

        //private void HelperFillRecurencyFilters(CategoryEntity parent)
        //{
        //    List<ProductAttributeVM> currentCategoryAttrs = Mapper.Map<List<ProductAttributeVM>>(category.Attributes);

        //    //Add to _currentAllCategoryFilters
        //    currentCategoryAttrs.ForEach(x => _currentAllCategoryFilters.Push(x));


        //    FillRecurencyFilters(category.Categories);
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public ICollection<ProductEntity> GetCategoryProducts(int? categoryId)
        //{
        //    var products = db.Products.Where(product => product.CatId == categoryId ).OrderByDescending(m => m.ProductId).ToList();

        //    HomeVM homeVm = new HomeVM();
        //    if (categoryId != null)
        //    {
        //        ICollection<ProductEntity> prducts = GetProductsByCategory(db.Categories.Find(categoryId));

        //        homeVm.Prducts = Mapper.Map<ICollection<ProductVM>>(prducts);
        //    }

            

        //    //var sim = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
        //    //if (User.Identity.IsAuthenticated)
        //    //{
        //    //    var currentUser = db.Users.Find(User.Identity.GetUserId());

        //    //    if (currentUser != null)
        //    //    {
        //    //    }
        //    //    else
        //    //    {
        //    //        RedirectToAction("LogOff", "Account");
        //    //    }
        //    //}

        //    return PartialView("_CategoryResultsPartial", homeVm.Prducts);
        //}

        [HttpGet]
        public ActionResult ProductsAttributes()
        {

            ICollection<ProductAttributesEntity> attrs = db.ProductsAttributes.ToList();

            var vm = Mapper.Map<ICollection<ProductAttributesVM>>(db.ProductsAttributes);

            return View(vm);
        }

        private ICollection<ProductEntity> GetProductsByCategory(CategoryEntity category, List<ProductEntity> result = null)
        {
            if (category == null) return null;

            if (result == null)
                result = new List<ProductEntity>();

            result.AddRange(category.Products);

            foreach (var catItem in category.Categories)
            {
                GetProductsByCategory(catItem, result);
            }

            return result;
        }

        //private void HelperFillRecurencyFilters(CategoryEntity cat)
        //{
        //    foreach (var attr in cat.Attributes)
        //    {
        //        _currentAllCategoryFilters.Push(Mapper.Map<ProductAttributeVM>(attr));
        //    }

        //    var parent = db.Categories.Find(cat.ParentId);

        //    if (parent == null) return;

        //    HelperFillRecurencyFilters(parent);
        //}
    }
}