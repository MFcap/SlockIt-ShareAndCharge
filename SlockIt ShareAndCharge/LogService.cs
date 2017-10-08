using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlockIt_ShareAndCharge
{
    class LogService : INotifyPropertyChanged
    {


        private static LogService _instance;

        public static LogService Instance
        {
            get
            {
                if (_instance == null) { _instance = new LogService(); };
                return _instance;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;


        

        private bool _outputConsole;
        private bool _outputEvent;


        private List<MessageView> _lst;
        //public List<MessageView> Messages { get => _lst == null ? _lst : _lst.OrderByDescending(x => x.Date).ToList(); }
        public List<MessageView> Messages { get => _lst; }


        public bool OutputConsole
        {
            get { return _outputConsole; }
            set {
                _outputConsole = value;
                NotifyPropertyChanged();
            }
        }

        public bool OutputEvent{ 
            get { return _outputEvent; }
            set {
                _outputEvent = value;
                NotifyPropertyChanged();
            }
        }




        private LogService()
        {
            _lst = new List<MessageView>();
            _outputConsole = false;
            _outputEvent = true;
           
        }


        public void Log(MessageView mv)
        {
            _lst.Add(mv);
            NotifyPropertyChanged("Messages");
            if (_outputConsole) Console.WriteLine(mv.ToString());
        }

        public void Log(string message)
        {
            MessageView mw = new MessageView(message);
            Log(mw);
        }

        public void Log(string message, string header)
        {
            MessageView mw = new MessageView(message, header);
            Log(mw);
        }



        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
