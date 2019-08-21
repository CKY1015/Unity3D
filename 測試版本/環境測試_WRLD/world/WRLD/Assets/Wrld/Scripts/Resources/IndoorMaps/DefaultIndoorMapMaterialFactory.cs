using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Wrld.Resources.IndoorMaps
{
    public class DefaultIndoorMapMaterialFactory : IIndoorMapMaterialFactory
    {
        Material m_templateMaterial;
        Material m_highlightTemplateMaterial;
        Material m_prepassMaterial;
        Dictionary<string, Material> m_materialArchtypesByType = new Dictionary<string, Material>();

        public DefaultIndoorMapMaterialFactory()
        {
            m_templateMaterial = GetOrLoadMaterialArchetype("InteriorsDiffuseTexturedMaterial");
            m_highlightTemplateMaterial = GetOrLoadMaterialArchetype("InteriorsHighlightMaterial");
            m_prepassMaterial = GetOrLoadMaterialArchetype("InteriorsStencilMirrorMaskMaterial");
        }

        public IIndoorMapMaterial CreateMaterialFromDescriptor(IndoorMaterialDescriptor descriptor)
        {
            var sourceMaterial = descriptor.MaterialName.Contains("highlight") ? m_highlightTemplateMaterial : m_templateMaterial;
            string materialType;

            if (descriptor.Strings.TryGetValue("MaterialType", out materialType))
            {
                if (materialType.StartsWith("Interior"))
                {
                    sourceMaterial = GetOrLoadMaterialArchetype(materialType);
                }
            }
            else
            {
                materialType = string.Empty;
            }
            
            var material = new Material(sourceMaterial);

            Color diffuseColor;

            if (!descriptor.Colors.TryGetValue("DiffuseColor", out diffuseColor))
            {
                diffuseColor = Color.white;
            }

            material.color = diffuseColor;
            material.name = descriptor.MaterialName;            
            bool requiresStencilPrePass = materialType == "InteriorsStencilMirrorMaterial";
            bool isForReflectionGeometry = materialType == "InteriorsReflectionMaterial";
            bool hasCustomDrawOrder = requiresStencilPrePass || isForReflectionGeometry || diffuseColor.a < 1.0f;

            return new DefaultIndoorMapMaterial(material, diffuseColor, hasCustomDrawOrder, requiresStencilPrePass ? m_prepassMaterial : null);
        }

        private Material GetOrLoadMaterialArchetype(string materialType)
        {
            if (!m_materialArchtypesByType.ContainsKey(materialType))
            {
                m_materialArchtypesByType[materialType] = (Material)UnityEngine.Resources.Load(Path.Combine("WrldMaterials/Archetypes", materialType), typeof(Material));
            }

            return m_materialArchtypesByType[materialType];
        }
    }
}


