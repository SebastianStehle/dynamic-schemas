using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class AppController : Controller
    {
        private readonly GraphQLService service;

        public AppController(GraphQLService service)
        {
            this.service = service;
        }

        public Task Index()
        {
            return Task.CompletedTask;
        }

        [Route("/api/apps/{app}/graphql/{**slug}")]
        public Task GraphQL(string app)
        {
            var basePath = Url.Action(nameof(GraphQL), new { app, slug = (string)null })[..^8];

           return service.ExecuteAsync(HttpContext, app, basePath, "/graphql");
        }
    }
}
