using System;
using System.Text.RegularExpressions;
using System.Xml;
using TemplatingExtensions.Helpers;
using Tridion;
using Tridion.ContentManager;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.Publishing.Rendering;
using Tridion.ContentManager.Templating;

namespace TemplatingExtensions.Utilities
{
    public class RTFUtilities
    {
        private readonly Engine _engine;
        private readonly Package _package;

        public RTFUtilities(Engine engine, Package package)
        {
            _engine = engine;
            _package = package;
        }

        public XmlDocument ConvertLinksInRtf(XmlDocument rtfContent)
        {
            return ConvertLinksInRtf(rtfContent, "tcm:0-0-0");
        }

        /// <summary>
        /// Converts links in RT Fields
        /// 
        /// If it's a BinaryLink, it publishes the binary and creates the link
        /// </summary>
        /// <param name="rtfContent"></param>
        /// <returns></returns>
        public XmlDocument ConvertLinksInRtf(XmlDocument rtfContent, string componentTemplateUri)
        {
            XmlDocument xmlContent = rtfContent;
            foreach (XmlNode node in xmlContent.SelectNodes("//xhtml:a[@xlink:href]", BinaryHandler.NSManager ))
            {
                TcmUri item = new TcmUri(node.Attributes["href", Constants.XlinkNamespace].Value);
                if (item.ItemType == ItemType.Component)
                {
                    Component comp = _engine.GetObject(item) as Component;
                    if (comp != null && comp.BinaryContent != null)
                    {
                        CreateBinaryLink(xmlContent, node, item, comp);
                    }
                    else
                    {
                        node.ParentNode.ReplaceChild(CreateComponentLink(node, componentTemplateUri, item), node); 
                    }

                }
            }
            return xmlContent;
        }

        private void CreateBinaryLink(XmlDocument xmlContent, XmlNode node, TcmUri item, Component c)
        {
            _package.PushItem(_package.CreateMultimediaItem(item));
            Binary binary = _engine.PublishingContext.RenderedItem.AddBinary(c, "");
            node.Attributes.RemoveAll();
            String publishedPath = GetUrlForBinary(binary.Url);
            XmlAttribute attr = xmlContent.CreateAttribute("href");
            attr.Value = publishedPath;
            node.Attributes.SetNamedItem(attr);

            attr = xmlContent.CreateAttribute("title");
            attr.Value = c.Title;
            node.Attributes.SetNamedItem(attr);
        }

        private static string GetUrlForBinary(String Url)
        {
            return Regex.Replace(Url, @"_tcm[\d]*-[\d]*.", ".");
        }

        private XmlNode CreateComponentLink(XmlNode node, string componentTemplateUri, TcmUri item)
        {
            XmlElement componentLink = node.OwnerDocument.CreateElement("tridion", "ComponentLink", "http://tridion.com/");

            componentLink.SetAttribute("PageURI", _package.GetByName("Page.ID").GetAsString());
            componentLink.SetAttribute("ComponentURI", item.ToString());
            componentLink.SetAttribute("TemplateURI", componentTemplateUri);
            componentLink.SetAttribute("LinkText", node.InnerXml);
            componentLink.SetAttribute("LinkAttributes", "title=\"" + node.Attributes["title", Constants.XlinkNamespace].Value + "\"");
            componentLink.SetAttribute("runat", "server");
            componentLink.SetAttribute("TextOnFail", "true");

            return componentLink;
        }
    }
}
