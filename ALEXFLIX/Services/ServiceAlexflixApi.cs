using ALEXFLIX.Helpers;
using Newtonsoft.Json;
using NuGetAlexflixModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ALEXFLIX.Services
{
    public class ServiceAlexflixApi
    {
        private Uri UrlApi;
        private MediaTypeWithQualityHeaderValue Header;

        public ServiceAlexflixApi(String url)
        {
            this.UrlApi = new Uri(url);
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
        }

        private async Task<T> CallApi<T>(String request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = this.UrlApi;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        #region USERS
        public async Task<List<User>> GetUsers()
        {
            String request = "api/Users";
            return await this.CallApi<List<User>>(request);
        }

        public async Task<User> GetUserId(int id)
        {
            String request = "api/Users/" + id;
            return await this.CallApi<User>(request);
        }

        public async Task<User> GetUserUsername(String username)
        {
            String request = "api/Users/GetUserUsername/" + username;
            return await this.CallApi<User>(request);
        }

        public async Task<List<User>> GetListUsersIds(List<int> ids)
        {
            using (HttpClient client = new HttpClient())
            {
                String request = "api/Users/GetListUsersIds";
                client.BaseAddress = this.UrlApi;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                
                String jsonids = JsonConvert.SerializeObject(ids);

                StringContent content = new StringContent(jsonids, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    List<User> data = await response.Content.ReadAsAsync<List<User>>();
                    return data;
                }
                else
                {
                    return default(List<User>);
                }
            }
        }

        public async Task<User> GetUserIdCorreo(int idusuario, String correo)
        {
            String request = "api/Users/GetUserIdCorreo/" + idusuario + "/" + correo;
            return await this.CallApi<User>(request);
        }

        public async Task<int> GetNewIdUsers()
        {
            String request = "api/Users/GetNewIdUsers";
            return await this.CallApi<int>(request);
        }

        public async Task InsertUser(String username, String nombre, String correo, DateTime fechaNacimiento, String password)
        {
            using (HttpClient client = new HttpClient())
            {
                String request = "api/Users";
                client.BaseAddress = this.UrlApi;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                UserInsert user = new UserInsert();
                user.Username = username;
                user.Nombre = nombre;
                user.Correo = correo;
                user.FechaNacimiento = fechaNacimiento;
                user.Password = password;

                String jsonUser = JsonConvert.SerializeObject(user);

                StringContent content = new StringContent(jsonUser, Encoding.UTF8, "application/json");
                await client.PostAsync(request, content);
            }
        }

        public async Task<User> ValidarUsuario(String usuario, String password)
        {
            String resquest = "api/Users/ValidarUsuario/" + usuario + "/" + password;
            return await this.CallApi<User>(resquest);
        }

        public async Task CambiarContrasenia(int idusuario, String password)
        {
            using (HttpClient client = new HttpClient())
            {
                String request = "api/Users/CambiarContrasenia/" + idusuario + "/" + password;
                client.BaseAddress = this.UrlApi;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                await client.PutAsync(request, new StringContent(""));
            }
        }
        #endregion


        #region PELICULAS

        public async Task<List<Pelicula>> GetPeliculas()
        {
            String request = "api/Peliculas";
            List<Pelicula> peliculas = await this.CallApi<List<Pelicula>>(request);
            return peliculas;
        }

        public async Task<Pelicula> GetPeliculaId(int idpelicula)
        {
            String request = "api/Peliculas/" + idpelicula;
            Pelicula pelicula = await this.CallApi<Pelicula>(request);
            return pelicula;
        }

        public async Task<Pelicula> GetPeliculaTitulo(String titulo)
        {
            String request = "/api/Peliculas/GetPeliculaTitulo/" + titulo;
            Pelicula pelicula = await this.CallApi<Pelicula>(request);
            return pelicula;
        }

        public async Task InsertPelicula(String titulo, int genero, String sinopsis, int valoracion, int duracion, String imagen, String video)
        {
            using (HttpClient client = new HttpClient())
            {
                String request = "api/Peliculas";
                client.BaseAddress = this.UrlApi;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                PeliculaInsert pelicula = new PeliculaInsert();
                pelicula.Titulo = titulo;
                pelicula.IdGenero = genero;
                pelicula.Sinopsis = sinopsis;
                pelicula.Valoracion = valoracion;
                pelicula.Duracion = duracion;
                pelicula.Imagen = imagen;
                pelicula.Video = video;
                String jsonpelicula = JsonConvert.SerializeObject(pelicula);

                StringContent content = new StringContent(jsonpelicula, Encoding.UTF8, "application/json");
                await client.PostAsync(request, content);
            }
        }
        public async Task<int> GetNewIdPeliculas()
        {
            String request = "api/Peliculas/GetNewIdPeliculas";
            return await this.CallApi<int>(request);
        }

        //OBTENER LAS PELICULAS FAVORITAS DE LOS USUARIOS
        public async Task<List<Pelicula>> GetPeliculasIdUsuario(int idUsuario)
        {
            String request = "api/Peliculas/GetPeliculasIdUsuario/" + idUsuario;
            return await this.CallApi<List<Pelicula>>(request);
        }

        public async Task PonerFavoritaPelicula(int idUsuario, int idPelicula)
        {
            using (HttpClient client = new HttpClient())
            {
                String request = "api/Peliculas/PonerFavoritaPelicula/" + idUsuario + "/" + idPelicula;
                client.BaseAddress = this.UrlApi;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                await client.PostAsync(request, new StringContent(""));
            }
        }

        public async Task QuitarFavoritaPelicula(int idUsuario, int idPelicula)
        {
            using (HttpClient client = new HttpClient())
            {
                String request = "api/Peliculas/QuitarFavoritaPelicula/" + idUsuario + "/" + idPelicula;
                client.BaseAddress = this.UrlApi;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                await client.DeleteAsync(request);
            }
        }
        public async Task<Boolean> SaberFavorita(int idUsuario, int idPelicula)
        {
            String request = "/api/Peliculas/SaberFavorita/" + idUsuario + "/" + idPelicula;
            return await this.CallApi<Boolean>(request);
        }

        public async Task<int> GetNewIdUP()
        {
            String request = "/api/Peliculas/GetNewIdUP";
            return await this.CallApi<int>(request);
        }

        public async Task<List<Pelicula>> GetPeliculasGenero(int idGenero)
        {
            String request = "api/Peliculas/GetPeliculasGenero/" + idGenero;
            return await this.CallApi<List<Pelicula>>(request);
        }

        #endregion

        #region GENEROS
        public async Task<List<Genero>> GetGeneros()
        {
            String request = "api/Generos";
            return await this.CallApi<List<Genero>>(request);
        }

        public async Task<String> GetGeneroNombreId(int id)
        {
            String request = "api/Generos/" +id;
            return await this.CallApi<String>(request);
        }
        #endregion

        #region COMENTARIOS PELICULAS
        public async Task<List<Comentario>> GetComentarios()
        {
            String request = "api/Comentarios";
            return await this.CallApi<List<Comentario>>(request);
        }

        public async Task<List<Comentario>> GetComentariosIdPelicula(int idPelicula)
        {
            String request = "api/Comentarios/" + idPelicula;
            return await this.CallApi<List<Comentario>>(request);
        }
        public async Task<int> GetNewIdComentarios()
        {
            String request = "api/Comentarios/GetNewIdComentarios";
            return await this.CallApi<int>(request);
        }

        public async Task InsertComentario(int idpelicula, int idusuario, String comentario, int valoracion)
        {
            using (HttpClient client = new HttpClient())
            {
                String request = "api/Comentarios";
                client.BaseAddress = this.UrlApi;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                ComentarioInsert com = new ComentarioInsert();
                com.IdPelicula = idpelicula;
                com.IdUsario = idusuario;
                com.ComentarioTexto = comentario;
                com.Valoracion = valoracion;
                String jsoncomentario = JsonConvert.SerializeObject(com);

                StringContent content = new StringContent(jsoncomentario, Encoding.UTF8, "application/json");
                await client.PostAsync(request, content);
            }
        }



        public async Task<List<ComentarioUser>> GetComentariosUsersIdPelicula(int idPelicula)
        {
            String request = "/api/Comentarios/GetComentariosUsersIdPelicula/" + idPelicula;
            return await this.CallApi<List<ComentarioUser>>(request);
        }
        #endregion

        #region ACTORES
        public async Task<List<Actor>> GetActoresPeliculaId(int id)
        {
            String request = "api/Actores/" + id;
            return await this.CallApi<List<Actor>>(request);
        }
        #endregion
    }
}