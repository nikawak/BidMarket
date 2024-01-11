using CG.Web.MegaApiClient;
namespace BidMarket.Services
{
    public class MegaService : ICloudService
    {
        private readonly string _login;
        private readonly string _password;
        private readonly MegaApiClient _client;
        public MegaService(IConfiguration configuration)
        {
            _login = configuration["MegaService:MegaLogin"];
            _password = configuration["MegaService:MegaPassword"];
            _client = new MegaApiClient();
            _client.Login(_login, _password);
        }
        public async Task<string> UploadImageAsync(IFormFile image, string imageName)
        {
            IEnumerable<INode> nodes = _client.GetNodes();

            INode folder = nodes.Single(x => x.Name == "nikawak");

            INode file = await _client.UploadAsync(image.OpenReadStream(), imageName, folder);
            Uri downloadLink = _client.GetDownloadLink(file);
            var path = downloadLink.OriginalString;

            return path;
        }

        public async Task<byte[]> DownloadImageAsync(string imageName)
        {
            Uri fileLink = new Uri(imageName);
            INode node = _client.GetNodeFromLink(fileLink);

            var stream = await _client.DownloadAsync(node);
            var bytes = new byte[stream.Length];

            stream.Read(bytes);

            return bytes;
        }
    }
}
