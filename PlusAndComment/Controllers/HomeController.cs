

using AutoMapper;
using Newtonsoft.Json;
using PlusAndComment.Models;
using PlusAndComment.Models.Entities;
using PlusAndComment.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlusAndComment.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private List<CategoryAttributesVM> _currentAllCategoryFilters = new List<CategoryAttributesVM>();

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index(int? categoryId, string attrs = null)
        {
            List<CategoryAttributesVM> searchedFilters = GetSearchedFilters();

            List<ProductVM> productsX = GetFilteredProducts(searchedFilters);

            //if (!string.IsNullOrEmpty(attrs))
            //{
            //    var attributes = JsonConvert.DeserializeObject<List<CategoryAttributesVM>>(attrs);

            //    products = FilterProducts(attributes);
            //}
            //else
            //{
            //    products = GetProductsByCategory(db.Categories.Find(categoryId))?.ToList() ?? new List<ProductEntity>();
            //}

            var products = GetProductsByCategory(db.Categories.Find(categoryId))?.ToList() ?? new List<ProductEntity>();

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

        private List<ProductVM> GetFilteredProducts(List<CategoryAttributesVM> filters)
        {
            foreach (var filter in filters)
            {

            }

            return null;
        }

        private List<CategoryAttributesVM> GetSearchedFilters()
        {
            List<CategoryAttributesVM> resultAttrs = new List<CategoryAttributesVM>();

            var queryString = Request.QueryString;

            //Select filers from querstring
            if (queryString.AllKeys.Contains("filters") && queryString["filters"] == true.ToString())
            {
                foreach (var key in queryString.AllKeys)
                {
                    string filterName = string.Empty;

                    if (key.StartsWith("MultiSelect"))
                    {
                        filterName = key.Split('_').Skip(1).FirstOrDefault();
                        var comboboxId = key.Split('_').Skip(2).FirstOrDefault();

                        var attr = db.CategoryAttributes.FirstOrDefault(x => x.Name == filterName);
                        var attrVM = Mapper.Map<CategoryAttributesVM>(attr);
                        attrVM.ComboboxValues.FirstOrDefault(g => g.Id == Convert.ToInt32(comboboxId)).Checked = true;
                        resultAttrs.Add(attrVM);
                    }
                    else
                    {
                        if (key == "filters")
                            continue;

                        var attr = db.CategoryAttributes.FirstOrDefault(x => x.Name == key);
                        attr.Value = queryString[key];
                        var attrVM = Mapper.Map<CategoryAttributesVM>(attr);
                        resultAttrs.Add(attrVM);
                    }
                }
            }

            return resultAttrs;
        }

        [HttpPost]
        public  ActionResult Index (List<CategoryAttributesVM> filters)
        {
            List<CategoryAttributesVM> filledFilters = GetOnlyFilledFilters(filters);

            var uriBuilder = new UriBuilder("http://localhost:44393/");
            var parameters = HttpUtility.ParseQueryString(string.Empty);

            BuildParametersFromFilters(ref parameters, filledFilters);

            uriBuilder.Query = parameters.ToString();

            return Redirect(uriBuilder.Uri.ToString());
        }

        private void BuildParametersFromFilters(ref NameValueCollection parameters, List<CategoryAttributesVM> filledFilters)
        {
            foreach (var item in filledFilters)
            {
                if (item.Value != null)
                {
                    parameters[item.Name] = item.Value;
                }
                else if (item.ComboboxValues.Count > 0)
                {
                    foreach (var combo2 in item.ComboboxValues)
                    {
                        if (combo2.Value == "on")
                            parameters["MultiSelect_" + item.Name + "_" + combo2.Id.ToString()] = true.ToString();
                    }
                }
            }

            if (parameters.Count > 0)
                parameters["filters"] = true.ToString();
        }

        private List<CategoryAttributesVM> GetOnlyFilledFilters(List<CategoryAttributesVM> filters)
        {
            List<CategoryAttributesVM> filledFilters = new List<CategoryAttributesVM>();

            foreach (var item in filters)
            {
                if (item.Value != null)
                {
                    filledFilters.Add(item);
                    continue;
                }

                if (item.ComboboxValues?.Count > 0)
                {
                    filledFilters.Add(item);
                }
            }

            return filledFilters;
        }

        public List<ProductEntity> FilterProducts(List<CategoryAttributesVM> attrs)
        {
            List<ProductEntity> productsEnt = new List<ProductEntity>();

            foreach (var attr in attrs)
            {
                var attrEnt = db.CategoryAttributes.Find(attr.PKAttributeId);
                var products = attrEnt.CategoryAttribute.Products.Where(m => m.Attributes.Any(x => x.Value == attr.Value));

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
                if(cat.Attributes != null)
                foreach (var attribute in cat.Attributes)
                {
                    _currentAllCategoryFilters.Add(Mapper.Map<CategoryAttributesVM>(attribute));
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
                _currentAllCategoryFilters.Add(Mapper.Map<CategoryAttributesVM>(attr));
            }

            var parent = db.Categories.Find(cat.ParentId);

            if (parent == null) return;

            HelperFillRecurencyFilters(parent);
        }

        [HttpGet]
        public ActionResult ProductsAttributes()
        {

            ICollection<CategoryAttributesEntity> attrs = db.CategoryAttributes.ToList();

            var vm = Mapper.Map<ICollection<CategoryAttributesVM>>(attrs);

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