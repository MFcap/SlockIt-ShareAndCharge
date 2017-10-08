using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlockIt_ShareAndCharge
{
    class MessageView
    {

        private DateTime _date = DateTime.Now;
        private string _message;
        private string _header;

        public DateTime Date { get => _date; set => _date = value; }
        public string Message { get => _message; set => _message = value; }
        public string Header { get => _header; set => _header = value; }

        public MessageView(string message)
        {
            _message = message;
        }

        public MessageView(string message, string header)
        {
            _message = message;
            _header = header;
        }


        public override string ToString()
        {
            return _date.ToString("hh:mm:ss dd.MM.yyyy") + " - " + _message;
        }
    }
}
