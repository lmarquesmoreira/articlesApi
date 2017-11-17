using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using System.Net;
using ArticlesApi.Model;
using ArticlesApi.Services;

namespace ArticlesApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private static DocumentClient primaryClient = Settings.GetDocumentClient(DbType.Primary);
        private static DocumentClient seccondClient = Settings.GetDocumentClient(DbType.Second); 
        private ItemService _service;

        public ValuesController() {
            this._service = new ItemService();
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<Item> Get()
        {
            return _service.Get(primaryClient, seccondClient);
            // var location = System.Net.Dns.GetHostName();
            // return new string[] { 
            //     primaryClient != null ? "master client": "false 01", 
            //     seccondClient != null ? "seccond client": "false 02", 
            //     $"location is {location}"
            // };
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult> Post([FromBody]Item value, [FromQuery] int location = 0)
        {
            var client = primaryClient;
            if (location == 1){
                client = seccondClient;
            }

            var result = await _service.Insert(client, value);

            if (result.Saved)
                return Ok(result.Message); 
            else 
                return BadRequest(result.Message);
        }
    }
}
