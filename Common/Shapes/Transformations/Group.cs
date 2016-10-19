using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Shapes.Transformations
{
    public class Group : CommandHandler
    {
        string name;
        List<ShapeNode> nodes = new List<ShapeNode>();

        public string Name
        {
            get { return name; }
            set { ChangeProperty("Name", value); }
        }

        public Group(string name)
        {
            this.name = name;
        }

        public void AddNode(ShapeNode node)
        {
            this.nodes.Add(node);
        }
            
        protected override void ChangeProperty(string property, object newValue)
        {
            switch(property)
            {
                case "Name":
                    name = (string)newValue;
                    break;
                default:
 	                throw new NotImplementedException();
            }
        }

        protected override void ChangeList(string list, object element, Common.ListChangedCommand.Type type)
        {
 	        throw new NotImplementedException();
        }
    }
}
