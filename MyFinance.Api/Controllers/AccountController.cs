using Microsoft.AspNetCore.Mvc;
using MyFinance.Core.Services;
using MyFinance.Common.Models;
using MyFinance.Api.Dtos;

namespace MyFinance.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<AccountDto>> GetAll()
        {
            var accounts = _accountService.GetAll();
            return Ok(accounts);
        }

        [HttpPost]
        public ActionResult<AccountDto> Create([FromBody] CreateAccountDto dto)
        {
            var account = _accountService.Create(dto.Name, dto.Identifier, dto.Currency, dto.Balance);
            return CreatedAtAction(nameof(GetAll), new { id = account.Id }, account);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var deleted = _accountService.Delete(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
