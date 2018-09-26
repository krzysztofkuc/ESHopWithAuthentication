using AutoMapper;
using PlusAndComment.Models;
using PlusAndComment.Models.Entities;
using PlusAndComment.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PlusAndComment.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private List<CategoryAttributeVM> _currentAllCategoryFilters = new List<CategoryAttributeVM>();

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index(int? categoryId, string attrs = null)
        {
            ICollection<ProductEntity> products = new List<ProductEntity>();
            var categories = db.Categories.ToList();

            #region SEARCH_PRODUCTS_BY_ATTRIBUTES

                List<CategoryAttributeVM> searchedFilters = GetSearchedFilters();
                if (searchedFilters.Count > 0)
                {
                    foreach (var filter in searchedFilters)
                    {
                        foreach (var product in filter.CategoryAttribute.Products)
                        {
                            foreach (var attribute in product.Attributes)
                            {
                                bool shouldAdd = false;

                                switch(attribute.CategoryAttribute.AttributeType)
                                {
                                    case "date":
                                        var value = DateTime.Parse(attribute.Value);

                                        if (!string.IsNullOrEmpty(filter.dateFrom) && value > DateTime.Parse(filter.dateFrom))
                                        {
                                            shouldAdd = true;
                                            break;
                                        }

                                        if (!string.IsNullOrEmpty(filter.dateTo) &&  value < DateTime.Parse(filter.dateTo))
                                        {
                                            shouldAdd = true;
                                            break;
                                        }
                                        break;
                                    case "number":

                                        if (filter.numberFrom != null && Convert.ToDouble(attribute.Value) > filter.numberFrom)
                                        {
                                            shouldAdd = true;
                                            break;
                                        }

                                        if (filter.numberTo != null && Convert.ToDouble(attribute.Value) < filter.numberTo)
                                        {
                                            shouldAdd = true;
                                            break;
                                        }
                                        break;
                                    case "text":
                                        break;
                                }


                                if(shouldAdd)
                                {
                                    products.Add(Mapper.Map<ProductEntity>(product));

                                }
                            }
                        }
                    }
                }
                else
                {
                    products = GetProductsByCategory(db.Categories.Find(categoryId))?.ToList() ?? new List<ProductEntity>();
                }

            #endregion SEARCH_PRODUCTS_BY_ATTRIBUTES

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

        private List<CategoryAttributeVM> GetSearchedFilters()
        {
            List<CategoryAttributeVM> resultAttrs = new List<CategoryAttributeVM>();

            var queryString = Request.QueryString;
            var previousAttribute = string.Empty;

            //Select filers from querstring
            if (queryString.AllKeys.Contains("filters") && queryString["filters"] == true.ToString())
            {
                foreach (var key in queryString.AllKeys)
                {
                    string filterName = string.Empty;
                    string categoryAttributeName = key;

                    //add handle by type
                    if (key.StartsWith("MultiSelect"))
                    {
                        filterName = key.Split('_').Skip(1).FirstOrDefault();
                        var comboboxId = key.Split('_').Skip(2).FirstOrDefault();

                        var attr = db.CategoryAttributes.FirstOrDefault(x => x.Name == filterName);
                        var attrVM = Mapper.Map<CategoryAttributeVM>(attr);
                        attrVM.ComboboxValues.FirstOrDefault(g => g.Id == Convert.ToInt32(comboboxId)).Checked = true;
                        resultAttrs.Add(attrVM);
                    }
                    else
                    {
                        if (key == "filters")
                            continue;

                        //Split to dateFrom dateTo numberFro numberTo by "-"
                        if(key.Contains("-"))
                        {
                            filterName = key.Split('-').Skip(1).FirstOrDefault();
                            categoryAttributeName = key.Split('-').FirstOrDefault();
                            

                            if(!string.IsNullOrEmpty(categoryAttributeName))
                            {
                                previousAttribute = categoryAttributeName;
                            }
                            else
                            {
                                categoryAttributeName = previousAttribute;
                            }
                        }

                        var attr = db.CategoryAttributes.FirstOrDefault(x => x.Name == categoryAttributeName);

                        //here is the robblem with encoding polish signs
                        attr.Value = queryString[HttpUtility.UrlDecode(key)];

                        var attrVM = Mapper.Map<CategoryAttributeVM>(attr);
                        CategoryAttributeVM existingAttribute = resultAttrs.Find(x => x.Name == categoryAttributeName);

                        switch (filterName)
                        {
                            case "dateFrom":
                                attrVM.dateFrom = attr.Value;
                                break;
                            case "dateTo":
                                if (existingAttribute != null)
                                {
                                    existingAttribute.dateTo = attr.Value;
                                    continue;
                                }
                                else
                                {
                                    attrVM.dateTo = attr.Value;
                                }
                                break;
                            case "numberFrom":
                                attrVM.numberFrom = Convert.ToDouble(attr.Value);
                                break;
                            case "numberTo":
                                if (existingAttribute != null)
                                {
                                    existingAttribute.numberTo = Convert.ToDouble(attr.Value);
                                    continue;
                                }
                                else
                                {
                                    attrVM.numberTo = Convert.ToDouble(attr.Value);
                                }
                                break;
                        }

                        resultAttrs.Add(attrVM);
                    }
                }
            }

            return resultAttrs;
        }

        [HttpPost]
        public  ActionResult Index (List<CategoryAttributeVM> filters)
        {
            List<CategoryAttributeVM> filledFilters = GetOnlyFilledFilters(filters);

            //TODO: should by dynamic
            var uriBuilder = new UriBuilder("http://localhost:44393/");
            var parameters = HttpUtility.ParseQueryString(string.Empty);

            BuildParametersFromFilters(ref parameters, filledFilters);

            uriBuilder.Query = parameters.ToString();

            return Redirect(uriBuilder.Uri.ToString());
        }

        /// <summary>
        ///     Build URL parameters from searched filters
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="filledFilters"></param>
        private void BuildParametersFromFilters(ref NameValueCollection parameters, List<CategoryAttributeVM> filledFilters)
        {
            foreach (var item in filledFilters)
            {
                //add handle by type
                if (item.Value != null)
                {
                    parameters[item.Name] = item.Value;
                }
                if (item.numberFrom != null && item.numberTo != null)
                {
                    parameters[item.Name + "-numberFrom"] = item.numberFrom.ToString();
                    parameters["-numberTo"] = item.numberTo.ToString();
                }
                else if (item.numberTo != null)
                {
                    parameters[item.Name + "-numberTo"] = item.numberTo.ToString();
                }
                else if (item.numberFrom != null)
                {
                    parameters[item.Name + "-numberFrom"] = item.numberFrom.ToString();
                }

                if (item.dateFrom != null && item.dateTo != null)
                {
                    parameters[item.Name + "-dateFrom"] = item.dateFrom;
                    parameters["-dateTo"] = item.dateTo;
                }
                else if (item.dateFrom != null)
                {
                    parameters[item.Name + "-dateFrom"] = item.dateFrom;
                }
                else if(item.dateTo != null)
                {
                    parameters[item.Name + "-dateTo"] = item.dateTo;
                }

                if (item.ComboboxValues?.Count > 0)
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

        private List<CategoryAttributeVM> GetOnlyFilledFilters(List<CategoryAttributeVM> filters)
        {
            List<CategoryAttributeVM> filledFilters = new List<CategoryAttributeVM>();

            foreach (var item in filters)
            {
                if (item.Value != null || item.numberFrom != null || item.numberTo != null || item.dateFrom != null || item.dateTo != null)
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

        public List<ProductEntity> FilterProducts(List<CategoryAttributeVM> attrs)
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
            var attributes = db.ProductAttributes;
             
            foreach (var cat in categories)
            {
                if(cat.Attributes != null)
                    foreach (var attribute in cat.Attributes)
                    {
                            //Get all ProductAtrtributes with same Name
                            var productAttributes = attributes.Where(p => p.CategoryAttribute.Name == attribute.Name).ToList();

                            foreach (var productAttribute in productAttributes)
                            {
                                //Fill category multiSelect by all proprietary productAttributes values
                                if(productAttribute.ComboboxValues.Count > 0 )
                                {
                                    foreach (var comboValue in productAttribute.ComboboxValues)
                                    {
                                        var exist = attribute.ComboboxValues.Any(x => x.Value == comboValue.Value);

                                        if(!exist)
                                            attribute.ComboboxValues.Add(comboValue);
                                    }
                                }
                            }

                            _currentAllCategoryFilters.Add(Mapper.Map<CategoryAttributeVM>(attribute));
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
                _currentAllCategoryFilters.Add(Mapper.Map<CategoryAttributeVM>(attr));
            }

            var parent = db.Categories.Find(cat.ParentId);

            if (parent == null) return;

            HelperFillRecurencyFilters(parent);
        }

        [HttpGet]
        public ActionResult ProductsAttributes()
        {

            ICollection<ProductAttributeEntity> attrs = db.ProductAttributes.ToList();

            var vm = Mapper.Map<ICollection<ProductAttributeVM>>(attrs);

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