using ALEXFLIX.Extensions;
using NuGetAlexflixModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ALEXFLIX.Services;

namespace ALEXFLIX.Controllers
{
    public class IdentityController : Controller
    {
        ServiceAlexflixApi service;
        public IdentityController(ServiceAlexflixApi service)
        {
        this.service = service;
}

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(String username, String password)
        {
            User user = await this.service.ValidarUsuario(username, password);
            if (user == null)
            {
                ViewData["MENSAJE"] = "Usuario/ Password incorrecto";
                return View();
            }
            else
            {
                await this.CreateClaimsIdentity(password, username, user.Rol);
                HttpContext.Session.SetObject("usuario", user);

                //REDIRECCION DINAMICA
                String action = TempData["action"].ToString();
                String controller = TempData["controller"].ToString();
                
                return RedirectToAction(action, controller);
            }
        }
        private async Task CreateClaimsIdentity(String password, String username, String rol)
        {
            ClaimsIdentity identity = new ClaimsIdentity
                    (CookieAuthenticationDefaults.AuthenticationScheme,
                    ClaimTypes.Name,
                    ClaimTypes.Role);

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, password));
            identity.AddClaim(new Claim(ClaimTypes.Name, username));
            identity.AddClaim(new Claim(ClaimTypes.Role, rol));

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync
                (CookieAuthenticationDefaults.AuthenticationScheme
                , principal
                , new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.Now.AddMinutes(15)
                });
        }

        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Peliculas");
        }
        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}
