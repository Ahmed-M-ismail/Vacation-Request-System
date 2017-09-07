using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace YouxelVacationRequest.Filters
{
    public class ApprovalPrivilege : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
        
            if ((string) HttpContext.Current.Session["empType"] == "normalEmp" && (string)HttpContext.Current.Session["UserTypeID"] != "Admin")
            {

                filterContext.Result = new RedirectResult("~/Home/Logout");

                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}