using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SlockIt_ShareAndCharge
{
    [FunctionOutput]
    class LogUpdateGenLibEvent
    {
        [Parameter("uint256", "index", 1, false)]
        public BigInteger Index { get; set; }

        [Parameter("address", "", 2, false)]
        public string Address { get; set; }
    }
}
