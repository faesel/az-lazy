using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using az_lazy.Commands.Container.Dto;
using az_lazy.Manager;
using Spectre.Console;

namespace az_lazy.Commands.Container.Executor
{
    public class TreeExecutor : ICommandExecutor<ContainerOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureContainerManager AzureContainerManager;

        public TreeExecutor(
            ILocalStorageManager localStorageManager,
            IAzureContainerManager azureContainerManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureContainerManager = azureContainerManager;
        }

        public async Task Execute(ContainerOptions opts)
        {
            if(!string.IsNullOrEmpty(opts.Tree))
            {
                try
                {
                    AnsiConsole.MarkupLine($"Getting blobs in container {opts.Tree} ... ");
                    var selectedConnection = LocalStorageManager.GetSelectedConnection();
                    var treeNodes = await AzureContainerManager.ContainerTree(
                        selectedConnection.ConnectionString,
                        opts.Tree,
                        opts.Depth,
                        opts.Detailed,
                        opts.Prefix);

                    if(treeNodes.Count > 0)
                    {
                        AnsiConsole.MarkupLine($"Getting blobs in container {opts.Tree} ... [bold green]Successful[/]");
                        AnsiConsole.Render(new Rule());

                        //First node always represents the container name
                        var firstNode = treeNodes[0];
                        var tree = new Tree(firstNode.Name)
                            .Guide(TreeGuide.BoldLine)
                            .Style("yellow");

                        AddChildren(tree, null, firstNode.Children);

                        AnsiConsole.Render(tree);
                    }
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"Getting blobs in container {opts.Tree} ... [bold red]Failed[/]");
                    AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                }
            }
        }

        private void AddChildren(Tree tree, TreeNode childNode, List<BlobTreeNode> children)
        {
            if(childNode == null)
            {
                foreach(var child in children)
                {
                    var information = string.IsNullOrEmpty(child.Information) ? string.Empty : $"[grey] - {child.Information}[/]";
                    var node = tree.AddNode(markup: child.Name + information);

                    AddChildren(tree, node, child.Children);
                }
            }
            else
            {
                foreach(var child in children)
                {
                    var information = string.IsNullOrEmpty(child.Information) ? string.Empty : $"[grey] - {child.Information}[/]";
                    var node = childNode.AddNode(child.Name + information);

                    AddChildren(tree, node, child.Children);
                }
            }
        }
    }
}