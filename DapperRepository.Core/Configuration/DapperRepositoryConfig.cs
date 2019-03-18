using System;
using System.Configuration;
using System.Xml;

namespace DapperRepository.Core.Configuration
{
    public class DapperRepositoryConfig : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new DapperRepositoryConfig();

            var startupNode = section.SelectSingleNode("CurrentActivedDbType");
            config.ActivedDbTypeName = GetString(startupNode, "ActivedDbTypeName");

            return config;
        }

        private static string GetString(XmlNode node, string attrName)
        {
            return SetByXElement(node, attrName, Convert.ToString);
        }

        private static T SetByXElement<T>(XmlNode node, string attrName, Func<string, T> converter)
        {
            if (node == null || node.Attributes == null) return default(T);
            var attr = node.Attributes[attrName];
            if (attr == null) return default(T);
            var attrVal = attr.Value;
            return converter(attrVal);
        }

        public string ActivedDbTypeName { get; set; }
    }
}
