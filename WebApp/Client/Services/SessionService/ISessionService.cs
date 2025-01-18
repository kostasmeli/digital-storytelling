using BlazorApp.Shared;

namespace BlazorApp.Client.Services.SessionService
{
    public interface ISessionService
    {
        List<DialogueSession> Sessions { get; set; }

        DialogueSession Session { get; set; }

        Task GetSessions ();

        Task <List<DialogueSession>> GetUserSessions (string username);

        Task<string> CreateSession (int score, string username,string Title,int MaxScore);
    }
}
