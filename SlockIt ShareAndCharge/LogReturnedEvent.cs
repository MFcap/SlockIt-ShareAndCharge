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
    class LogReturnedEvent
    {
        public long Index { get; set; }

        [Parameter("bytes32", "poleID", 1, true)]
        public byte[] PoleID { get; set; }

        [Parameter("uint256", "chargeAmount", 2, false)]
        public BigInteger ChargeAmount { get; set; }

        [Parameter("uint256", "elapsedSeconds", 3, false)]
        public BigInteger ElapsedSeconds { get; set; }

        [Parameter("uint256", "watt", 4, false)]
        public BigInteger Watt { get; set; }

        [Parameter("uint8", "contractType", 5, false)]
        public BigInteger ContractType { get; set; }
    }
}
