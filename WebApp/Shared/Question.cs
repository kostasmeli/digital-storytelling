
namespace BlazorApp.Shared
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<Answer> Answers { get; set; }
        public int CorrectAnswerId { get; set; }
        public bool isActive { get; set; }
    }
}
