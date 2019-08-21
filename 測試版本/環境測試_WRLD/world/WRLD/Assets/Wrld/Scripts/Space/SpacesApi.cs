using Assets.Wrld.Scripts.Maths;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Wrld.Utilities;
using Wrld.Interop;

namespace Wrld.Space
{
    /// <summary>
    /// Contains functionality for working with object transforms and positions in various coordinate systems.
    /// </summary>
    public class SpacesApi
    {
        /// <summary>
        /// Obtain a ray in ECEF coordinates from the current camera location and passing through the specified screen point.
        /// </summary>
        /// <param name="screenPoint">The screen point, in pixels, with screen origin bottom-left.</param>
        /// <returns>An ECEF ray.</returns>
        public DoubleRay ScreenPointToRay(Vector2 screenPoint)
        {
            var screenPointOriginTopLeft = new Vector2(screenPoint.x, Screen.height - screenPoint.y);
            DoubleRay ray = NativeSpacesApi_ScreenPointToRay(NativePluginRunner.API, ref screenPointOriginTopLeft);
            return ray;
        }

        /// <summary>
        /// Obtain a ray in ECEF coordinates in a vertically downwards direction, and starting at a point high above the Earth's surface at the specified LatLong point.
        /// </summary>
        /// <param name="latLong">A LatLong point through which the vertical ray passes.</param>
        /// <returns>An ECEF ray.</returns>
        public DoubleRay LatLongToVerticallyDownRay(LatLong latLong)
        {
            var latLongInterop = latLong.ToLatLongInterop();
            DoubleRay ray = NativeSpacesApi_LatLongToVerticallyDownRay(NativePluginRunner.API, ref latLongInterop);
            return ray;
        }

        [DllImport(NativePluginRunner.DLL, CallingConvention = CallingConvention.StdCall)]
        private static extern DoubleRay NativeSpacesApi_ScreenPointToRay(IntPtr ptr, ref Vector2 screenPoint);

        [DllImport(NativePluginRunner.DLL, CallingConvention = CallingConvention.StdCall)]
        private static extern DoubleRay NativeSpacesApi_LatLongToVerticallyDownRay(IntPtr ptr, ref LatLongInterop latLongInterop);

    }
}
