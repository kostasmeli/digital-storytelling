using BlazorApp.Shared;
namespace BlazorApp.Client.Services.QuestionService
{
    public interface IQuestionService
    {
        List<Question> questions { get; set; }
        Question question { get; set; }

        Task CreateQuestion(Question question); 

        Task GetQuestions();

        Task <Question> GetQuestionById(int id);

        Task<string> UpdateQuestion(int id, Question updatedQuestion);

        Task DeleteQuestion(int id);
    }
}
