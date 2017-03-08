using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web
{
    public class MyAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["LOGINPWD"] != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            else
            {
                string path = context.HttpContext.Request.Path;
                //string strUrl = "/Home/Login?returnUrl={0}";
                string strUrl = "/User/Login?returnUrl={0}";

                context.HttpContext.Response.Redirect(string.Format(strUrl, HttpUtility.UrlEncode(path)), true);

            }
        }
    }
}