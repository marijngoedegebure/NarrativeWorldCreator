using Microsoft.Xna.Framework;
using SharpDX.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.ViewModel
{
    public class ObjectPlacementViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Vector3ViewModel> _placementOptions;
        public ObservableCollection<Vector3ViewModel> PlacementOptions
        {
            get
            {
                return _placementOptions;
            }
            set
            {
                _placementOptions = value;
                OnPropertyChanged("PlacementOptions");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public void Load(List<Vector3> options)
        {
            ObservableCollection<Vector3ViewModel> pooc = new ObservableCollection<Vector3ViewModel>();
            foreach (var option in options)
            {
                var v3VM = new Vector3ViewModel();
                v3VM.Load(option);
                pooc.Add(v3VM);
            }
            PlacementOptions = pooc;
        }
    }
}
