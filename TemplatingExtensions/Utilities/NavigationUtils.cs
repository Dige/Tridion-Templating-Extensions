using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using TemplatingExtensions.Helpers;
using Tridion.ContentManager;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.ContentManagement.Fields;
using Tridion.ContentManager.Publishing;

namespace TemplatingExtensions.Utilities
{
    /* TODO: CLEAN THIS CLASS */
    //public static class NavigationUtils
    //{
    //    /// <summary>
    //    /// Returns the list of structure groups of the current publication. This call is cheap, but the results 
    //    /// are shallow.
    //    /// </summary>
    //    /// <remarks>
    //    /// Note that the objects returned are not TOM.NET StructureGroup objects, but a more shallow
    //    /// type of object. If you require the full functionality of TOM.NET StructureGroup objects, you should 
    //    /// call the more expensive <c>GetStructureGroups</c> method.
    //    /// </remarks>
    //    /// <example>
    //    /// The results are unsorted. Sorting the result in place can be done with:
    //    /// <code>
    //    /// SGs.Sort(
    //    ///     delegate(ListItem item1, ListItem item2)
    //    ///     {
    //    ///         return item1.Title.CompareTo(item2.Title);
    //    ///     }
    //    /// );
    //    /// </code>
    //    /// </example>
    //    /// <returns>the list of structure groups of the current publication</returns>
    //    /// <seealso cref="ListItem"/>
    //    /// <seealso cref="GetStructureGroups"/>
    //    public static IList<ListItem> GetListStructureGroups(Publication pub)
    //    {
    //        OrganizationalItemsFilter filter = new OrganizationalItemsFilter(pub.Session)
    //                                               {
    //                                                   ItemTypes = new List<ItemType> {ItemType.StructureGroup},
    //                                                   BaseColumns = ListBaseColumns.Extended,
    //                                                   IncludePathColumn = true
    //                                               };

    //        XmlElement orgItems = pub.GetListOrganizationalItems(filter);

    //        XmlNodeList itemElements = orgItems.SelectNodes("*");
    //        List<ListItem> result = new List<ListItem>(itemElements.Count);

    //        result.AddRange(from XmlElement item in itemElements select new ListItem(item));

    //        return result;
    //    }

    //    /// <summary>
    //    /// Returns an XML document describing the entire navigation structure of the publication on which this 
    //    /// template is invoked.
    //    /// </summary>
    //    /// <param name="rootSG">the structure group from which to start building the navigation</param>
    //    /// <param name="SGs">the list of all structure groups in the publication, as retrieved from GetListStructureGroups</param>
    //    /// <returns>an XML document describing the entire navigation structure of the publication on which this 
    //    /// template is invoked</returns>
    //    /// <example>
    //    /// The following code:
    //    /// <code>
    //    ///     XmlDocument sitemap = GetSiteMap(getRootSG(), GetListStructureGroups());
    //    /// </code>
    //    /// will result in the following XML:
    //    /// <code>
    //    /// &lt;StructureGroup id="tcm:1-7-4" title="www.tridion.com" url="/">
    //    ///     &lt;StructureGroup id="tcm:1-10-4" title="200 Products" url="/Products">
    //    ///         &lt;StructureGroup id="tcm:1-11-4" title="210 R5" url="/Products/R5">
    //    /// 	        &lt;Page id="tcm:1-85-64" title="211 Content Creation" url="/Products/R5/ContentCreation.html"/>
    //    /// 	        &lt;Page id="tcm:1-95-64" title="215 Dynamic Content Broker" url="/Products/R5/DynamicContentBroker.html"/>
    //    /// 	        &lt;Page id="tcm:1-66-64" title="217 Archive Manager" url="/Products/R5/ArchiveManager.html"/>
    //    ///         &lt;/StructureGroup>
    //    ///         &lt;StructureGroup id="tcm:1-13-4" title="240 Interactive Web Applications" url="/Products/InteractiveWebApplications"/>
    //    ///     &lt;/StructureGroup>
    //    ///     &lt;StructureGroup id="tcm:1-14-4" title="100 Solutions" url="/Solutions">
    //    ///         &lt;StructureGroup id="tcm:1-22-4" title="110 For you industry" url="/Solutions/ForYouIndustry"/>
    //    ///         &lt;StructureGroup id="tcm:1-26-4" title="150 Download center" url="/Solutions/DownloadCenter"/>
    //    ///     &lt;/StructureGroup>
    //    ///     &lt;StructureGroup id="tcm:1-15-4" title="300 Customers" url="/Customers"/>
    //    ///     &lt;StructureGroup id="tcm:1-16-4" title="400 Service &amp; Support" url="/ServiceSupport"/>
    //    ///     &lt;StructureGroup id="tcm:1-17-4" title="500 Partners" url="/Partners">
    //    ///         &lt;Page id="tcm:1-111-64" title="Indivirtual" url="/Partners/Indivirtual.html"/>
    //    ///     &lt;/StructureGroup>
    //    ///     &lt;StructureGroup id="tcm:1-19-4" title="700 About Tridion" url="/AboutTridion"/>
    //    /// &lt;/StructureGroup>
    //    /// </code>
    //    /// </example>
    //    /// <seealso cref="GetListStructureGroups"/>
    //    /// <seealso cref="GetRootSG"/>
    //    public static XmlDocument GetSiteMap(ListItem rootSG, IList<ListItem> SGs)
    //    {
    //        return GetSiteMap(rootSG, SGs, null, null, null);
    //    }

    //    /// <summary>
    //    /// Returns an XML document describing the entire navigation structure of the publication on which this 
    //    /// template is invoked.
    //    /// </summary>
    //    /// <param name="rootSG">the structure group from which to start building the navigation</param>
    //    /// <param name="SGs">the list of all structure groups in the publication, as retrieved from GetListStructureGroups</param>
    //    /// <param name="regex">the regex that items must match to be included in the menu</param>
    //    /// <returns>an XML document describing the entire navigation structure of the publication on which this 
    //    /// template is invoked</returns>
    //    /// <example>
    //    /// The following code:
    //    /// <code>
    //    ///     Regex regex = ...
    //    ///     XmlDocument sitemap = GetSiteMap(getRootSG(), GetListStructureGroups(), regex);
    //    /// </code>
    //    /// will result in the following XML:
    //    /// <code>
    //    /// &lt;StructureGroup id="tcm:1-7-4" title="www.tridion.com" url="/">
    //    ///     &lt;StructureGroup id="tcm:1-10-4" title="200 Products" url="/Products">
    //    ///         &lt;StructureGroup id="tcm:1-11-4" title="210 R5" url="/Products/R5">
    //    /// 	        &lt;Page id="tcm:1-85-64" title="211 Content Creation" url="/Products/R5/ContentCreation.html"/>
    //    /// 	        &lt;Page id="tcm:1-95-64" title="215 Dynamic Content Broker" url="/Products/R5/DynamicContentBroker.html"/>
    //    /// 	        &lt;Page id="tcm:1-66-64" title="217 Archive Manager" url="/Products/R5/ArchiveManager.html"/>
    //    ///         &lt;/StructureGroup>
    //    ///         &lt;StructureGroup id="tcm:1-13-4" title="240 Interactive Web Applications" url="/Products/InteractiveWebApplications"/>
    //    ///     &lt;/StructureGroup>
    //    ///     &lt;StructureGroup id="tcm:1-14-4" title="100 Solutions" url="/Solutions">
    //    ///         &lt;StructureGroup id="tcm:1-22-4" title="110 For you industry" url="/Solutions/ForYouIndustry"/>
    //    ///         &lt;StructureGroup id="tcm:1-26-4" title="150 Download center" url="/Solutions/DownloadCenter"/>
    //    ///     &lt;/StructureGroup>
    //    ///     &lt;StructureGroup id="tcm:1-15-4" title="300 Customers" url="/Customers"/>
    //    ///     &lt;StructureGroup id="tcm:1-16-4" title="400 Service &amp; Support" url="/ServiceSupport"/>
    //    ///     &lt;StructureGroup id="tcm:1-17-4" title="500 Partners" url="/Partners">
    //    ///         &lt;Page id="tcm:1-111-64" title="Indivirtual" url="/Partners/Indivirtual.html"/>
    //    ///     &lt;/StructureGroup>
    //    ///     &lt;StructureGroup id="tcm:1-19-4" title="700 About Tridion" url="/AboutTridion"/>
    //    /// &lt;/StructureGroup>
    //    /// </code>
    //    /// </example>
    //    /// <seealso cref="GetListStructureGroups"/>
    //    /// <seealso cref="GetRootSG"/>
    //    public static XmlDocument GetSiteMap(ListItem rootSG, IList<ListItem> SGs, Regex regex)
    //    {
    //        return GetSiteMap(rootSG, SGs, null, null, regex);
    //    }

    //    /// <summary>
    //    /// Returns an XML document describing the entire navigation structure of the publication on which this 
    //    /// template is invoked.
    //    /// </summary>
    //    /// <param name="rootSG">the structure group from which to start building the navigation</param>
    //    /// <param name="SGs">the list of all structure groups in the publication, as retrieved from GetListStructureGroups</param>
    //    /// <param name="doc">the document to use for creating the new XML elements. If null, a new document will be created</param>
    //    /// <param name="parent">the element to append the new XML elements to. If null, the element(s) will be appended to the document.</param>
    //    /// <param name="regex">the regex that items must match to be included in the menu</param>
    //    /// <returns>an XML document describing the entire navigation structure of the publication on which this 
    //    /// template is invoked</returns>
    //    private static XmlDocument GetSiteMap(ListItem rootSG, IEnumerable<ListItem> SGs, XmlDocument doc, XmlElement parent, Regex regex)
    //    {
    //        string title = rootSG.Title;

    //        if (MustInclude(regex, ref title) || doc == null)
    //        {
    //            if (doc == null)
    //            {
    //                doc = new XmlDocument();
    //                parent = null;
    //            }

    //            StructureGroup objSG = (StructureGroup)_engine.GetObject(rootSG.Id);
    //            XmlElement elmSG = doc.CreateElement("node");
    //            elmSG.SetAttribute("id", objSG.Id);
    //            elmSG.SetAttribute("title", title);
    //            elmSG.SetAttribute("url", objSG.PublishLocationUrl);
    //            // TODO: add roles (SCD) and orgpub

    //            // for each child SG
    //            foreach (ListItem sg in SGs)
    //            {
    //                if (sg.ParentId == rootSG.Id)
    //                {
    //                    // recursively add SG + children
    //                    GetSiteMap(sg, SGs, doc, elmSG, regex);
    //                }
    //            }

    //            // get the pages for this SG
    //            IList<Page> pages = GetPagesInSG(rootSG);

    //            // add the pages
    //            foreach (Page page in pages)
    //            {
    //                title = page.Title;
    //                if (MustInclude(regex, ref title))
    //                {
    //                    XmlElement elmPage = doc.CreateElement("node");
    //                    elmPage.SetAttribute("id", page.Id);
    //                    elmPage.SetAttribute("title", title);
    //                    elmPage.SetAttribute("url", page.PublishLocationUrl);
    //                    // TODO: add SCD roles and orgpub
    //                    elmSG.AppendChild(elmPage);
    //                }
    //            }

    //            if (parent != null)
    //            {
    //                parent.AppendChild(elmSG);
    //            }
    //            else
    //            {
    //                doc.AppendChild(elmSG);
    //            }
    //        }

    //        return doc;
    //    }


    //    private static bool MustInclude(Regex regex, ref string title)
    //    {
    //        if (regex == null) return true;
    //        MatchCollection matches = regex.Matches(title);
    //        if (matches.Count > 0)
    //        {
    //            // remove the prefix (using the first match)
    //            title = title.Substring(matches[0].Groups[0].Length);
    //            return true;
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }

    //    /// <summary>
    //    /// Returns the list of structure groups of the current publication.
    //    /// </summary>
    //    /// <remarks>
    //    /// This call is expensive, as each object must be retrieved from the underlying layers separately.
    //    /// For a cheaper alternative (with less functionality) you should consider <c>GetListStructureGroups</c>
    //    /// </remarks>
    //    /// <returns>the list of structure groups of the current publication</returns>
    //    /// <seealso cref="StructureGroup"/>
    //    /// <seealso cref="GetListStructureGroups"/>
    //    protected IList<StructureGroup> GetStructureGroups()
    //    {
    //        CheckInitialized();
    //        Publication publication = GetPublication();

    //        OrganizationalItemsFilter filter = new OrganizationalItemsFilter(_engine.GetSession())
    //        {
    //            ItemTypes = new List<ItemType> { ItemType.StructureGroup }
    //        };

    //        return GetObjectsFromXmlList<StructureGroup>(_engine, publication.GetListOrganizationalItems(filter));
    //    }



    //    public static List<ListItem> GetPages(StructureGroup sg)
    //    {

    //        OrganizationalItemItemsFilter filter = new OrganizationalItemItemsFilter(sg.Session)
    //        {
    //            ItemTypes = new List<ItemType> { ItemType.Page },
    //            BaseColumns = ListBaseColumns.Extended,
    //            IncludeRelativeWebDavUrlColumn = true
    //        };

    //        XmlElement orgItems = sg.GetListItems(filter);

    //        XmlNodeList itemElements = orgItems.SelectNodes("*");
    //        List<ListItem> result = new List<ListItem>(itemElements.Count);

    //        result.AddRange(from XmlElement itemElement in itemElements select new ListItem(itemElement));

    //        return result;
    //    }

    //    protected IList<Page> GetPagesInSG(ListItem sg)
    //    {
    //        OrganizationalItemItemsFilter filter = new OrganizationalItemItemsFilter(this._engine.GetSession())
    //                                                   {
    //                                                       ItemTypes = new List<ItemType> {ItemType.Page},
    //                                                       BaseColumns = ListBaseColumns.Extended,
    //                                                       IncludeRelativeWebDavUrlColumn = true
    //                                                   };

    //        // TODO: find a way to avoid retrieving the SG
    //        StructureGroup structuregroup = _engine.GetObject(sg.Id) as StructureGroup;
    //        List<RepositoryLocalObject> rlos = (List<RepositoryLocalObject>)structuregroup.GetItems(filter);

    //        List<Page> pages = new List<Page>(rlos.Count);
    //        pages.AddRange(rlos.Cast<Page>());

    //        return pages;
    //    }

    //    /// <summary>
    //    /// Check if structure group contains index page. 
    //    /// This method is used to determine whether link to StructureGroup.PublishLocationUrl should be generated
    //    /// </summary>
    //    /// <param name="sg"></param>
    //    /// <returns></returns>
    //    public static bool ContainsIndexPage(StructureGroup sg)
    //    {
    //        var nameCandidates = new[] { "index", "default" };

    //        var filter = new OrganizationalItemItemsFilter(sg.Session)
    //        {
    //            ItemTypes = new[] { ItemType.Page }
    //        };

    //        return
    //            sg.GetItems(filter)
    //                .Cast<Page>()
    //                .Any(page => nameCandidates.Any(candidate => string.Equals(candidate, page.FileName, StringComparison.OrdinalIgnoreCase)));
    //    }

    //    /// <summary>
    //    /// Traverse up the structure group tree to find the first structure group that contains index page        
    //    /// </summary>
    //    /// <param name="page"></param>
    //    /// <returns>the navigable structure group which contains the child page</returns>
    //    public static StructureGroup GetNavigationContextStructureGroup(Page page)
    //    {
    //        StructureGroup contextSG = page.OrganizationalItem as StructureGroup;
    //        while (!ContainsIndexPage(contextSG))
    //        {
    //            contextSG = contextSG.OrganizationalItem as StructureGroup;
    //        }
    //        return contextSG;
    //    }

    //    public static Page GetIndexPage(StructureGroup sg)
    //    {
    //        OrganizationalItemItemsFilter filter = new OrganizationalItemItemsFilter(_engine.GetSession()) { ItemTypes = new List<ItemType> { ItemType.Page } };

    //        foreach (Page page in sg.GetItems(filter))
    //        {

    //            if (page.FileName.EndsWith("ndex"))
    //            {
    //                if (_engine.PublishingContext != null && _engine.PublishingContext.PublicationTarget != null)
    //                {
    //                    if (PublishEngine.IsPublished(page, _engine.PublishingContext.PublicationTarget))
    //                        return page;
    //                }
    //                else
    //                {
    //                    return page;
    //                }
    //            }

    //        }
    //        return null;
    //    }

    //    /// <summary>
    //    /// retrieves the prefix number from the title provided
    //    /// </summary>
    //    /// <param name="sgTitle"></param>
    //    /// <returns></returns>
    //    public static int GetNavigationTitlePrefix(string sgTitle)
    //    {
    //        string m_MNPrefix = "todelete";
    //        int order = -1;
    //        if (sgTitle.Length > 4)
    //        {
    //            string prefix = string.Empty;
    //            if (sgTitle.StartsWith(m_MNPrefix))
    //            {
    //                if (sgTitle.Length > 5)
    //                    prefix = sgTitle.Substring(2, 3);
    //            }
    //            else
    //            {
    //                prefix = sgTitle.Substring(0, 3);
    //            }

    //            if (Int32.TryParse(prefix, out order) && (sgTitle.Substring(3, 1) == " " || sgTitle.Substring(3, 1) == "_" || (sgTitle.StartsWith(m_MNPrefix) && sgTitle.Substring(5, 1) == " " || sgTitle.Substring(5, 1) == "_")))
    //                return order;
    //        }
    //        return order;
    //    }
    //}
}
