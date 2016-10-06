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
        public static Narrative narrative = new Narrative();

        public Parser()
        {
            
        }

        public void parseDomain(String domainPath)
        {
            DomainParser.parseDomain(domainPath, narrative);
        }

        public void parseProblem(String problemPath)
        {
            ProblemParser.parseProblem(problemPath, narrative);
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

        internal void parsePlan(string planPath)
        {
            PlanParser.parsePlan(planPath, narrative);
        }
    }
}
