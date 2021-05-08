using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SqGoods.DomainLogic.DataAccess;
using SqGoods.Infrastructure;

namespace SqGoods.Controllers
{
    [ApiController]
    [Route("api/data")]
    public class DataController : ControllerBase
    {
        private readonly IDatabaseManager _databaseManager;

        public DataController(IDatabaseManager databaseManager)
        {
            this._databaseManager = databaseManager;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new FileCallbackResult("application/json", s => this._databaseManager.WriteDbJsonDataToStream(s, 512))
            {
                FileDownloadName = "db_data.json"
            };
        }
    }
}