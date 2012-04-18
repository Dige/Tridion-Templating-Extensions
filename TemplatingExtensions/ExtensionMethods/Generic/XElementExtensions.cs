using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace TemplatingExtensions.ExtensionMethods.Generic
{
    /// <summary>
    /// Contains extension methods to instances of the XElement class.
    /// </summary>
    /// <example>
    /// XElement example = new XElement("example", new XAttrribute("title","example title",
    ///  new XElement("Body","Body Text")));
    ///  
    /// string body = example.InnerXmlAsString(); // returns <Body>Body Text</Body>
    /// 
    /// string title = example.AttributeOrDefault("title"); // returns "example title" or null
    /// 
    /// string attribute = example.AttributeOrEmpry("name"); // returns the attribute value, but in this case string.Empty
    /// </example>
    public static class XElementExtensions
    {
        /// <summary>
        /// Returns all XML nodes directly descending from the root element.
        /// </summary>
        /// <remarks>
        /// As this is an extension method,
        /// the element does not need to be passed in the method
        /// </remarks>
        /// <param name="element">The root element as XElement.</param>
        /// <returns>The inner XML as string</returns>
        public static string InnerXmlAsString(this XElement element)
        {
            var reader = element.CreateReader();
            reader.MoveToContent();
            return reader.ReadInnerXml();
        }


        /// <summary>
        /// Retrieves the attribute requested by the caller.
        /// When the attribute does not exist, the method returns null. 
        /// 
        /// This is a convenience method to defensively program Linq queries and
        /// and other places where attributes are needed
        /// to make sure that for example:
        /// myXelement.Attribute("attribute").Value never throws a NPE.
        /// </summary>
        /// <remarks>
        /// As this is an extension method,
        /// the element does not need to be passed in the method
        /// </remarks>
        /// <param name="element">The element as XElement</param>
        /// <param name="attributeName">The name of the attribute to be retrieved</param>
        /// <returns>The attribute value or null</returns>
        public static string AttributeValueOrDefault(this XElement element, string attributeName)
        {
            XAttribute attr = element.Attribute(attributeName);
            return attr == null ? null : attr.Value;
        }


        /// <summary>
        /// Retrieves the attribute requested by the caller.
        /// When the attribute does not exist, the method returns null. 
        /// 
        /// This is a convenience method to defensively program Linq queries 
        /// and other places where attributes are needed, to make sure that for example:
        /// myXelement.Attribute("attribute").Value never throws a NPE.
        /// </summary>
        /// <remarks>
        /// As this is an extension method,
        /// the element does not need to be passed in the method
        /// </remarks>
        /// <param name="element">The element as XElement</param>
        /// <param name="attributeName">The name of the attribute to be retrieved</param>
        /// <returns>The attribute value or string.Empty</returns>
        public static string AttributeValueOrEmpty(this XElement element, string attributeName)
        {
            XAttribute attr = element.Attribute(attributeName);
            return attr == null ? string.Empty : attr.Value;
        }


        /// <summary>
        /// Retrieves the element's requested by the caller.
        /// When the element does not exist, the method returns an empty string. 
        /// 
        /// This is a convenience method to defensively program Linq queries 
        /// and other places where element are needed, to make sure that for example:
        /// myXelement.Element("element").Value never throws a NPE.
        /// </summary>
        /// <remarks>
        /// As this is an extension method,
        /// the element does not need to be passed in the method
        /// </remarks>
        /// <param name="element">The element as XElement</param>
        /// <returns>The element value or string.Empty</returns>
        public static string ElementValueOrEmpty(this XElement element)
        {
            return element == null ? string.Empty : element.Value;
        }

        /// <summary>
        /// Orders a list of elements based on value of an inner element
        /// E.g. <root><element><innerElementValue>2</innerElementValue></element><element><innerElementValue>1</innerElementValue></element></root>
        /// root.Elements().SortByInnerElementValue("innerElementValue") // would return "element"-elements of example XML in reverse order
        /// </summary>
        /// <remarks>
        /// As this is an extension method,
        /// the elements do not need to be passed in the method
        /// </remarks>
        /// <param name="elements">The element as XElement</param>
        /// <param name="elementName">Name of the inner element</param>
        /// <returns>Elements ordered by the given inner element name</returns>
        public static IOrderedEnumerable<XElement> SortByInnerElementValue(this IEnumerable<XElement> elements, string elementName)
        {
            return elements.OrderBy(x => x.Element(elementName).Value);
        }

        /// <summary>
        /// Gets the XmlDocument from an XElement
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static XmlDocument GetXmlDocument(this XElement element)
        {
            using (XmlReader xmlReader = element.CreateReader())
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlReader);
                return xmlDoc;
            }
        }

        public static XElement ConvertToXElement(this XmlElement xmlElement)
        {
            return XElement.Load(xmlElement.CreateNavigator().ReadSubtree());
        }
    }
}
