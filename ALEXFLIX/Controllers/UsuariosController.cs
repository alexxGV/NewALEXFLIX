using ALEXFLIX.Data;
using NuGetAlexflixModels;
using ALEXFLIX.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALEXFLIX.Extensions;
using ALEXFLIX.Filters;
using ALEXFLIX.Helpers;

namespace ALEXFLIX.Controllers
{
    public class UsuariosController : Controller
    {
        ServiceAlexflixApi service;
        MailService MailService;

        public UsuariosController(ServiceAlexflixApi service, MailService mailService)
        {
            this.service = service;
            this.MailService = mailService;
        }

        #region ANTES DE CAMBIO DE VALIDACION DE USUARIOS


        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(String username, String nombre, String correo, DateTime fechaNacimiento, String password, String confirm)
        {
            if (!password.Equals(confirm))
            {
                ViewData["MENSAJE"] = "Las contraseñas no coinciden";
                return View();
            }
            else
            {
                if (await this.service.GetUserUsername(username) != null)
                {
                    ViewData["MENSAJE"] = "Este nombre de usuario ya esta en uso, pruebe otro";
                    return View();
                }
                else
                {
                    await this.service.InsertUser(username, nombre, correo, fechaNacimiento, password);

                    //FALTA REVISAR CORREOO CORPORATIVO Y TODO RELACIONADO CON CORREO(api, service, cotroller)

                    this.MailService.EnviarCorreoBienvenida(correo, username);
                    return RedirectToAction("Perfil", "Usuarios");
                }
            }
        }


        public IActionResult Bienvenida()
        {
            User usuario = HttpContext.Session.GetObject<User>("usuario");
            ViewData["usuario"] = usuario;
            return View();
        }

        #endregion

        [OnlyUsers]
        public IActionResult Perfil()
        {
            return View();
        }

        //CUANDO LE DEMOS A CERRAR SESION DEL MENU ABRE UNA CONFIRMACION POR SI QUIERES CERRRAR
        //DENTRO TIENE UN BOTON QUE LLAMA AL METODO CERRARSESION DEL IDENTITY
        public IActionResult LogOut()
        {
            return View();
        }

        [AuthorizeUsers]
        public async Task<IActionResult> ListUsers()
        {
            List<User> users = await this.service.GetUsers();
            return View(users);
        }

        //VERIFICACION DE CORREO ELECTRONICO Y ENVIO DE NUEVO SALT Y CREACION DE NUEVA CONTRASEÑA
        public async Task<IActionResult> RecuperarContrasenia()
        {
            var user = HttpContext.User.Identity.Name;
            User usuario = await this.service.GetUserUsername(user);
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> RecuperarContrasenia(int idusuario, String correo)
        {
            var user = await this.service.GetUserIdCorreo(idusuario, correo);
            if (user == null)
            {
                ViewData["MENSAJE"] = "El correo debe ser el mismo con el que se registro en la pagina";
                var userbien = await this.service.GetUserId(idusuario);
                return View(userbien);
            }
            else
            {
                String numSecret = ToolkitService.GenerarNumeroSecreto();
                HttpContext.Session.SetString("NUMSECRET", numSecret);
                HttpContext.Session.SetInt32("IDUSUARIO", idusuario);
                //TempData["NUMSECRET"] = numSecret;
                //TempData["IDUSER"] = idusuario;

                //FALTA REVISAR CORREOO CORPORATIVO Y TODO RELACIONADO CON CORREO(api, service, cotroller)
                this.MailService.EnviarCorreoValidacion(user.Correo, numSecret, user.Username);
                return RedirectToAction("NuevaContrasenia");
            }
        }

        public IActionResult NuevaContrasenia()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> NuevaContrasenia(String numSecret, String password, String confirm)
        {
            String numData = HttpContext.Session.GetString("NUMSECRET");
            int idUser = Convert.ToInt32(HttpContext.Session.GetInt32("IDUSUARIO"));

            if (!numData.Equals(numSecret))
            {
                ViewData["MENSAJE"] = "El numero secreto debe coincidir con el enviado por correo";
                return View();
            }
            else
            {
                if (!password.Equals(confirm))
                {
                    ViewData["MENSAJE"] = "Las contraseñas no coinciden";
                    return View();
                }
                else
                {
                    await this.service.CambiarContrasenia(idUser, password);
                    HttpContext.Session.Remove("NUMSECRET");
                    HttpContext.Session.Remove("IDUSUARIO");
                    return RedirectToAction("Perfil", "Usuarios");
                }
            }
        }
    }
}
