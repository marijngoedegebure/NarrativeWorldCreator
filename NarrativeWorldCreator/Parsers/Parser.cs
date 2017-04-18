using NarrativeWorldCreator.Models.NarrativeInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Parsers
{
    public static class Parser
    {
        public static string LocationTypeName { get; set; }
        public static string CharacterTypeName { get; set; }
        public static string ObjectTypeName { get; set; }
        public static string MoveActionName { get; set; }

        public static Narrative narrative { get; set; }

        public static Narrative parse(string LocationTypeName, string CharacterTypeName, string ObjectTypeName, string MoveActionName, string domainPath, string problemPath, string planPath)
        {
            Parser.LocationTypeName = LocationTypeName;
            Parser.CharacterTypeName = CharacterTypeName;
            Parser.ObjectTypeName = ObjectTypeName;
            Parser.MoveActionName = MoveActionName;
            Parser.narrative = new Narrative();
            parseDomain(domainPath);
            parseProblem(problemPath);
            parsePlan(planPath);
            return narrative;
        }
        
        public static void parseDomain(String domainPath)
        {
            DomainParser.parseDomain(domainPath);
        }

        public static void parseProblem(String problemPath)
        {
            ProblemParser.parseProblem(problemPath);
        }

        public static String[] parseText(String text)
        {
            text = text.Replace("(", "");
            text = text.Replace(")", "");
            text = text.Replace(":", "");
            text = text.Replace("\t", "");
            text = text.Replace("\r", "");
            String[] lines = text.Split(new char[] { '\n' });
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Trim();
            }
            return lines;
        }

        public static void parsePlan(string planPath)
        {
            PlanParser.parsePlan(planPath);
        }
    }
}
