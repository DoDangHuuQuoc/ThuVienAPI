using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebAPI.Models.DTO
{
    public class ImageUploadRequestDTO
    {
        [Required]
        public IFormFile File { get; set; }

        [Required]
        public string FileName { get; set; }

        public string? FileDescription { get; set; }

        // Hàm kiểm tra định dạng và kích thước file
        public bool ValidateFileUpload(ModelStateDictionary modelState)
        {
            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(File.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                modelState.AddModelError("file", "Định dạng file không hỗ trợ! Chỉ chấp nhận .jpg, .jpeg, .png");
            }

            if (File.Length > 10485760) // 10MB
            {
                modelState.AddModelError("file", "Dung lượng file vượt quá giới hạn 10MB.");
            }

            return modelState.IsValid;
        }
    }
}