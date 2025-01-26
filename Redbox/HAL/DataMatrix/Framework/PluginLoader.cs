using Redbox.HAL.Component.Model;
using Redbox.HAL.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Redbox.HAL.DataMatrix.Framework
{
    internal sealed class PluginLoader
    {
        private readonly string PluginsDirectory;
        private readonly List<CameraPluginDescriptor> PluginDescriptors;

        internal static PluginLoader Instance => Singleton<PluginLoader>.Instance;

        internal CameraPluginDescriptor LocatePlugin(string name, out ICameraPlugin plugin)
        {
            plugin = (ICameraPlugin)null;
            CameraPluginDescriptor descriptor = this.PluginDescriptors.Find((Predicate<CameraPluginDescriptor>)(cameraPluginDescriptor_0 => name.Equals(cameraPluginDescriptor_0.PluginName, StringComparison.CurrentCultureIgnoreCase)));
            if (descriptor == null)
                return (CameraPluginDescriptor)null;
            return !this.LocatePluginInner(descriptor, out plugin) ? (CameraPluginDescriptor)null : descriptor;
        }

        private PluginLoader()
        {
            this.PluginsDirectory = ServiceLocator.Instance.GetService<IRuntimeService>().RuntimePath("camera.plugins");
            string[] files = Directory.GetFiles(this.PluginsDirectory, "*.xml");
            this.PluginDescriptors = new List<CameraPluginDescriptor>();
            if (files.Length == 0)
            {
                LogHelper.Instance.Log("[Camera Plugin Manager] There are no plugin descriptor files.", LogEntryType.Error);
            }
            else
            {
                foreach (string xmlPath in files)
                {
                    CameraPluginDescriptor pluginDescriptor = CameraPluginDescriptor.FromXml(xmlPath);
                    if (pluginDescriptor != null)
                        this.PluginDescriptors.Add(pluginDescriptor);
                }
            }
        }

        private bool LocatePluginInner(CameraPluginDescriptor descriptor, out ICameraPlugin plugin)
        {
            plugin = (ICameraPlugin)null;
            foreach (Type exportedType in Assembly.LoadFrom(Path.Combine(this.PluginsDirectory, descriptor.AssemblyName)).GetExportedTypes())
            {
                if (exportedType.FullName.Equals(descriptor.PluginNamespace))
                {
                    MethodInfo method = exportedType.GetMethod(descriptor.PluginServiceProviderMethodName, BindingFlags.Static | BindingFlags.Public);
                    if (method != null)
                    {
                        plugin = method.Invoke((object)null, (object[])null) as ICameraPlugin;
                        return plugin != null;
                    }
                }
            }
            return false;
        }
    }
}
