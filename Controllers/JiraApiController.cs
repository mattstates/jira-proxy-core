using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JiraProxyCore.Controllers
{
    [ApiController]
    [Route("/plugins/servlet/applinks/proxy/")]
    //[EnableCors(JiraProxyOriginPolicy)]
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

            return Ok(result);
        }

        public async Task<string> GetJiraData(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _settings.Value.ApiCredentials);
            HttpResponseMessage response;
            using (var client = _clientFactory.CreateClient())
            {
                response = await client.SendAsync(request);
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
