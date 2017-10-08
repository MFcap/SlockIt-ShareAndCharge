using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlockIt_ShareAndCharge
{
    class ContractFactory
    {

        private static ContractFactory _instance;
        public static ContractFactory Instance
        {
            get
            {
                if (_instance == null) { _instance = new ContractFactory(); };
                return _instance;
            }
        }


        private LibManager _libManager;
        public LibManager LibManager { get => _libManager; set => _libManager = value; }


        private ContractFactory()
        {
            _libManager = new LibManager();
            _libManager.Address = ConfigurationManager.AppSettings["AddressMain"];
            _libManager.ChargingPoles = ConfigurationManager.AppSettings["AddressCharge"];
            _libManager.Mobility = ConfigurationManager.AppSettings["AddressMobility"];
        }




        public enum ShareAndChargeContract
        {
            LibManager = 1,
            ChargePoles,
            MobilityToken
        }


        public Nethereum.Contracts.Contract Contract(ShareAndChargeContract type, Nethereum.Web3.Web3 web3)
        {
            string abi;
            string address;

            switch (type) {
                case ShareAndChargeContract.LibManager:
                    abi = SlockIt_ShareAndCharge.Properties.Resources.AbiMainLibJson;
                    //abi = await ReadAbiFromFile(@"C:\Test\BlockChain\AbiMainLibJson.txt");
                    address = _libManager.Address;
                    break;
                case ShareAndChargeContract.ChargePoles:
                    abi = SlockIt_ShareAndCharge.Properties.Resources.AbiChargePolesJson;
                    //abi = await ReadAbiFromFile(@"C:\Test\BlockChain\AbiChargePolesJson.txt");
                    address = _libManager.ChargingPoles;
                    break;
                case ShareAndChargeContract.MobilityToken:
                    abi = SlockIt_ShareAndCharge.Properties.Resources.AbiMobilityJson;
                    //abi = await ReadAbiFromFile(@"C:\Test\BlockChain\AbiMobilityJson.txt");
                    address = _libManager.Mobility;
                    break;
                default:                  
                    return null;
            }

            return web3.Eth.GetContract(abi, address);
        }


        public Nethereum.Contracts.Contract[] ContractHistoryChargePole(Nethereum.Web3.Web3 web3)
        {
            string[] con = new string[4];
            con[0] = "0x61c810e21659032084A4448D8D2F498789f81CB5";     //erste Addresse aus dem Blog
            con[1] = "0xB7395c47EC901f4cE872dbEB253966D7b4f620c3";     //zweite Addresse aus der PDF
            con[2] = "0xb642a68bD622D015809bb9755d07EA3006b85843";     //dritte Addresse aus Request auf LibManager
            con[3] = "0x1CF3314a8b6A1511A67633B3fdf99E3D344Ab298";     //vierte Addresse aus Etherscan -> intern transfer to this address, a few blocks after last trans

            Nethereum.Contracts.Contract[] lst = new Nethereum.Contracts.Contract[con.Length];
            for (int i = 0; i < con.Length; i++)
            {
                Nethereum.Contracts.Contract contract = Contract(ContractFactory.ShareAndChargeContract.ChargePoles, web3);
                contract.Address = con[i];
                lst[i] = contract;
            }
            return lst;
        }




        private static async Task<string> ReadAbiFromFile(string file)
        {
            // Read the file as one string.
            string line;
            using (StreamReader reader = new StreamReader(file, Encoding.UTF8))
            {
                line =  reader.ReadToEnd();
            }
            return line;
        }
    }
}
