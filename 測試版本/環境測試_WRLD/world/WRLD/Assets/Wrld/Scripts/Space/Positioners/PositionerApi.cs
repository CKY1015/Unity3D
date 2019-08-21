using Assets.Wrld.Scripts.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using Wrld.Common.Maths;
using Wrld.Space.Positioners;

namespace Wrld.Space.Positioners
{
    public class PositionerApi
    {
        private PositionerApiInternal m_apiInternal;
        internal PositionerApi(PositionerApiInternal apiInternal)
        {
            m_apiInternal = apiInternal;
        }

        /// <summary>
        /// Creates an instance of a Positioner.
        /// </summary>
        /// <param name="positionerOptions">The PositionerOptions object which defines creation parameters for this Positioner.</param>
        public Positioner CreatePositioner(PositionerOptions positionerOptions)
        {
            return m_apiInternal.CreatePositioner(positionerOptions);
        }

        internal PositionerApiInternal GetApiInternal()
        {
            return m_apiInternal;
        }

    }
}
