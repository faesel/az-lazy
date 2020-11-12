using System;
using System.Collections.Generic;
using System.Drawing;
using Pastel;

namespace az_lazy.Helpers
{
    public class Tree
    {
        private static readonly Color TreeColor = Color.Yellow;
        private static readonly string _cross = " ├─".Pastel(TreeColor);
        private static readonly string _corner = " └─".Pastel(TreeColor);
        private static readonly string _vertical = " │ ".Pastel(TreeColor);
        private const string _space = "   ";

        public Tree(List<TreeNode> treeNodeList)
        {
            foreach (var node in treeNodeList)
            {
                PrintNode(node, indent: "");
            }
        }

        static void PrintNode(TreeNode node, string indent)
        {
            Console.WriteLine(node.Name);

            var numberOfChildren = node.Children.Count;
            for (var i = 0; i < numberOfChildren; i++)
            {
                var child = node.Children[i];
                var isLast = (i == (numberOfChildren - 1));
                PrintChildNode(child, indent, isLast);
            }
        }

        static void PrintChildNode(TreeNode node, string indent, bool isLast)
        {
            Console.Write(indent);

            if (isLast)
            {
                Console.Write(_corner);
                indent += _space;
            }
            else
            {
                Console.Write(_cross);
                indent += _vertical;
            }

            PrintNode(node, indent);
        }
    }
}