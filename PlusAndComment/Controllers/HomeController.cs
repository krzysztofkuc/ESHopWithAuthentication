

using AutoMapper;
using PlusAndComment.Models;
using PlusAndComment.Models.Entities;
using PlusAndComment.Models.ViewModel;
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

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            var products = db.Products.OrderByDescending(m => m.ProductId).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var categories = db.Categories.ToList();

            HomeVM homeVm = new HomeVM();
            homeVm.Prducts = Mapper.Map<ICollection<ProductVM>>(products);
            homeVm.Categories = Mapper.Map<ICollection<CategoryVM>>(categories);

            //var sim = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            //if (User.Identity.IsAuthenticated)
            //{
            //    var currentUser = db.Users.Find(User.Identity.GetUserId());

            //    if (currentUser != null)
            //    {
            //    }
            //    else
            //    {
            //        RedirectToAction("LogOff", "Account");
            //    }
            //}

            return View(homeVm);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetCategoryProducts(int categoryId)
        {
            var products = await db.Products.Where(product => product.CatId == categoryId ).OrderByDescending(m => m.ProductId).ToListAsync();

            HomeVM homeVm = new HomeVM();
            homeVm.Prducts = Mapper.Map<ICollection<ProductVM>>(products);

            //var sim = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            //if (User.Identity.IsAuthenticated)
            //{
            //    var currentUser = db.Users.Find(User.Identity.GetUserId());

            //    if (currentUser != null)
            //    {
            //    }
            //    else
            //    {
            //        RedirectToAction("LogOff", "Account");
            //    }
            //}

            return PartialView("_CategoryResultsPartial", homeVm.Prducts);
        }

        public ActionResult AddCategory()
        {
            ViewBag.Message = "Add category page.";

            AddCategoryVM acVM = new AddCategoryVM();
            //var entities = db.Categories.ToList();

            var cats = db.Categories.ToList();
            acVM.AllCategories = Mapper.Map<List<CategoryVM>>(cats);

            return View(acVM);
        }

        [HttpPost]
        public ActionResult AddCategory(AddCategoryVM cat)
        {
            if (ModelState.IsValid)
            {
                var category = Mapper.Map<CategoryEntity>(cat.Category);

                db.Categories.Add(category);
                db.SaveChanges();

                ViewBag.Message = "Category Added succefully";
            }
            AddCategoryVM acVM = new AddCategoryVM();
           // acVM.AllCategories = Mapper.Map<List<CategoryVM>>(db.Categories);

            return View(acVM);
        }

        public ActionResult AddProduct()
        {
            ViewBag.Message = "Add product page.";
            AddProductVM acVM = new AddProductVM();
            var cats = db.Categories.ToList();
            acVM.AllCategories = Mapper.Map<List<CategoryVM>>(cats);
            acVM.AllProducts = Mapper.Map<ICollection<ProductVM>>(db.Products);
            
            acVM.CurrentProduct = new ProductVM();

            return View(acVM);
        }

        [HttpPost]
        public ActionResult AddProduct(AddProductVM product)
        {
            if (ModelState.IsValid)
            {
                var entity = Mapper.Map<ProductEntity>(product.CurrentProduct);
                db.Products.Add(entity);
                db.Entry(entity).State = EntityState.Added;

                db.SaveChanges();
            }

            return RedirectToAction("AddProduct");
        }

        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            var entity = db.Products.Find(id);
            var vm = Mapper.Map<ProductVM>(entity);

            return View(vm);
        }

        [HttpPost]
        public ActionResult EditProduct(ProductVM product)
        {
            var entity = Mapper.Map<ProductEntity>(product);
            db.Products.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("AddProduct");
        }

        [HttpGet]
        public ActionResult DeleteProduct(int id)
        {
            var entity = db.Products.Find(id);
            db.Products.Remove(entity);
            db.Entry(entity).State = EntityState.Deleted;

            db.SaveChanges();

            //var allProducts = Mapper.Map<ICollection<ProductVM>>(db.Products); 

            return RedirectToAction("AddProduct");
        }

    }
}