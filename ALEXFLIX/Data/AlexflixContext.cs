using NuGetAlexflixModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALEXFLIX.Data
{
    public class AlexflixContext : DbContext
    {
        public AlexflixContext(DbContextOptions<AlexflixContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Actor> Actores { get; set; }
        public DbSet<PeliculasActoresUnion> PeliculasActoresUnion { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<UsuariosPeliculas> UsuariosPeliculas { get; set; }
    }
}
