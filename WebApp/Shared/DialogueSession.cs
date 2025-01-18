using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BlazorApp.Shared
{
    public class DialogueSession
    {
        [Key]
        public Guid id { get; set; }
        public DateTime date { get; set; }
        public int Score { get; set; }
		[ForeignKey("Username")]
		public virtual User User { get; set; }
        public string Username { get; set; }
        public string DialogueTitle { get; set; }
        public int MaxScore { get; set; }
    }
}
