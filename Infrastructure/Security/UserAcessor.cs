using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Security
{
    public class UserAcessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserAcessor(IHttpContextAccessor httpContextAccessor) 
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string GetUserName()
        {
            return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }
    }
}
