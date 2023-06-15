using API.Helpers;
using API.interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        //string cloud, string apiKey, string apiSecret
        public PhotoService(IOptions<CloudinarySettings> config){
          var account = new Account(config.Value.CloudName,config.Value.ApiKey,config.Value.ApiSecret);
          _cloudinary = new Cloudinary(account);
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if(file.Length > 0) { // verifica si viene un archivo o no
                using var stream = file.OpenReadStream(); // obtiene los datos del archivo
                var uploadParams = new ImageUploadParams {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    Folder = "da-net7"
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            return await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}