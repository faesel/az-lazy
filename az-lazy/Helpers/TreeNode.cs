using System.Collections.Generic;

namespace az_lazy.Helpers
{
    public class TreeNode
    {
        public string Name { get; set; }
        public List<TreeNode> Children { get; set; } = new List<TreeNode>();
    }
}