using System.Text.Json;
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

            // Workaround: serialize to string and return a ContentResult so the
            // System.Text.Json output formatter (which uses PipeWriter) is not invoked.
            var json = JsonSerializer.Serialize(accounts, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return Content(json, "application/json");
        }

        [HttpPost]
        public ActionResult<AccountDto> Create([FromBody] CreateAccountDto dto)
        {
            var account = _accountService.Create(dto.Name, dto.Identifier, dto.Currency, dto.Balance);

            // Serialize and return a ContentResult with 201 to avoid the System.Text.Json
            // output formatter path that triggers the PipeWriter error in the test host.
            var json = JsonSerializer.Serialize(account, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Optionally set a Location header pointing to the collection endpoint
            var location = Url.Action(nameof(GetAll));
            if (!string.IsNullOrEmpty(location))
            {
                Response.Headers.Location = location;
            }

            return new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = StatusCodes.Status201Created
            };
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
