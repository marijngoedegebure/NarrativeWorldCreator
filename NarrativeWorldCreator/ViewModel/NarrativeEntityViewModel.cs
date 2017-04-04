using NarrativeWorlds;
using Semantics.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator
{
    public class NarrativeEntityViewModel
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private TangibleObject _tangibleObject;
        public TangibleObject TangibleObject
        {
            get
            {
                return _tangibleObject;
            }
            set
            {
                _tangibleObject = value;
                OnPropertyChanged("TangibleObject");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public void Load(NarrativeObjectEntikaLink no)
        {
            Name = no.NarrativeObject.Name;
            TangibleObject = no.TangibleObject;
        }
    }
}
