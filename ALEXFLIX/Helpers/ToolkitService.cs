using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using ALEXFLIX.Extensions;


namespace ALEXFLIX.Helpers
{
    public class ToolkitService
    {
        public static String SerializeJsonObject(Object objecto)
        {
            return JsonConvert.SerializeObject(objecto);
        }
        
        public static T DeserializeJsonObject<T>(String json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static bool CompararArrayBytes(byte[] a , byte[] b)
        {
            if(a.Length != b.Length)
            {
                return false;
            }
            for(int i = 0; i< a.Length; i++){
                if(!a[i].Equals(b[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static String VerificarEdadUser(DateTime fechaNac)
        {
            String rol = "";
            if (fechaNac.AddYears(18) > DateTime.Now) {
                rol = "KID";
            }else
            {
                rol = "USER";
            }
            return rol;
        }

        public static String GenerarNumeroSecreto()
        {
            Random random = new Random();
            String numSecret = "";

            for (int i = 0; i < 10; i++)
            {
                int aleat = random.Next(0, 10);
                numSecret += aleat;
            }
            return numSecret;
        }

        public static void GuardarAcctionController(AuthorizationFilterContext context)
        {
            //RECUPERAR LA DIRECCION A DONDE QUEREMOS IR
            String action = context.RouteData.Values["action"].ToString();
            String controller = context.RouteData.Values["controller"].ToString();

            //RECUPERAR TEMPDATA PROVIDER
            ITempDataProvider provider = (ITempDataProvider)
                context.HttpContext.RequestServices.GetService(typeof(ITempDataProvider));

            //RECUPERAR EL TEMPDATA
            var TempData = provider.LoadTempData(context.HttpContext);

            //GUARDAMOS LOS DATOS ACTION Y CONTROLLER
            TempData["action"] = action;
            TempData["controller"] = controller;

            //GUARDAR TEMPDATA PARA PODER LLEGAR AL CONTROLLER
            provider.SaveTempData(context.HttpContext, TempData);
        }

        public static RedirectToRouteResult GetRedirectToRoute(String controller, String action)
        {
            RouteValueDictionary ruta = new RouteValueDictionary(new
            {
                controller = controller,
                action = action
            });
            RedirectToRouteResult redirect = new RedirectToRouteResult(ruta);
            return redirect;
        }
    }
}
