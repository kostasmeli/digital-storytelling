using BlazorApp.Shared.DTO;
using System.Net.Http.Json;

namespace BlazorApp.Client.Services.SessionService
{
    public class SessionService : ISessionService
    {
        private readonly HttpClient _http;
        
        public SessionService(HttpClient http)
        {
            _http = http;
        }
        public List<DialogueSession> Sessions { get; set; } = new List<DialogueSession>();

        public DialogueSession Session { get; set; } = new DialogueSession();

        public async Task<string> CreateSession(int Score, string username,string Title,int MaxScore)
        {
            SessionDTO session = new SessionDTO();
            session.id = Guid.NewGuid();
            session.date = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            session.Score = Score;
            session.Username = username;
            session.DialogueTitle = Title;
            session.MaxScore = MaxScore;

			var result = await _http.PostAsJsonAsync("api/Session", session);
            var response = await result.Content.ReadAsStringAsync();
            return response;
        }

        public async Task GetSessions()
        {
            var result = await _http.GetFromJsonAsync<List<DialogueSession>>("api/Session");
            if (result != null)
            {
                Sessions = result;
            }
        }

        public async Task<List<DialogueSession>> GetUserSessions(string username)
        {
            var result = await _http.GetFromJsonAsync<List<DialogueSession>>($"api/Session/{username}");
            if (result != null)
            {
                 return result;
            }
            throw new Exception("No sessions found for this username");
        }
    }
}
