using Infastructure.Entities.Identity;
using Core.Models.Users;

namespace Core.Interfaces
{
    public interface IUsersService
    {
        IQueryable<RoleEntity> Roles { get; }
        Task<UserSearchResultViewModel> GetUsersAsync(UserSearchViewModel search);
        Task EditUserAsync(UserEditViewModel model);
    }
}
