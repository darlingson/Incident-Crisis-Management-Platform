using Microsoft.AspNetCore.Authorization;

namespace Api.Attributes
{
    public class PermissionAttribute : AuthorizeAttribute
    {
        public PermissionAttribute(string permission) : base(policy: permission)
        {
        }
    }
}