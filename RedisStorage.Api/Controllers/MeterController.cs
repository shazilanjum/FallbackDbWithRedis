using Microsoft.AspNetCore.Mvc;
using RedisStorage.Core.Abstractions;
using RedisStorage.Core.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedisStorage.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeterController : ControllerBase
    {
        public readonly IRedisService _redisService;

        public MeterController(IRedisService redisService) 
        {
            _redisService = redisService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(await _redisService.GetMetersAsync());
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Meter meter)
        {
            try
            {
                await _redisService.UpdateMeterAsync(meter);
                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put(int length)
        {
            try
            {
                await _redisService.SeedData(length);
                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

    }
}
