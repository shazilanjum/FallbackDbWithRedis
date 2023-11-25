using Microsoft.EntityFrameworkCore;
using RedisStorage.Core.Abstractions;
using RedisStorage.Core.Entities;
using RedisStorage.Infrastructure.Data;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
#pragma warning disable CS8604,CS8603,CS8619

namespace RedisStorage.Application.Services
{
    public class RedisService : IRedisService
    {
        private readonly FallbackContext _fallbackContext;
        public RedisService(FallbackContext fallbackContext)
        {
            _fallbackContext = fallbackContext;
        }

        public async Task<List<Meter>> GetMeterAsync()
        {
            using ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync("localhost:32768");
            var redisDatabase = redis.GetDatabase();
            //var redisValue = redisDatabase.StringGet($"Meter:{0}");
            //if (redisValue.HasValue)
            //{
            //    return JsonSerializer.Deserialize<Meter>(redisValue);
            //}

            var keys = redis.GetServer(redis.GetEndPoints()[0]).Keys(pattern: "Meter:*");

            if(keys.Any())
            {
                return keys.Select(k=>JsonSerializer.Deserialize<Meter>(redisDatabase.StringGet(k))).ToList();
            }
            

            return await _fallbackContext.Meters.ToListAsync();
        }

        public async Task UpdateMeterAsync(Meter meter)
        {
            using var redis = await ConnectionMultiplexer.ConnectAsync("localhost:32768");
            var redisDatabase = redis.GetDatabase();
            redisDatabase.StringSet($"Meter:{meter.Id}", JsonSerializer.Serialize<Meter>(meter));

            // Update Fallback Database
            
            var existingMeter = await _fallbackContext.Meters.FindAsync(meter.Id);
            if (existingMeter != null)
            {
                existingMeter = meter; // Update existing meter data
            }
            else
            {
                _fallbackContext.Meters.Add(meter); // Add new meter data
            }

            await _fallbackContext.SaveChangesAsync();
            
        }
    }
}
