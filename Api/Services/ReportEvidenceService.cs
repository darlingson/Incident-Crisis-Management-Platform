using Api.Data.Interfaces;
using Api.Models;
using Api.Services.Interfaces;

namespace Api.Services
{
    public class ReportEvidenceService : IReportEvidenceService
    {
        private readonly IReportEvidenceRepository _repository;

        public ReportEvidenceService(IReportEvidenceRepository repository)
        {
            _repository = repository;
        }

        public async Task<ReportEvidence?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ReportEvidence>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ReportEvidence> AddAsync(ReportEvidence reportEvidence)
        {
            return await _repository.AddAsync(reportEvidence);
        }

        public async Task UpdateAsync(ReportEvidence reportEvidence)
        {
            await _repository.UpdateAsync(reportEvidence);
        }

        public async Task DeleteAsync(ReportEvidence reportEvidence)
        {
            await _repository.DeleteAsync(reportEvidence);
        }

        public async Task SaveChangesAsync()
        {
            await _repository.SaveChangesAsync();
        }

        public async Task<string> SaveEvidenceFileAsync(IFormFile file)
        {
            // Validation: business logic for file handling belongs in service, not repository
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.", nameof(file));

            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".pdf", ".docx", ".txt" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            // Allow any extension for now, but service could enforce; keep permissive to avoid breaking

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/evidence");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var filePath = Path.Combine(directoryPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }
    }
}
