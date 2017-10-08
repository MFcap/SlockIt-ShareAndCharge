using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlockIt_ShareAndCharge
{
    class RequestService
    {

        private static RequestService _instance;

        public static RequestService Instance
        {
            get
            {
                if (_instance == null) { _instance = new RequestService(); };
                return _instance;
            }
        }

        

        private LogService logger = LogService.Instance;

        private Nethereum.Web3.Web3 _web3;

        private ulong _maxBlock = 4343888L;
        private string _web3URI;
        public string Web3URI { get => _web3URI;
            set
            {
                if (value is null || String.Empty.Equals(value)) return;
                if (!Uri.IsWellFormedUriString(value, UriKind.Absolute)) return;
                _web3 = new Nethereum.Web3.Web3(value);
                _web3URI = value;
                //write to AppConfig
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["EtherClient"].Value = value;
                config.Save(ConfigurationSaveMode.Modified);
                //log
                logger.Log("Client wurde neu initialsiert... neue Addresse: " + value);
            }
        }


        



        private RequestService()
        {
            _web3URI = ConfigurationManager.AppSettings["EtherClient"];
            _web3 = new Nethereum.Web3.Web3(_web3URI);            
        }



        #region "client tests"


        public async Task<string> ReqCurrentBlockNumber()
        {
            logger.Log("Request for CurrentBlockNumber");
            
            Nethereum.RPC.Eth.Blocks.EthBlockNumber ethBlockNumber = _web3.Eth.Blocks.GetBlockNumber;
            Nethereum.Hex.HexTypes.HexBigInteger hexBigInteger = await ethBlockNumber.SendRequestAsync();
            System.Numerics.BigInteger bNr = hexBigInteger.Value;

            //
            _maxBlock = (ulong) bNr;
            logger.Log("Current blockNumber is " + bNr);
            return bNr.ToString();
        }

        public async Task ReqSyncingStatus()
        {
            logger.Log("Starte Request via Web3...for Syncing Status");

            Nethereum.RPC.Eth.EthSyncing ethSync = _web3.Eth.Syncing;
            Nethereum.RPC.Eth.DTOs.SyncingOutput output = await ethSync.SendRequestAsync();

            logger.Log("status received...");
            logger.Log("CurrentBlock: " + (output.CurrentBlock != null ? output.CurrentBlock.Value.ToString() : "null"));
            logger.Log("HighestBlock: " + (output.HighestBlock != null ? output.HighestBlock.Value.ToString() : "null"));
            logger.Log("StartingBlock: " + (output.StartingBlock != null ? output.StartingBlock.Value.ToString() : "null"));
            logger.Log("IsSyncing: " + output.IsSyncing);

        }

        public async Task ReqContractTest()
        {

            Nethereum.Contracts.Contract contract;
           

            //MainLib
            try
            {
                contract = ContractFactory.Instance.Contract(ContractFactory.ShareAndChargeContract.LibManager, _web3);
                logger.Log("Test 1 Main Contract - Success");

                await FctReqLibAddress(contract, LibManager.Libraries.ChargingPoles);
                logger.Log("Test 1 Main Fct - Success");
            } catch (Exception e) {
                logger.Log("Test 1 Main - Fehler  : " + e.Message);
            }
            

            //Charge 1
            try
            {
                contract = ContractFactory.Instance.Contract(ContractFactory.ShareAndChargeContract.ChargePoles, _web3);
                logger.Log("Test 2 Charge1- Success");

                await FctReqLibManagerFromSubContract(contract);
                await FctReqDbFromSubContract(contract);
                logger.Log("Test 2 Charge1 Fct - Success");
            }
            catch (Exception e)
            {
                logger.Log("Test 2 Charge1- Fehler  : " + e.Message);
            }

            //Mobility
            try
            {
                contract = ContractFactory.Instance.Contract(ContractFactory.ShareAndChargeContract.MobilityToken, _web3);
                logger.Log("Test 3 Mobility - Success");

                logger.Log("get libAddress and dbAddress...");
                await FctReqLibManagerFromSubContract(contract);
                await FctReqDbFromSubContract(contract);
                logger.Log("Test 3 Mobility Fct - Success");
            }
            catch (Exception e)
            {
                logger.Log("Test 3 Mobility- Fehler  : " + e.Message);
            }

        }

        #endregion

        #region "mobility lib"




#endregion


        #region "helper functions"

        public async Task<DateTime> RequestBlockDate(Nethereum.Hex.HexTypes.HexBigInteger blockNumber)
        {
            var blockReq = _web3.Eth.Blocks.GetBlockWithTransactionsByNumber;
            blockReq.BuildRequest(blockNumber.Value);
            var block = await blockReq.SendRequestAsync(blockNumber);

            return GetBlockDate(block);
        }

        private DateTime GetBlockDate(Nethereum.RPC.Eth.DTOs.Block block)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            return dtDateTime.AddSeconds((double)block.Timestamp.Value).ToLocalTime();
        }

        private List<Nethereum.RPC.Eth.DTOs.NewFilterInput> GetRequests(int totalReqSizeInBlocks, int step)
        {
            List<Nethereum.RPC.Eth.DTOs.NewFilterInput> lst = new List<Nethereum.RPC.Eth.DTOs.NewFilterInput>();
            ulong startBlock = _maxBlock - Convert.ToUInt64(totalReqSizeInBlocks);
            for (ulong block = startBlock; block < _maxBlock; block += Convert.ToUInt64(step))
            {
                Nethereum.RPC.Eth.DTOs.NewFilterInput filter = new Nethereum.RPC.Eth.DTOs.NewFilterInput();
                filter.FromBlock = new Nethereum.RPC.Eth.DTOs.BlockParameter(block);
                filter.ToBlock = new Nethereum.RPC.Eth.DTOs.BlockParameter(Math.Min(block + Convert.ToUInt64(step) - 1, _maxBlock));
                lst.Add(filter);
            }

            return lst;

        }
        #endregion



        #region "MainLib Functions"



        public async Task ReqLibUpdate()
        {
            await FctLibUpdateLog(ContractFactory.Instance.Contract(ContractFactory.ShareAndChargeContract.LibManager, _web3));
            await FctLibUpdateGenLog(ContractFactory.Instance.Contract(ContractFactory.ShareAndChargeContract.LibManager, _web3));
        }

        /// <summary>
        /// Historie der Lib Änderungen
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        private async Task FctLibUpdateLog(Nethereum.Contracts.Contract contract)
        {
            string fctName = "LogUpdateLib";
            Nethereum.Contracts.Event ev = contract.GetEvent(fctName);
            Nethereum.RPC.Eth.DTOs.NewFilterInput filter = new Nethereum.RPC.Eth.DTOs.NewFilterInput();
            filter.FromBlock = new Nethereum.RPC.Eth.DTOs.BlockParameter(_maxBlock - _maxBlock);
            filter.ToBlock = new Nethereum.RPC.Eth.DTOs.BlockParameter(_maxBlock);

            int aswSize = 0;
            try
            {
                //build the filter
                Nethereum.Hex.HexTypes.HexBigInteger res = await ev.CreateFilterBlockRangeAsync(filter.FromBlock, filter.ToBlock);

                //request
                var asw = await ev.GetAllChanges<LogUpdateLibEvent>(res);

                //get the block date
                foreach (var ele in asw)
                {
                    DateTime dt = await RequestBlockDate(ele.Log.BlockNumber);
                    logger.Log("BlockNr: " + ele.Log.BlockNumber.Value + "  - Date: " + dt.ToString("HH:mm:ss dd.MM.yyyy")
                        + "Name: " + ele.Event.Name + "  NewAddresse: " + ele.Event.NewAddress, fctName);
                }
                aswSize = asw.Count;
            }
            catch (Exception e)
            {
                logger.Log("Fehler bei " + fctName + " EventCall. Msg: " + e.Message, fctName);
            }

            //chargLib.Index = index;
            logger.Log(fctName + " - Size: " + aswSize, fctName);
            logger.Log(fctName + " ------------------", fctName);
        }

        /// <summary>
        /// Historie der LibGen Änderungen
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        private async Task FctLibUpdateGenLog(Nethereum.Contracts.Contract contract)
        {
            string fctName = "LogUpdateGenLib";
            Nethereum.Contracts.Event ev = contract.GetEvent(fctName);
            Nethereum.RPC.Eth.DTOs.NewFilterInput filter = new Nethereum.RPC.Eth.DTOs.NewFilterInput();
            filter.FromBlock = new Nethereum.RPC.Eth.DTOs.BlockParameter(_maxBlock - _maxBlock);
            filter.ToBlock = new Nethereum.RPC.Eth.DTOs.BlockParameter(_maxBlock);

            int aswSize = 0;
            try
            {
                //build the filter
                Nethereum.Hex.HexTypes.HexBigInteger res = await ev.CreateFilterBlockRangeAsync(filter.FromBlock, filter.ToBlock);

                //request
                var asw = await ev.GetAllChanges<LogUpdateGenLibEvent>(res);

                //get the block date
                foreach (var ele in asw)
                {
                    DateTime dt = await RequestBlockDate(ele.Log.BlockNumber);
                    logger.Log("BlockNr: " + ele.Log.BlockNumber.Value + "  - Date: " + dt.ToString("HH:mm:ss dd.MM.yyyy")
                        + "Index: " + ele.Event.Index + "  Addresse: " + ele.Event.Address, fctName);
                }
                aswSize = asw.Count;
            }
            catch (Exception e)
            {
                logger.Log("Fehler bei " + fctName + " EventCall. Msg: " + e.Message, fctName);
            }

            //chargLib.Index = index;
            logger.Log(fctName + " - Size: " + aswSize, fctName);
            logger.Log(fctName + " ------------------", fctName);
        }

        
        /// <summary>
        /// aktuelle ChargePolesAddresse
        /// </summary>
        /// <returns></returns>
        public async Task ReqLibraries()
        {
            for (int i = 1; i < 8; i++)
            {
                await FctReqLibAddress(ContractFactory.Instance.Contract(ContractFactory.ShareAndChargeContract.LibManager, _web3), (LibManager.Libraries) i );
            }
            logger.Log("~~~~~ Request Libraries finished ~~~~~~~");
        }



        private async Task<string> FctReqLibAddress(Nethereum.Contracts.Contract contract, LibManager.Libraries lib)
        {
            string fctName = LibManager.GetLibrariesFctName(lib);
            Nethereum.Contracts.Function fct = contract.GetFunction(fctName);
            string asw = String.Empty;
            try
            {
                var res = await fct.CallDeserializingToObjectAsync<FctLibraryAddress>().ConfigureAwait(false);
                logger.Log(fctName + " Address is " + (res != null ? res.Address : "./."));
                asw = res.Address;
                ContractFactory.Instance.LibManager.addAddress(res.Address, lib);
            } catch (Exception e)
            {
                logger.Log("Fehler bei " + fctName + " ...Msg: " + e.Message);
            }
            return asw;
        }
        
        #endregion



        #region "Functions for SubContract"


        private async Task FctReqLibManagerFromSubContract(Nethereum.Contracts.Contract contract)
        {
            Nethereum.Contracts.Function fct = contract.GetFunction("libManager");
            try
            {
                var chargLib = await fct.CallDeserializingToObjectAsync<FctLibraryAddress>().ConfigureAwait(false);
                logger.Log("Address LibManager is " + (chargLib != null ? chargLib.Address : "./."));
            }
            catch (Exception e)
            {
                logger.Log("Fehler beim FctReqLibManager... " + e.Message);
            }
        }

        private async Task FctReqDbFromSubContract(Nethereum.Contracts.Contract contract)
        {
            Nethereum.Contracts.Function fct = contract.GetFunction("db");
            try
            {
                var chargLib = await fct.CallDeserializingToObjectAsync<FctLibraryAddress>().ConfigureAwait(false);
                logger.Log("Address DB is " + (chargLib != null ? chargLib.Address : "./."));
            }
            catch (Exception e)
            {
                logger.Log("Fehler beim FctDb... " + e.Message);
            }
        }

        #endregion




        #region "ChargePoles Events"



        public async Task ReqLogRented()
        {

            Nethereum.Contracts.Contract[] con = ContractFactory.Instance.ContractHistoryChargePole(_web3);

            for (int i = 0; i < con.Length; i++) {                
                logger.Log("Log Rented Contract: " + i + "    Address: " + con[i].Address);
                await FctPolesLogRented(con[i]);
            }
            logger.Log("~~~~~ LogRented finished ~~~~~~~");
        }

        private async Task FctPolesLogRented(Nethereum.Contracts.Contract contract)
        {
            string fctName = "LogRented";
            Nethereum.Contracts.Event ev = contract.GetEvent(fctName);
            Nethereum.RPC.Eth.DTOs.NewFilterInput filter = new Nethereum.RPC.Eth.DTOs.NewFilterInput();
            filter.FromBlock = new Nethereum.RPC.Eth.DTOs.BlockParameter(_maxBlock - _maxBlock);
            filter.ToBlock = new Nethereum.RPC.Eth.DTOs.BlockParameter(_maxBlock);

            int aswSize = 0;
            try
            {
                //build the filter
                Nethereum.Hex.HexTypes.HexBigInteger res = await ev.CreateFilterBlockRangeAsync(filter.FromBlock, filter.ToBlock);

                //request
                var asw = await ev.GetAllChanges<LogRentedEvent>(res);

                //get the block date
                foreach (var ele in asw)
                {
                    DateTime dt = await RequestBlockDate(ele.Log.BlockNumber);
                    logger.Log("BlockNr: " + ele.Log.BlockNumber.Value + "  - Date: " + dt.ToString("HH:mm:ss dd.MM.yyyy"), fctName);
                }
                aswSize = asw.Count;
            }
            catch (Exception e)
            {
                logger.Log("Fehler bei " + fctName + " EventCall. Msg: " + e.Message, fctName);
            }

            //chargLib.Index = index;
            logger.Log(fctName + " - Size: " + aswSize, fctName);
            logger.Log(fctName + " ------------------", fctName);
        }




        public async Task ReqLogReturned()
        {
            Nethereum.Contracts.Contract[] con = ContractFactory.Instance.ContractHistoryChargePole(_web3);
            for (int i = 0; i < con.Length; i++)
            {
                logger.Log("Log Returned Contract: " + i +  "    Address: " + con[i].Address);
                await FctPolesLogReturned(con[i]);
            }
            logger.Log("~~~~~ LogReturned finished ~~~~~~~");
        }

        private async Task FctPolesLogReturned(Nethereum.Contracts.Contract contract)
        {
            string fctName = "LogReturned"; 
            Nethereum.Contracts.Event ev = contract.GetEvent(fctName);
            Nethereum.RPC.Eth.DTOs.NewFilterInput filter = new Nethereum.RPC.Eth.DTOs.NewFilterInput();
            filter.FromBlock = new Nethereum.RPC.Eth.DTOs.BlockParameter(_maxBlock - _maxBlock);
            filter.ToBlock = new Nethereum.RPC.Eth.DTOs.BlockParameter(_maxBlock);

            int aswSize = 0;
            try
            {
                //build the filter
                Nethereum.Hex.HexTypes.HexBigInteger res = await ev.CreateFilterBlockRangeAsync(filter.FromBlock, filter.ToBlock);

                //request
                var asw = await ev.GetAllChanges<LogReturnedEvent>(res);

                //get the block date
                foreach (var ele in asw)
                {
                    DateTime dt = await RequestBlockDate(ele.Log.BlockNumber);
                    logger.Log("BlockNr: " + ele.Log.BlockNumber.Value + "  - Date: " + dt.ToString("HH:mm:ss dd.MM.yyyy"), fctName);
                }
                aswSize = asw.Count;
            }
            catch (Exception e)
            {
                logger.Log("Fehler bei " + fctName + " EventCall. Msg: " + e.Message, fctName);
            }

            //chargLib.Index = index;
            logger.Log(fctName + " - Size: " + aswSize, fctName);
            logger.Log(fctName + " ------------------", fctName);
        }


#endregion


    }
}
