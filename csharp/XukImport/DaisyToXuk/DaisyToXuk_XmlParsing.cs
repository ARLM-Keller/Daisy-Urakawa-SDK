﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Xml;
using System.Diagnostics;
using System.Reflection;

namespace XukImport
{
    public partial class DaisyToXuk
    {
        private XmlDocument readXmlDocument(string path)
        {
            XmlReaderSettings settings = new XmlReaderSettings();

            settings.ProhibitDtd = false;
            settings.ValidationType = ValidationType.None;
            settings.ConformanceLevel = ConformanceLevel.Auto;
            settings.XmlResolver = new LocalXmlUrlResolver(true);
        
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;

            using (XmlReader xmlReader = XmlReader.Create(path, settings))
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.XmlResolver = null;
                try
                {
                    xmldoc.Load(xmlReader);
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);

                    // No message box: use debugging instead (inspect stack trace, watch values)
                    //MessageBox.Show(e.ToString());

                    // The Fail() method is better:
                    //System.Diagnostics.Debug.Fail(e.Message);

                    //Or you can explicitely break:
#if DEBUG
                    Debugger.Break();
#endif
                }
                finally
                {
                    xmlReader.Close();
                }

                return xmldoc;
            }
        }

        public class LocalXmlUrlResolver : XmlUrlResolver
        {
            private bool m_EnableHttpCaching;
            private ICredentials m_Credentials;
            private Dictionary<string, string> m_EmbeddedEntities;

            //resolve resources from cache (if possible) when m_EnableHttpCaching is set to true
            //resolve resources from source when enableHttpcaching is set to false 
            public LocalXmlUrlResolver(bool enableHttpCaching)
            {
                m_EnableHttpCaching = enableHttpCaching;

                m_EmbeddedEntities = new Dictionary<String, String>();

                // -//W3C//DTD XHTML 1.0 Transitional//EN
                // http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd

                m_EmbeddedEntities.Add("dtbook110.dtd", "DaisyToXuk.Resources.dtbook110.dtd");

                m_EmbeddedEntities.Add("xhtml-lat1.ent", "DaisyToXuk.Resources.xhtml-lat1.ent");
                m_EmbeddedEntities.Add("xhtml-symbol.ent", "DaisyToXuk.Resources.xhtml-symbol.ent");
                m_EmbeddedEntities.Add("xhtml-special.ent", "DaisyToXuk.Resources.xhtml-special.ent");

                m_EmbeddedEntities.Add("HTMLlat1", "DaisyToXuk.Resources.xhtml-lat1.ent");
                m_EmbeddedEntities.Add("HTMLsymbol", "DaisyToXuk.Resources.xhtml-symbol.ent");
                m_EmbeddedEntities.Add("HTMLspecial", "DaisyToXuk.Resources.xhtml-special.ent");

                m_EmbeddedEntities.Add("//W3C//ENTITIES%20Latin%201%20for%20XHTML//EN", "DaisyToXuk.Resources.xhtml-lat1.ent");
                m_EmbeddedEntities.Add("//W3C//ENTITIES%20Symbols%20for%20XHTML//EN", "DaisyToXuk.Resources.xhtml-symbol.ent");
                m_EmbeddedEntities.Add("//W3C//ENTITIES%20Special%20for%20XHTML//EN", "DaisyToXuk.Resources.xhtml-special.ent");

                m_EmbeddedEntities.Add("//W3C//DTD%20XHTML%201.0%20Transitional//EN", "DaisyToXuk.Resources.xhtml1-transitional.dtd");
                m_EmbeddedEntities.Add("//W3C//DTD%20XHTML%201.1//EN", "DaisyToXuk.Resources.xhtml11.dtd");
                m_EmbeddedEntities.Add("//NISO//DTD%20ncx%202005-1//EN", "DaisyToXuk.Resources.ncx-2005-1.dtd");
                m_EmbeddedEntities.Add("//W3C//DTD%20XHTML%201.1%20plus%20MathML%202.0%20plus%20SVG%201.1//EN", "DaisyToXuk.Resources.xhtml-math-svg-flat.dtd");
                m_EmbeddedEntities.Add("//NISO//DTD%20dtbook%202005-1//EN", "DaisyToXuk.Resources.dtbook-2005-1.dtd");
                m_EmbeddedEntities.Add("//NISO//DTD%20dtbook%202005-2//EN", "DaisyToXuk.Resources.dtbook-2005-2.dtd");
                m_EmbeddedEntities.Add("//NISO//DTD%20dtbook%202005-3//EN", "DaisyToXuk.Resources.dtbook-2005-3.dtd");
                m_EmbeddedEntities.Add("//W3C//ENTITIES%20MathML%202.0%20Qualified%20Names%201.0//EN", "DaisyToXuk.Resources.mathml2.dtd");
                m_EmbeddedEntities.Add("//NISO//DTD%20dtbsmil%202005-2//EN", "DaisyToXuk.Resources.dtbsmil-2005-2.dtd");
                m_EmbeddedEntities.Add("//ISBN%200-9673008-1-9//DTD%20OEB%201.2%20Package//EN", "DaisyToXuk.Resources.oebpkg12.dtd");
                m_EmbeddedEntities.Add("//NISO//DTD%20dtbsmil%202005-1//EN", "DaisyToXuk.Resources.dtbsmil-2005-1.dtd");
            }

            public override Uri ResolveUri(Uri baseUri, string relativeUri)
            {
                if ((baseUri == null) || (!baseUri.IsAbsoluteUri && (baseUri.OriginalString.Length == 0)))
                {
                    var uri = new Uri(relativeUri, UriKind.RelativeOrAbsolute);
                    if (!uri.IsAbsoluteUri && (uri.OriginalString.Length > 0))
                    {
                        uri = new Uri(Path.GetFullPath(relativeUri));
                    }
                    return uri;
                }

                return !String.IsNullOrEmpty(relativeUri) ? new Uri(baseUri, relativeUri) : baseUri;
            }

            public override ICredentials Credentials
            {
                set
                {
                    m_Credentials = value;
                    base.Credentials = value;
                }
            }

            public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
            {
                if (absoluteUri == null)
                {
                    throw new ArgumentNullException("absoluteUri");
                }

                // Resolve local known entities
                Stream localStream = mapUri(absoluteUri);
                if (localStream != null)
                {
                    return localStream;
                }

                //resolve resources from cache (if possible)
                if (absoluteUri.Scheme == "http" && m_EnableHttpCaching && (ofObjectToReturn == null || ofObjectToReturn == typeof(Stream)))
                {
                    WebRequest webReq = WebRequest.Create(absoluteUri);
                    webReq.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Default);
                    if (m_Credentials != null)
                    {
                        webReq.Credentials = m_Credentials;
                    }
                    WebResponse resp = webReq.GetResponse();
                    return resp.GetResponseStream();
                }

                // No need to look for a local file that does not exist.
                if (absoluteUri.Scheme == "file" && !File.Exists(absoluteUri.LocalPath))
                {
                    return null;
                }

                //otherwise use the default behavior of the XmlUrlResolver class (resolve resources from source)
                return base.GetEntity(absoluteUri, role, ofObjectToReturn);
            }

            public Stream mapUri(Uri absoluteUri)
            {
                Assembly myAssembly = Assembly.GetExecutingAssembly();
                //string[] names = myAssembly.GetManifestResourceNames();

                //if (!absoluteUri.AbsolutePath.EndsWith("opf")
                //    && !absoluteUri.AbsolutePath.EndsWith("xhtml")
                //    && !absoluteUri.AbsolutePath.EndsWith("html"))
                //{
                //    Debugger.Break();
                //}

                Stream dtdStream = null;
                foreach (String key in m_EmbeddedEntities.Keys)
                {
                    if (absoluteUri.AbsolutePath.Contains(key))
                    {
                        dtdStream = myAssembly.GetManifestResourceStream(m_EmbeddedEntities[key]);
                        Console.WriteLine("XML Entity Resolver [" + m_EmbeddedEntities[key] + "]: " + (dtdStream != null ? dtdStream.Length + " bytes resource. " : "resource not found ?? ") + " ( " + absoluteUri + " )");
                        return dtdStream;
                    }
                }

                return null;
            }
        }
    }
}
