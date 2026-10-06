using WebAPI_simple.Data;
using WebAPI_simple.Models.Domain;

namespace WebAPI_simple.Repositories
{
    public class LocalImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _dbContext;

        public LocalImageRepository(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, AppDbContext dbContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        public Image Upload(Image image)
        {
            var imagesFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Images");
            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }

            var localFilePath = Path.Combine(imagesFolder, $"{image.FileName}{image.FileExtension}");

            using var stream = new FileStream(localFilePath, FileMode.Create);
            image.File.CopyTo(stream);

            var request = _httpContextAccessor.HttpContext?.Request;
            var urlFilePath = $"{request?.Scheme}://{request?.Host}{request?.PathBase}/Images/{image.FileName}{image.FileExtension}";
            image.FilePath = urlFilePath;

            _dbContext.Images.Add(image);
            _dbContext.SaveChanges();

            return image;
        }

        public List<Image> GetAllInfoImages()
        {
            return _dbContext.Images.ToList();
        }

        public (byte[]?, string?, string?) DownloadFile(int Id)
        {
            var image = _dbContext.Images.FirstOrDefault(x => x.Id == Id);
            if (image == null) return (null, null, null);

            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", $"{image.FileName}{image.FileExtension}");
            if (!File.Exists(path)) return (null, null, null);

            var bytes = File.ReadAllBytes(path);
            return (bytes, "application/octet-stream", $"{image.FileName}{image.FileExtension}");
        }
    }
}