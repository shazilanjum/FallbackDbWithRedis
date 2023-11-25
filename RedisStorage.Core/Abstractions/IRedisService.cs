using RedisStorage.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisStorage.Core.Abstractions
{
    public interface IRedisService
    {
        Task<List<Meter>> GetMeterAsync();
        Task UpdateMeterAsync(Meter meter);
    }
}
