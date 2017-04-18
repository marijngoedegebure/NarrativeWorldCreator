using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NarrativeWorldCreator.RegionPage;

namespace NarrativeWorldCreator.ViewModel
{
    public class ModeViewModel : INotifyPropertyChanged
    {
        private string[] _pageModes = new string[2] { "Creation mode", "Filling mode"};

        private RegionPageMode _regionPageMode;
        public RegionPageMode RegionPageMode
        {
            get
            {
                return _regionPageMode;
            }
            set
            {
                _regionPageMode = value;
                OnPropertyChanged("RegionPageMode");
                OnPropertyChanged("RegionPageModeString");
            }
        }

        public string RegionPageModeString
        {
            get
            {
                return _pageModes[Convert.ToInt32(this.RegionPageMode)];
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public void ChangeModes(RegionPageMode newMode)
        {
            RegionPageMode = newMode;
        }
    }
}
