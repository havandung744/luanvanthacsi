using luanvanthacsi.Data.Entities;
using Microsoft.AspNetCore.Identity;
using NHibernate;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;

namespace luanvanthacsi.Data.Services
{
    public class UserService : IUserService
    {
        async Task<User> IUserService.GetUserByIdAsync(string id)
        {
            User user;
            using (var session = FluentNHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        user = await session.GetAsync<User>(id);
                        return user;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
