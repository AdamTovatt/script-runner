using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Net;
using System.Security.Principal;

namespace SkippyBackend.Helpers
{
    public class CloudinaryHelper
    {
        public static CloudinaryHelper Instance { get { if (instance == null) instance = new CloudinaryHelper(); return instance; } }
        private static CloudinaryHelper? instance;

        private Cloudinary cloudinary;

        public CloudinaryHelper()
        {
            string cloud = EnvironmentHelper.GetEnvironmentVariable("CLOUDINARY_CLOUD");
            string apiKey = EnvironmentHelper.GetEnvironmentVariable("CLOUDINARY_KEY");
            string secret = EnvironmentHelper.GetEnvironmentVariable("CLOUDINARY_SECRET");
            cloudinary = new Cloudinary(new Account(cloud, apiKey, secret));
        }

        public async Task<string> UploadImageAsync(string name, byte[] bytes)
        {
            using (MemoryStream problemStream = new MemoryStream(bytes))
            {
                ImageUploadParams problemImage = new ImageUploadParams() { PublicId = name };
                problemImage.File = new FileDescription(name, problemStream);
                ImageUploadResult uploadResult = await cloudinary.UploadAsync(problemImage);

                return uploadResult.SecureUrl.ToString();
            }
        }
    }
}
