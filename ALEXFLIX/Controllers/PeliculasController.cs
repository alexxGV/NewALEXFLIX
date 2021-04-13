using ALEXFLIX.Extensions;
using ALEXFLIX.Filters;
using ALEXFLIX.Helpers;
using NuGetAlexflixModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ALEXFLIX.Services;

namespace ALEXFLIX.Controllers
{
    public class PeliculasController : Controller
    {
        ServiceAlexflixApi service;
        PathProvider pathProvider;
        UploadService UploadService;
        ServiceStorageBlobs ServiceBlobs;

        public PeliculasController(ServiceAlexflixApi service, PathProvider pathProvider,
            UploadService UploadService, ServiceStorageBlobs serviceBlobs)
        {
            this.service = service;
            this.pathProvider = pathProvider;
            this.UploadService = UploadService;
            this.ServiceBlobs = serviceBlobs;
        }

        //[ResponseCache(Duration = 180)]
        public async Task<IActionResult> Details(int idPelicula)
        {
            Pelicula pelicula = await this.service.GetPeliculaId(idPelicula);
            String genero = await this.service.GetGeneroNombreId(pelicula.IdGenero);
            List<Actor> actores = await this.service.GetActoresPeliculaId(pelicula.IdPelicula);

            //SACO LOS COMENTARIOS Y EL USUARIO QUE LO CREO
            List<ComentarioUser> comentariosUsers = await this.service.GetComentariosUsersIdPelicula(idPelicula);


            VistaPelicula vistaPelicula = new VistaPelicula();
            vistaPelicula.Pelicula = pelicula;
            vistaPelicula.Genero = genero;
            vistaPelicula.Actores = actores;
            vistaPelicula.ComentariosUsers = comentariosUsers;

            User usuario = HttpContext.Session.GetObject<User>("usuario");
            if (usuario != null)
            {
                ViewData["FAVORITA"] = await this.service.SaberFavorita(usuario.IdUsuario, idPelicula);
            }

            return View(vistaPelicula);
        }

        public async Task<IActionResult> Favorito(int idPelicula, int idUsuario, int favorito)
        {
            if (favorito == 1)
            {
                await this.service.QuitarFavoritaPelicula(idUsuario, idPelicula);
            }
            else
            {
                await this.service.PonerFavoritaPelicula(idUsuario, idPelicula);
            }
            return RedirectToAction("Details", new { IdPelicula = idPelicula });

        }

        [HttpPost]
        public async Task<IActionResult> Comentario(int idpelicula, int idusuario, String comentario, int valoracion)
        {
            await this.service.InsertComentario(idpelicula, idusuario, comentario, valoracion);
            return RedirectToAction("Details", new { IdPelicula = idpelicula });
        }

        //PARTIAL APARTADO DINAMICO
        public IActionResult GetComentariosPartial(int id)
        {
            Pelicula pelicula = new Pelicula();
            pelicula.IdPelicula = 1;
            ViewData["IdPelicula"] = id;
            return PartialView("_ComentariosPartial", pelicula);
        }

        [AuthorizeUsers]
        public async Task<IActionResult> List()
        {
            List<Pelicula> peliculas = await this.service.GetPeliculas();
            return View(peliculas);
        }

        [AuthorizeUsers]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create
            (String titulo, int idgenero, String sinopsis, int valoracion, int duracion,
            IFormFile fileImagen, IFormFile fileVideo)
        {

            //await this.UploadService.UploadFileAsync(fileImagen, Folders.Images);
            //await this.UploadService.UploadFileAsync(fileVideo, Folders.Videos);

            String blobImagenName = fileImagen.FileName;
            using (var stream = fileImagen.OpenReadStream())
            {
                await this.ServiceBlobs.UploadBlobAsync(Folders.Images, blobImagenName, stream);
            }

            String blobVideoName = fileVideo.FileName;
            using (var stream = fileVideo.OpenReadStream())
            {
                await this.ServiceBlobs.UploadBlobAsync(Folders.Videos, blobVideoName, stream);
            }

            await this.service.InsertPelicula(titulo, idgenero, sinopsis, valoracion, duracion, fileImagen.FileName, fileVideo.FileName);
            return RedirectToAction("List");
        }

        public async Task<IActionResult> Favoritas()
        {
            User usuario = HttpContext.Session.GetObject<User>("usuario");
            //ViewData["USUARIO"] = await this.service.GetUserId(usuario.IdUsuario);
            ViewData["USUARIO"] = usuario;
            List<Pelicula> peliculas = await this.service.GetPeliculasIdUsuario(usuario.IdUsuario);
            if (peliculas == null)
            {
                ViewData["MENSAJE"] = "No tiene peliculas favoritas, debe añadir las que más le gusten";
                return View();
            }
            return View(peliculas);
        }

        public async Task<IActionResult> Index()
        {
            ////Si esta logeado el usuario crear la sesion
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                if (HttpContext.Session.GetObject<User>("usuario") == null)
                {
                    User user = await this.service.GetUserUsername(HttpContext.User.Identity.Name);
                    HttpContext.Session.SetObject("usuario", user);
                }
            }
            List<Pelicula> peliculas = await this.service.GetPeliculas();
            return View(peliculas);
        }

        public async Task<IActionResult> PeliculasGenero(int idGenero)
        {
            List<Pelicula> peliculas = await this.service.GetPeliculasGenero(idGenero);
            ViewData["GENERO"] = await this.service.GetGeneroNombreId(idGenero);
            return View(peliculas);
        }

    }
}


