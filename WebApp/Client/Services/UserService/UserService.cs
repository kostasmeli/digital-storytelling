using BlazorApp.Shared;
using System.Net.Http.Json;

namespace BlazorApp.Client.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly HttpClient _http;
        public UserService(HttpClient http)
        { 
            _http = http;
        }

        public List<User> Users { get; set; } = new List<User>();

        public User User { get; set; } = new User();


        public async Task GetSingleUser(string username)
        {
            var result = await _http.GetFromJsonAsync<User>($"api/User/{username}");
            if(result!=null)
            {
                User= result;
            }
            else
            {
                User = null;
            }
        }

        public async Task GetUsers()
        {
            var result = await _http.GetFromJsonAsync<List<User>>("api/User");
            if (result != null)
            {
                Users = result;
            }
            else
            {
                Users = null;
            }
        }

		public async Task<User> CreateUser(User createuser)
		{

            try
            {
                var result = await _http.PostAsJsonAsync("api/User", createuser);
                var response = await result.Content.ReadFromJsonAsync<User>();
                return response;
            }
            catch
            {
                return null;
            }

		}

	}
}
