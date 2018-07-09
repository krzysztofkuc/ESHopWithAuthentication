

using AutoMapper;
using Newtonsoft.Json;
using PlusAndComment.Models;
using PlusAndComment.Models.Entities;
using PlusAndComment.Models.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PlusAndComment.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private List<ProductAttributesVM> _currentAllCategoryFilters = new List<ProductAttributesVM>();

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index(int? categoryId, string attrs = null)
        {
            var products = new List<ProductEntity>();

            if (!string.IsNullOrEmpty(attrs))
            {
                var attributes = JsonConvert.DeserializeObject<List<ProductAttributesVM>>(attrs);

                products = FilterProducts(attributes);
            }
            else
            {
                products = GetProductsByCategory(db.Categories.Find(categoryId))?.ToList() ?? new List<ProductEntity>();
            }

            var categories = db.Categories.ToList();
            var currentCategory = categories.FirstOrDefault(x => x.CategoryId == categoryId);

            var listCategories = new List<CategoryEntity>();

            if (currentCategory != null)
                listCategories.Add(currentCategory);

            FillCurrentAllCategoryChildsFilters(listCategories);

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

        public List<ProductEntity> FilterProducts(List<ProductAttributesVM> attrs)
        {
            List<ProductEntity> productsEnt = new List<ProductEntity>();

            foreach (var attr in attrs)
            {
                var attrEnt = db.ProductsAttributes.Find(attr.ProductAttributeId);
                var products = attrEnt.CategoryAttribute.Products.Where(m => m.Attributes.Any(x => x.Name == attr.Name && x.Value == attr.Value));

                if(products.Count() > 0)
                {
                    productsEnt.AddRange(products.ToList());
                }
            }

            return productsEnt;
        }

        private void FillCurrentAllCategoryChildsFilters(List<CategoryEntity> categories)
        {
            foreach (var cat in categories)
            {
                foreach (var attribute in cat.Attributes)
                {
                    _currentAllCategoryFilters.Add(Mapper.Map<ProductAttributesVM>(attribute));
                }

                if(cat.Categories.Count > 0)
                {
                    FillCurrentAllCategoryChildsFilters(cat.Categories.ToList());
                }
            }
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
    }
}