﻿using System;
using System.Threading.Tasks;
using LongUrl.Core;
using LongUrl.Data;
using LongUrl.Models;
using Microsoft.AspNetCore.Mvc;

namespace LongUrl.Controllers
{
    /// <summary>
    /// API Controller
    /// </summary>
    [Route("api/2.0")]
    [ApiController]
    public class UrlController : ControllerBase
    {
	    private readonly ITokensRepository _tokensRepository;

        public UrlController(ITokensRepository token)
        {
	        _tokensRepository = token;

        }
        /// <summary>
        /// Action-method for processing incoming data (i.e. short URL) via WebAPI
        /// </summary>
        /// <param name="url">Input {get} param: ShortURL for decoding</param>
        /// <param name="antivirus">Input {get} param: whether antivirus is On, either Off</param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [ActionName("Request")]
        public async Task<ResponseUrl> RequestData(string token, string url, bool antivirus = false)
        {
            if (!_tokensRepository.IsTokenValid(token))
            {
                return new ResponseUrl();
            }

            try
            {
                return await Task.Run(async () =>
                {
                    var requestUrl = new RequestUrl(url, antivirus);    // forming RequestUrl Model
                    var longUri = new LongUri(requestUrl);
                    var responseUrl = await longUri.Get();              // decoding ShortURL
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