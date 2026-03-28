using System;
using System.Web.Mvc;

namespace Round.Filters
{
    public class AuthFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["UserId"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                        { "controller", "Login" },
                        { "action", "Index" }
                    });
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
