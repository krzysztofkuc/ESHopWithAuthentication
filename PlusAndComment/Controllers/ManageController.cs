using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using PlusAndComment.Common;
using PlusAndComment.Models;
using PlusAndComment.Models.Entities;
using PlusAndComment.Models.ViewModel;

namespace PlusAndComment.Controllers
{
    //[Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext dbContext = new ApplicationDbContext();

        private ApplicationDbContext db = new ApplicationDbContext();

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Twoje hasło zostało pomyślnie zmienione."
                : message == ManageMessageId.SetPasswordSuccess ? "Twoje hasło zostawło ustawione."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var userId = User.Identity.GetUserId();
            var currentUser = await UserManager.FindByIdAsync(userId);
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
                Settings = AutoMapper.Mapper.Map<UserProfileSettingsVM>(currentUser.UserProfileSettings)
            };
            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }


        public ActionResult ManageRoles()
        {
            var users = (from user in dbContext.Users select user).ToList();
            var model = new MenageRolesVM();

            var usersFromDb = from u in dbContext.Users
                              from ur in u.Roles.DefaultIfEmpty()
                              join ro in dbContext.Roles.DefaultIfEmpty() on ur.RoleId equals ro.Id into newTable
                              from r in newTable.DefaultIfEmpty()
                              select new UserVM
                              {
                                  Id = u.Id,
                                  Name = u.UserName,
                                  LockoutEnabled = u.LockoutEnabled,
                                  Banned = u.Banned,
                                  BannEndDate = u.BannEndDate,
                                  Rola = new RolaVM { Name = r.Name, Id = r.Id },
                              };

            model.Users = usersFromDb.ToList();

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AddUserToRole(string userId, string role)
        {
            UserManager.AddToRole(userId, role);

            var model = new MenageRolesVM();
            var usersFromDb = from u in dbContext.Users
                              from ur in u.Roles.DefaultIfEmpty()
                              join ro in dbContext.Roles.DefaultIfEmpty() on ur.RoleId equals ro.Id into newTable
                              from r in newTable.DefaultIfEmpty()
                              select new UserVM
                              {
                                  Id = u.Id,
                                  Name = u.UserName,
                                  LockoutEnabled = u.LockoutEnabled,
                                  Rola = new RolaVM { Name = r.Name, Id = r.Id },
                              };

            model.Users = usersFromDb.ToList();

            return View("ManageRoles", model);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult RemoveUserFromRole(string userId, string role)
        {
            UserManager.RemoveFromRole(userId, role);

            var model = new MenageRolesVM();
            var usersFromDb = from u in dbContext.Users
                              from ur in u.Roles.DefaultIfEmpty()
                              join ro in dbContext.Roles.DefaultIfEmpty() on ur.RoleId equals ro.Id into newTable
                              from r in newTable.DefaultIfEmpty()
                              select new UserVM
                              {
                                  Id = u.Id,
                                  Name = u.UserName,
                                  LockoutEnabled = u.LockoutEnabled,
                                  Rola = new RolaVM { Name = r.Name, Id = r.Id },
                              };

            model.Users = usersFromDb.ToList();

            return View("ManageRoles", model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult LockoutUser(string userId, bool lockout)
        {
            var result = UserManager.SetLockoutEnabled(userId, lockout);

            var model = new MenageRolesVM();
            var usersFromDb = from u in dbContext.Users
                              from ur in u.Roles.DefaultIfEmpty()
                              join ro in dbContext.Roles.DefaultIfEmpty() on ur.RoleId equals ro.Id into newTable
                              from r in newTable.DefaultIfEmpty()
                              select new UserVM
                              {
                                  Id = u.Id,
                                  Name = u.UserName,
                                  LockoutEnabled = u.LockoutEnabled,
                                  Rola = new RolaVM { Name = r.Name, Id = r.Id },
                              };

            model.Users = usersFromDb.ToList();

            return View("ManageRoles", model);
        }

        //[Authorize(Roles = "Admin")]
        public ActionResult EditCompanyInformation()
        {
            using (var db = new ApplicationDbContext())
            {
                var entity = db.CompanyInformationEntities.FirstOrDefault();
                var vm = Mapper.Map<CompanyInformationVM>(entity);

                return View(vm);
            }
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditCompanyInformation(CompanyInformationVM companyVM)
        {
            if (!ModelState.IsValid)
            {
                return View(companyVM);
            }

            using (var db = new ApplicationDbContext())
            {
                var ent = Mapper.Map<CompanyInformationEntity>(companyVM);

                var entFromDB = db.CompanyInformationEntities.FirstOrDefault();

                if (entFromDB == null)
                    db.CompanyInformationEntities.Add(ent);
                else
                    db.Entry(entFromDB).CurrentValues.SetValues(ent);

                db.SaveChanges();
            }

            return RedirectToAction("Index","Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

#region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion

        #region Manage Products
        [HttpGet]
        public ActionResult EditProductAttribute(int productAttributeId)
        {
            var entity = db.ProductAttributes.Find(productAttributeId);
            var attrVm = Mapper.Map<ProductAttributeVM>(entity);

            if(attrVm == null)
            {
                return RedirectToAction("AddProductAttribute",new { productAttributeId });
            }

            var vm = Mapper.Map<AddProductAttributeVM>(attrVm);
            vm.CurrentProduct = Mapper.Map<ProductVM>(attrVm.Product);
            vm.AttributeType = attrVm.CategoryAttribute.AttributeType;

            vm.AllCategories = Mapper.Map<ICollection<CategoryVM>>(db.Categories.ToList());

            return View(vm);
        }

        [HttpPost]
        public ActionResult EditProductAttribute(AddProductAttributeVM attr)
        {
            var attrVM = Mapper.Map<CategoryAttributeVM>(attr);

            var productAttribute = db.ProductAttributes.Find(attr.ProductAttributeId);
            productAttribute.Value = attr.Value;
            productAttribute.ComboboxValues = Mapper.Map<ICollection<AttributeValueListEntity>>(attr.ComboboxValues);

            if (ModelState.IsValid)
            {
                db.ProductAttributes.Attach(productAttribute);
                db.Entry(productAttribute).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                return View(attr);
            }

            return RedirectToAction("ProductsAttributes");
        }

        [HttpGet]
        public ActionResult AddProductAttribute(int? productId = null)
        {
            var vm = new AddProductAttributeVM();

            if (productId != null)
            {
                var product = Mapper.Map<ProductVM>(db.Products.Find(productId));
                vm.CurrentProduct = product;
                vm.ProductOfAttributeId = productId;
            }

            vm.CategoryAttributeId = db.Products.Find(productId)?.CatId;

            vm.AllCategories = Mapper.Map<ICollection<CategoryVM>>(db.Categories.ToList());

            return View(vm);
        }

        [HttpPost]
        public ActionResult AddProductAttribute(AddProductAttributeVM attr)
        {
            //var cateoryAttrEntity = db.CategoryAttributes.Find(attr.PKAttributeId);
            //var cateoryAttrVM = Mapper.Map<CategoryAttributesVM>(cateoryAttrEntity);
            //cateoryAttrVM.Value = 

            var productEntity = db.Products.Find(attr.CurrentProduct.ProductId);

            ProductAttributeVM pa = new ProductAttributeVM();


            //add new ListId
            int listId = 0;
            if (attr.ComboboxValues?.Count > 0)
            {
                listId = db.ComboboxValues.OrderByDescending(x => x.ListId).Count();
                listId++;

                foreach (var item in attr.ComboboxValues)
                {
                    item.ListId = listId;
                }
            }

            pa.FK_CategoryAttributes = attr.FK_CategoryAttributes;

            if (!string.IsNullOrEmpty(attr.Value))
            {
                pa.Value = attr.Value;
            }
            else if(attr.ComboboxValues?.Count > 0)
            {
                pa.ComboboxValues = attr.ComboboxValues;
            }

            var attrEntity = Mapper.Map<ProductAttributeEntity>(pa);

            productEntity.Attributes.Add(attrEntity);

            //db.ProductAttributes.Add(attrEntity);
            //product.Attributes.Add(pa);

            //var productEntity = Mapper.Map<ProductEntity>(product);
            //db.Products.Add(productEntity);
            db.Entry(productEntity).State = System.Data.Entity.EntityState.Modified;
            db.Entry(attrEntity).State = System.Data.Entity.EntityState.Added;

            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult CreateCategoryAttribute(int categoryId)
        {
            var vm = new AddCategoryAttributeVM();
            vm.Category = Mapper.Map<CategoryVM>(db.Categories.Find(categoryId));

            return View(vm);
        }

        [HttpPost]
        public ActionResult CreateProductAttribute(AddProductAttributeVM attr)
        {
            if (ModelState.IsValid)
            {
                //add new ListId
                int listId = 0;
                if (attr.ComboboxValues?.Count > 0)
                {
                    listId = db.ComboboxValues.OrderByDescending(x => x.ListId).Count();
                    listId++;

                    foreach (var item in attr.ComboboxValues)
                    {
                        item.ListId = listId;
                        item.FK_CategoryAttrId = attr.CategoryAttributeId;
                        item.FK_ProductAttrId = null;
                    }
                }

                //TO DO: add attributes and bind it with caetegories etc
                var vm = new AddProductAttributeVM();
                var attrVM = Mapper.Map<CategoryAttributeVM>(attr);
                var entity = Mapper.Map<CategoryAttributeEntity>(attrVM);
                db.CategoryAttributes.Add(entity);
                db.Entry(entity).State = EntityState.Added;

                db.SaveChanges();
            }
            else
            {
                return View(attr);
            }

            return RedirectToAction("ProductsAttributes", "Home", null);
        }

        [HttpPost]
        public ActionResult CreateCategoryAttribute(AddCategoryAttributeVM attr)
        {
            if(ModelState.IsValid)
            {
                var attrVM = Mapper.Map<CategoryAttributeVM>(attr);

                var entity = Mapper.Map<CategoryAttributeEntity>(attrVM);
                db.CategoryAttributes.Add(entity);
                db.Entry(entity).State = EntityState.Added;

                db.SaveChanges();
            }

            return RedirectToAction("ProductsAttributes", "Home", null);
        }

        [HttpGet]
        public ActionResult DeleteProductAttribute(int id)
        {
            var entity = db.ProductAttributes.Find(id);
            db.ProductAttributes.Remove(entity);
            db.Entry(entity).State = EntityState.Deleted;
            db.SaveChanges();

            return RedirectToAction("ProductsAttributes", "Home");
        }

        [HttpGet]
        public ActionResult EditAllProductsAttributes(int id)
        {
            var entity = db.CategoryAttributes.Find(id);
            db.CategoryAttributes.Remove(entity);
            db.Entry(entity).State = EntityState.Deleted;
            db.SaveChanges();

            return RedirectToAction("ProductsAttributes");
        }

        //Category
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

                var parentCategory = db.Categories.Find(category.ParentId);

                if (parentCategory != null)
                {
                    var exist = parentCategory.Categories.Any(c => c.Name == cat.Category.Name);

                    if (exist)
                    {
                        ModelState.AddModelError("error_msg", "Current category exist in the parent category");
                        var cats = db.Categories.ToList();
                        cat.AllCategories = Mapper.Map<List<CategoryVM>>(cats);
                        return View(cat);
                    }
                }
                db.Categories.Add(category);
                db.SaveChanges();

                ViewBag.Message = "Category Added succefully";
            }
            AddCategoryVM acVM = new AddCategoryVM();

            return View(acVM);
        }

        [HttpGet]
        public ActionResult DeleteCategory(int categoryId)
        {
            var entity = db.Categories.Find(categoryId);
            db.Categories.Remove(entity);
            db.Entry(entity).State = EntityState.Deleted;

            db.SaveChanges();

            return RedirectToAction("Index","Home");
        }

        //Product

        public ActionResult AddProduct()
        {
            ViewBag.Message = "Add product page.";
            AddProductVM acVM = new AddProductVM();
            var cats = db.Categories.ToList();
            acVM.AllCategories = Mapper.Map<List<CategoryVM>>(cats);
            acVM.AllProducts = Mapper.Map<ICollection<ProductVM>>(db.Products.ToList());


            acVM.CurrentProduct = new ProductVM();
            acVM.CurrentProduct.Pictures = new List<PictureVM>();

            //fill blank pictures
            for(int i = 0; i< 4; i++)
            {
                acVM.CurrentProduct.Pictures.Add(new PictureVM());
            }

            return View(acVM);
        }

        [HttpPost]
        public ActionResult AddProduct(AddProductVM product)
        {
            for(int i=0; i< product.CurrentProduct.Pictures.Count; i++)
            {
                var item = product.CurrentProduct.Pictures[i];
                if (string.IsNullOrEmpty(item.Path))
                { 
                    product.CurrentProduct.Pictures.Remove(item);
                    i--;
                }
            }

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
            var product = Mapper.Map<ProductVM>(entity);

            var vmAddProduct = new AddProductVM();
            vmAddProduct.CurrentProduct = product;

            return View(vmAddProduct);
        }

        [HttpPost]
        public ActionResult EditProduct(AddProductVM product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            var entity = Mapper.Map<ProductEntity>(product.CurrentProduct);
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

        [HttpPost]
        public JsonResult Upload()
        {
            string pathUrl = string.Empty;

            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                var ext =  Path.GetExtension(file.FileName).ToLower();
                if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
                {
                    var fileName = Path.GetFileName(file.FileName);
                    //pathUrl = Path.Combine("..\\Storage", fileName);
                    //var savefileName = Path.GetFileName(file.FileName);

                    var savePath = Path.Combine(Server.MapPath("~/Storage"), file.FileName);
                    var savefileName = CheckIffILExist(savePath);
                    file.SaveAs(savefileName);
                    //post.FilePath = savefileName;
                    //post.ReferenceUrl = string.Empty;
                    
                    Picture pic = new Picture();
                    pic.FileName = Path.GetFileName(savefileName);
                    pic.PathRelative = Url.Content("~/Storage/" + pic.FileName);

                    return Json(pic, JsonRequestBehavior.AllowGet);
                }
            }

            return null;
        }

        private string CheckIffILExist(string fullPath)
        {
            int count = 1;

            string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            string extension = Path.GetExtension(fullPath);
            string path = Path.GetDirectoryName(fullPath);
            string newFullPath = fullPath;

            while (System.IO.File.Exists(newFullPath))
            {
                string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                newFullPath = Path.Combine(path, tempFileName + extension);
            }

            return newFullPath;
        }
        #endregion ManageProducts
    }
}