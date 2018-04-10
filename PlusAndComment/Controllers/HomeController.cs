using HtmlAgilityPack;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PlusAndComment.Common;
using PlusAndComment.Common.CustomAttributes;
using PlusAndComment.Models;
using PlusAndComment.Models.AddPostVMs;
using PlusAndComment.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static PlusAndComment.Common.Enums;

namespace PlusAndComment.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext dbContext = new ApplicationDbContext();

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index(int page = 1, int pageSize = 10)
        { 
            var posts = dbContext.MainMems.OrderByDescending( m => m.ID).Where(m => !m.PostEntity.Removed).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            HomeVM homeVm = new HomeVM();
            homeVm.CurrentPage = page;
            homeVm.PostsCapacity = dbContext.MainMems.OrderByDescending(m => m.ID).Where(m => !m.PostEntity.Removed).Count();
            homeVm.MainMems = AutoMapper.Mapper.Map<ICollection<MainMems>,ICollection<MainMemVM>>(posts);

            //var sim = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();


            if (User.Identity.IsAuthenticated)
            {
                var currentUser = dbContext.Users.Find(User.Identity.GetUserId());

                if (currentUser != null)
                {
                    homeVm.ShowNeedAgePics = currentUser.UserProfileSettings == null ? false : currentUser.UserProfileSettings.ShowNeedAgePics;
                }else
                {
                    RedirectToAction("LogOff","Account");
                }
            }

            //homeVm.Posts = JoinPostsWithUsers(homeVm.Posts);

            return View(homeVm);
        }

        [HttpGet]
        public ActionResult MemGenerator()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Suchary()
        {
            var dbSuchary = dbContext.Suchary.OrderByDescending(m => m.Id).ToList();
            var suchary = AutoMapper.Mapper.Map<ICollection<SucharEntity>, ICollection<SucharVM>>(dbSuchary);
            
            var sucharyVM = new SucharyVM();
            sucharyVM.Suchary = suchary.ToList();
            return View(sucharyVM);
        }

        [HttpGet]
        public ActionResult Articles()
        {
            var articles = dbContext.Articles.OrderByDescending(m => m.Id).ToList();

            //var articlesPosts = dbContext.Posts.Where(m => m.PostEntity_ID == null && m.PostType == UIPostType.Article.ToString()).ToList();

            
            ////var posts = dbContext.Posts.OrderByDescending(m => m.ID).Where(m => m.PostEntity_ID == null && !m.Removed && m.CommentParent == UIPostType.mainMeme.ToString()).Skip((page - 1) * pageSize).Take(pageSize).ToList();


            //List<MainPostVM> mappedComments = AutoMapper.Mapper.Map<ICollection<PostEntity>, ICollection<MainPostVM>>(articlesPosts).ToList();

            List<ArticleVM> mapedArt = AutoMapper.Mapper.Map<ICollection<ArticleEntity>, ICollection<ArticleVM>>(articles).ToList();

            //foreach (var article in mapedArt)
            //{
            //    article.Posts = mappedComments.Where(m => m.PostEntity_ID == article.ID).ToList();
            //}



            var articlesVM = new ArticlesVM();
            articlesVM.Articles = mapedArt;
            return View(articlesVM);
        }

        [HttpGet]
        public ActionResult CreateArticle()
        {
            var article  = TempData["article"] as AddArticleVM;
            var articleEnt = AutoMapper.Mapper.Map<AddArticleVM, ArticleEntity>(article);
            articleEnt.AddedTime = DateTime.Now;
            dbContext.Articles.Add(articleEnt);
            dbContext.Entry(articleEnt).State = EntityState.Added;
            dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        private ICollection<MainMemVM> JoinPostsWithUsers(ICollection<MainMemVM> posts)
        {
            foreach (var post in posts)
            {
                post.Post.UsersVotesOnThisPost = dbContext.UserPosts.Where(m => m.IdPost == post.ID).Select(x => (ApplicationUser)x.User).ToList();

                if (post.Post.Posts.Count > 0)
                    JoinPostsWithUsers(post.Post.Posts);
            }

            return posts;
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        [HttpPost]
        [Authorize]
        public ActionResult AddPostLink(AddLinkVM post)
        {
            var postEntity = AutoMapper.Mapper.Map<PostEntity>(post);
            postEntity.ApplicationUser_Id = User.Identity.GetUserId();

            dbContext.Posts.Add(postEntity);
            dbContext.Entry(postEntity).State = EntityState.Added;

            dbContext.SaveChanges();

            var mainMem = AutoMapper.Mapper.Map<PostEntity>(post);


            //ent.PostEntity_ID = parentPost.ID;
            //parentPost.Posts.Add(ent);
            //dbContext.Entry(ent).State = EntityState.Added;

            //dbContext.Posts.Attach(parentPost);
            //dbContext.Entry(parentPost).State = EntityState.Modified;

            MainMems mm = new MainMems();
            mm.PostEntity = postEntity;
            dbContext.MainMems.Add(mm);
            dbContext.Entry(mm).State = EntityState.Added;

            dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddPostPicturefromDisk(AddPictureFromDiskVM post)
        {
            var postEntity = AutoMapper.Mapper.Map<PostEntity>(post);
            postEntity.ApplicationUser_Id = User.Identity.GetUserId();

            dbContext.Posts.Add(postEntity);
            dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddPostHumour(AddHumourVM post)
        {
            var filePath = SaveTextAsImage(post.Content);

            var joke = AutoMapper.Mapper.Map<AddHumourVM, PostEntity>(post);
            joke.FilePath = filePath;

            //joke.AddedTime = DateTime.Now;|

            //id that not done earlier ?
            //joke.ReferenceUrl = SaveTextAsImage(post.Content);

            dbContext.Posts.Add(joke);

            dbContext.SaveChanges();


            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddPostSuchar(AddSucharVM post)
        {
            var sucharEnt = AutoMapper.Mapper.Map<AddSucharVM, SucharEntity>(post);
            sucharEnt.AddedTime = DateTime.Now;

            //id that not done earlier ?
            sucharEnt.RelThumbPath = SaveTextAsImage(post.Content);
            
            dbContext.Suchary.Add(sucharEnt);

            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddPostArticle(AddArticleVM post)
        {
            var articleEnt = AutoMapper.Mapper.Map<AddArticleVM, ArticleEntity>(post);
            articleEnt.AddedTime = DateTime.Now;  
            

            dbContext.Articles.Add(articleEnt);

            dbContext.SaveChanges();

            return RedirectToAction("Articles");
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddPost(AddPostVM post)
        {
            if (!ModelState.IsValid)
            {
                return View(post);
            }

            var postEntity = AutoMapper.Mapper.Map<PostEntity>(post);
            postEntity.ApplicationUser_Id = User.Identity.GetUserId();

            switch (post.Type)
            {
                case UIPostType.MainMeme:
                    if (post.Link.Url != null)
                    {
                        System.Uri uri = new Uri(post.Link.Url);

                        if (!post.Link.Url.Contains("youtube") && !string.IsNullOrWhiteSpace(uri.Query))
                        {
                            post.Link.Url = uri.AbsoluteUri.Replace(uri.Query, string.Empty);
                        }
                    }

                    dbContext.Posts.Add(postEntity);

                    //var newEnt = SavePostLink(post);
                    break;
                case UIPostType.Humour:
                    break;
                case UIPostType.Suchar:
                    break;
                case UIPostType.Article:
                    //dbContext.Articles.Add(postEntity);
                    break;
            }

            //================================================================================================


            //if(post.Tab == "5zakladka")
            //{
            //    TempData["article"] = post.Article;
            //    return RedirectToAction("CreateArticle");
            //}

            //if (p .Url != null)
            //{
            //    System.Uri uri = new Uri(post.Url);

            //    if (!post.Url.Contains("youtube") && !string.IsNullOrWhiteSpace(uri.Query))
            //    {
            //        post.Url = uri.AbsoluteUri.Replace(uri.Query, string.Empty);
            //    }
            //}

            

            
            dbContext.SaveChanges();
            return RedirectToAction("Index", dbContext.Posts.ToList());
        }

        private string SaveTextAsImage(string content)
        {
            int numLines = content.Split('\n').Length;
            int numberOfCharacters = content.Length;
            int fontSize = 35;
            int maxNumChar = 380;
            int maxNumLine = 10;
            int y = 50;
            int x = 70;

            if (numLines < 7)
            {
                fontSize = 40;
            }

            if (numLines > maxNumLine - 2 || numberOfCharacters > maxNumChar)
            {
                fontSize = 30;
                y -= 25;
            }
            if (numLines > maxNumLine || numberOfCharacters > maxNumChar)
            {
                fontSize = 27;
                y -= 20;
            }
            if (numLines > maxNumLine + 4 || numberOfCharacters > maxNumChar + 200)
            {
                fontSize = 25;
            }

            var pathX = Server.MapPath("~/Storage/textBckg.png");
            Image img = Image.FromFile(pathX);
            //Bitmap bmp = new Bitmap(imgX);

            RectangleF rectf = new RectangleF(x, y, 800, 560);

            Graphics g = Graphics.FromImage(img);
            //Graphics gThm = Graphics.FromImage(img);
            //gThm.DrawImage(img,0,0, 270, 180);

            

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawString(content, new Font("Tahoma", fontSize, FontStyle.Bold), Brushes.White, rectf);

            //var gThm = Graphics.FromImage


            g.Flush();

            ImageCodecInfo bmpCodec = GetEncoderInfo("image/png");
            EncoderParameters parameters = new EncoderParameters();
            parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.ColorDepth, 32);
            var newPath = CheckIffILExist(Server.MapPath("~/Storage/") + "joke.png");
            img.Save(newPath, bmpCodec, parameters);

            return Path.Combine("..\\Storage", Path.GetFileName(newPath));
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        [HttpGet]
        [Authorize]
        public ActionResult AddPost()
        {
            return View(Url.Content("~/Views/Home/AddPost/AddPost.cshtml"));
        }

        [HttpPost]
        [AjaxAuthorize]
        public JsonResult AddPlusToPostAsync(int id)
        {
            UserPosts up = new UserPosts();
            var sim = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();

            var post = dbContext.Posts.Find(id);
            var currentUser = sim.UserManager.FindByName(User.Identity.Name);

            var objInUserPosts = dbContext.UserPosts.FirstOrDefault(m => m.IdUser == currentUser.Id && m.IdPost == id);

            if (objInUserPosts != null)
            {
                dbContext.UserPosts.Remove(objInUserPosts);
                dbContext.Entry(objInUserPosts).State = EntityState.Deleted;

                post.PlusAmount--;
                dbContext.SaveChanges();
                Response.AddHeader("IsMinused", "true");

                dynamic result = new { innerHtml = "<i class='glyphicon glyphicon-ok'></i> " + post.PlusAmount, postId = id, isMinused = true, isPlused = false };

                return Json(result, JsonRequestBehavior.AllowGet);
            }

            post.PlusAmount++;
            Response.AddHeader("IsPlused", "true");

            up.IdUser = currentUser.Id;
            up.IdPost = post.ID;

            dbContext.UserPosts.Attach(up);
            dbContext.UserPosts.Add(up);
            dbContext.Entry(up).State = EntityState.Added;

            dbContext.Posts.Attach(post);
            dbContext.Entry(post).State = EntityState.Modified;
            dbContext.SaveChanges();

            dynamic result1 = new { innerHtml = "<i class='glyphicon glyphicon-ok'></i> " + post.PlusAmount, postId = id, isMinused = false, isPlused = true };

            return Json(result1, JsonRequestBehavior.AllowGet);
        }

        [AjaxAuthorize]
        public ActionResult AddMinusToPostAsync(int id)
        {
            var post = dbContext.Posts.Find(id);

            post.MinusAmount++;
            dbContext.Posts.Attach(post);
            dbContext.Entry(post).State = EntityState.Modified;
            dbContext.SaveChangesAsync();
            return Content(post.MinusAmount.ToString());
        }

        [AllowAnonymous]
        public ActionResult LeaveComment(int id, string type, bool isMainComment)
        {
            MainPostVM newPost = new MainPostVM();

            if (isMainComment)
            {
                newPost.Parent_ID = id;
            }

            newPost.PostEntity_ID = id;
            newPost.isMainComment = isMainComment;


            PostEntity parentPost = dbContext.Posts.FirstOrDefault(m => m.ID == id && m.PostType == type);
            
            newPost.CommentParent = parentPost?.CommentParent;
            newPost.PostType = type;

            return PartialView("_LeaveCommentPartial", newPost);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> AddComment(MainPostVM post)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.AppendHeader("objectId", post.ID.ToString());
                return PartialView("_LeaveCommentPartial",post);
            }

            var parentPost = dbContext.Posts.FirstOrDefault(m => m.ID == post.PostEntity_ID && m.PostType == post.PostType);

            var user =  dbContext.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (user != null)
            {
                //parentPost.ApplicationUser_Id = user.Id;
                post.ApplicationUser_Id = user.Id;
            }

            var ent = AutoMapper.Mapper.Map<PostEntity>(post);

            if (post.isMainComment)
            {
                switch(post.PostType)
                {
                    case "Article":
                        ent.Article_ID = post.Parent_ID;
                        post.Parent_ID = null;
                        break;
                    case "MainMeme":
                        ent.MainMems_ID_Comment = post.Parent_ID;

                        //problem when when attached main ameme and comment
                        //ent.PostEntity_ID = null;
                        post.Parent_ID = null;
                        break;

                }
            }
            //else
            //{
            //    switch (post.PostType)
            //    {
            //        case "Article":
            //            ent.Article_ID = post.Parent_ID;
            //            post.Parent_ID = null;
            //            break;
            //        case "MainMeme":
            //            break;

            //    }
            //}

            if (parentPost != null)
            {
                ent.PostEntity_ID = parentPost.ID;
                parentPost.Posts.Add(ent);
                dbContext.Entry(ent).State = EntityState.Added;

                dbContext.Posts.Attach(parentPost);
                dbContext.Entry(parentPost).State = EntityState.Modified;
            }
            else
            {
                dbContext.Posts.Add(ent);
                dbContext.Entry(ent).State = EntityState.Added;
            }

            dbContext.SaveChanges();
            post.ID = ent.ID;

            return PartialView("_ShowCommentPartial", post);
        }

        [ChildActionOnly]
        [AllowAnonymous]
        public ActionResult ShowCommentChild(PostEntity post)
        {

            return PartialView("_ShowCommentPartial", post);
        }

        [AllowAnonymous]
        public ActionResult ShowAllMainPostComments(int id, string type)
        {
            IQueryable post = null;

            switch (type)
            {
                case "Article":
                    var article = dbContext.Articles.Find(id);
                    var articleVM = AutoMapper.Mapper.Map<ArticleVM>(article);
                    return PartialView("_ShowAllCommentsPartial", articleVM as MainPostVM);
                case "MainPost":
                    var mem = dbContext.Articles.Find(id);
                    var memVM = AutoMapper.Mapper.Map<MainPostVM>(mem);
                    return PartialView("_ShowAllCommentsPartial", memVM);

            }
            //post = dbContext.Posts.FirstOrDefault(m => m.ID == id && m.PostType == UIPostType.Article.ToString());

            //var post = dbContext.Posts.Find(id);

            //var postVM = AutoMapper.Mapper.Map<MainPostVM>(post);

            //postVM.Posts = JoinPostsWithUsers(postVM.Posts);

            //how to pass viewdictdata to view
            ViewBag.showAllComments = true;
            //return PartialView("_ShowAllCommentsPartial", postVM);

            return null;
        }

        [HttpPost]
        
        public JsonResult Upload()
        {
            string pathUrl = string.Empty;

            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                var ext = Path.GetExtension(file.FileName);
                if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif")
                {
                    var fileName = Path.GetFileName(file.FileName);
                    //pathUrl = Path.Combine("..\\Storage", fileName);
                    //var savefileName = Path.GetFileName(file.FileName);

                    var savePath = Path.Combine(Server.MapPath("~/Storage"), file.FileName);
                    var savefileName = CheckIffILExist(savePath);
                    file.SaveAs(savefileName);
                    //post.FilePath = savefileName;
                    //post.ReferenceUrl = string.Empty;
                    if(ext == ".jpg"|| ext == ".png" || ext == ".jpeg")
                    {
                        Picture pic = new Picture();
                        pic.FileName = Path.GetFileName(savefileName);
                        pic.PathRelative = Url.Content("~/Storage/" + pic.FileName);

                        return Json(pic, JsonRequestBehavior.AllowGet);
                    }
                    if (ext == ".gif")
                    {
                        Image gifImg = Image.FromStream(file.InputStream);
                        FrameDimension dimension = new FrameDimension(gifImg.FrameDimensionsList[0]);
                        int frameCount = gifImg.GetFrameCount(dimension);
                        var hhh = gifImg.SelectActiveFrame(dimension, 0);

                        //string pngFile = Path.ChangeExtension(savePath, "png");
                        string pngFile = Path.ChangeExtension(savePath, "png");
                        string pngUrl = Path.Combine("/Storage", Path.ChangeExtension(Path.GetFileName(savefileName), "png"));

                        Gif post = new Gif();

                        post.FirstFramePathRelative = Url.Content("~/Storage/") + Path.ChangeExtension(Path.GetFileName(savefileName), "png");
                        post.PathRelative = Url.Content("~/Storage/") + Path.GetFileName(savefileName);

                        post.EmbedUrl = pngUrl;
                        post.Url = pngUrl;

                        var newName = Path.ChangeExtension(savefileName, "png");

                        gifImg.Save(newName, ImageFormat.Png);

                        return Json(post, JsonRequestBehavior.AllowGet);
                    }
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

        [Authorize]
        public ActionResult RemovePost(int id)
        {
            var post = dbContext.Posts.Find(id);
            post.Removed = true;

            dbContext.Entry(post).State = EntityState.Modified;
            dbContext.SaveChanges();

            return null;
        }

        [Authorize]
        public bool ChangeNeedAgeProfileSettings()
        {
            var currentUser = dbContext.Users.Find(User.Identity.GetUserId());

            if (currentUser.UserProfileSettings == null)
            {
                var result = dbContext.UserProfileSettings.Add(new UserProfileSettings { ShowNeedAgePics = false });
                currentUser.UserProfileSettings = new UserProfileSettings { Id = result.Id, ShowNeedAgePics = result.ShowNeedAgePics };

            }
            else
            {
                currentUser.UserProfileSettings.ShowNeedAgePics = !currentUser.UserProfileSettings.ShowNeedAgePics;
            }

            dbContext.Entry(currentUser).State = EntityState.Modified;
            dbContext.SaveChanges();

            return currentUser.UserProfileSettings.ShowNeedAgePics;
        }

        [Authorize]
        public JsonResult ChangeNeedAge(string id)
        {
            var postId = Convert.ToInt16(id.Split('_')[1]);
            var post = dbContext.Posts.Find(postId);
            post.NeedAge = !post.NeedAge;
            dbContext.Entry(post).State = EntityState.Modified;
            dbContext.SaveChanges();

            dynamic result = new ExpandoObject();
            result.id = postId;
            result.needAge = post.NeedAge;

            return Json(result);
        }

        [Authorize]
        public JsonResult FindWebSite(string selectedValue)
        {
            if (selectedValue.Equals(string.Empty))
                return null;

            System.Uri uri = new Uri(selectedValue);

            if (!selectedValue.Contains("youtube") && !string.IsNullOrWhiteSpace(uri.Query))
            {
                selectedValue = uri.AbsoluteUri.Replace(uri.Query, string.Empty);
            }

            if (selectedValue.Contains("vimeo.com"))
            {
                AddLinkVM thumbVideo = new AddLinkVM();
                thumbVideo.Type = PostType.mov.ToFriendlyString();

                thumbVideo.EmbedUrl = ConvertToEmbedVideoUrl(selectedValue);
                thumbVideo.Url = thumbVideo.Url;
                thumbVideo.Header = thumbVideo.Url;

                return Json(thumbVideo, JsonRequestBehavior.AllowGet);
            }

            if (selectedValue.Contains("dailymotion.com"))
            {
                AddPostVM thumbVideo = new AddPostVM();
                thumbVideo.Link.Type = PostType.mov.ToFriendlyString();

                thumbVideo.Link.EmbedUrl = ConvertToEmbedDailyMotionUrl(selectedValue);
                thumbVideo.Link.Url = selectedValue;
                thumbVideo.Link.Header = thumbVideo.Link.Url;

                return Json(thumbVideo, JsonRequestBehavior.AllowGet);
            }

            if (selectedValue.Contains("youtube"))
            {
                AddLinkVM thumbVideo = new AddLinkVM();
                thumbVideo.Type = PostType.mov.ToFriendlyString();

                thumbVideo.EmbedUrl = ConvertToEmbedYouTubeUrl(selectedValue);
                thumbVideo.Url = selectedValue;


                return Json(thumbVideo, JsonRequestBehavior.AllowGet);
            }

            if (selectedValue.Contains("facebook.com") && !selectedValue.ToLower().EndsWith("&theater"))
            {
                AddLinkVM thumbVideo = new AddLinkVM();
                thumbVideo.Type = PostType.mov.ToFriendlyString();

                thumbVideo.EmbedUrl = ConvertToEmbedFacebookUrl(selectedValue);
                thumbVideo.Url = selectedValue;
                thumbVideo.Header = thumbVideo.Url;

                return Json(thumbVideo, JsonRequestBehavior.AllowGet);
            }

            if (selectedValue.ToLower().EndsWith(".jpg") || selectedValue.ToLower().EndsWith(".jpeg") || selectedValue.ToLower().EndsWith(".png"))
            {
                AddLinkVM thumbphoto = new AddLinkVM();
                thumbphoto.Picture = new Picture();
                thumbphoto.Type = PostType.img.ToFriendlyString();

                thumbphoto.Url = selectedValue;
                thumbphoto.Picture.Url= selectedValue;
                thumbphoto.Header = thumbphoto.Url;

                return Json(thumbphoto, JsonRequestBehavior.AllowGet);
            }
            else if (selectedValue.EndsWith(".gif"))
            {
                Gif thumbphoto =  ManageGif(selectedValue);
                thumbphoto.EmbedUrl = selectedValue;
                //thumbphoto.Link.Header = thumbphoto.Link.Url;

                return Json(thumbphoto, JsonRequestBehavior.AllowGet);
            }

            if (!selectedValue.StartsWith("http://") && !selectedValue.StartsWith("https://"))
                return null;

            var document = HttpTools.GetHtmlDocument(selectedValue);

            List<HtmlNode> imgsNodes = document.DocumentNode.SelectNodes("//img").ToList();

            var headerX = document.DocumentNode.SelectNodes("//title").ToList().FirstOrDefault().InnerHtml;


            HtmlNode head = document.DocumentNode.SelectNodes("//head").FirstOrDefault();

            var scripts = head.SelectNodes("//script");

            foreach (var item in scripts)
                item.Remove();

            List<Image> photos = new List<Image>();
            string resultImageName = string.Empty;

            try
            {
                foreach (var item in imgsNodes.Take(5))
                {
                    using (WebClient client = new WebClient())
                    {
                        var src = item.Attributes["src"].Value;

                        byte[] imageBytes = client.DownloadData(src);

                        Image photo = (Bitmap)((new ImageConverter()).ConvertFrom(imageBytes));
                        photo.Tag = selectedValue;
                        photos.Add(photo);
                    }
                }
            }
            catch(Exception exc)
            {
                throw (exc);
            }

            var longestWidth = photos.OrderByDescending(p => p.Width).FirstOrDefault();
            var longestHeight = photos.OrderByDescending(p => p.Height).FirstOrDefault();

            var path = Server.MapPath("~/Storage/Pictures");
            string filePath = string.Empty;

            AddPostVM thumb = new AddPostVM();

            if (longestWidth.Width > longestHeight.Height)
            {
                thumb.Link.Picture.Url= longestHeight.Tag.ToString();
            }
            else
            {
                thumb.Link.Picture.Url = longestWidth.Tag.ToString();
            }

            thumb.Header = headerX;

            return Json(thumb, JsonRequestBehavior.AllowGet);
        }

        private Gif ManageGif(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream stream = httpWebReponse.GetResponseStream();

            var name = url.Substring(url.LastIndexOf("/") + 1).Split('.')[0];
            var folder = "Storage/";
            var absoluteImgPath = Server.MapPath("~/Storage/" + name);

            var newFileWithID = CheckIffILExist(absoluteImgPath + ".gif");
            var fielName = Path.GetFileNameWithoutExtension(newFileWithID);
            var directory = Path.GetDirectoryName(newFileWithID);

            var absolutePathGif = directory + "\\" + fielName + ".gif";
            var absolutePathPng = directory + "\\" + fielName + ".png";
            var newFileNameWithoutExt = Path.GetFileNameWithoutExtension(absolutePathPng);

            using (FileStream fs = System.IO.File.Create(absolutePathGif))
            {
                stream.CopyTo(fs);
                stream.Dispose();
            }

            Image img = null;
            using (var wc = new WebClient())
            {
                img = Image.FromStream(wc.OpenRead(url));
            }

            var pngPath = Path.ChangeExtension(newFileNameWithoutExt, "png");
            img.Save(absolutePathPng);

            Gif gif = new Gif();
            gif.FirstFramePathAbsolute = absolutePathPng;
            gif.FirstFramePathRelative = folder + newFileNameWithoutExt + ".png";
            gif.PathRelative= folder + newFileNameWithoutExt + ".gif";
            gif.FileName = newFileNameWithoutExt + ".gif";
            gif.Url = url;

            return gif;
        }

        private string ConvertToEmbedVideoUrl(string url)
        {
            var uri = new Uri(url);

            return "https://player.vimeo.com/video" + uri.LocalPath;
        }

        private string ConvertToEmbedDailyMotionUrl(string url)
        {
            var uri = new Uri(url);
            var tail = uri.Segments[2];
            var videoId = tail.Split('_')[0];

            return "//"+ uri.Host +"/embed/video/" + videoId;
        }

        string ConvertToEmbedYouTubeUrl(string url)
        {
            var uri = new Uri(url);
            var videoId = HttpUtility.ParseQueryString(uri.Query).Get("v");

            return "https://www.youtube.com/embed/" + videoId;
        }

        string ConvertToEmbedFacebookUrl(string url)
        {
            string baseUrl = "https://www.facebook.com/plugins/video.php?href=https%3A%2F%2Fwww.facebook.com";
            string tail = "&show_text=0&width=560";
            var uri = new Uri(url);
            var videoId = uri.LocalPath;
            var chagedUrl = videoId.Replace("/", "%2F");
            var result = baseUrl + chagedUrl + tail;

            return result;
        }

        [HttpGet]
        public ActionResult SingleMainPost(int id)
        {
            HomeVM homeVm = new HomeVM();
            var post = dbContext.Posts.Find(id);

            var postMapped = AutoMapper.Mapper.Map<MainPostVM>(post);

            return View(postMapped);
        }

        [HttpGet]
        public ActionResult Suchar(int id)
        {
            AddSucharVM homeVm = new AddSucharVM();
            var post = dbContext.Suchary.Find(id);

            var postMapped = AutoMapper.Mapper.Map<AddSucharVM>(post);

            return View(postMapped);
        }

        [HttpPost]
        public JsonResult GetMoreMainPosts(int amount)
        {
            int BlockSize = 5;

            var posts = dbContext.Posts.Where(m => m.Removed == false && m.PostEntity_ID== null).OrderByDescending(m =>m.ID ).Skip(amount).Take(BlockSize).ToList();

            HomeVM homeVm = new HomeVM();
            homeVm.MainMems = AutoMapper.Mapper.Map<ICollection<PostEntity>, ICollection<MainMemVM>>(posts);

            //homeVm.Posts = JoinPostsWithUsers(homeVm.Posts);
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = dbContext.Users.Find(User.Identity.GetUserId());
                homeVm.ShowNeedAgePics = currentUser.UserProfileSettings == null ? false : currentUser.UserProfileSettings.ShowNeedAgePics;
            }


            StringBuilder resultHtml = new StringBuilder();

            foreach (var item in homeVm.MainMems)
            {
                if (item.Post.PostEntity_ID == null && !item.Post.Removed)
                {
                    resultHtml.Append("<div class='row' style='margin-bottom: 20px; background-color: #3C4B53; border-radius: 4px;'>");
                    resultHtml.Append(RenderPartialViewToString("_PostPartial", item, homeVm.ShowNeedAgePics, amount));
                    resultHtml.Append("</div>");
                }
            }

            dynamic result = new ExpandoObject();
            result.html = resultHtml.ToString();
            result.id = amount;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected string RenderPartialViewToString(string viewName, object model, bool showNeedPics, int part)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;
            ViewBag.showNeedAgePics = showNeedPics;
            ViewBag.amountPart = part;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext (ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}