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

        private bool _placed;
        public bool Placed
        {
            get
            {
                return _placed;
            }
            set
            {
                _placed = value;
                OnPropertyChanged("Placed");
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

        public void Load(NarrativeCharacter nc)
        {
            Name = nc.Name;
            Placed = nc.Placed;
            TangibleObject = nc.TangibleObject;
        }

        public void Load(NarrativeThing nt)
        {
            Name = nt.Name;
            Placed = nt.Placed;
            TangibleObject = nt.TangibleObject;
        }
    }
}
