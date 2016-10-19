using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.TreeLayouter
{
    public abstract class Layouter
    {
        public Layout Create(Node root)
        {
            return CreateLayout(root);
        }

        protected abstract Layout CreateLayout(Node root);

        protected int GetMaximumNumberOfNodesOnSingleLevel(Node node)
        {
            List<int> numberOfNodesPerLevel = new List<int>();
            GetNumberOfNodesPerLevel(numberOfNodesPerLevel, node, 0);
            if (numberOfNodesPerLevel.Count == 0)
                return 0;
            numberOfNodesPerLevel.Sort();
            return numberOfNodesPerLevel[numberOfNodesPerLevel.Count - 1];
        }

        private void GetNumberOfNodesPerLevel(List<int> numberOfNodesPerLevel, Node node, int currentLevel)
        {
            if (numberOfNodesPerLevel.Count <= currentLevel)
                numberOfNodesPerLevel.Add(0);
            foreach (Node child in node.Children)
            {
                GetNumberOfNodesPerLevel(numberOfNodesPerLevel, child, currentLevel + 1);
                ++numberOfNodesPerLevel[currentLevel];
            }
        }

        protected int GetNumberOfLevels(Node node)
        {
            int max = 0;
            foreach (Node n in node.Children)
                max = Math.Max(max, GetNumberOfLevels(n));
            return max + 1;
        }
    }
}
