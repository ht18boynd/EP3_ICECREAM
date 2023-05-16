using System.Web;
using System.Web.Mvc;

namespace EP3_ICE_CREAM
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
