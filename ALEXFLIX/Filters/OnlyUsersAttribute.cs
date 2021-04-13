using ALEXFLIX.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALEXFLIX.Filters
{
    public class OnlyUsersAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user.Identity.IsAuthenticated == false)
            {
                //REDIRECCION DINAMICA
                ToolkitService.GuardarAcctionController(context);
                //LOGIN
                context.Result = ToolkitService.GetRedirectToRoute("Identity", "Login");
            }
        }
    }
}
