using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.TreeLayouter
{
    public class SimpleLayouter : Layouter
    {
        static SimpleLayouter instance = new SimpleLayouter();
        public static SimpleLayouter Instance { get { return instance; } }

        private SimpleLayouter() { }

        protected override Layout CreateLayout(Node root)
        {
            Layout layout = new Layout();
            int levels = GetNumberOfLevels(root);
            float levelHeight = 1f / (float)levels;
            Vec2 currentPosition = new Vec2(0.5f, 0.5f * levelHeight);
            layout.Add(root, currentPosition);
            DivideChildren(layout, root, currentPosition, 0, 1, levelHeight);
            return layout;
        }

        private void DivideChildren(Layout layout, Node node, Vec2 nodePosition, float minX, float maxX, float levelHeight)
        {
            int totalWeight = 0;
            List<int> weights = new List<int>();
            foreach (Node child in node.Children)
            {
                int max = Math.Max(1, GetMaximumNumberOfNodesOnSingleLevel(child));
                totalWeight += max;
                weights.Add(max);
            }

            int index = 0;
            float currentX = minX;
            foreach (Node child in node.Children)
            {
                int weight = weights[index];
                float relativeWeight = (float)weight / (float)totalWeight;
                float width = relativeWeight * (maxX - minX);

                Vec2 position = new Vec2(currentX + 0.5f * width, nodePosition.Y + levelHeight);
                layout.Add(child, position);
                DivideChildren(layout, child, position, currentX, currentX + width, levelHeight);

                currentX += width;
                ++index;
            }
        }
    }
}
