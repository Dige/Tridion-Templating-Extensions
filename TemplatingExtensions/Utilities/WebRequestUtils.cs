using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace TemplatingExtensions.Utilities
{
    public static class WebRequestUtils
    {
        public static string GetWebRequestResponse(string url, ICredentials credentials)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Credentials = credentials ?? CredentialCache.DefaultCredentials;
            request.KeepAlive = false;

            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException webEx)
            {
                throw new Exception(String.Format("Could not get {0} due to the following exception.", url), webEx);
            }

            Stream receiveStream = response.GetResponseStream();
            Encoding encode = Encoding.GetEncoding("utf-8");

            StreamReader reader = new StreamReader(receiveStream, encode);

            string responseText = String.Empty;

            const int bufferLength = 256;
            Char[] buffer = new Char[bufferLength];

            int count = reader.Read(buffer, 0, bufferLength);

            while (count > 0)
            {
                responseText += new String(buffer, 0, count);

                count = reader.Read(buffer, 0, bufferLength);
            }

            reader.Close();
            response.Close();

            return responseText;
        }


        public static XmlDocument GetXmlDocument(string url, ICredentials credentials)
        {
            XmlUrlResolver resolver = new XmlUrlResolver
            {
                Credentials = credentials ?? CredentialCache.DefaultCredentials
            };

            XmlReaderSettings settings = new XmlReaderSettings { XmlResolver = resolver };

            XmlReader reader = XmlReader.Create(url, settings);

            XmlDocument document = new XmlDocument();
            document.Load(reader);

            return document;
        }
    }
}
