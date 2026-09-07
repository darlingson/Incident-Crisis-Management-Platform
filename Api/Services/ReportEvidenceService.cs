using Api.Data.Interfaces;
using Api.Models;
using Api.Services.Interfaces;

namespace Api.Services
{
    public class ReportEvidenceService : IReportEvidenceService
    {
        private readonly IReportEvidenceRepository _repository;
        private readonly IFileStorageService _fileStorage;

        public ReportEvidenceService(IReportEvidenceRepository repository, IFileStorageService fileStorage)
        {
            _repository = repository;
            _fileStorage = fileStorage;
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
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.", nameof(file));

            return await _fileStorage.SaveFileAsync(file, "evidence");
        }
    }
}
