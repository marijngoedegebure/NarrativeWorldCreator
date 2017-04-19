using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NarrativeWorldCreator.Models.NarrativeInput;
using NarrativeWorldCreator.Models.NarrativeRegionFill;

namespace NarrativeWorldCreator.Parsers
{
    class DomainParser
    {
        public static void parseDomain(String domainPath)
        {
            string[] lines = Parser.parseText(File.ReadAllText(domainPath));
            bool readPredicatesMode = false;
            bool readActionMode = false;
            bool readTypeMode = false;
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
                {
                    Parser.narrative.Name = words.Last();
                    continue;
                }
                if (words[0].Equals("requirements"))
                    continue;
                if (words[0].Equals("types"))
                {
                    readTypeMode = true;
                    if (words.Length > 1)
                        types = readTypes(words, types);
                    continue;
                }
                if (words[0].Equals("predicates"))
                {
                    readTypeMode = false;
                    readPredicatesMode = true;
                    if (words.Length > 1)
                        predicateTypes.Add(readPredicateTypes(words, types));
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
                if (readTypeMode)
                {
                    types.AddRange(readTypes(words, types));
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
                        currentNarrativeAction.Parameters = readParameters(words, types);
                        continue;
                    }
                    if (words[0].Equals("preconditions"))
                    {
                        currentNarrativeAction.Preconditions = readValuedPredicates(words, currentNarrativeAction, predicateTypes);
                        continue;
                    }
                    if (words[0].Equals("effect"))
                    {
                        currentNarrativeAction.Effects = readValuedPredicates(words, currentNarrativeAction, predicateTypes);
                        narrativeActions.Add(currentNarrativeAction);
                        continue;
                    }
                }
            }
            Parser.narrative.PredicateTypes = predicateTypes;
            Parser.narrative.NarrativeObjectTypes = types;
            Parser.narrative.NarrativeActions = narrativeActions;
        }

        private static List<NarrativeEffect> readValuedPredicates(string[] words, NarrativeAction currentNarrativeAction, List<PredicateType> predicateTypes)
        {
            List<NarrativeEffect> predicates = new List<NarrativeEffect>();
            var currentValuedPredicate = new NarrativeEffect();
            for (int i = 1; i < words.Length; i++)
            {
                if (words[i].Equals(""))
                    continue;
                if (words[i].Equals("and"))
                {
                    continue;
                }
                if (words[i].Equals("preconditions"))
                {
                    continue;
                }
                if (words[i].First() == '?')
                {
                    continue;
                }
                if (words[i].Equals("not"))
                {
                    currentValuedPredicate.Value = false;
                    continue;
                }
                var type = (from predicateType in predicateTypes
                           where predicateType.Name.Equals(words[i])
                           select predicateType).FirstOrDefault();

                if (type != null)
                {
                    if (type.Arguments.Count + i >= words.Length)
                    {
                        throw new Exception("Not enough arguments for predicate");
                    }
                    for (int j = 1; j <= type.Arguments.Count; j++)
                    {
                        if (words[i + j].First() != '?')
                        {
                            throw new Exception("Not enough arguments for predicate");
                        }
                        else
                        {
                            for (int k = 0; k < currentNarrativeAction.Parameters.Count; k++)
                            {
                                if (currentNarrativeAction.Parameters[k].Name.Equals(words[i + j]))
                                {
                                    currentValuedPredicate.ArgumentsAffected.Add(currentNarrativeAction.Parameters[k]);
                                    break;
                                }
                            }
                        }
                    }
                    currentValuedPredicate.PredicateType = type;
                    predicates.Add(currentValuedPredicate);
                    currentValuedPredicate = new NarrativeEffect();
                }
            }
            return predicates;
        }

        private static List<NarrativeArgument> readParameters(string[] words, List<NarrativeObjectType> types)
        {
            List<NarrativeArgument> parameters = new List<NarrativeArgument>();
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
                            narrativeArgument.Name = argument;
                            narrativeArgument.Type = type;
                            parameters.Add(narrativeArgument);
                        }
                        arguments.Clear();
                        break;
                    }
                }
            }
            return parameters;
        }

        private static PredicateType readPredicateTypes(string[] words, List<NarrativeObjectType> types)
        {
            PredicateType predicate = new PredicateType();
            predicate.Name = words[0];
            List<String> arguments = new List<string>();
            int startIndex = 0;
            if (words[0].Equals("predicates"))
                startIndex = 1;
            for (int i = startIndex; i < words.Length; i++)
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
                            narrativeArgument.Name = argument;
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

        private static List<NarrativeObjectType> readTypes(string[] words, List<NarrativeObjectType> types)
        {
            List<NarrativeObjectType> outputTypes = new List<NarrativeObjectType>();
            int startIndex = 0;
            if (words[0].Equals("types"))
                startIndex = 1;
            bool parentTypeMode = false;
            for (int i = startIndex; i < words.Length; i++)
            {
                if (words[i].Equals("-"))
                {
                    parentTypeMode = true;
                    continue;
                }
                NarrativeObjectType type = new NarrativeObjectType();
                type.Name = words[i];
                if (parentTypeMode)
                {
                    NarrativeObjectType parentType = type;
                    bool typeAlreadyExist = false;
                    foreach (NarrativeObjectType potentialParentType in types)
                    {
                        if (type.Equals(potentialParentType))
                        {
                            parentType = potentialParentType;
                            typeAlreadyExist = true;
                        }
                    }
                    foreach (NarrativeObjectType ntype in outputTypes)
                    {
                        ntype.ParentType = parentType;
                    }
                    if (typeAlreadyExist)
                        continue;
                }
                outputTypes.Add(type);
            }
            return outputTypes;
        }
    }
}
