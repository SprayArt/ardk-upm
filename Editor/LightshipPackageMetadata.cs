using System.Collections.Generic;
using Niantic.Lightship.AR.Loader;
using UnityEngine;
using UnityEditor;
using UnityEditor.XR.Management.Metadata;

namespace Niantic.Lightship.AR.Editor
{
    class XRPackage : IXRPackage
    {
        private class LightshipLoaderMetadata : IXRLoaderMetadata
        {
            public string loaderName { get; set; }
            public string loaderType { get; set; }
            public List<BuildTargetGroup> supportedBuildTargets { get; set; }
        }

        private class LightshipPackageMetadata : IXRPackageMetadata
        {
            public string packageName { get; set; }
            public string packageId { get; set; }
            public string settingsType { get; set; }
            public List<IXRLoaderMetadata> loaderMetadata { get; set; }
        }

        private static IXRPackageMetadata s_Metadata = new LightshipPackageMetadata()
        {
            packageName = LightshipPackageInfo.displayName,
            packageId = LightshipPackageInfo.identifier,
            settingsType = typeof(LightshipSettings).FullName,
            loaderMetadata = new List<IXRLoaderMetadata>()
            {
                new LightshipLoaderMetadata()
                {
                    loaderName = "Niantic Lightship SDK",
                    loaderType = typeof(LightshipStandaloneLoader).FullName,
                    supportedBuildTargets = new List<BuildTargetGroup>() { BuildTargetGroup.Standalone, }
                },
                new LightshipLoaderMetadata()
                {
                    loaderName = "Niantic Lightship SDK",
                    loaderType = typeof(LightshipARCoreLoader).FullName,
                    supportedBuildTargets = new List<BuildTargetGroup>() { BuildTargetGroup.Android, }
                },
                new LightshipLoaderMetadata()
                {
                    loaderName = "Niantic Lightship SDK",
                    loaderType = typeof(LightshipARKitLoader).FullName,
                    supportedBuildTargets = new List<BuildTargetGroup>() { BuildTargetGroup.iOS, }
                }
            }
        };

        public IXRPackageMetadata metadata => s_Metadata;

        public bool PopulateNewSettingsInstance(ScriptableObject obj)
        {
            try
            {
                EditorBuildSettings.AddConfigObject(LightshipSettings.SettingsKey, obj, true);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.Log($"Error adding new Lightship Settings object to build settings.\n{ex.Message}");
            }

            return false;
        }
    }
}
