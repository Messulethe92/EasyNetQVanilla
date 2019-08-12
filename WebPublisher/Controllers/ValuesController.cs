using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebPublisher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        PublishRabbitMQHostedService publishRabbitMQHostedService;

        public ValuesController(PublishRabbitMQHostedService publishRabbitMQHostedService)
        {
            this.publishRabbitMQHostedService = publishRabbitMQHostedService;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/someMessage
        [HttpGet("{message}")]
        public ActionResult<Task> Get(string message)
        {
            return publishRabbitMQHostedService.SendMessage(new Messages.TextMessage() { Text = message });
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
