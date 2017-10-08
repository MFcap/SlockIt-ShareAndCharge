using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlockIt_ShareAndCharge
{
    class LibManager
    {

        private string _address;

        private string _chargingPoles;
        private string _mobility;
        private string _feeManagement;
        private string _priceProvider;
        private string _genLib;
        private string _dateTime;
        private string _admin;


        public string ChargingPoles { get => _chargingPoles; set => _chargingPoles = value; }
        public string Mobility { get => _mobility; set => _mobility = value; }
        public string FeeManagement { get => _feeManagement; set => _feeManagement = value; }
        public string PriceProvider { get => _priceProvider; set => _priceProvider = value; }
        public string GenLib { get => _genLib; set => _genLib = value; }
        public string DateTime { get => _dateTime; set => _dateTime = value; }
        public string Admin { get => _admin; set => _admin = value; }
        public string Address { get => _address; set => _address = value; }



        public enum Libraries
        {
            ChargingPoles = 1,
            Mobility,
            FeeManagement,
            PriceProvider,
            GenLib,
            DateTime,
            Admin
        }


        public static string GetLibrariesFctName(Libraries lib)
        {
            switch (lib)
            {
                case Libraries.ChargingPoles: return "chargingPolesLib";
                case Libraries.Mobility: return "mobilityTokenLib";
                case Libraries.FeeManagement: return "feeManagementLib";
                case Libraries.PriceProvider: return "priceProviderLibs";
                case Libraries.GenLib: return "genLibs";
                case Libraries.DateTime: return "dateTimeLib";
                case Libraries.Admin: return "admin";
                default: return String.Empty;
            }
        }

        public void addAddress(string address, Libraries lib)
        {
            switch (lib)
            {
                case Libraries.ChargingPoles:   _chargingPoles  = address; break;
                case Libraries.Mobility:        _mobility       = address; break;
                case Libraries.FeeManagement:   _feeManagement  = address; break;
                case Libraries.PriceProvider:   _priceProvider  = address; break;
                case Libraries.GenLib:          _genLib         = address; break;
                case Libraries.DateTime:        _dateTime       = address; break;
                case Libraries.Admin:           _admin          = address; break;
                default: break;
            }
        }
    }
}
