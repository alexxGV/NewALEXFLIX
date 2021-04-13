using ALEXFLIX.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALEXFLIX.Filters
{
    public class AuthorizeUsersAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user.Identity.IsAuthenticated == false)
            {

                //REDIRECCION DINAMICA
                ToolkitService.GuardarAcctionController(context);

                //LOGIN
                //Redirigo al Login
                context.Result = ToolkitService.GetRedirectToRoute("Identity", "Login");
            }
            else
            {
                //SOLO LOS ADMINISTRADORES PUEDEN ENTRAR
                if (user.IsInRole("ADMIN") == false)
                {
                    //context.Result = this.GetRedirectToRoute("Identity", "AccesoDenegado");
                    context.Result = ToolkitService.GetRedirectToRoute("Identity", "AccesoDenegado");
                }
            }
        }

    }
}
