using System;
using System.Collections.Generic;
using System.Text;
using Common.MathParser;
using Common;

namespace Common.Shapes.Transformations
{
    public class TransformationType
    {
        static List<TransformationType> allTypes = GetAllTypes();

        public delegate List<object> TransformationFunction(TransformationNode.ApplicationData data, object inputShape, CustomTermEvaluater termEvaluater, List<string> parameters);

        private static List<TransformationType> GetAllTypes()
        {
            List<TransformationType> list = new List<TransformationType>();
            list.Add(new TransformationType("Split lines", Transformations.SplitLines,
                                            "li: lil, pol; lil: lil, pol; eli: epol, elil; elil: epol, elil ; pa: lil, pol",
                                            "split distance: 1: distance between 2 splits; equal pieces: true: if false, \nsplits are split distance except the last one"));
            list.Add(new TransformationType("Flatten extruded shapes", Transformations.FlattenExtrudedShapes,
                                            "epo: po ; eli: li ; esh: sh ; epol: pol ; elil: lil ; eshl : shl", ""));
            list.Add(new TransformationType("Brake up a shape or path in points and lines", Transformations.BrakeUpShape,
                                    "pa : pol, lil ; pal : pol, lil ; sh : pol, lil ; esh : epol, elil ; shl : pol, lil ; eshl : epol, elil", 
                                    ""));
            list.Add(new TransformationType("Get shape border", Transformations.GetShapeBorder,
                                            "sh: pa ; esh: pa ; shl: pal ; eshl: pal", ""));
            list.Add(new TransformationType("Create rows in a shape", Transformations.CreateRows,
                                            "sh: lil ; esh: elil",
                                            "row width: 1 ; " +
                                            "first/last row width: 0: an alternative width for first/last row\nenter 0 to use normal row width ; " +
                                            "row distance: 1: the distance between two rows ; " + 
                                            "orientation: longest: index of shape's line that's parallel to the rows\nor enter first, last, random, longest, shortest, xaxis or yaxis"));
            list.Add(new TransformationType("Get center of shape", Transformations.GetCenter, "po, epo, li, eli, sh, esh, pa: po", ""));
            return list;
        }

        public static void AddExtraTransformationType(TransformationType type)
        {
            allTypes.Add(type);
        }

        internal TransformationFunction transformation;
        string name;
        Dictionary<ShapeType, List<ShapeType>> possibleInputShapesWithOutput = new Dictionary<ShapeType, List<ShapeType>>();
        List<ShapeType> outputWithouthInput = new List<ShapeType>();
        internal List<Tuple<string, string, string>> parametersWithDefaultValueAndComment = new List<Tuple<string, string, string>>();

        public string Name { get { return name; } }

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="transformation"></param>
        /// <param name="possibleInputShapesWithOutput">format: 'Po, EPo: PoL, LiL ; Li: LiL, ELi; EPo: Po ; ...'</param>
        /// <param name="parameterList">format: p1: default value; p2: default value; p3: default value, ...</param>
        public TransformationType(string name, TransformationFunction transformation, string possibleInputShapesWithOutput, string parameterList)
        {
            this.name = name;
            this.transformation = transformation;
            if (possibleInputShapesWithOutput.Trim() != "")
            {
                string[] split = possibleInputShapesWithOutput.Split(';');
                foreach (string s1 in split)
                {
                    string[] split2 = s1.Split(':');
                    if (split2.Length != 2)
                        throw new Exception("Doe dat eens goed eh gast!!!");
                    int count = 0;
                    foreach (ShapeType st in ShapeType.GetListOfShapeTypesFromShorthandString(split2[0]))
                    {
                        this.possibleInputShapesWithOutput.Add(st, ShapeType.GetListOfShapeTypesFromShorthandString(split2[1]));
                        ++count;
                    }
                    if (count == 0)
                        outputWithouthInput.AddRange(ShapeType.GetListOfShapeTypesFromShorthandString(split2[1]));
                }
            }
            if (parameterList.Trim() != "")
            {
                string[] split = parameterList.Split(';');
                foreach (string s1 in split)
                {
                    string[] split2 = s1.Split(':');
                    if (split2.Length != 2 && split2.Length != 3)
                        throw new Exception("Doe dat eens goed eh gast!!!");
                    this.parametersWithDefaultValueAndComment.Add(new Tuple<string, string, string>(split2[0], split2[1], 
                                                                split2.Length > 2 ? split2[2] : ""));
                }
            }
        }

        internal List<ShapeType> GetOutputFor(ShapeNode input)
        {
            List<ShapeType> ret = new List<ShapeType>();
            if (possibleInputShapesWithOutput.ContainsKey(input.type))
                ret.AddRange(possibleInputShapesWithOutput[input.type]);
            ret.AddRange(outputWithouthInput);
            return ret;
        }

        internal TransformationNode Transform(ShapeNode input, out List<ShapeNode> outputNodes)
        {
            List<ShapeType> outputTypes = GetOutputFor(input);

            outputNodes = new List<ShapeNode>();
            foreach (ShapeType st in outputTypes)
                outputNodes.Add(new ShapeNode(input.Parent, st));

            return new TransformationNode(this, input, outputNodes);
        }

        internal int GetParameterIndex(string parameterName)
        {
            int count = 0;
            foreach (Tuple<string, string, string> t in parametersWithDefaultValueAndComment)
            {
                if (t.Item1 == parameterName)
                    return count;
                ++count;
            }
            return -1;
        }

        public static List<TransformationType> GetPossibleTransformations(ShapeType shapeType)
        {
            List<TransformationType> ret = new List<TransformationType>();
            foreach (TransformationType tt in allTypes)
                if (tt.possibleInputShapesWithOutput.ContainsKey(shapeType) || tt.outputWithouthInput.Count > 0)
                    ret.Add(tt);
            return ret;
        }

        internal static TransformationType GetFromName(string p)
        {
            foreach (TransformationType tt in allTypes)
                if (tt.name == p)
                    return tt;
            return null;
        }
    }
}
