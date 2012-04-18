using System;
using System.Xml;
using System.Xml.Linq;
using TemplatingExtensions.ExtensionMethods.Generic;
using Tridion.ContentManager;

namespace TemplatingExtensions.Helpers
{
		/// <summary>
		/// This class represents an item as it can be constructed from an XML list.
		/// </summary>
		/// <remarks>
		/// The input XML is typically what you get back from GetListItems or GetListOrganizationalItems.
		/// </remarks>
		/// <remarks>
		/// This information is shallow, meaning that you can't jump from object to object using
		/// this class. For every object you'll have to call the Engine or Session again.
		/// </remarks>
		/// <example>
		/// Calling <c>GetListOrganizationalItems</c> like this: <code>
		///     Filter sgFilter = new Filter();
		///     sgFilter.Conditions["ItemType"] = ItemType.StructureGroup;
		///     sgFilter.BaseColumns = ListBaseColumns.Extended;
		///     sgFilter.AdditionalColumns.Add("url");
		///     XmlElement orgItems = publication.GetListOrganizationalItems(sgFilter)
		/// </code>
		/// 
		/// Returns a list of the following elements:
		/// <code>
		/// &lt;tcm:Item ID="tcm:1-7-4" ParentOrgItemID="tcm:0-1-1" Title="www.tridion.com" 
		///  Path="\Templating test publication" Type="4" Modified="2007-06-05T15:33:23" 
		///  IsShared="false" IsLocalized="false" Trustee="tcm:0-11-65552" Icon="T4L0P0" 
		///  URL="/webdav/Tridion/www%2Etridion%2Ecom" />
		/// </code>
		/// 
		/// Which you can then easily iterate using this class like:
		/// <code>
		///     XmlNodeList itemElements = orgItems.SelectNodes("*");
		///     List&lt;ListItem> result = new List&lt;ListItem>(itemElements.Count);
		///     foreach (XmlElement itemElement in itemElements)
		///     {
		///         ListItem sg = new ListItem(itemElement);
		///         if (sg.ParentId.PublicationId == -1)
		///         {
		///             // this is the root structure group
		///             return sg
		///         }
		///     }
		/// </code>
		/// </example>
		/// <seealso cref="Tridion.ContentManager.Session"/>
		/// <seealso cref="Tridion.ContentManager.Templating.Engine"/>
		public class ListItem
		{
			#region Private Members

			private readonly TcmUri _id;
			private readonly TcmUri _parentId;
			private readonly string _title;
			private readonly string _path;
			private readonly DateTime _modified;
			private readonly bool _isShared;
			private readonly bool _isLocalized;
			private readonly TcmUri _trusteeId;
			private readonly string _icon;
			private readonly string _url;

			#endregion

			public ListItem(XmlElement element)
			{
			    XElement el = element.ConvertToXElement();

				_id = new TcmUri(el.AttributeValueOrEmpty("ID"));
                _parentId = new TcmUri(el.AttributeValueOrEmpty("ParentOrgItemID"));
                _title = el.AttributeValueOrEmpty("Title");
                _path = el.AttributeValueOrEmpty("Path");
                _modified = DateTime.Parse(el.AttributeValueOrEmpty("Modified"));
                _isShared = Boolean.Parse(el.AttributeValueOrEmpty("IsShared"));
                _isLocalized = Boolean.Parse(el.AttributeValueOrEmpty("IsLocalized"));
    			_trusteeId = new TcmUri(el.AttributeValueOrEmpty("TrusteeID"));
				_icon = el.AttributeValueOrEmpty("Icon");
                _url = el.AttributeValueOrEmpty("URL");
			}

			public TcmUri Id { get { return _id; } }

			public TcmUri ParentId { get { return _parentId; } } // TOM.NET: OrganizationalItem

			public string Title { get { return _title; } }

			public string Path { get { return _path; } }

			public DateTime Modified { get { return _modified; } } // TOM.NET: RevisionDate

			public bool IsShared { get { return _isShared; } }

			public bool IsLocalized { get { return _isLocalized; } }

			public TcmUri TrusteeId  { get { return _trusteeId; } } // TOM.NET: Creator

			public string Icon { get { return _icon; } }

			public string Url { get { return _url; } } // TOM.NET: WebDavUrl?
		}
}
