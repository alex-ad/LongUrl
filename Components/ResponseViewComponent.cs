﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LongUrl.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LongUrl.Components
{
    public class ResponseViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IndexViewModel request)
        {
            return View(request);
        }
    }
}
