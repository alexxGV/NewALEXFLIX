using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ALEXFLIX.Helpers
{
    public enum Folders
    {
        Images = 0, Videos = 1
    }
    public class PathProvider
    {
        IWebHostEnvironment environment;

        public PathProvider(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }

        public String MapPath (String filename, Folders folder)
        {
            String carpeta = "";
            if(folder == Folders.Images)
            {
                carpeta = "images";
            }
            else if(folder == Folders.Videos)
            {
                carpeta = "videos";
            }

            String path = Path.Combine(this.environment.WebRootPath, carpeta, filename);
            return path;
        }
    }
}
