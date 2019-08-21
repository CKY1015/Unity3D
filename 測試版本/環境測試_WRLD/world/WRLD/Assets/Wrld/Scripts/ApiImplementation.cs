using UnityEngine;
using Wrld.Common.Maths;
using Wrld.Materials;
using Wrld.Streaming;
using Wrld.Space;
using Wrld.Common.Camera;
using Wrld.MapCamera;
using Wrld.Resources.Buildings;
using Wrld.Resources.IndoorMaps;
using Wrld.Space.Positioners;
using Wrld.Resources.Labels;
using Wrld.Space.EnvironmentFlattening;
using System.Runtime.InteropServices;
using Wrld.Meshes;

namespace Wrld
{
    // :TODO: Feels like it might be more natural/usual to split this into ECEF & UnityWorld classes & have both implement same interface.
    public class ApiImplementation
    {
        private NativePluginRunner m_nativePluginRunner;
        private CoordinateSystem m_coordinateSystem;
        private CameraApiInternal m_cameraApiInternal;
        private CameraApi m_cameraController;
        private BuildingsApi m_buildingsApi;
        private IndoorMapsApi m_indoorMapsApi;
        private GeographicApi m_geographicApi;
        private BuildingsApiInternal m_buildingsApiInternal;
        private IndoorMapsApiInternal m_indoorMapsApiInternal;
        private SpacesApi m_spacesApi;
        private PositionerApi m_positionerApi;
        private PositionerApiInternal m_positionerApiInternal;
        private EnvironmentFlatteningApi m_environmentFlatteningApi;
        private EnvironmentFlatteningApiInternal m_environmentFlatteningApiInternal;
        private UnityWorldSpaceCoordinateFrame m_frame;
        private DoubleVector3 m_originECEF;
        private InterestPointProvider m_interestPointProvider = new InterestPointProvider();
        private GameObjectStreamer m_terrainStreamer;
        private GameObjectStreamer m_roadStreamer;
        private GameObjectStreamer m_buildingStreamer;
        private GameObjectStreamer m_highlightStreamer;
        private GameObjectStreamer m_indoorMapStreamer;
        private MapGameObjectScene m_mapGameObjectScene;
        private LabelServiceInternal m_labelServiceInternal;
        private GameObject m_root;

        public ApiImplementation(string apiKey, CoordinateSystem coordinateSystem, Transform parentTransformForStreamedObjects, ConfigParams configParams)
        {
            var textureLoadHandler = new TextureLoadHandler();
            var materialRepository = new MaterialRepository(configParams.MaterialsDirectory, configParams.OverrideLandmarkMaterial, textureLoadHandler);

            var terrainCollision = (configParams.Collisions.TerrainCollision) ? CollisionStreamingType.SingleSidedCollision : CollisionStreamingType.NoCollision;
            var roadCollision = (configParams.Collisions.RoadCollision) ? CollisionStreamingType.DoubleSidedCollision : CollisionStreamingType.NoCollision;
            var buildingCollision = (configParams.Collisions.BuildingCollision) ? CollisionStreamingType.SingleSidedCollision : CollisionStreamingType.NoCollision;

            m_root = new GameObject("Root");
            m_root.transform.SetParent(parentTransformForStreamedObjects, false);

            m_terrainStreamer = new GameObjectStreamer("Terrain", materialRepository, m_root.transform, terrainCollision, true);
            m_roadStreamer = new GameObjectStreamer("Roads", materialRepository, m_root.transform, roadCollision, true);
            m_buildingStreamer = new GameObjectStreamer("Buildings", materialRepository, m_root.transform, buildingCollision, true);
            m_highlightStreamer = new GameObjectStreamer("Highlights", materialRepository, m_root.transform, CollisionStreamingType.NoCollision, false);
            m_indoorMapStreamer = new GameObjectStreamer("IndoorMaps", materialRepository, m_root.transform, CollisionStreamingType.NoCollision, false);

            var indoorMapMaterialRepository = new IndoorMapMaterialRepository();
            
            var indoorMapStreamedTextureObserver = new IndoorMapStreamedTextureObserver(indoorMapMaterialRepository);
            var indoorMapTextureStreamingService = new IndoorMapTextureStreamingService(textureLoadHandler, indoorMapStreamedTextureObserver);
            m_indoorMapsApiInternal = new IndoorMapsApiInternal(indoorMapTextureStreamingService);
            var indoorMapMaterialService = new IndoorMapMaterialService(indoorMapMaterialRepository, m_indoorMapsApiInternal);

            m_indoorMapsApi = new IndoorMapsApi(m_indoorMapsApiInternal);

            var meshUploader = new MeshUploader();
            var indoorMapScene = new IndoorMapScene(m_indoorMapStreamer, meshUploader, indoorMapMaterialService, m_indoorMapsApiInternal);
            m_mapGameObjectScene = new MapGameObjectScene(m_terrainStreamer, m_roadStreamer, m_buildingStreamer, m_highlightStreamer, m_indoorMapStreamer, meshUploader, indoorMapScene);
            m_labelServiceInternal = new LabelServiceInternal(UnityEngine.GameObject.Find("Canvas"), configParams.EnableLabels);

            m_positionerApiInternal = new PositionerApiInternal();
            m_positionerApi = new PositionerApi(m_positionerApiInternal);

            m_cameraApiInternal = new CameraApiInternal();

            m_buildingsApiInternal = new BuildingsApiInternal(materialRepository);
            m_buildingsApi = new BuildingsApi(m_buildingsApiInternal);

            m_nativePluginRunner = new NativePluginRunner(
                apiKey, 
                textureLoadHandler, 
                materialRepository, 
                m_mapGameObjectScene, 
                configParams, 
                indoorMapScene, 
                m_indoorMapsApiInternal, 
                indoorMapMaterialService, 
                m_labelServiceInternal, 
                m_positionerApiInternal,
                m_cameraApiInternal,
                m_buildingsApiInternal
                );

            m_cameraController = new CameraApi(this, m_cameraApiInternal);

            m_coordinateSystem = coordinateSystem;
            var defaultStartingLocation = LatLongAltitude.FromDegrees(
                configParams.LatitudeDegrees, 
                configParams.LongitudeDegrees, 
                coordinateSystem == CoordinateSystem.ECEF ? configParams.DistanceToInterest : 0.0);

            if (coordinateSystem == CoordinateSystem.ECEF)
            {
                m_originECEF = defaultStartingLocation.ToECEF();
            }
            else
            {
                m_frame = new UnityWorldSpaceCoordinateFrame(defaultStartingLocation);
            }

            m_geographicApi = new GeographicApi();
            
            m_environmentFlatteningApiInternal = new EnvironmentFlatteningApiInternal();
            m_environmentFlatteningApi = new EnvironmentFlatteningApi(m_environmentFlatteningApiInternal);

            m_spacesApi = new SpacesApi();
        }

        public void SetOriginPoint(LatLongAltitude lla)
        {
            if (m_coordinateSystem == CoordinateSystem.ECEF)
            {
                m_cameraController.MoveTo(lla.GetLatLong());
            }
            else
            {
                m_frame.SetCentralPoint(lla);
            }
        }

        internal void ApplyNativeCameraState(NativeCameraState nativeCameraState, UnityEngine.Camera controlledCamera)
        {
            controlledCamera.fieldOfView = nativeCameraState.fieldOfViewDegrees;
            controlledCamera.nearClipPlane = nativeCameraState.nearClipPlaneDistance;
            controlledCamera.farClipPlane = nativeCameraState.farClipPlaneDistance;
            
            var interestBasis = new EcefTangentBasis(
                    nativeCameraState.interestPointECEF,
                    nativeCameraState.interestBasisRightECEF,
                    nativeCameraState.interestBasisUpECEF,
                    nativeCameraState.interestBasisForwardECEF);

            Vector3 forward, up;
            DoubleVector3 positionECEF;
            CameraHelpers.CalculateLookAt(
                interestBasis.PointEcef,
                interestBasis.Forward,
                nativeCameraState.pitchDegrees * Mathf.Deg2Rad,
                nativeCameraState.distanceToInterestPoint,
                out positionECEF, out forward, out up);

            m_interestPointProvider.UpdateFromNative(nativeCameraState.interestPointECEF);

            if (m_coordinateSystem == CoordinateSystem.ECEF)
            {
                var position = (positionECEF - m_originECEF).ToSingleVector();
                controlledCamera.transform.position = position;
                controlledCamera.transform.LookAt((interestBasis.PointEcef - m_originECEF).ToSingleVector(), up);
            }
            else // if (m_coordinateSystem == CoordinateSystem.UnityWorld)
            {
                controlledCamera.transform.position = m_frame.ECEFToLocalSpace(positionECEF);
                controlledCamera.transform.LookAt(m_frame.ECEFToLocalSpace(interestBasis.PointEcef), m_frame.ECEFToLocalRotation * up);
            }
        }

        public void StreamResourcesForBuiltInCamera(UnityEngine.Camera streamingCamera)
        {
            m_nativePluginRunner.StreamResourcesForBuiltInCamera();

            // re-centre the camera for ECEF
            if (m_coordinateSystem == CoordinateSystem.ECEF)
            {
                m_originECEF += streamingCamera.transform.position;
                streamingCamera.transform.position = Vector3.zero;
            }
        }

        public void StreamResourcesForCamera(UnityEngine.Camera streamingCamera)
        {
            // This code would be a lot cleaner if there was a decent way of copying camera properties around.
            // Maybe we should just make StreamResourcesForCamera ingest something more abstract
            //UnityEngine.Camera zeroBasedCameraECEF = new UnityEngine.Camera();
            //zeroBasedCameraECEF.CopyFrom(streamingCamera);
            var savedPosition = streamingCamera.transform.position;

            if (m_coordinateSystem == CoordinateSystem.ECEF)
            {
                DoubleVector3 finalOriginECEF = m_originECEF + savedPosition;
                streamingCamera.transform.position = Vector3.zero;
                DoubleVector3 interestPointECEF = m_interestPointProvider.CalculateInterestPoint(streamingCamera, finalOriginECEF);
                m_nativePluginRunner.StreamResourcesForCamera(streamingCamera, finalOriginECEF, interestPointECEF);
                m_originECEF = finalOriginECEF; // :TODO: somehow update any other scene-relative cameras - OnRecentreScene event?
                // TODO: Why aren't restoring savedPosition here?
            }
            else // if (m_coordinateSystem == CoordinateSystem.UnityWorld)
            {
                var savedRotation = streamingCamera.transform.rotation;
                DoubleVector3 finalOriginECEF = m_frame.LocalSpaceToECEF(savedPosition);
                streamingCamera.transform.rotation = m_frame.LocalToECEFRotation * savedRotation;
                streamingCamera.transform.position = Vector3.zero;
                DoubleVector3 interestPointECEF = m_interestPointProvider.CalculateInterestPoint(streamingCamera, finalOriginECEF);
                m_nativePluginRunner.StreamResourcesForCamera(streamingCamera, finalOriginECEF, interestPointECEF);
                streamingCamera.transform.position = savedPosition;
                streamingCamera.transform.rotation = savedRotation;
            }
        }

        private void UpdateTransforms()
        {
            ITransformUpdateStrategy transformUpdateStrategy;

            if (m_coordinateSystem == CoordinateSystem.UnityWorld)
            {
                transformUpdateStrategy = new UnityWorldSpaceTransformUpdateStrategy(m_frame, m_environmentFlatteningApi.GetCurrentScale());
            }
            else
            {
                var cameraPosition = m_originECEF;// + cam.transform.localPosition;
                transformUpdateStrategy = new ECEFTransformUpdateStrategy(
                    cameraPosition,
                    cameraPosition.normalized.ToSingleVector(),
                    m_environmentFlatteningApi.GetCurrentScale());
            }

            m_nativePluginRunner.UpdateTransforms(transformUpdateStrategy);
            m_geographicApi.UpdateTransforms(transformUpdateStrategy);
        }

        public CameraApi CameraApi
        {
            get
            {
                return m_cameraController;
            }
        }

        public BuildingsApi BuildingsApi
        {
            get
            {
                return m_buildingsApi;
            }
        }

        public IndoorMapsApi IndoorMapsApi
        {
            get
            {
                return m_indoorMapsApi;
            }
        }

        public GeographicApi GeographicApi
        {
            get
            {
                return m_geographicApi;
            }
        }

        public SpacesApi SpacesApi
        {
            get
            {
                return m_spacesApi;
            }
        }

        public PositionerApi PositionerApi
        {
            get
            {
                return m_positionerApi;
            }
        }

        public EnvironmentFlatteningApi EnvironmentFlatteningApi
        {
            get
            {
                return m_environmentFlatteningApi;
            }
        }

        public void Update()
        {
            m_cameraController.UpdateInput();
            m_nativePluginRunner.Update();
            m_cameraController.Update(Time.deltaTime);
            UpdateTransforms();
        }

        public void UpdateCollision(ConfigParams.CollisionConfig collisions)
        {
            m_nativePluginRunner.UpdateCollisions(collisions);
        }

        internal void SetEnabled(bool enabled)
        {
            m_mapGameObjectScene.SetEnabled(enabled);
        }

        public void Destroy()
        {
            m_nativePluginRunner.OnDestroy();
            m_terrainStreamer.Destroy();
            m_roadStreamer.Destroy();
            m_buildingStreamer.Destroy();
            m_highlightStreamer.Destroy();
            m_indoorMapStreamer.Destroy();
            m_labelServiceInternal.Destroy();
            m_positionerApiInternal.Destroy();
            m_mapGameObjectScene.Destroy();
            m_cameraApiInternal.Destroy();

            GameObject.Destroy(m_root);
            m_root = null;
        }

        internal Vector3 GeographicToWorldPoint(LatLongAltitude position, Camera camera)
        {
            if (m_coordinateSystem == CoordinateSystem.UnityWorld)
            {
                return m_frame.ECEFToLocalSpace(position.ToECEF());
            }
            else
            {
                return (position.ToECEF() - m_originECEF).ToSingleVector();
            }
        }

        internal LatLongAltitude WorldToGeographicPoint(Vector3 position, Camera camera)
        {
            if (m_coordinateSystem == CoordinateSystem.UnityWorld)
            {
                return m_frame.LocalSpaceToLatLongAltitude(position);
            }
            else
            {
                var ecefPosition = m_originECEF + position;
                return LatLongAltitude.FromECEF(ecefPosition);
            }
        }

        internal Vector3 GeographicToViewportPoint(LatLongAltitude position, Camera camera)
        {
            if (m_coordinateSystem == CoordinateSystem.UnityWorld)
            {
                var point = m_frame.ECEFToLocalSpace(position.ToECEF());
                return camera.WorldToViewportPoint(point);
            }
            else
            {
                var point = (position.ToECEF() - m_originECEF).ToSingleVector();
                return camera.WorldToViewportPoint(point);
            }
        }

        internal LatLongAltitude ViewportToGeographicPoint(Vector3 viewportSpacePosition, UnityEngine.Camera camera)
        {
            var unityWorldSpacePosition = camera.ViewportToWorldPoint(viewportSpacePosition);

            if (m_coordinateSystem == CoordinateSystem.UnityWorld)
            {
                return m_frame.LocalSpaceToLatLongAltitude(unityWorldSpacePosition);
            }
            else
            {
                var finalPositionECEF = m_originECEF + unityWorldSpacePosition;

                return LatLongAltitude.FromECEF(finalPositionECEF);
            }
        }

        public void OnApplicationPaused()
        {
            m_nativePluginRunner.OnApplicationPaused();
        }

        public void OnApplicationResumed()
        {
            m_nativePluginRunner.OnApplicationResumed();
        }
    }
}
