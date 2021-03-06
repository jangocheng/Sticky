﻿using Microsoft.AspNetCore.Mvc;
using Sticky.Repositories.Advertisement;
using Sticky.Repositories.Common;
using System;
using System.Threading.Tasks;
using System.Web;

namespace Sticky.API.Advertisement.Controller
{
    [Produces("application/json")]
    public class ClickController : ControllerBase
    {
        private readonly IEncodeDecodeManager _encodeDecodeManager;
        private readonly IClickLogger _clickLogger;
        public ClickController(IEncodeDecodeManager encodeDecodeManager,IClickLogger clickLogger)
        {
            _clickLogger = clickLogger;
            _encodeDecodeManager = encodeDecodeManager;


        }
        [HttpGet]
        [Route("Click")]
        public async  Task<IActionResult> Click(string landing, int segmentId,string uadid)
        {
            var uriBuilder = new UriBuilder(new Uri(_encodeDecodeManager.Base64Decode(landing)).AbsoluteUri);
            var query = HttpUtility.ParseQueryString(HttpContext.Request.QueryString.ToString());
            query.Remove("extraQuery");
            query.Remove("landing");
            query.Remove("uadid");
            query.Remove("stpd");
            var campaign = query.Get("utm_campaign")??"";
            var referer = query.Get("utm_content")??"";
            uriBuilder.Query = query.ToString();
            var finalLanding = uriBuilder.ToString();
            await _clickLogger.IncreaseClick(segmentId+"%"+campaign+"%"+referer,string.IsNullOrEmpty(uadid)?_encodeDecodeManager.Base64Encode(segmentId+"$$$NoTemplate"):uadid);
            return Redirect(finalLanding);

        }
    }
}