using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SlockIt_ShareAndCharge
{
    class LogRentedEvent
    {

        //public long Index { get; set; }

        [Parameter("bytes32", "poleID", 1, true)]
        public byte[] PoleID { get; set; }

        [Parameter("address", "controller", 2, false)]
        public string Controller { get; set; }

        [Parameter("uint256", "wattPower", 3, false)]
        public BigInteger WattPower { get; set; }

        [Parameter("uint256", "hoursToRent", 4, false)]
        public BigInteger HoursToRe { get; set; }
    }
}
