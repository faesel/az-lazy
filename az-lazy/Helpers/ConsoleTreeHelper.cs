using System;
using System.Collections.Generic;
using System.Drawing;
using Pastel;

namespace az_lazy.Helpers
{
    public class Tree
    {
        private static readonly string cross = " ├─".Pastel(Colours.HighlightColour);
        private static readonly string corner = " └─".Pastel(Colours.HighlightColour);
        private static readonly string vertical = " │ ".Pastel(Colours.HighlightColour);
        private const string space = "   ";

        public Tree(List<TreeNode> treeNodeList)
        {
            foreach (var node in treeNodeList)
            {
                PrintNode(node, indent: "");
            }
        }

        private static void PrintNode(TreeNode node, string indent)
        {

            var information = string.IsNullOrEmpty(node.Information) ? string.Empty : $" - {node.Information.Pastel(Colours.InformationColour)}";

            Console.WriteLine(node.Name + information);

            var numberOfChildren = node.Children.Count;
            for (var i = 0; i < numberOfChildren; i++)
            {
                var child = node.Children[i];
                var isLast = i == (numberOfChildren - 1);
                PrintChildNode(child, indent, isLast);
            }
        }

        private static void PrintChildNode(TreeNode node, string indent, bool isLast)
        {
            Console.Write(indent);

            if (isLast)
            {
                Console.Write(corner);
                indent += space;
            }
            else
            {
                Console.Write(cross);
                indent += vertical;
            }

            PrintNode(node, indent);
        }
    }
}