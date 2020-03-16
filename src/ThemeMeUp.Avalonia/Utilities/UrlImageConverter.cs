using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;

namespace ThemeMeUp.Avalonia.Utilities
{
    public class UrlImageConverter
    {
        private readonly HttpClient _httpClient;

        public UrlImageConverter(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Bitmap> UrlToBitmapAsync(string url)
        {
            Bitmap bitmap;
            var byteArray = await _httpClient.GetByteArrayAsync(url);
            using (var ms = new MemoryStream(byteArray))
            {
                bitmap = new Bitmap(ms);
            }
            return bitmap;
        }
    }
}
