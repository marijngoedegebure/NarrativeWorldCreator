using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using NarrativeWorldCreator.Models.NarrativeGraph;

namespace NarrativeWorldCreator.ViewModel
{
    public class NodeViewModel : INotifyPropertyChanged
    {
        private LocationNode _selectedNode;
        public LocationNode SelectedNode
        {
            get
            {
                return _selectedNode;
            }
            set
            {
                _selectedNode = value;
                OnPropertyChanged("SelectedNode");
            }
        }

        internal void Load(LocationNode selectedNode)
        {
            this.SelectedNode = selectedNode;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
