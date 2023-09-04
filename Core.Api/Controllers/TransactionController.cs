using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Data.Abstract;
using Core.Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using Core.Business.ViewModels.Transaction;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController : BaseController
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(
            ITransactionService transactionService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _transactionService = transactionService;
        }

        [HttpGet("GetListHistoryTransaction")]
        public IActionResult GetListHistoryTransaction(string keySearch, int actionId, int month, int year, int? pageNum, int? pageSize)
        {
            var currentUserId = GetCurrentUserId();
            return _transactionService.GetListHistoryTransaction(keySearch ,currentUserId, actionId, month, year, pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        [HttpPost("CreateTransaction")]
        public async Task<IActionResult> CreateTransaction([FromForm] TransactionViewModelCreate model)
        {
            var currentUserId = GetCurrentUserId();
            return await _transactionService.CreateTransaction(model, currentUserId);
        }

        [HttpGet("GetAllTransactionAction")]
        public IActionResult GetAllTransactionAction()
        {
            var currentUserId = GetCurrentUserId();
            return _transactionService.GetAllTransactionAction(currentUserId);
        }

        [HttpGet("GetTransactionPending")]
        public IActionResult GetTransactionPending()
        {
            var currentUserId = GetCurrentUserId();
            return _transactionService.GetTransactionPending(currentUserId);
        }
    }
}
