#if !(UNITY_2017_3_OR_NEWER)
#define USE_OLD_OSX_PLATFORMS
#endif

using UnityEditor;
using UnityEngine;

namespace Wrld.Editor
{
    internal class PlatformHelpers
    {
        internal static bool IsSupportedBuildTarget(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                case BuildTarget.iOS:
#if USE_OLD_OSX_PLATFORMS
                case BuildTarget.StandaloneOSXIntel64:
#else
                case BuildTarget.StandaloneOSX:
#endif
                case BuildTarget.StandaloneWindows64:
                    return true;
            }

            return false;
        }

        internal static bool IsStandaloneOSX(BuildTarget target)
        {
            switch (target)
            {
#if USE_OLD_OSX_PLATFORMS
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXUniversal:
                case BuildTarget.StandaloneOSXIntel64:
#else
                case BuildTarget.StandaloneOSX:
#endif
                    return true;
                default:
                    return false;
            }
        }

        internal static bool TryGetBuildTargetOverride(BuildTarget target, out BuildTarget buildTargetOverride)
        {
            buildTargetOverride = target;

            switch (target)
            {
                case BuildTarget.StandaloneWindows:
                    {
                        buildTargetOverride = BuildTarget.StandaloneWindows64;
                        return true;
                    }
#if USE_OLD_OSX_PLATFORMS
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXUniversal:
                    {
                        buildTargetOverride = BuildTarget.StandaloneOSXIntel64;
                        return true;
                    }
#endif
            }

            return false;
        }

        internal static string GetOverridableTargetArchitectureString(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneWindows:
#if USE_OLD_OSX_PLATFORMS
                case BuildTarget.StandaloneOSXIntel:
#endif
                    {
                        return "x86";
                    }
#if USE_OLD_OSX_PLATFORMS
                case BuildTarget.StandaloneOSXUniversal:
                    {
                        return "Universal";
                    }
#endif
                default:
                    return "x86_64";
            }
        }

        internal static bool TryGetRuntimePlatformForBuildTarget(BuildTarget target, out RuntimePlatform platform)
        {
            platform = RuntimePlatform.OSXEditor;

            switch (target)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    platform = RuntimePlatform.WindowsPlayer;
                    break;
#if USE_OLD_OSX_PLATFORMS
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXUniversal:
                case BuildTarget.StandaloneOSXIntel64:
#else
                case BuildTarget.StandaloneOSX:
#endif
                    platform = RuntimePlatform.OSXPlayer;
                    break;
                case BuildTarget.Android:
                    platform = RuntimePlatform.Android;
                    break;
                case BuildTarget.iOS:
                    platform = RuntimePlatform.IPhonePlayer;
                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}

