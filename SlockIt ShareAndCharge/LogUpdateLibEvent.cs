using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlockIt_ShareAndCharge
{
    [FunctionOutput]
    class LogUpdateLibEvent
    {
        [Parameter("string", "name", 1, false)]
        public string Name { get; set; }

        [Parameter("address", "newAddress", 2, false)]
        public string NewAddress { get; set; }
    }
}
