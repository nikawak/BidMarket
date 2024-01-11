namespace BidMarket.Services
{
    public interface ICloudService
    {
        public Task<string> UploadImageAsync(IFormFile file, string name);
        public Task<byte[]> DownloadImageAsync(string imageUrl);
    }
}