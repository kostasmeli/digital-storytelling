
namespace BlazorApp.Shared.DTO
{
    public class SessionDTO
    {
        public Guid id { get; set; }
        public DateTime date { get; set; }
        public int Score { get; set; }
        public string Username { get; set; }

        public string DialogueTitle { get; set; }
        public int MaxScore { get; set; }
    }
}
