using BlazorApp.Shared;

namespace BlazorApp.Client.Services.UserService
{
    public interface IUserService
    {

        List<User> Users { get; set; }
        User User { get; set; }

        Task GetUsers();

        Task GetSingleUser(string username);

        Task<User> CreateUser(User createuser);
    }
}
