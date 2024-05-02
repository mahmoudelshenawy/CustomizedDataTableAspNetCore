using CustomizedDataTableAspNetCore.Models;

namespace CustomizedDataTableAspNetCore.Contracts
{
    public interface IUserService
    {
        Task<UsersViewModel> GetUsersList(UsersViewModel usersViewModel);
    }
}
