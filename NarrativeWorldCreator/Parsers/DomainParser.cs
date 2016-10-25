using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Narratives;

namespace NarrativeWorldCreator.Parsers
{
    class DomainParser
    {
        public static void parseDomain(String domainPath)
        {
            string[] lines = Parser.parseText(File.ReadAllText(domainPath));
            bool readPredicatesMode = false;
            bool readActionMode = true;
            List<PredicateType> predicateTypes = new List<PredicateType>();
            List<NarrativeObjectType> types = new List<NarrativeObjectType>();
            List<NarrativeAction> narrativeActions = new List<NarrativeAction>();
            NarrativeAction currentNarrativeAction = null;
            foreach (String line in lines)
            {
                string[] words = line.Split(null);
                if (words[0].Equals(""))
                    continue;
                if (words[0].Equals("define"))
                    continue;
                if (words[0].Equals("requirements"))
                    continue;
                if (words[0].Equals("types"))
                {
                    types = readTypes(words);
                    continue;
                }
                if (words[0].Equals("predicates"))
                {
                    readPredicatesMode = true;
                    continue;
                }
                if (words[0].Equals("action"))
                {
                    readPredicatesMode = false;
                    readActionMode = true;
                    currentNarrativeAction = new NarrativeAction();
                    currentNarrativeAction.Name = words[1];
                    continue;
                }
                if (readPredicatesMode)
                {
                    predicateTypes.Add(readPredicateTypes(words, types));
                    continue;
                }
                if (readActionMode)
                {
                    if(words[0].Equals("parameters"))
                    {
                        narrativeActions.Add(readParameters(words, currentNarrativeAction, types));
                        continue;
                    }
                    if(words[0].Equals("preconditions"))
                        continue;
                    if (words[0].Equals("effect"))
                        continue; 
                }
            }
            SystemStateTracker.NarrativeWorld.Narrative.PredicateTypes = predicateTypes;
            SystemStateTracker.NarrativeWorld.Narrative.NarrativeObjectTypes = types;
            SystemStateTracker.NarrativeWorld.Narrative.NarrativeActions = narrativeActions;
        }

        private static NarrativeAction readParameters(string[] words, NarrativeAction currentNarrativeAction, List<NarrativeObjectType> types)
        {
            List<String> arguments = new List<string>();
            for (int i = 1; i < words.Length; i++)
            {
                if (words[i].Equals(""))
                    continue;
                if (words[i].First() == '?')
                {
                    arguments.Add(words[i]);
                    continue;
                }
                if (words[i].Equals("-"))
                    continue;

                foreach (NarrativeObjectType type in types)
                {
                    if (words[i].Equals(type.Name))
                    {
                        foreach (String argument in arguments)
                        {
                            NarrativeArgument narrativeArgument = new NarrativeArgument();
                            narrativeArgument.Type = type;
                            currentNarrativeAction.Arguments.Add(narrativeArgument);
                        }
                        arguments.Clear();
                        break;
                    }
                }
            }
            return currentNarrativeAction;
        }

        private static PredicateType readPredicateTypes(string[] words, List<NarrativeObjectType> types)
        {
            PredicateType predicate = new PredicateType();
            predicate.Name = words[0];
            List<String> arguments = new List<string>();
            for (int i = 1; i < words.Length; i++)
            {
                if (words[i].Equals(""))
                    continue;
                if (words[i].First() == '?')
                {
                    arguments.Add(words[i]);
                    continue;
                }
                if (words[i].Equals("-"))
                    continue;

                foreach (NarrativeObjectType type in types)
                {
                    if (words[i].Equals(type.Name))
                    {
                        foreach (String argument in arguments)
                        {
                            NarrativeArgument narrativeArgument = new NarrativeArgument();
                            narrativeArgument.Type = type;
                            predicate.Arguments.Add(narrativeArgument);
                        }
                        arguments.Clear();
                        break;
                    }
                }
            }
            return predicate;
        }

        private static List<NarrativeObjectType> readTypes(string[] words)
        {
            List<NarrativeObjectType> types = new List<NarrativeObjectType>();
            for (int i = 1; i < words.Length; i++)
            {
                NarrativeObjectType type = new NarrativeObjectType();
                type.Name = words[i];
                using (var db = new NarrativeContext())
                {
                    db.NarrativeObjectTypes.Add(type);
                    db.SaveChanges();
                }
                types.Add(type);
            }
            return types;
        }
    }
}
