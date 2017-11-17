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
        private static DocumentClient secondClient = Settings.GetDocumentClient(DbType.Second); 
        private ItemService _service;

        public ValuesController() {
            this._service = new ItemService();
        }

        [HttpGet]
        public IEnumerable<Item> Get() =>  _service.Get(primaryClient, secondClient);

        [HttpGet]
        [Route("detailed")]
        public IEnumerable<dynamic> GetDetailed() =>  _service.GetDetailed(primaryClient, secondClient);

        [HttpPost]
        public async Task<ActionResult> Post([FromBody]Item value, [FromQuery] int location = 0)
        {
            var client = primaryClient;
            if (location == 1){
                client = secondClient;
            }

            var result = await _service.Insert(client, value);

            if (result.Saved)
                return Ok(result.Message); 
            else 
                return BadRequest(result.Message);
        }
    }
}
