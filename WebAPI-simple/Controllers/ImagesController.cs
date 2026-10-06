using Microsoft.AspNetCore.Mvc;
using WebAPI.Models.DTO;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;
using WebAPI_simple.Repositories;

namespace WebAPI_simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        [HttpPost("Upload")]
        public IActionResult Upload([FromForm] ImageUploadRequestDTO request)
        {
            if (!request.ValidateFileUpload(ModelState))
            {
                return BadRequest(ModelState);
            }

            var imageDomainModel = new Image
            {
                File = request.File,
                FileExtension = Path.GetExtension(request.File.FileName),
                FileSizeInBytes = request.File.Length,
                FileName = request.FileName,
                FileDescription = request.FileDescription
            };

            _imageRepository.Upload(imageDomainModel);
            return Ok(imageDomainModel);
        }

        [HttpGet]
        public IActionResult GetAllInfoImages()
        {
            var allImages = _imageRepository.GetAllInfoImages();
            return Ok(allImages);
        }

        [HttpGet("Download")]
        public IActionResult DownloadFile(int id)
        {
            var (bytes, contentType, fileName) = _imageRepository.DownloadFile(id);
            if (bytes == null)
            {
                return NotFound("Không tìm thấy file!");
            }

            return File(bytes, contentType, fileName);
        }
    }
}