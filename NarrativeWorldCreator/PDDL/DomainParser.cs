using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.PDDL
{
    class DomainParser
    {
        public static Narrative parseDomain(String domainPath, Narrative narrative)
        {
            string[] lines = Parser.parseText(File.ReadAllText(domainPath));
            bool readPredicatesMode = false;
            List<PredicateType> predicateTypes = new List<PredicateType>();
            List<Type> types = new List<Type>();
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
                    break;
                }
                if (readPredicatesMode)
                {
                    predicateTypes.Add(readPredicateTypes(words, types));
                    continue;
                }
            }
            narrative.predicateTypes = predicateTypes;
            narrative.types = types;
            return narrative;
        }

        private static PredicateType readPredicateTypes(string[] words, List<Type> types)
        {
            PredicateType predicate = new PredicateType(words[0]);
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

                foreach (Type type in types)
                {
                    if (words[i].Equals(type.name))
                    {
                        foreach (String argument in arguments)
                        {
                            predicate.addArgument(type);
                        }
                        arguments.Clear();
                        break;
                    }
                }
            }
            return predicate;
        }

        private static List<Type> readTypes(string[] words)
        {
            List<Type> types = new List<Type>();
            for (int i = 1; i < words.Length; i++)
            {
                types.Add(new Type(words[i]));
            }
            return types;
        }
    }
}
