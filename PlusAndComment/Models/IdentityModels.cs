using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PlusAndComment.Models.Entities;
using PlusAndComment.Models.ViewModel.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PlusAndComment.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [ForeignKey("UserProfileSettingsId")]
        public virtual UserProfileSettings UserProfileSettings { get; set; }
        public int? UserProfileSettingsId { get; set; }

        public bool Banned { get; set; }
        public DateTime RegisterDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? BannEndDate { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            //userIdentity.AddClaim(new Claim("ShowNeedAgePics", this.UserProfileSettings.ShowNeedAgePics.ToString(),ClaimValueTypes.Boolean));


            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer<ApplicationDbContext>(null);
        }

        public virtual DbSet<ArticleEntity> Articles { get; set; }
        public virtual DbSet<UserProfileSettings> UserProfileSettings { get; set; }
        public virtual DbSet<ProductEntity> Products { get; set; }
        public virtual DbSet<CategoryEntity> Categories { get; set; }
        public virtual DbSet<PictureEntity> Pictures { get; set; }
        public virtual DbSet<CartEntity> Carts { get; set; }
        public virtual DbSet<OrderEntity> Orders { get; set; }
        public virtual DbSet<OrderDetailEntity> OrderDetails { get; set; }
    

    //protected override void OnModelCreating(DbModelBuilder modelBuilder)
    //{

    //    modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

    //    modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(r => r.UserId);
    //    modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
    //    modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });
    //}

    public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //public System.Data.Entity.DbSet<PlusAndComment.Models.ViewModel.AddPostVMs.AddArticleVM> ArticleVMs { get; set; }

        //public System.Data.Entity.DbSet<PlusAndComment.Models.ViewModel.AddPostVMs.AddSucharVM> SucharVMs { get; set; }

        //public System.Data.Entity.DbSet<PlusAndComment.Models.ViewModel.AddPostVMs.AddPostVM> AddPostVMs { get; set; }
    }
}