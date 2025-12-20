using Microsoft.AspNetCore.Identity;

namespace Api.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; }
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}