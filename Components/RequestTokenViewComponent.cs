using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LongUrl.Models;
using Microsoft.AspNetCore.Mvc;

namespace LongUrl.Components
{
	public class RequestTokenViewComponent : ViewComponent
	{
		public IViewComponentResult Invoke(AccessToken request)
		{
			return View(request);
		}
	}
}
