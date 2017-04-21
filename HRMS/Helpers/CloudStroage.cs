using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace HRMS.Helpers
{
    public class CloudStroage
    {
        public string UploadImage(string filepath, string filename)
        {
            //connect to the cloudinary account
            var cloudinary = new Cloudinary(
                new Account(
                  "dzui75cgk",
                  "918668849717983",
                  "EHaERK60zJ-M39zlas9cgXzMW6A"
                )
            );

            //create the imageUpload object
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(filepath),
                PublicId = filename,
            };

            //upload the image to the cloud
            var uploadResult = cloudinary.Upload(uploadParams);

            //return the URL of the stored image
            return uploadResult.Uri.ToString();
        }

        public string UploadFile(string filepath, string filename)
        {
            var cloudinary = new Cloudinary(
                new Account(
                  "dzui75cgk",
                  "918668849717983",
                  "EHaERK60zJ-M39zlas9cgXzMW6A"
                )
            );

            var uploadParams = new RawUploadParams()
            {
                File = new FileDescription(filepath),
                PublicId = filename
            };

            var uploadResult = cloudinary.Upload(uploadParams);

            return uploadResult.Uri.ToString();
        }
    }
}