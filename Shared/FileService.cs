namespace E_Book.Shared
{
    public class FileService
    {
        private readonly string _imagePath = @"E:\\ESOFT CMPZ\\Projects\\AD\\AD CW 2\\AD CW2 Final Source Code\\";

        public async Task<string> SaveFile(IFormFile file, string[] allowedExtensions)
        {
            //var wwwPath = _environment.WebRootPath;
            var path = Path.Combine(_imagePath, "images");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var extension = Path.GetExtension(file.FileName);
            if (!allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException($"Only {string.Join(",", allowedExtensions)} files allowed");
            }
            string fileName = $"{Guid.NewGuid()}{extension}";
            string fileNameWithPath = Path.Combine(path, fileName);
            using var stream = new FileStream(fileNameWithPath, FileMode.Create);
            await file.CopyToAsync(stream);
            return fileName;
        }

        public void DeleteFile(string fileName)
        {
            var fileNameWithPath = Path.Combine(_imagePath, "images\\", fileName);
            if (!File.Exists(fileNameWithPath))
                throw new FileNotFoundException(fileName);
            File.Delete(fileNameWithPath);

        }

    }
}
