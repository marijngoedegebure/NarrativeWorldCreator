using Common;
using Microsoft.Xna.Framework;
using NarrativeWorldCreator.Models.NarrativeGraph;
using NarrativeWorldCreator.Models.NarrativeInput;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using Semantics.Components;
using Semantics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.NarrativeTime
{
    public class NarrativeTimePoint
    {
        // Narrative time point, increases by 1 for each time point, initial time point is 0
        public int TimePoint { get; set; }
        // Narrative event associated with this timepoint, just for potential later reference, should not be necessary
        public NarrativeEvent NarrativeEvent { get; set; }
        // Location mentioned in the narrative event
        public LocationNode Location { get; set; }

        // List of TangibleObjects selected for this region/timepoint
        public List<TangibleObject> AvailableTangibleObjects { get; set; }

        public List<NarrativeRegionFill.Predicate> AllPredicates { get; set; }

        // Story constraints for this timepoint, otherwise known as the goals
        public List<NarrativeRegionFill.Predicate> PredicatesFilteredByCurrentLocation { get; set; }

        public List<NarrativeRegionFill.Predicate> PredicatesCausedByInstancedObjectsAndRelations { get; set; }


        // Current configuration of a timepoint, used to generate new configurations from
        public Configuration Configuration { get; set; }

        public NarrativeTimePoint(int timePoint, List<TangibleObject> DefaultTangibleObjects)
        {
            this.TimePoint = timePoint;
            PredicatesFilteredByCurrentLocation = new List<NarrativeRegionFill.Predicate>();
            AllPredicates = new List<NarrativeRegionFill.Predicate>();
            PredicatesCausedByInstancedObjectsAndRelations = new List<NarrativeRegionFill.Predicate>();
            Configuration = new Configuration();
            AvailableTangibleObjects = DefaultTangibleObjects;
        }

        public void CopyPredicates(NarrativeTimePoint initial)
        {
            foreach (var predicate in initial.AllPredicates)
            {
                this.AllPredicates.Add(new NarrativeRegionFill.Predicate {
                    PredicateType = predicate.PredicateType,
                    EntikaClassNames = predicate.EntikaClassNames
                });
            }
        }

        // This function allows updating
        public void SetBaseShape(LocationNode selectedNode)
        {
            var floorInstance = this.Configuration.InstancedObjects.Where(io => io.Name.Equals(Constants.Floor)).FirstOrDefault();
            if (floorInstance == null)
            {
                // Create floor instance
                SetupFloorInstance(selectedNode);
            }
            else
            {
                // Update floor instance
                floorInstance.Polygon = new Polygon(selectedNode.Shape.Points);
                floorInstance.UpdateBoundingBoxAndShape(null);
            }
        }

        // This only allows the new instantiation of the current floor instance when the timepoint is newly selected
        public void SwitchTimePoints(LocationNode selectedNode)
        {
            if (this.Configuration.InstancedObjects.Where(io => io.Name.Equals(Constants.Floor)).FirstOrDefault() == null)
            {
                SetupFloorInstance(selectedNode);
            }
        }

        private void SetupFloorInstance(LocationNode selectedNode)
        {
            // Setup EntikaInstance with on(X, floor) relationship
            var floorInstance = new EntikaInstance(Constants.Floor, new Polygon(selectedNode.Shape.Points));
            this.Configuration.InstancedObjects.Add(floorInstance);
        }

        internal void InstantiateRelationship(RelationshipInstance relationInstance)
        {
            // All relationships should have 3 arguments, source, target and location
            var predicateType = SystemStateTracker.NarrativeWorld.NarrativeTimeline.GetPredicateType(relationInstance.BaseRelationship.RelationshipType.DefaultName);
            if (predicateType == null)
            {
                // Add the new predicateType to the predicateTypes
                predicateType = GetNewRelationshipPredicateType(relationInstance.BaseRelationship);
                SystemStateTracker.NarrativeWorld.NarrativeTimeline.PredicateTypes.Add(predicateType);
            }
            this.PredicatesCausedByInstancedObjectsAndRelations.Add(new NarrativeRegionFill.Predicate
            {
                PredicateType = predicateType,
                EntikaClassNames = new List<string> { relationInstance.Source.TangibleObject.DefaultName, relationInstance.Target.TangibleObject.DefaultName, this.Location.LocationName }
            });
        }

        public static List<NarrativeRegionFill.Predicate> GetPredicates(Relationship r, TangibleObject source, TangibleObject target, string location)
        {
            var ret = new List<NarrativeRegionFill.Predicate>();
            var predicateType = SystemStateTracker.NarrativeWorld.NarrativeTimeline.GetPredicateType(r.RelationshipType.DefaultName);
            if (predicateType == null)
            {
                // Add the new predicateType to the predicateTypes
                predicateType = GetNewRelationshipPredicateType(r);
            }
            ret.Add(new NarrativeRegionFill.Predicate
            {
                PredicateType = predicateType,
                EntikaClassNames = new List<string> { r.Source.DefaultName, r.Targets[0].DefaultName, location }
            });
            ret.Add(GetNewAtPredicate(source, location));
            ret.Add(GetNewAtPredicate(target, location));
            return ret;
        }

        public static NarrativeRegionFill.Predicate GetNewRelationshipPredicate(Relationship r, string location)
        {
            return new NarrativeRegionFill.Predicate
            {
                PredicateType = GetNewRelationshipPredicateType(r),
                EntikaClassNames = new List<string> { r.Source.DefaultName, r.Targets[0].DefaultName, location }
            };
        }

        public static PredicateType GetNewRelationshipPredicateType(Relationship r)
        {
            return new PredicateType
            {
                Name = r.RelationshipType.DefaultName,
                Arguments = new List<NarrativeArgument>() {
                        new NarrativeArgument {
                            Type = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectType("thing"),
                            Name = "?x"
                        },
                        new NarrativeArgument
                        {
                            Type = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectType("thing"),
                            Name = "?y"
                        },
                        new NarrativeArgument
                        {
                            Type = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectType("place"),
                            Name = "?z"
                        }
                    }
            };
        }

        public static NarrativeRegionFill.Predicate GetNewAtPredicate(TangibleObject t, string location)
        {
            // Create At predicate
            var predicateType = SystemStateTracker.NarrativeWorld.NarrativeTimeline.GetPredicateType("at");
            return new NarrativeRegionFill.Predicate
            {
                PredicateType = predicateType,
                EntikaClassNames = new List<string> { t.DefaultName, location }
            };
        }

        internal void InstantiateAtPredicateForInstance(EntikaInstance instanceOfObjectToAdd)
        {
            this.PredicatesCausedByInstancedObjectsAndRelations.Add(GetNewAtPredicate(instanceOfObjectToAdd.TangibleObject, this.Location.LocationName));            
        }

        public List<NarrativeRegionFill.Predicate> GetRemainingPredicates()
        {
            var ret = new List<NarrativeRegionFill.Predicate>();
            var temp = new NarrativeRegionFill.Predicate[this.PredicatesCausedByInstancedObjectsAndRelations.Count];
            this.PredicatesCausedByInstancedObjectsAndRelations.CopyTo(temp);
            var predicatesOfInstancesCopy = temp.ToList();
            foreach (var predicateFiltered in PredicatesFilteredByCurrentLocation)
            {
                NarrativeRegionFill.Predicate foundPredicate = null;
                foreach (var placedPredicate in predicatesOfInstancesCopy)
                {
                    if (predicateFiltered.Equals(placedPredicate))
                    {
                        foundPredicate = placedPredicate;
                        break;
                    }
                }
                if (foundPredicate == null)
                {
                    ret.Add(predicateFiltered);
                }
                else
                {
                    predicatesOfInstancesCopy.Remove(foundPredicate);
                }
            }
            return ret;
        }

        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            NarrativeTimePoint e = (NarrativeTimePoint)obj;
            // Equals if either both from nodes are equal and both to nodes are equal or if they are reversed.
            return this.TimePoint.Equals(e.TimePoint);
        }

        public override int GetHashCode()
        {
            return this.TimePoint.GetHashCode();
        }

        internal void RegeneratePredicates()
        {
            this.PredicatesCausedByInstancedObjectsAndRelations = new List<NarrativeRegionFill.Predicate>();
            foreach (var rel in this.Configuration.InstancedRelations)
            {
                InstantiateRelationship(rel);
            }

            foreach (var obj in this.Configuration.InstancedObjects)
            {
                this.InstantiateAtPredicateForInstance(obj);
            }
        }
    }
}
