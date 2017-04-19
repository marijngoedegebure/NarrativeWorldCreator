using NarrativeWorldCreator.Models.NarrativeInput;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Parsers
{
    class ProblemParser
    {
        public static void parseProblem(String problemPath)
        {
            string[] lines = Parser.parseText(File.ReadAllText(problemPath));
            bool readInitMode = false;
            bool readObjectsMode = false;
            List<NarrativeObject> narrativeObjects = new List<NarrativeObject>();
            List<Predicate> predicates = new List<Predicate>();
            foreach (String line in lines)
            {
                string[] words = line.Split(null);
                if (words[0].Equals(""))
                    continue;
                if (words[0].Equals("define"))
                    continue;
                if (words[0].Equals("domain"))
                    continue;
                if (words[0].Equals("objects"))
                {
                    readObjectsMode = true;
                    if (words.Length > 1)
                        narrativeObjects.AddRange(readObjects(words, Parser.narrative.NarrativeObjectTypes));
                    continue;
                }
                if (words[0].Equals("init"))
                {
                    readObjectsMode = false;
                    readInitMode = true;
                    if (words.Length > 1)
                        predicates.Add(readPredicate(words, Parser.narrative.PredicateTypes, Parser.narrative.NarrativeObjectTypes, narrativeObjects));
                    continue;
                }
                if (readObjectsMode)
                {
                    narrativeObjects.AddRange(readObjects(words, Parser.narrative.NarrativeObjectTypes));
                    continue;
                }

                if (readInitMode)
                {
                    predicates.Add(readPredicate(words, Parser.narrative.PredicateTypes, Parser.narrative.NarrativeObjectTypes, narrativeObjects));
                    continue;
                }
            }
            Parser.narrative.NarrativeObjects = narrativeObjects;
            Parser.narrative.StartingPredicates = predicates;
        }

        private static Predicate readPredicate(string[] words, ICollection<PredicateType> predicateTypes, ICollection<NarrativeObjectType> types, ICollection<NarrativeObject> narrativeObjects)
        {
            // Check if predicatetype exists:
            Predicate predicate = new Predicate();
            int startIndex = 0;
            if (words[0].Equals("init"))
                startIndex = 1;
            foreach(PredicateType predicateType in predicateTypes)
            {
                if (predicateType.Name.Equals(words[startIndex]))
                {
                    predicate.PredicateType = predicateType;
                    break;
                }
            }
            if (predicate.PredicateType == null)
                throw new Exception("Predicate type does not exist");
            for (int i = startIndex + 1; i < words.Length; i++)
            {
                // Check if narrative object exists and whether type of it is equal to one of predicateType
                var narrativeObject = (from no in narrativeObjects
                                      where words[i].Equals(no.Name)
                                      select no).FirstOrDefault();
                if (narrativeObject != null)
                {
                    if (narrativeObject.Type.Name.Equals(predicate.PredicateType.Arguments[i - 1].Type.Name) || narrativeObject.Type.ParentType.Name.Equals(predicate.PredicateType.Arguments[i - 1].Type.Name))
                    {
                        predicate.EntikaClassNames.Add(narrativeObject.Name);
                    }
                    else
                    {
                        throw new Exception("Narrative object type not equal to narrative predicate type");
                    }
                }
                else
                {
                    throw new Exception("Narrative object in predicate not defined as object");
                }
            }
            return predicate;
        }

        private static ICollection<NarrativeObject> readObjects(string[] words, ICollection<NarrativeObjectType> types)
        {
            List<NarrativeObject> narrativeObjects = new List<NarrativeObject>();
            bool typeExists = false;
            NarrativeObjectType typeToUse = types.First();
            // check if type exists
            foreach(NarrativeObjectType type in types)
            {
                if(words.Last().Equals(type.Name))
                {
                    typeExists = true;
                    typeToUse = type;
                    break;
                }
            }
            if (!typeExists)
                throw new Exception("Narrative type does not exist");
            int startIndex = 0;
            if (words[0].Equals("objects"))
                startIndex = 1;
            for (int i = startIndex; i < words.Length; i++)
            {
                if (words[i].Equals("-"))
                    break;
                else
                {
                    NarrativeObject narrativeObject = new NarrativeObject();
                    narrativeObject.Name = words[i];
                    narrativeObject.Type = typeToUse;
                    narrativeObjects.Add(narrativeObject);
                }
            }
            return narrativeObjects;
        }
    }
}
