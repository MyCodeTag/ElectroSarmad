using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System.Threading.Tasks;

namespace Sarmad.Services
{
    public class ImageResizer
    {
        public int MaxX { get; set; }

        public int MaxY { get; set; }

        public bool TrimImage { get; set; }

        public ImageResizer(int x, int y)
        {
            MaxX = x;
            MaxY = y;
        }

        public async Task<bool> ResizeAsync(
            string source,
            string target,
            string format = "jpg")
        {
            try
            {
                using Image image = await Image.LoadAsync(source);

                var options = new ResizeOptions
                {
                    Size = new Size(MaxX, MaxY),
                    Mode = TrimImage
                        ? ResizeMode.Crop
                        : ResizeMode.Max
                };

                image.Mutate(x => x.Resize(options));

                switch (format.ToLower())
                {
                    case "webp":

                        await image.SaveAsWebpAsync(
                            target,
                            new WebpEncoder
                            {
                                Quality = 75
                            });

                        break;

                    case "png":

                        await image.SaveAsPngAsync(target);

                        break;

                    default:

                        await image.SaveAsJpegAsync(
                            target,
                            new JpegEncoder
                            {
                                Quality = 90
                            });

                        break;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
