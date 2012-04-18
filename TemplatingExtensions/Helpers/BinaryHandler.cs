using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Tridion.ContentManager;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.Templating;
using TemplatingExtensions.ExtensionMethods.Tridion;

namespace TemplatingExtensions.Helpers
{
    /* TODO: CLEAN THIS CLASS */
    public class BinaryHandler
    {
        private static XmlNamespaceManager _NSM;

        public static XmlNamespaceManager NSManager
        {
            get
            {
                if (_NSM == null)
                {
                    _NSM = new XmlNamespaceManager(new NameTable());

                    _NSM.AddNamespace("tcm", "http://www.tridion.com/ContentManager/5.0");
                    _NSM.AddNamespace("xlink", "http://www.w3.org/1999/xlink");
                    _NSM.AddNamespace("xhtml", "http://www.w3.org/1999/xhtml");
                }

                return _NSM;
            }
        }

        private readonly Engine _engine;
        private readonly Package _package;
        private readonly TemplatingLogger _logger;

        public BinaryHandler(Engine engine, Package package)
        {
            _engine = engine;
            _package = package;
            _logger = TemplatingLogger.GetLogger(GetType());
        }

        public void AddBinariesFromFolder(string rootSGWebDavUrl, Folder folder, string path)
        {
            //Try to get a reference to the sub Structure Group based on the folder title
            StructureGroup sg = _engine.GetObject(rootSGWebDavUrl + path) as StructureGroup;
            if (sg == null)
            {
                Exception ex = new Exception(String.Format("Could not find Structure Group {0}. Please create this and republish", rootSGWebDavUrl + path));
                _logger.Error(ex.Message);
                throw ex;
            }

            //Loop through all components in the folder
            foreach (Component comp in folder.GetComponents(true))
            {
                if (comp.ComponentType == ComponentType.Multimedia)
                {
                    AddBinary(comp, sg);
                }
            }

        }

        /// <summary>
        /// Add a binary to the package and ensure it is published into the given structure group
        /// </summary>
        /// <param name="mmComp">The binary to add</param>
        protected Item AddBinary(Component mmComp)
        {
            TcmUri uri = null;
            return AddBinary(mmComp, uri);
        }

        /// <summary>
        /// Add a binary to the package and ensure it is published into the given structure group
        /// </summary>
        /// <param name="mmComp">The binary to add</param>
        /// <param name="structureGroupUri">The target SG Uri</param>
        protected Item AddBinary(Component mmComp, TcmUri structureGroupUri)
        {
            string filename = Utilities.Utilities.GetFilename(mmComp.BinaryContent.Filename);

            //Check if the binary is already in the package (for example, if the DWT already added it)
            Item packageItem = GetItemFromPackage(mmComp, filename) ?? PushItemToPackage(mmComp, filename);

            PublishBinaryToSG(mmComp, structureGroupUri, filename, packageItem);

            return packageItem;
        }

        private Item GetItemFromPackage(Component mmComp, string filename)
        {
            Item packageItem = _package.GetByName(filename);
            if (packageItem != null)
            {
                _logger.Debug("An item with the same name exists in the package");
                KeyValuePair<string, string> pair = new KeyValuePair<string, string>("TCMURI", mmComp.Id.ToString());
                if (!packageItem.Properties.Contains(pair))
                {
                    //its a different item so we should push our item in the package
                    packageItem = null;
                }
            }
            return packageItem;
        }

        private Item PushItemToPackage(Component mmComp, string filename)
        {
            _logger.Debug(String.Format("Pushing item {0} to the package", filename));
            Item packageItem = _package.CreateMultimediaItem(mmComp.Id);
            _package.PushItem(filename, packageItem);
            return packageItem;
        }

        private void PublishBinaryToSG(Component mmComp, TcmUri structureGroupUri, string filename, Item packageItem)
        {
            using (Stream itemStream = packageItem.GetAsStream())
            {
                try
                {
                    byte[] data = new byte[itemStream.Length];
                    itemStream.Read(data, 0, data.Length);
                    _logger.Info(String.Format("Adding binary component {0} ({1}) ", mmComp.Title, mmComp.Id));

                    string publishedPath = _engine.AddBinary(mmComp.Id, null, structureGroupUri, data, filename);
                    packageItem.Properties[Item.ItemPropertyPublishedPath] = publishedPath;
                }
                finally
                {
                    itemStream.Close();
                }
            }
        }
        /// <summary>
        /// Add a binary to the package and ensure it is published into the given structure group
        /// </summary>
        /// <param name="mmComp">The binary to add</param>
        /// <param name="sg">The target SG</param>
        protected Item AddBinary(Component mmComp, StructureGroup sg)
        {
            return AddBinary(mmComp, sg.Id);
        }

        public string AddBinaryWithUniqueFilename(Component comp)
        {
            MemoryStream ms = new MemoryStream();
            comp.BinaryContent.WriteToStream(ms);
            string filename = CreateUniqueFileNameForBinary(comp);

            _logger.Debug(String.Format("Adding binary to package: {0}", filename));
            return _engine.AddBinary(comp.Id, TcmUri.UriNull, null, ms.ToArray(), filename);
        }

        public static string CreateUniqueFileNameForBinary(Component comp)
        {
            string filename = Utilities.Utilities.GetFilename(comp.BinaryContent.Filename);
            //make sure filename is unique by appending pub and component id
            string suffix = String.Format("_tcm{0}-{1}", comp.Id.PublicationId, comp.Id.ItemId);
            int pos = filename.LastIndexOf(".");
            if (pos > 0)
            {
                filename = filename.Substring(0, pos) + suffix + filename.Substring(pos);
            }
            else
                filename += suffix;
            return filename;
        }


        /// <summary>
        /// Gets the filename.
        /// </summary>
        /// <param name="multimediaXml">The multimedia XML.</param>
        /// <returns></returns>
        public static string GetFilename(XmlElement multimediaXml)
        {
            XmlNamespaceManager nsManager = NSManager;
            XmlNode mmFilename = multimediaXml.SelectSingleNode("//tcm:MultimediaFilename", nsManager);
            string filename = String.Empty;

            if (mmFilename != null)
            {
                filename = mmFilename.InnerText;
                if ((filename.LastIndexOf('\\') + 1) > 0)
                {
                    filename = filename.Substring(filename.LastIndexOf('\\') + 1);
                }
            }

            return filename;
        }

        public void PushMMCompToPackage(string mmCompUri)
        {
            Component mmComp = _engine.GetObject(mmCompUri) as Component;
            if (mmComp != null)
            {
                if (mmComp.ComponentType == ComponentType.Multimedia)
                {
                    string filename = GetFilename(mmComp.ToXml());
                    _package.PushItem(filename, _package.CreateMultimediaItem(new TcmUri(mmCompUri)));
                    _logger.Debug("Pushed multimedia component to the package:" + mmCompUri);
                }
            }
        }
    }
}
