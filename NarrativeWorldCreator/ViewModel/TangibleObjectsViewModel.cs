using NarrativeWorlds;
using Semantics.Data;
using Semantics.Entities;
using SharpDX.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator
{
    public class TangibleObjectsViewModel
    {
        private ObservableCollection<TangibleObject> _tangibleObjects;
        public ObservableCollection<TangibleObject> TangibleObjects
        {
            get
            {
                return _tangibleObjects;
            }
            set
            {
                _tangibleObjects = value;
                OnPropertyChanged("TangibleObjects");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public void Load(NarrativeTimePoint ntp)
        {
            ObservableCollection<TangibleObject> octo = new ObservableCollection<TangibleObject>();
            var filteredTo = ntp.AvailableTangibleObjects.Where(x => x.Children.Count == 0);
            foreach (var to in filteredTo)
            {
                octo.Add(to);
            }
            TangibleObjects = octo;
        }
    }
}
