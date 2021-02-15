using System.Collections.Generic;

namespace az_lazy.Commands.Container.Dto
{
    public class BlobTreeNode
    {
        public string Name { get; set; }
        public string Information { get; set; }
        public List<BlobTreeNode> Children { get; set; } = new List<BlobTreeNode>();
    }
}