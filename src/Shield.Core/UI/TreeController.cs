﻿namespace Shield.Core.UI
{
    using System.Linq;
    using System.Net.Http.Formatting;
    using Models;
    using Umbraco.Web.Models.Trees;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The Umbraco Access Tree Controller for the custom section
    /// </summary>
    [PluginController(Constants.App.Name)]
    [Umbraco.Web.Trees.Tree(Constants.App.Alias, Constants.Tree.Alias, Constants.Tree.Title)]
    public class TreeController : Umbraco.Web.Trees.TreeController
    {
        /// <summary>
        /// Gets the menu for a node by it's Id.
        /// </summary>
        /// <param name="id">
        /// The Id of the node.
        /// </param>
        /// <param name="queryStrings">
        /// The query string parameters
        /// </param>
        /// <returns>
        /// Menu Item Collection containing the Menu Item(s).
        /// </returns>
        protected override MenuItemCollection GetMenuForNode(string idText, FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();
            int id = int.Parse(idText);

            if(id == global::Umbraco.Core.Constants.System.Root || id == Constants.Tree.EnvironmentsRootId)
            {
                menu.Items.Add(new MenuItem("createEnvironment", "Create Environment"));
            }

            return menu;
        }

        /// <summary>
        /// Gets the Tree Node Collection.
        /// </summary>
        /// <param name="id">
        /// The Id.
        /// </param>
        /// <param name="queryStrings">
        /// the query string parameters.
        /// </param>
        /// <returns>
        /// Tree Node Collection containing the Tree Node(s).
        /// </returns>
        protected override TreeNodeCollection GetTreeNodes(string idText, FormDataCollection queryStrings)
        {
            int id = int.Parse(idText);
            var treeNodeCollection = new TreeNodeCollection();
            var environments = Operation.JobService.Instance.Environments;
            
            if (id == global::Umbraco.Core.Constants.System.Root)
            {
                treeNodeCollection.Add(
                    CreateTreeNode(
                        Constants.Tree.EnvironmentsRootId.ToString(),
                        global::Umbraco.Core.Constants.System.Root.ToString(),
                        queryStrings,
                        "Environments",
                        "icon-folder",
                        environments.Any()));

                return treeNodeCollection;
            }

            if (id == Constants.Tree.EnvironmentsRootId)
            {
                if(environments != null && environments.Any())
                {
                    foreach(var environment in environments)
                    {
                        var environmentId = environment.Key.Id.ToString();
                        var node = CreateTreeNode(
                            environmentId,
                            Constants.Tree.EnvironmentsRootId.ToString(),
                            queryStrings,
                            environment.Key.Name,
                            ((Environment) environment.Key).Icon,
                            environment.Value.Any());

                        //var publish = false;
                        //foreach (var job in environment.Value)
                        //{
                        //    if (job.ReadConfiguration().Enable)
                        //    {
                        //        publish = true;
                        //        break;
                        //    }
                        //}

                        //if (!publish)
                        //{
                        //    node.SetNotPublishedStyle();
                        //}

                        treeNodeCollection.Add(node);
                    }
                }
                return treeNodeCollection;
            }

            foreach (var environment in environments)
            {
                if (environment.Key.Id == id)
                {
                    foreach (var job in environment.Value)
                    {
                        var app = App<IConfiguration>.Create(job.AppId);
                        var jobId = job.Id.ToString();
                        var node = CreateTreeNode(
                            jobId,
                            environment.Key.Id.ToString(),
                            queryStrings,
                            app.Name,
                            app.Icon,
                            false);

                        if (!job.ReadConfiguration().Enable)
                        {
                            node.SetNotPublishedStyle();
                        }
                        treeNodeCollection.Add(node);
                    }
                    return treeNodeCollection;
                }
            }
            return treeNodeCollection;
        }
    }
}
