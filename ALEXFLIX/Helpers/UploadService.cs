using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ALEXFLIX.Helpers
{
    public class UploadService
    {

        PathProvider PathProvider;

        public UploadService(PathProvider pathprovider)
        {
            this.PathProvider = pathprovider;
        }

        public async Task<String> UploadFileAsync(IFormFile formfile, Folders folder)
        {
            String filename = formfile.FileName;
            //String filename = HelperToolkit.NormalizeName(formfile.FileName);
            String path = this.PathProvider.MapPath(filename, folder);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await formfile.CopyToAsync(stream);
            }
            return path;
        }
    }
}
