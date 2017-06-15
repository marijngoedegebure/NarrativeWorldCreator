using NarrativeWorldCreator.Models.NarrativeGraph;
using NarrativeWorldCreator.Models.NarrativeTime;
using Semantics.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.ViewModel
{
    public class TangibleObjectsSwapViewModel : INotifyPropertyChanged
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

        public void Load(List<TangibleObject> tos)
        {
            var toOC = new ObservableCollection<TangibleObject>();
            foreach (var to in tos)
            {
                toOC.Add(to);
            }
            this.TangibleObjects = toOC;
        }

        internal void Remove(TangibleObject selectedItem)
        {
            this.TangibleObjects.Remove(selectedItem);
        }

        internal void Add(TangibleObject selectedItem)
        {
            this.TangibleObjects.Add(selectedItem);
        }
    }
}
