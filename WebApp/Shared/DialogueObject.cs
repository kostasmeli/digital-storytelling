using System.ComponentModel.DataAnnotations;


namespace BlazorApp.Shared
{
    public class DialogueObject
    {
        [Key]
        public string Title { get; set; }
        public string url { get; set; }
        public List<Question> QuestionSet{ get; set; }
    }
}
