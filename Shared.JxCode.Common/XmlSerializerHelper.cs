using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace JxCode.Common
{
    public class XmlSerializerHelper
    {
        public static string Serialize(object obj, Type[] child = null)
        {
            XmlSerializer ser = new XmlSerializer(obj.GetType(), child);
            XmlWriterSettings xws = new XmlWriterSettings()
            {
                Indent = true,
            };
            StringBuilder sb = new StringBuilder();
            XmlWriter xtw = XmlTextWriter.Create(sb, xws);
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(
                new XmlQualifiedName[]
                {
                    new XmlQualifiedName(string.Empty,"empty")
                }
            );
            ser.Serialize(xtw, obj, namespaces);
            xtw.Close();
            return sb.ToString();
        }

        public static T Deserialize<T>(string xml)
        {
            XmlSerializer xmlser = new XmlSerializer(typeof(T));
            StringReader sr = new StringReader(xml);
            return (T)xmlser.Deserialize(sr);
        }
        

    }
}
