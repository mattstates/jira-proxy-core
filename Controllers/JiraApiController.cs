using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using System.Net.Http;
using System.Net.Http.Headers;

namespace jira_proxy_core.Controllers
{
    [ApiController]
    [Route("/plugins/servlet/applinks/proxy/")]
    [EnableCors("Jira-Proxy-Origins-Policy")]
    public class JiraApiController : ControllerBase
    {
        private readonly ILogger<JiraApiController> _logger;
        private IOptions<AppConfig> _settings;
        private readonly IHttpClientFactory _clientFactory;

        public JiraApiController(ILogger<JiraApiController> logger, IOptions<AppConfig> settings, IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _settings = settings;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var queryCollection = HttpContext.Request.Query;

            // Remove Path Key and App ID
            var filteredKeys = queryCollection.Keys.ToList().Where(q => (q != _settings.Value.AppIdQueryParam && q != _settings.Value.PathQueryParam)).ToList();
            var searchUrl = $"{queryCollection[_settings.Value.PathQueryParam]}&{String.Join("&", filteredKeys.Select(q => $"{q}={queryCollection[q]}").ToList())}";

            var result = await GetJiraData(searchUrl);
            var content = await result.Content.ReadAsStringAsync();
            
            return Ok(content);
        }

        public async Task<HttpResponseMessage> GetJiraData(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _settings.Value.ApiCredentials);

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            return response;
        }
    }
}
