using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlockIt_ShareAndCharge
{
    [FunctionOutput]
    class FctLibraryAddress
    {
        public long Index { get; set; }

        [Parameter("address", "", 1)]
        public string Address { get; set; }
    }
}
