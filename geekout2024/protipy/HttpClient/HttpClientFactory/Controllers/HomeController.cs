using HttpClientFactory.Services;
using Microsoft.AspNetCore.Mvc;

namespace HttpClientFactory.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IHttpClientFactory httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
            => this.httpClientFactory = httpClientFactory;

        public async Task<string> IndexAsync()
        {
            var httpClientFactoryService = new HttpClientFactoryService(httpClientFactory);

            var todo = await httpClientFactoryService.GetToDo();

            return todo.Title + " " + todo.Completed;
        }
    }
}
