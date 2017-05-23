using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Galary.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected virtual CustomPrincipal CurrentUser { get { return HttpContext.Current.User as CustomPrincipal; } }

        public bool IsAuthorized { get; set; }


        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            if(filterContext.HttpContext.Request.IsAuthenticated)
            {
                //if(!string.IsNullOrEmpty(Users))
                //{
                //    if(!Users.Contains(CurrentUser.CurrentUserName))
                //    {
                //        filterContext.Result = new RedirectToRouteResult(
                //            new RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
                //    }
                //}
                if (CurrentUser != null)
                {
                    if (!CurrentUser.IsInRole("1") && !(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "Catalog" || filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "Authentication"))
                    {
                        filterContext.Result = new RedirectToRouteResult(
                                new RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
                    }
                }
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Authentication", action = "Login" }));
            }
        }
    }
}