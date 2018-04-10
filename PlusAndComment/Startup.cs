using Microsoft.Owin;
using Owin;
using PlusAndComment.App_Start;

[assembly: OwinStartupAttribute(typeof(PlusAndComment.Startup))]
namespace PlusAndComment
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesandUsers();
            AutoMapperConfig.Configure();
        }
    }
}
