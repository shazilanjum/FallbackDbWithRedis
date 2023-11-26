using EFCore.BulkExtensions;
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

        public async Task<List<Meter>> GetMetersAsync()
        {
            using ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync("localhost:32768");
            var redisDatabase = redis.GetDatabase();
            //var redisValue = redisDatabase.StringGet($"Meter:{0}");
            //if (redisValue.HasValue)
            //{
            //    return JsonSerializer.Deserialize<Meter>(redisValue);
            //}

            var keys = redis.GetServer(redis.GetEndPoints().First()).Keys(pattern: "Meter:*");

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
            
            var existingMeter = await _fallbackContext.Meters.FindAsync(meter.Id);
            if (existingMeter != null)
            {
                existingMeter = meter;
            }
            else
            {
                _fallbackContext.Meters.Add(meter);
            }

            await _fallbackContext.SaveChangesAsync();
            
        }

        public async Task SeedData(int length)
        {
            var meterData = Enumerable.Range(0, length).Select(s => new Meter
            {
                Id = s,
                ExecutionDatetime = DateTime.Now,
                MSN = $"S{s}",
                Name = $"Test {s}",
                SerialNumber = (100000 + s).ToString(),
                State = "state",
                TimeSchedule = "Now"
            }).ToList();


            //await _fallbackContext.AddRangeAsync(meterData);

           // await _fallbackContext.SaveChangesAsync();

            await _fallbackContext.BulkInsertAsync(meterData);

            using var redis = await ConnectionMultiplexer.ConnectAsync("localhost:32768");
            var redisDatabase = redis.GetDatabase();

            meterData.ForEach(s => redisDatabase.StringSet($"Meter:{s.Id}", JsonSerializer.Serialize(s)));
            //redisDatabase.StringSet($"Meter:{meter.Id}", s);

            //var existingMeter = await _fallbackContext.Meters.FindAsync(meter.Id);
            //if (existingMeter != null)
            //{
            //    existingMeter = meter;
            //}
            //else
            //{
            //    _fallbackContext.Meters.Add(meter);
            //}


        }
    }
}
