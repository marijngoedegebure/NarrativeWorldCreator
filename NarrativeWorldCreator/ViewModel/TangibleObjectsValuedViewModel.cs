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

        public void LoadAll(NarrativeTimePoint ntp)
        {
            ObservableCollection<TOTreeTangibleObject> octo = new ObservableCollection<TOTreeTangibleObject>();
            var listOfValuedTangibleObjects = TangibleObjectMetricEngine.GetOrderingTO(ntp, ntp.AvailableTangibleObjects.Where(x => x.Children.Count == 0).ToList(), ntp.GetRemainingPredicates());
            foreach (var to in listOfValuedTangibleObjects)
            {
                if (!to.TangibleObject.DefaultName.Equals("floor"))
                    octo.Add(to);
            }
            octo.OrderBy(to => to.EndValue);
            TangibleObjectsValued = octo;
        }

        internal void LoadRequired(NarrativeTimePoint ntp)
        {
            ObservableCollection<TOTreeTangibleObject> octo = new ObservableCollection<TOTreeTangibleObject>();
            var listOfValuedTangibleObjects = TangibleObjectMetricEngine.GetRequiredOrderingTO(ntp, ntp.AvailableTangibleObjects.Where(x => x.Children.Count == 0).ToList(), ntp.GetRemainingPredicates());
            foreach (var to in listOfValuedTangibleObjects)
            {
                if (!to.TangibleObject.DefaultName.Equals("floor"))
                    octo.Add(to);
            }
            octo.OrderBy(to => to.EndValue);
            TangibleObjectsValued = octo;
        }

        internal void LoadDecoration(NarrativeTimePoint ntp)
        {
            ObservableCollection<TOTreeTangibleObject> octo = new ObservableCollection<TOTreeTangibleObject>();
            var listOfValuedTangibleObjects = TangibleObjectMetricEngine.GetDecorationOrderingTO(ntp, ntp.AvailableTangibleObjects.Where(x => x.Children.Count == 0).ToList(), ntp.GetRemainingPredicates());
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
