using Redbox.HAL.Component.Model;
using System;
using System.Xml;

namespace Redbox.HAL.DataMatrix.Framework
{
    internal sealed class CameraPluginDescriptor
    {
        internal static CameraPluginDescriptor FromXml(string xmlPath)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlPath);
                if (xmlDocument.DocumentElement == null)
                {
                    LogHelper.Instance.Log(LogEntryType.Error, "[Camera Plugin Manager] The descriptor file '{0}' doesn't have a root element.", (object)xmlPath);
                    return (CameraPluginDescriptor)null;
                }
                XmlNode xmlNode = xmlDocument.SelectSingleNode("Plugin");
                if (xmlNode == null)
                {
                    LogHelper.Instance.Log(LogEntryType.Error, "[Camera Plugin Manager] The descriptor file '{0}' is missing the required Plugin node.", (object)xmlPath);
                    return (CameraPluginDescriptor)null;
                }
                CameraPluginDescriptor pluginDescriptor = new CameraPluginDescriptor();
                pluginDescriptor.DescriptorPath = xmlPath;
                foreach (XmlNode childNode in xmlNode.ChildNodes)
                {
                    if (childNode.Name.Equals("PluginType"))
                    {
                        pluginDescriptor.PluginName = childNode.Attributes.GetNamedItem("Name").Value;
                        pluginDescriptor.Version = childNode.Attributes.GetNamedItem("Version").Value;
                    }
                    else if (childNode.Name.Equals("Assembly"))
                        pluginDescriptor.AssemblyName = childNode.Attributes.GetNamedItem("Name").Value;
                    else if (childNode.Name.Equals("PluginProvider"))
                    {
                        pluginDescriptor.PluginServiceProviderMethodName = childNode.Attributes.GetNamedItem("Method").Value;
                        pluginDescriptor.PluginNamespace = childNode.Attributes.GetNamedItem("Namespace").Value;
                    }
                }
                return pluginDescriptor;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log("[Camera Plugin Manager] Unable to read plugin descriptor.", LogEntryType.Error);
                LogHelper.Instance.Log("[Camera Plugin Manager] Detailed exception: ", ex);
                return (CameraPluginDescriptor)null;
            }
        }

        internal string PluginName { get; set; }

        internal string AssemblyName { get; set; }

        internal string PluginNamespace { get; set; }

        internal string PluginServiceProviderMethodName { get; set; }

        internal string Version { get; set; }

        internal string DescriptorPath { get; private set; }
    }
}
