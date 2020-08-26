using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LongUrl.Core;
using LongUrl.Models;
using Microsoft.AspNetCore.Http;
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
                    RequestUrl requestUrl = new RequestUrl(url, antivirus);
                    LongUri longUri = new LongUri(requestUrl);
                    ResponseUrl responseUrl = await longUri.Get();
                    return responseUrl;
                });
            }
            catch (Exception)
            {
                ResponseUrl responseUrl = new ResponseUrl();
                return responseUrl;
            }
        }
    }
}
