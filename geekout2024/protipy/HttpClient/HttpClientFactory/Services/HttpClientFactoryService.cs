using HttpClientFactory.Models;
using Newtonsoft.Json;

namespace HttpClientFactory.Services
{
    public class HttpClientFactoryService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public HttpClientFactoryService(IHttpClientFactory httpClientFactory) 
            => this.httpClientFactory = httpClientFactory;

        public async Task<ToDo> GetToDo()
        {
            var httpClient = httpClientFactory.CreateClient("ToDoClient");

            var response = await httpClient.GetAsync("/todos/1");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var todo = JsonConvert.DeserializeObject<ToDo>(content);

            return todo;
        }
    }
}
