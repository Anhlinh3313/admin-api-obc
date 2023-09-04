using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels.Expense;
using Core.Data.Abstract;
using Core.Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpenseController : BaseController
    {
        private readonly IExpenseService _expenseService;

        public ExpenseController(
            IExpenseService expenseService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _expenseService = expenseService;
        }
        // GET: api/<UserController>
        [HttpGet("GetListExpense")]
        public async Task<IActionResult> GetListExpense(string keySearch, int? pageNum, int? pageSize)
        {
            return await _expenseService.GetListExpenseAsync(keySearch, pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        // GET api/<UserController>/5
        [HttpGet("GetDetailExpense")]
        public async Task<IActionResult> GetDetailExpense(int expenseId)
        {
            return await _expenseService.GetDetailExpenseAsync(expenseId);
        }

        // POST api/<UserController>
        [HttpPost("CreateExpense")]
        public async Task<IActionResult> CreateExpense([FromBody]ExpenseViewModel model)
        {
            return await _expenseService.CreateExpenseAsync(model);
        }

        // PUT api/<UserController>/5
        [HttpPost("UpdateExpense")]
        public async Task<IActionResult> UpdateExpense([FromBody] ExpenseViewModel model)
        {
            return await _expenseService.UpdateExpenseAsync(model);
        }

        [HttpGet("DeActiveExpense")]
        public async Task<IActionResult> DeActiveExpense(int expenseId)
        {
            return await _expenseService.DeActiveExpenseAsync(expenseId);
        }

        [HttpGet("DeEnableExpense")]
        public async Task<IActionResult> DeEnableExpense(int expenseId)
        {
            return await _expenseService.DeEnabledExpenseAsync(expenseId);
        }

        [HttpGet("GetAllExpense")]
        public async Task<IActionResult> GetAllExpense()
        {
            return await _expenseService.GetAllExpense();
        }

        [HttpGet("GetAllExpenseNotEnabled")]
        public IActionResult GetAllExpenseNotEnabled()
        {
            return _expenseService.GetAllExpenseNotEnabled();
        }
    }
}
