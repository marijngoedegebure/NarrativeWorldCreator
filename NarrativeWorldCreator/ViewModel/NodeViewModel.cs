using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NarrativeWorlds;
using System.ComponentModel;

namespace NarrativeWorldCreator
{
    public class NodeViewModel
    {
        private Node _selectedNode;
        public Node SelectedNode
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

        internal void Load(Node selectedNode)
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
