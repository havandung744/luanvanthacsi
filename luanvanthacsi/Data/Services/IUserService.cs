using luanvanthacsi.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace luanvanthacsi.Data.Services
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(string id);
    }
}
