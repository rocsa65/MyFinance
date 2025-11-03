using Microsoft.AspNetCore.Mvc;
using MyFinance.Infrastructure.Data;

namespace MyFinance.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly FinanceDbContext _context;

        public HealthController(FinanceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                // Check database connectivity
                await _context.Database.CanConnectAsync();

                return Ok(new
                {
                    status = "healthy",
                    timestamp = DateTime.UtcNow,
                    version = "1.0.0",
                    database = "connected"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(503, new
                {
                    status = "unhealthy",
                    timestamp = DateTime.UtcNow,
                    version = "1.0.0",
                    database = "disconnected",
                    error = ex.Message
                });
            }
        }
    }
}