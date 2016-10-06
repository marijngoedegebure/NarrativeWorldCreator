using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.PDDL
{
    class ProblemParser
    {
        public static Narrative parseProblem(String problemPath, Narrative narrative)
        {
            string[] lines = Parser.parseText(File.ReadAllText(problemPath));
            bool readInitMode = false;
            bool readObjectsMode = false;
            List<NarrativeObject> narrativeObjects = new List<NarrativeObject>();
            List<NarrativePredicate> narrativePredicates = new List<NarrativePredicate>();
            foreach (String line in lines)
            {
                string[] words = line.Split(null);
                if (words[0].Equals(""))
                    continue;
                if (words[0].Equals("define"))
                    continue;
                if (words[0].Equals("objects"))
                {
                    readObjectsMode = true;
                    continue;
                }
                if (words[0].Equals("init"))
                {
                    readObjectsMode = false;
                    readInitMode = true;
                    continue;
                }
                if (readObjectsMode)
                {
                    narrativeObjects.AddRange(readObjects(words, narrative.types));
                    continue;
                }
                if (readInitMode)
                {
                    narrativePredicates.Add(readNarrativePredicate(words, narrative.predicateTypes, narrative.types, narrativeObjects));
                    continue;
                }
            }
            narrative.narrativeObjects = narrativeObjects;
            narrative.narrativePredicates = narrativePredicates;
            return narrative;
        }

        private static NarrativePredicate readNarrativePredicate(string[] words, List<PredicateType> predicateTypes, List<Type> types, List<NarrativeObject> narrativeObjects)
        {
            // Check if predicatetype exists:
            PredicateType usedPredicateType = predicateTypes.First();
            NarrativePredicate narrativePredicate = new NarrativePredicate();
            foreach(PredicateType predicateType in predicateTypes)
            {
                if (predicateType.name.Equals(words[0]))
                {
                    narrativePredicate.predicateType = predicateType;
                    break;
                }
            }
            if (narrativePredicate.predicateType == null)
                throw new Exception("Predicate type does not exist");
            for (int i = 1; i < words.Length; i++)
            {
                // Check if narrative object exists and whether type of it is equal to one of predicateType
                foreach(NarrativeObject narrativeObject in narrativeObjects)
                {
                    if (words[i].Equals(narrativeObject.name))
                    {
                        if (narrativeObject.type.name.Equals(narrativePredicate.predicateType.arguments[i-1].type.name))
                        {
                            narrativePredicate.narrativeObjects.Add(narrativeObject);
                            break;
                        }
                        else
                        {
                            throw new Exception("Narrative object type not equal to narrative predicate type");
                        }
                    }
                }
            }
            return narrativePredicate;
        }

        private static List<NarrativeObject> readObjects(string[] words, List<Type> types)
        {
            List<NarrativeObject> narrativeObjects = new List<NarrativeObject>();
            bool typeExists = false;
            Type typeToUse = types.First();
            // check if type exists
            foreach(Type type in types)
            {
                if(words.Last().Equals(type.name))
                {
                    typeExists = true;
                    typeToUse = type;
                    break;
                }
            }
            if (!typeExists)
                throw new Exception("Narrative type does not exist");
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Equals("-"))
                    break;
                else
                {
                    narrativeObjects.Add(new NarrativeObject(words[i], typeToUse));
                }

            }
            return narrativeObjects;
        }
    }
}
