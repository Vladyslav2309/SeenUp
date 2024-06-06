using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Webp;

namespace Core.Helpers
{
    public static class ImageWorker
    {
        public static async Task<string> SaveImage(IFormFile image, IConfiguration configuration)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                await image.CopyToAsync(ms);
                var fileName = Path.GetRandomFileName() + ".webp";
                string[] imageSizes = ((string)configuration.GetValue<string>("ImageSizes")).Split(" ");
                await Save(ms.ToArray(), fileName, imageSizes);
                return fileName;
            }
        }

        public static async Task<string> SaveImage(string url, IConfiguration configuration)
        {
            var fileName = Path.GetRandomFileName() + ".webp";
            using (HttpClient client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    string[] imageSizes = ((string)configuration.GetValue<string>("ImageSizes")).Split(" ");
                    //список байт фото
                    byte[] bytes = response.Content.ReadAsByteArrayAsync().Result;
                    await Save(bytes, fileName, imageSizes);
                }
            }
            return fileName;
        }

        private static async Task Save(byte[] bytes, string fileName, string[] sizes)
        {
            foreach (var imageSize in sizes)
            {
                int size = int.Parse(imageSize);
                string dirSaveImage = Path.Combine(Directory.GetCurrentDirectory(), "images", $"{size}x{size}_{fileName}");
                //var saveImage = await SaveBytesCompresAsync(bytes, size, size);
                using (var image = Image.Load(bytes))
                {
                    image.Mutate(x =>
                    {
                        x.Resize(new ResizeOptions
                        {
                            Size = new Size(size, size),
                            Mode = ResizeMode.Max
                        });
                    });

                    using (var stream = File.Create(dirSaveImage))
                    {
                        await image.SaveAsync(stream, new WebpEncoder());
                    }
                }
            }
        }


        public static void DeleteAllImages(string fileName, IConfiguration configuration)
        {
            try
            {
                string[] imageSizes = ((string)configuration.GetValue<string>("ImageSizes")).Split(" ");
                foreach (var imageSize in imageSizes)
                {
                    int size = int.Parse(imageSize);
                    string dirRemoveImage = Path.Combine(Directory.GetCurrentDirectory(), "images", $"{size}x{size}_{fileName}");
                    System.IO.File.Delete(dirRemoveImage);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
