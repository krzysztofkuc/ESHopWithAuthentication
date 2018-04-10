namespace PlusAndComment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AAA : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Articles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        Header = c.String(),
                        ShortDesc = c.String(),
                        LongDesc = c.String(),
                        AbsThumbPath = c.String(),
                        RelThumbPath = c.String(),
                        AddedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ArticleVMs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        Header = c.String(),
                        ShortDesc = c.String(),
                        LongDesc = c.String(),
                        AbstThumbPath = c.String(),
                        RelThumbPath = c.String(),
                        AddedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PostEntity",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Header = c.String(),
                        FilePath = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                        PostEntity_ID = c.Int(),
                        PlusAmount = c.Int(nullable: false),
                        MinusAmount = c.Int(nullable: false),
                        ShortDescription = c.String(),
                        LongDescription = c.String(),
                        ReferenceUrl = c.String(),
                        Url = c.String(),
                        Removed = c.Boolean(nullable: false),
                        EmbedUrl = c.String(),
                        PostType = c.String(),
                        NeedAge = c.Boolean(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.PostEntity", t => t.PostEntity_ID)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.PostEntity_ID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserProfileSettingsId = c.Int(),
                        Banned = c.Boolean(nullable: false),
                        RegisterDate = c.DateTime(nullable: false),
                        BannEndDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfileSettings", t => t.UserProfileSettingsId)
                .Index(t => t.UserProfileSettingsId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.UserProfileSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShowNeedAgePics = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.UserPosts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdUser = c.String(maxLength: 128),
                        IdPost = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PostEntity", t => t.IdPost, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.IdUser)
                .Index(t => t.IdUser)
                .Index(t => t.IdPost);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserPosts", "IdUser", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserPosts", "IdPost", "dbo.PostEntity");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.PostEntity", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "UserProfileSettingsId", "dbo.UserProfileSettings");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PostEntity", "PostEntity_ID", "dbo.PostEntity");
            DropIndex("dbo.UserPosts", new[] { "IdPost" });
            DropIndex("dbo.UserPosts", new[] { "IdUser" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "UserProfileSettingsId" });
            DropIndex("dbo.PostEntity", new[] { "PostEntity_ID" });
            DropIndex("dbo.PostEntity", new[] { "ApplicationUser_Id" });
            DropTable("dbo.UserPosts");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.UserProfileSettings");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.PostEntity");
            DropTable("dbo.ArticleVMs");
            DropTable("dbo.Articles");
        }
    }
}
