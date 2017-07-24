using NarrativeWorldCreator.MetricEngines;
using NarrativeWorldCreator.Models.Metrics;
using NarrativeWorldCreator.Models.Metrics.TOTree;
using NarrativeWorldCreator.Models.NarrativeTime;
using NarrativeWorldCreator.Solvers;
using Semantics.Data;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NarrativeWorldCreator.Models.NarrativeGraph;
using NarrativeWorldCreator.Models.NarrativeRegionFill;

namespace NarrativeWorldCreator.ViewModel
{
    public class TangibleObjectsValuedViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<TOTreeTangibleObject> _tangibleObjectsValued;
        public ObservableCollection<TOTreeTangibleObject> TangibleObjectsValued
        {
            get
            {
                return _tangibleObjectsValued;
            }
            set
            {
                _tangibleObjectsValued = value;
                OnPropertyChanged("TangibleObjectsValued");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public TangibleObjectsValuedViewModel()
        {
            ObservableCollection<TOTreeTangibleObject> octo = new ObservableCollection<TOTreeTangibleObject>();
            TangibleObjectsValued = octo;
        }

        public void LoadList(List<TOTreeTangibleObject> input)
        {
            ObservableCollection<TOTreeTangibleObject> octo = new ObservableCollection<TOTreeTangibleObject>();
            foreach (var to in input)
            {
                if (!to.TangibleObject.DefaultName.Equals("floor"))
                    octo.Add(to);
            }
            octo.OrderBy(to => to.EndValue);
            TangibleObjectsValued = octo;
        }

        public void LoadAll(LocationNode ln, NarrativeTimePoint ntp, Configuration c)
        {
            ObservableCollection<TOTreeTangibleObject> octo = new ObservableCollection<TOTreeTangibleObject>();
            var listOfValuedTangibleObjects = MetricEngine.GetOrderingTOUsingTOAndConfig(ln, c, ln.AvailableTangibleObjects.Where(x => x.Children.Count == 0).ToList(), ntp.GetRemainingPredicates());
            foreach (var to in listOfValuedTangibleObjects)
            {
                if (!to.TangibleObject.DefaultName.Equals("floor"))
                    octo.Add(to);
            }
            octo.OrderBy(to => to.EndValue);
            TangibleObjectsValued = octo;
        }

        internal void LoadRequired(LocationNode ln, NarrativeTimePoint ntp, Configuration c)
        {
            ObservableCollection<TOTreeTangibleObject> octo = new ObservableCollection<TOTreeTangibleObject>();
            var listOfValuedTangibleObjects = MetricEngine.GetRequiredOrderingTOUsingTOAndConfig(ln, c, ln.AvailableTangibleObjects.Where(x => x.Children.Count == 0).ToList(), ntp.GetRemainingPredicates());
            foreach (var to in listOfValuedTangibleObjects)
            {
                if (!to.TangibleObject.DefaultName.Equals("floor"))
                    octo.Add(to);
            }
            octo.OrderBy(to => to.EndValue);
            TangibleObjectsValued = octo;
        }

        internal void LoadDecoration(LocationNode ln, NarrativeTimePoint ntp, Configuration c)
        {
            ObservableCollection<TOTreeTangibleObject> octo = new ObservableCollection<TOTreeTangibleObject>();
            var listOfValuedTangibleObjects = MetricEngine.GetDecorationOrderingTOUsingTOAndConfig(ln, c, ln.AvailableTangibleObjects.Where(x => x.Children.Count == 0).ToList(), ntp.GetRemainingPredicates());
            foreach (var to in listOfValuedTangibleObjects)
            {
                if (!to.TangibleObject.DefaultName.Equals("floor"))
                    octo.Add(to);
            }
            octo.OrderBy(to => to.EndValue);
            TangibleObjectsValued = octo;
        }
    }
}
