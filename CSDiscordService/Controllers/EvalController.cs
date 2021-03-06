﻿using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using CSDiscordService.Eval;
using Microsoft.Extensions.Logging;
using CSDiscordService.Infrastructure;

namespace CSDiscordService.Controllers
{
    [Authorize(AuthenticationSchemes = "Token")]
    [Route("[controller]")]
    public class EvalController : Controller
    {
        private CSharpEval _eval;
        private TelemetryClient _telemetryClient;
        private ILogger<EvalController> _logger;

        public EvalController(CSharpEval eval, TelemetryClient telemetryClient, ILogger<EvalController> logger)
        {
            _eval = eval;
            _telemetryClient = telemetryClient;
            _logger = logger;
        }

        [HttpPost]
        [Produces("application/json")]
        [Consumes("text/plain")]
        public async Task<IActionResult> Post([FromBody] string code)
        {
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            var result = await _eval.RunEvalAsync(code);

            result.TrackResult(_telemetryClient, _logger);

            if (string.IsNullOrWhiteSpace(result.Exception))
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}
