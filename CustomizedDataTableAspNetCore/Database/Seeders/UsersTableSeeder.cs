using Bogus;
using CustomizedDataTableAspNetCore.Database.Entities;

namespace CustomizedDataTableAspNetCore.Database.Seeders
{
    public class UsersTableSeeder
    {
        public static string GetRole()
        {
            var roles = new string[] { "Admin", "Writer", "Subscriber", "Accountant", "Visitor" };

            Random random = new Random();
            int index = random.Next(roles.Length);

            return roles[index];
        }
        public static List<User> GenerateUsers()
        {
            var users = new List<User>();
            int userId = 1;
            var userFactory = new Faker<User>()
                .RuleFor(x => x.Id, _ => userId++)
                .RuleFor(x => x.FirstName, f => f.Name.FirstName())
                .RuleFor(x => x.LastName, f => f.Name.LastName())
                .RuleFor(x => x.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(x => x.Password, _ => "123456")
                .RuleFor(x => x.Address, f => f.Address.FullAddress())
                .RuleFor(x => x.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(x => x.Role , f => GetRole());

            users = userFactory.Generate(1000);
            return users;
        }
    }
}
