using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LongUrl.Models;
using LongUrl.Core;
using LongUrl.ViewModels;

namespace LongUrl.Controllers
{
    public class HomeController : Controller
    {

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IndexViewModel data)
        {
            ResponseUrl responseUrl = await GetLong(data);
            data.OutAntivirusMessage = responseUrl.AntivirusMessage;
            data.OutAntivirusStatus = responseUrl.AntivirusStatus;
            data.OutSuccess = responseUrl.Success;
            data.OutUrl = responseUrl.Url;
            data.OutSafeBrowsing = responseUrl.SafeBrowsing;
            return View(data);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [NonAction]
        private async Task<ResponseUrl> GetLong(IndexViewModel request)
        {
            try
            {
                return await Task.Run(async () =>
                {
                    RequestUrl requestUrl = new RequestUrl(request);
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
