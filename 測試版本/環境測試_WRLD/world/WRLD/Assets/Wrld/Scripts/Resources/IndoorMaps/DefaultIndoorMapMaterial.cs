using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Wrld.Resources.IndoorMaps
{
    public class DefaultIndoorMapMaterial : IIndoorMapMaterial
    {
        private Color m_diffuseColor;
        private bool m_hasCustomDrawOrder;
        private Material m_prepassMaterial;

        public Material MaterialInstance { get; private set; }
        public Action<string, Texture> OnStreamingTextureReceived { get; set; }

        public DefaultIndoorMapMaterial(Material material, Color diffuseColor, bool hasCustomDrawOrder, Material prepassMaterial)
        {
            MaterialInstance = material;
            m_diffuseColor = diffuseColor;
            m_hasCustomDrawOrder = hasCustomDrawOrder;
            m_prepassMaterial = prepassMaterial;

            OnStreamingTextureReceived += OnStreamingTextureReceivedHandler;
        }

        public void AssignToMeshRenderer(MeshRenderer renderer)
        {
            if (m_prepassMaterial != null)
            {
                renderer.sharedMaterials = new[] { MaterialInstance, m_prepassMaterial };
            }
            else
            {
                renderer.sharedMaterial = MaterialInstance;
            }
        }

        public void PrepareToRender(IndoorMapRenderable renderable)
        {
            var color = renderable.GetColor() * m_diffuseColor;
            SetMaterialColor(color, renderable.GetFloorIndex(), renderable.gameObject);
        }

        public void PrepareToRender(IndoorMapHighlightRenderable renderable)
        {
            var color = renderable.GetColor();
            const float maxHighlightAlpha = 0.5f;
            color.a *= maxHighlightAlpha;
            SetMaterialColor(color, renderable.GetFloorIndex(), renderable.gameObject);
        }

        public void PrepareToRender(InstancedIndoorMapRenderable renderable)
        {
            Color color;

            if (!renderable.TryGetHighlightColor(out color))
            {
                color = renderable.GetColor() * m_diffuseColor;
            }

            SetMaterialColor(color, renderable.GetFloorIndex(), renderable.gameObject);
        }

        public IIndoorMapMaterial CreateCopy()
        {
            return new DefaultIndoorMapMaterial(new Material(MaterialInstance), m_diffuseColor, m_hasCustomDrawOrder, m_prepassMaterial);
        }

        public void OnStreamingTextureReceivedHandler(string textureKey, Texture texture)
        {
            if (textureKey.Equals("CubeMapTexturePath"))
            {   
                MaterialInstance.SetTexture(Shader.PropertyToID("_Cube"), texture);
            }
            else
            {
                MaterialInstance.SetTexture(Shader.PropertyToID("_MainTex"), texture);
            }
        }

        private void SetMaterialColor(Color color, int floorIndex, GameObject gameObject)
        {
            // https://docs.unity3d.com/Manual/MaterialsAccessingViaScript.html
            const string fadeRenderMode = "_ALPHABLEND_ON";

            bool isVisible = color.a > 0.0f;
            bool isOpaque = color.a >= 1.0f;

            var meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>(true);

            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                var material = meshRenderer.sharedMaterial;
                meshRenderer.receiveShadows = false;
                
                if (isVisible)
                {
                    meshRenderer.enabled = true;
                    meshRenderer.shadowCastingMode = ShadowCastingMode.Off;

                    material.SetFloat("_ZWrite", isOpaque ? 1.0f : 0.0f);

                    if (!m_hasCustomDrawOrder)
                    {
                        material.renderQueue = isOpaque ? ((int)UnityEngine.Rendering.RenderQueue.Geometry) : ((int)UnityEngine.Rendering.RenderQueue.Transparent) + floorIndex + 1;
                    }

                    if (isOpaque)
                    {
                        material.DisableKeyword(fadeRenderMode);
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    }
                    else
                    {
                        material.EnableKeyword(fadeRenderMode);
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    }

                    material.color = color;
                }
                else
                {
                    meshRenderer.enabled = false;
                }
            }
        }
    }
}