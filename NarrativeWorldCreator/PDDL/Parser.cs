using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.PDDL
{
    static class Parser
    {
        public static Narrative narrative = new Narrative();

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
