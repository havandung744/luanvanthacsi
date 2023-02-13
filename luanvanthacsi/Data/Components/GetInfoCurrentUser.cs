using Microsoft.AspNetCore.Http;
using System.Security.Claims;
namespace luanvanthacsi.Data.Components
{
    public class GetInfoCurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetInfoCurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            var currentUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value; 
        }
    }
}
