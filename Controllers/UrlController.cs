using System;
using System.Threading.Tasks;
using LongUrl.Core;
using LongUrl.Models;
using Microsoft.AspNetCore.Mvc;

namespace LongUrl.Controllers
{
    [Route("api/2.0")]
    [ApiController]
    public class UrlController : ControllerBase
    {
        [HttpGet]
        [Produces("application/json")]
        [ActionName("Request")]
        public async Task<ResponseUrl> RequestData(string url, bool antivirus = false)
        {
            try
            {
                return await Task.Run(async () =>
                {
                    var requestUrl = new RequestUrl(url, antivirus);
                    var longUri = new LongUri(requestUrl);
                    var responseUrl = await longUri.Get();
                    return responseUrl;
                });
            }
            catch (Exception)
            {
                var responseUrl = new ResponseUrl();
                return responseUrl;
            }
        }
    }
}