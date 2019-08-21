#if UNITY_IOS

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using UnityEditor.iOS.Xcode;
using System.IO;

// adapted from ideas discussed in https://forum.unity3d.com/threads/unity-xcode-api.281305/
public class XcodeProjectUpdater 
{
	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
			
            PBXProject project = new PBXProject();
            project.ReadFromString(File.ReadAllText(projectPath));

            string target = project.TargetGuidByName("Unity-iPhone");
            AddFrameworks(project, target);
            AddDylibs(project, target);
            project.SetBuildProperty(target, "ENABLE_BITCODE", "false");

            // Unity will find and copy this x86_64 bundle into the project as a framework, if 
            // it is allowed, preventing it from archiving correctly.  As we're on iOS in this 
            // case we don't need it and can remove it.
            string osxBundleGuid = project.FindFileGuidByProjectPath("Frameworks/Wrld/Plugins/x86_64/StreamAlpha.bundle");
            
            if (osxBundleGuid != null)
            {
                project.RemoveFile(osxBundleGuid);
            }

            File.WriteAllText(projectPath, project.WriteToString());
        }
    }

    private static void AddFrameworks(PBXProject project, string target)
    {
        const bool weak = false;
        project.AddFrameworkToProject(target, "MobileCoreServices.framework", weak);
        project.AddFrameworkToProject(target, "Security.framework", weak);
    }

    private static void AddDylibs(PBXProject project, string target)
    {
        AddDylib(project, target, "libz.dylib");
    }

    private static void AddDylib(PBXProject project, string target, string dylib)
    {
        string file = project.AddFile("usr/lib/" + dylib, "Frameworks/" + dylib, PBXSourceTree.Sdk);
        project.AddFileToBuild(target, file);
    }
}

#endif