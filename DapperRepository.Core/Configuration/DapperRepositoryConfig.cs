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

            var dbTypeNode = section.SelectSingleNode("CurrentDbTypeName");
            config.CurrentDbTypeName = GetString(dbTypeNode, "CurrentDbTypeName");

            var redisCachingNode = section.SelectSingleNode("RedisCaching");
            config.RedisEnabled= GetBool(redisCachingNode, "Enabled");
            config.RedisCachingConnectionString = GetString(redisCachingNode, "ConnectionString");

            return config;
        }

        private static string GetString(XmlNode node, string attrName)
        {
            return SetByXElement(node, attrName, Convert.ToString);
        }

        private static bool GetBool(XmlNode node, string attrName)
        {
            return SetByXElement(node, attrName, Convert.ToBoolean);
        }

        private static T SetByXElement<T>(XmlNode node, string attrName, Func<string, T> converter)
        {
            if (node?.Attributes == null) return default(T);
            var attr = node.Attributes[attrName];
            if (attr == null) return default(T);
            var attrVal = attr.Value;
            return converter(attrVal);
        }

        public bool RedisEnabled { get; set; }

        public string CurrentDbTypeName { get; set; }

        /// <summary>
        /// Redis connection string. Used when Redis caching is enabled
        /// </summary>
        public string RedisCachingConnectionString { get; private set; }
    }
}
