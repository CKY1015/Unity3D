using System;
using Wrld.Common.Maths;

namespace Wrld.Space.Positioners
{
    /// <summary>
    /// A Positioner represents a single point on the map. Use a GeographicTransform in conjunction with this to position GameObjects on the map.
    /// </summary>
    public class Positioner
    {
        /// <summary>
        /// Uniquely identifies this object instance.
        /// </summary>
        public int Id { get; private set; }

        private static int InvalidId = 0;

        private PositionerApiInternal m_positionerApiInternal;

        private DoubleVector3 m_positionerECEFLocation;
        
        // Use Api.Instance.PositionerApi.CreatePositioner for public construction
        internal Positioner(
            PositionerApiInternal positionerApiInternal,
            int id,
            bool usingFloorId)
            {
                if (positionerApiInternal == null)
                {
                    throw new ArgumentNullException("positionerApiInternal");
                }

                if (id == InvalidId)
                {
                    throw new ArgumentException("invalid id");
                }

                this.m_positionerApiInternal = positionerApiInternal;
                this.Id = id;          
            }

        /// <summary>
        /// Get the location of this Positioner, in ECEF space. Use this with a GeographicTransform object to position GameObjects.
        /// </summary>
        public DoubleVector3 GetECEFLocation()
        {
            return m_positionerECEFLocation;
        }

        /// <summary>
        /// Set the location of this Positioner, at the specified latitude and longitude.
        /// </summary>
        /// <param name="latitudeDegrees">The desired latitude, in degrees.</param>
        /// <param name="longitudeDegrees">The desired longitude, in degrees.</param>
        public void SetLocation(double latitudeDegrees, double longitudeDegrees)
        {
            m_positionerApiInternal.SetPositionerLocation(this, latitudeDegrees, longitudeDegrees);
        }

        /// <summary>
        /// Set the elevation of this Positioner, in meters. The behaviour of this depends on the ElevationMode.
        /// </summary>
        /// <param name="elevation">The desired elevation, in meters.</param>
        public void SetElevation(double elevation)
        {
            m_positionerApiInternal.SetPositionerElevation(this, elevation);
        }

        /// <summary>
        /// Set the ElevationMode of this Positioner. See the ElevationMode documentation for more details.
        /// </summary>
        /// <param name="elevationMode">The desired ElevationMode of this positioner.</param>
        public void SetElevationMode(ElevationMode elevationMode)
        {
            m_positionerApiInternal.SetPositionerElevationMode(this, elevationMode);
        }

        /// <summary>
        /// Sets the Indoor Map of this Positioner. If this is unset, the Positioner will be outside instead.
        /// </summary>
        /// <param name="indoorMapId">The Indoor Map id string for the desired Indoor Map. See the IndoorMapApi documentation for more details.</param>
        /// <param name="indoorMapFloorId">The floor of the Indoor Map that this Positioner should be placed upon.</param>
        public void SetIndoorMap(string indoorMapId, int indoorMapFloorId)
        {
            m_positionerApiInternal.SetPositionerIndoorMap(this, indoorMapId, indoorMapFloorId);
        }

        /// <summary>
        /// Set the location of this Positioner, at the specified ECEF location. 
        /// </summary>
        /// <param name="ecefLocation">The desired location as a DoubleVector3, in ECEF space.</param>
        public void SetECEFLocation(DoubleVector3 ecefLocation)
        {
            m_positionerECEFLocation = ecefLocation;
        }

        /// <summary>
        /// Destroys the Positioner.
        /// </summary>
        public void Discard()
        {
            m_positionerApiInternal.DestroyPositioner(this);
            InvalidateId();
        }

        private void InvalidateId()
        {
            Id = InvalidId;
        }


    }
}
