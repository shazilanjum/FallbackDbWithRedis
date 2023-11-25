using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisStorage.Core.Entities
{
    public class Meter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public string TimeSchedule { get; set; }
        public DateTime ExecutionDatetime { get; set; }
        public string MSN { get; set; }
        public string SerialNumber { get; set; }
    }
}
