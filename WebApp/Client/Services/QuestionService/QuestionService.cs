using BlazorApp.Shared;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace BlazorApp.Client.Services.QuestionService
{
    public class QuestionService : IQuestionService
    {

        private readonly HttpClient _http;

        public QuestionService(HttpClient http)
        {
            _http = http;
        }

        public List<Question> questions { get; set; } = new List<Question>();
        public Question question { get; set; } = new Question();

        public async Task CreateQuestion(Question question)
        {
            var result = await _http.PostAsJsonAsync("api/Question", question);
            var response = await result.Content.ReadFromJsonAsync<Question>();
            question = response;
        }

        public async Task<Question> GetQuestionById(int id)
        {
            var result = await _http.GetFromJsonAsync<Question>($"api/Question/{id}");
            if(result != null)
            {
                question = result;
            }
            return null;
        }

        public async Task GetQuestions()
        {
            var result = await _http.GetFromJsonAsync<List<Question>>("api/Question");
            if(result !=null)
            {
                questions = result;
            }
            else
            {
                questions = null;
            }
        }

        public async Task<string> UpdateQuestion(int id,Question updatedQuestion)
        {
            var result = await _http.PutAsJsonAsync($"api/Question", updatedQuestion);
            if(result != null )
            {
                return "Updated";
            }
            else
            {
                return "error";
            }

        }

        public async Task DeleteQuestion(int id)
        {
            var result = await _http.DeleteAsync($"api/Question/{id}");
            var response = await result.Content.ReadFromJsonAsync<List<Question>>();
            questions = response;
        }
    }
}
