using PlusAndComment.Common.CustomAttributes;
using System.Web;
using System.Web.Mvc;

namespace PlusAndComment
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new AjaxAuthorizeAttribute());
        }
    }
}
