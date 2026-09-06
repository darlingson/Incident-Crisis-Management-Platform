using Api.Data.Interfaces;
using Api.DTOs.Reports;
using Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IApplicationDbContext _context;

        public UserRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserSelectionDto>> GetAssignableUsersAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Select(u => new UserSelectionDto
                {
                    Id = u.Id,
                    FullName = u.FirstName + " " + u.LastName,
                    Email = u.Email!
                })
                .ToListAsync();
        }
    }
}
