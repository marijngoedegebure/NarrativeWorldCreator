using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.PDDL
{
    class Parser
    {
        public List<Type> types = new List<Type>();
        public List<Predicate> predicates = new List<Predicate>();

        public Parser()
        {

        }

        // Domain parser can either come across the following kinds of words (in order): requirements, types, predicates, action
        public void parseDomain(String domainPath)
        {
            // List<String> lines = new List<String>(File.ReadAllLines(domainPath));
            String text = File.ReadAllText(domainPath);
            text = text.Replace("(", "");
            text = text.Replace(")", "");
            text = text.Replace(":", "");
            text = text.Replace("\t", "");
            text = text.Replace("\r", "");
            string[] lines = text.Split(new char[] { '\n' });
            bool readPredicatesMode = false;
            foreach(String line in lines)
            {
                string[] words = line.Split(null);
                if (readPredicatesMode)
                {
                    readPredicates(words);
                    continue;
                }
                if (words[0].Equals("define"))
                    continue;
                if (words[0].Equals("requirements"))
                    continue;
                if (words[0].Equals("types"))
                {
                    readTypes(words);
                    continue;
                }
                if (words[0].Equals("predicates"))
                {
                    readPredicatesMode = true;
                    continue;
                }
                if (words[0].Equals("actions"))
                {
                    readPredicatesMode = false;
                    break;
                }
            }
        }

        private void readPredicates(string[] words)
        {
            Predicate predicate = new Predicate(words[0]);
            List<String> arguments = new List<string>();
            for(int i = 1; i < words.Length; i++)
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

                foreach(Type type in types)
                {
                    if (words[i].Equals(type.name))
                    {
                        foreach(String argument in arguments)
                        {
                            predicate.addArgument(type);
                        }
                        arguments.Clear();
                        break;
                    }
                }
            }
            predicates.Add(predicate);
        }

        private void readTypes(string[] words)
        {
            for(int i = 1; i < words.Length; i++)
            {
                types.Add(new Type(words[i]));
            }
        }
    }
}
