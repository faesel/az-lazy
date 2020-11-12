using System;
using System.Collections.Generic;

namespace az_lazy.Helpers
{
    public class Node
    {
        public string Name { get; set; }
        public List<Node> Children { get; set; } = new List<Node>();
    }

    public class Tree
    {
        // Constants for drawing lines and spaces
        private const string _cross = " ├─";
        private const string _corner = " └─";
        private const string _vertical = " │ ";
        private const string _space = "   ";

        public Tree()
        {
            List<Node> topLevelNodes = CreateNodeList();

            foreach (var node in topLevelNodes)
            { 
                PrintNode(node, indent: "");
            }
        }

        static void PrintNode(Node node, string indent)
        {
            Console.WriteLine(node.Name);

            // Loop through the children recursively, passing in the
            // indent, and the isLast parameter
            var numberOfChildren = node.Children.Count;
            for (var i = 0; i < numberOfChildren; i++)
            {
                var child = node.Children[i];
                var isLast = (i == (numberOfChildren - 1));
                PrintChildNode(child, indent, isLast);
            }
        }

        static void PrintChildNode(Node node, string indent, bool isLast)
        {
            // Print the provided pipes/spaces indent
            Console.Write(indent);

            // Depending if this node is a last child, print the
            // corner or cross, and calculate the indent that will
            // be passed to its children
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

        private static List<Node> CreateNodeList()
        {
            // Load/Create the nodes from somewhere
            List<Node> nodeList = new List<Node>
            {
                new Node()
                {
                    Name = "First Node",
                    Children = new List<Node>() { 
                        new Node() { Name = "Second" },
                        new Node() { Name = "Third" },
                        new Node() { 
                            Name = "Fourth",
                            Children = new List<Node>() 
                            { 
                                new Node() { Name = "Fifth" }
                            }
                        },
                    }
                }
            };
            return nodeList;
        }
    }
}