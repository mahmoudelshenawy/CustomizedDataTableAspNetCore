using CustomizedDataTableAspNetCore.Contracts;
using CustomizedDataTableAspNetCore.Database;
using CustomizedDataTableAspNetCore.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace CustomizedDataTableAspNetCore.Repositories
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UsersViewModel> GetUsersList(UsersViewModel usersViewModel)
        {
            try
            {
                var orderBy = usersViewModel.OrderBy ?? "Id";
                var direction = usersViewModel.Direction ?? "Desc";

                string search = string.Empty;

                if (!string.IsNullOrEmpty(usersViewModel.Search))
                {
                    search = usersViewModel.Search.Trim().ToLower();
                }
                int skip = (usersViewModel.PageNo - 1) * usersViewModel.RowsPerPage; //1-1 * 10 = 0
                int take = usersViewModel.RowsPerPage;

                var query = _context.Users.Where(x =>
                (string.IsNullOrEmpty(search) ||
                x.FirstName.ToLower().Contains(search) ||
                x.LastName.ToLower().Contains(search) ||
                (x.FirstName.ToLower() + " " + x.LastName.ToLower()).Contains(search) ||
                x.Email.ToLower().Contains(search) ||
                x.Address.ToLower().Contains(search) ||
                x.Phone.ToLower().Contains(search)
                ) &&
                (string.IsNullOrEmpty(usersViewModel.Role) ||
                x.Role.ToLower().Contains(usersViewModel.Role.ToLower()))
                ).OrderBy($"{orderBy} {direction}");
                   
                usersViewModel.TotalCount = await query.CountAsync();
                usersViewModel.StartFrom = (usersViewModel.PageNo - 1) * usersViewModel.RowsPerPage;
                if(usersViewModel.ExportAll > 0)
                {
                    usersViewModel._Items = await query.ToListAsync();
                }
                else
                {
                    usersViewModel._Items = await query
                   .Skip(skip)
                   .Take(take)
                   .ToListAsync();
                }
               
            }
            catch (Exception ex)
            {
                //LOG
            }

            return usersViewModel;
        }
    }
}
