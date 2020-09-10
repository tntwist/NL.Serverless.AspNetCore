using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NL.Serverless.AspNetCore.WebApp.ORM;

namespace NL.Serverless.AspNetCore.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ValuesController> _logger;
        private readonly WebAppDbContext _dbContext;

        public ValuesController(
            IConfiguration configuration,
            ILogger<ValuesController> logger,
            WebAppDbContext dbContext
        )
        {
            _configuration = configuration;
            _logger = logger;
            _dbContext = dbContext;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            _logger.LogTrace("TRACE");
            _logger.LogDebug("DEBUG");
            _logger.LogInformation("INFO");
            _logger.LogError("ERROR");
            _logger.LogCritical("CRITICAL");

            var result = _dbContext.ValueEntries
                .Select(v => v.Value)
                .ToList();

            return result;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return _configuration.GetValue<string>("Value");
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
