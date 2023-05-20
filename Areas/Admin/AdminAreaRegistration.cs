using EP3_ICE_CREAM.Areas.Admin.Controllers;
using System.Diagnostics.Eventing.Reader;
using System.Web.Mvc;

namespace EP3_ICE_CREAM.Areas.Admin
{
    
    public class AdminAreaRegistration : AreaRegistration 
    {

        private Admin.Controllers.AdminController admin = new AdminController();
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            context.MapRoute(
                              "Admin_default",
                              "Admin/{controller}/{action}/{id}",
                              new { action = "Index", id = UrlParameter.Optional }
                          );



        }
    }
}