using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StockNote.BusinessLayer.Interfaces;
using StockNote.Models;
using System.Net;

namespace StockNote.WebAPI.Controller.V0
{
    [ApiVersion("0", Deprecated = true)]
    [ApiController]
    [Route("[controller]")]
    public class StockTransactionController : ControllerBase
    {
        private IStockTransactionManager stockTransactionManager;
        private ILogger<StockTransactionController> logger;
        private readonly AppSettings appSettings;
        public StockTransactionController(IStockTransactionManager _stockTransactionManager, ILogger<StockTransactionController> _logger, IOptions<AppSettings> _stockNoteSetting)
        {
            this.stockTransactionManager = _stockTransactionManager;
            this.logger = _logger;
            this.appSettings = _stockNoteSetting.Value;
        }

        /// <summary>
        /// Create a sale transaction.
        /// After creation is success, new sale transaction ID will be return with HttpStatusCode 201 for Created.
        /// Otherwise, it will return HttpStatusCode 417 for ExpectationFailed.
        /// </summary>
        /// <param name="_stockTransaction"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateSaleTransaction")]
        public async Task<IActionResult> CreateSaleTransaction([FromBody] StockTransaction _saleTransaction)
        {
            Guid saleTransactionID = await stockTransactionManager.CreateSaleTransaction(_saleTransaction);
            if (saleTransactionID == Guid.Empty)
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
            else
                return StatusCode((int)HttpStatusCode.Created, saleTransactionID);
        }

        /// <summary>
        /// Create a stock out transaction.
        /// After creation is success, new stock out transaction ID will be return with HttpStatusCode 201 for Created.
        /// Otherwise, it will return HttpStatusCode 417 for ExpectationFailed.
        /// </summary>
        /// <param name="_stockTransaction"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateStockTransaction")]
        public async Task<IActionResult> CreateStockTransaction([FromBody] StockTransaction _stockTransaction)
        {
            Guid stockTransactionID = await stockTransactionManager.CreateStockTransaction(_stockTransaction);
            if (stockTransactionID == Guid.Empty)
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
            else
                return StatusCode((int)HttpStatusCode.Created, stockTransactionID);
        }

        /// <summary>
        /// Create a amendment transaction.
        /// After creation is success, new amendment transaction ID will be return with HttpStatusCode 201 for Created.
        /// Otherwise, it will return HttpStatusCode 417 for ExpectationFailed.
        /// </summary>
        /// <param name="AmendmentTransaction"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("CreateAmendmentTransaction")]
        public async Task<IActionResult> CreateAmendmentTransaction([FromBody] AmendmentTransaction _amendmentTransaction)
        {
            Guid newStockTransactionID = await stockTransactionManager.AmendmentTransaction(_amendmentTransaction.OldStockTransaction, _amendmentTransaction.NewStockTransaction);
            if (newStockTransactionID == Guid.Empty)
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
            else
                return StatusCode((int)HttpStatusCode.Created, newStockTransactionID);
        }

        /// <summary>
        /// Getting stock transaction history by date.
        /// If success, stock transaction list will be return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return HttpStatusCode 204 for processing success but no content. 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetTransactionHistory")]
        public async Task<IActionResult> GetTransactionHistory([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            List<StockTransaction> stockTransactions = await stockTransactionManager.GetStockTransactionHistory(startDate, endDate);
            if (stockTransactions != null && stockTransactions.Count > 0)
                return StatusCode((int)HttpStatusCode.OK, stockTransactions);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Get stock transaction by ID
        /// </summary>
        /// <param name="_transactionID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetTransactionByID/{_transactionID}")]
        public async Task<IActionResult> GetTransactionByID(Guid _transactionID)
        {
            StockTransaction stockTransaction = await stockTransactionManager.GetStockTransaction(_transactionID);
            if (stockTransaction != null)
                return StatusCode((int)HttpStatusCode.OK, stockTransaction);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Getting stock transaction history by warehouse.
        /// If success, stock transaction list will be return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return HttpStatusCode 204 for processing success but no content.  
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="warehouseID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetTransactionHistoryByWarehouse")]
        public async Task<IActionResult> GetTransactionHistoryByWarehouse([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] Guid warehouseID)
        {
            List<StockTransaction> stockTransactions = await stockTransactionManager.GetStockTransactionHistory(startDate, endDate, warehouseID);
            if (stockTransactions != null && stockTransactions.Count > 0)
                return StatusCode((int)HttpStatusCode.OK, stockTransactions);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }
    }
}
