using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StockNote.BusinessLayer;
using StockNote.BusinessLayer.Interfaces;
using StockNote.Models;
using System.Net;

namespace StockNote.WebAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("V{version:apiVersion}/[controller]s")]
    [ApiController]
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
        [Route("{transactiontype:alpha?}")] // Post v1/stocktransactions
        public async Task<IActionResult> CreateTransaction([FromBody] StockTransaction _transaction, string transactiontype = "")
        {
            Guid transactionID = Guid.Empty;
            if (transactiontype.Trim().ToLower() == "sale")
                transactionID = await stockTransactionManager.CreateSaleTransaction(_transaction);
            else
                transactionID = await stockTransactionManager.CreateStockTransaction(_transaction);

            if (transactionID == Guid.Empty)
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
            else
                return StatusCode((int)HttpStatusCode.Created, transactionID);
        }


        /// <summary>
        /// Create a amendment transaction.
        /// After creation is success, new amendment transaction ID will be return with HttpStatusCode 201 for Created.
        /// Otherwise, it will return HttpStatusCode 417 for ExpectationFailed.
        /// </summary>
        /// <param name="AmendmentTransaction"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")] // Put v1/stocktransactions
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
        [Route("transactions")] // Get v1/stocktransactions/?startDate=1900-01-01&endDate=2000-01-01
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
        [Route("{_transactionID:guid}")] // Get v1/stocktransactions/{_transactionID}
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
        [Route("{warehouseID:guid}/transactions")]
        public async Task<IActionResult> GetTransactionHistoryByWarehouse(Guid warehouseID, [FromQuery] DateTime startDate,  [FromQuery] DateTime endDate)
        {
            List<StockTransaction> stockTransactions = await stockTransactionManager.GetStockTransactionHistory(startDate, endDate, warehouseID );
            if (stockTransactions != null && stockTransactions.Count > 0)
                return StatusCode((int)HttpStatusCode.OK, stockTransactions);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }
    }

    
}
