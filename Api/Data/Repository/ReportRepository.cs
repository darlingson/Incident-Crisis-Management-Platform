namespace Api.Data.Repository
{
    using Api.Data.Interfaces;
    using Api.Models;
    using Api.Data;
    using Microsoft.EntityFrameworkCore;
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Report>> GetAllAsync(){
            return await _context.Reports.ToListAsync();
        }
        public async Task<Report?> GetByIdAsync(int id){
            return await _context.Reports.FindAsync(id);
        }
        public async Task<Report> AddAsync(Report report){
            var NewReport = await _context.Reports.AddAsync(report);
            return NewReport.Entity;
        }
        public  Task SaveChangesAsync(){
            return _context.SaveChangesAsync();
        }
    }
}