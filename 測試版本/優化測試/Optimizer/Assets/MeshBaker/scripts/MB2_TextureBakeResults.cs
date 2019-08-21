using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using DigitalOpus.MB.Core;

/// <summary>
/// Used internally during the material baking process
/// </summary>
[Serializable]
public class MB_AtlasesAndRects{
	public Texture2D[] atlases;
    [NonSerialized]
	public List<MB_MaterialAndUVRect> mat2rect_map;
	public string[] texPropertyNames;
}

[System.Serializable]
public class MB_MultiMaterial{
	public Material combinedMaterial;
	public List<Material> sourceMaterials = new List<Material>();
}

[System.Serializable]
public class MB_MaterialAndUVRect
{
    public Material material;

    //The rectangle in the atlas where the texture (including all tiling) was copied to
    public Rect atlasRect;

    //the INVERTED subrectangle of atlasRect assuming uvs 0,0,1,1 (no UV tiling)
    //public Rect atlasSubrectMaterialOnly; 

    //for debugging. The name of the first srcObj that uses this MaterialAndUVRect.
    public string srcObjName;

    //The UV rect in the source texture that was used when copying from source texture to destinationAtlas.
    //If _fixOutOfBoundsUVs it may be smaller than, same size, or larger than 0..1. larger than means that these meshes have out of bounds UVs
    //If !_fixOutOfBoundsUVs it will be 0..1
    public Rect samplingRectMatAndUVTiling;

    //The material tiling on the source material
    public Rect sourceMaterialTiling;

    //The encapsulating sampling rect that was used to sample for the atlas
    public Rect samplingEncapsulatinRect;

    public MB_MaterialAndUVRect(Material m, Rect destRect, Rect samplingRectMatAndUVTiling, Rect sourceMaterialTiling, Rect samplingEncapsulatinRect, string objName)
    {
        material = m;
        atlasRect = destRect;
        //atlasSubrectMaterialOnly = matTilingTransformRect;
        this.samplingRectMatAndUVTiling = samplingRectMatAndUVTiling;
        this.sourceMaterialTiling = sourceMaterialTiling;
        this.samplingEncapsulatinRect = samplingEncapsulatinRect;
        srcObjName = objName;
    }

    public override int GetHashCode()
    {
        return material.GetInstanceID() ^ samplingEncapsulatinRect.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (!(obj is MB_MaterialAndUVRect)) return false;
        return (material == ((MB_MaterialAndUVRect)obj).material && samplingEncapsulatinRect == ((MB_MaterialAndUVRect)obj).samplingEncapsulatinRect);
    }
}

/// <summary>
/// This class stores the results from an MB2_TextureBaker when materials are combined into atlases. It stores
/// a list of materials and the corresponding UV rectangles in the atlases. It also stores the configuration
/// options that were used to generate the combined material.
/// 
/// It can be saved as an asset in the project so that textures can be baked in one scene and used in another.
/// 
/// </summary>

public class MB2_TextureBakeResults : ScriptableObject {
	public MB_AtlasesAndRects[] combinedMaterialInfo;

    //Depricated
	public Material[] materials;
    public Rect[] prefabUVRects;

    public MB_MaterialAndUVRect[] materialsAndUVRects;


	public Material resultMaterial;
	public MB_MultiMaterial[] resultMaterials;
	public bool doMultiMaterial;
	public bool fixOutOfBoundsUVs;

	/// <summary>
	/// Creates for materials on renderer.
	/// </summary>
	/// <returns>Generates an MB2_TextureBakeResult that can be used if all objects to be combined use the same material.
	/// Returns a MB2_TextureBakeResults that will map all materials used by renderer r to
	/// the rectangle 0,0..1,1 in the atlas.</returns>
	/// <param name="r">The red component.</param>
	public static MB2_TextureBakeResults CreateForMaterialsOnRenderer(GameObject[] gos)
    {

        HashSet<Material> fullMaterialList = new HashSet<Material>();
        for (int i = 0; i < gos.Length; i++)
        {
            if (gos[i] == null)
            {
                Debug.LogError(string.Format("Game object {0} in list of objects to add was null", i));
                return null;
            }
            Material[] oMats = MB_Utility.GetGOMaterials(gos[i]);
            if (oMats == null)
            {
                Debug.LogError(string.Format("Game object {0} in list of objects to add no renderer", i));
                return null;
            }
            for (int j = 0; j < oMats.Length; j++)
            {
                fullMaterialList.Add(oMats[j]);
            }
        }
        Material[] rms = new Material[fullMaterialList.Count];
        fullMaterialList.CopyTo(rms);

        MB2_TextureBakeResults tbr = (MB2_TextureBakeResults) ScriptableObject.CreateInstance( typeof(MB2_TextureBakeResults) );
        //Material[] ms = rms;
        //MB_MaterialAndUVRect[] mss = new MB_MaterialAndUVRect[rms.Length];
        List<MB_MaterialAndUVRect> mss = new List<MB_MaterialAndUVRect>();
        Material[] ms;
        for (int i = 0; i < rms.Length; i++)
        {
            if (rms[i] != null)
            {
                MB_MaterialAndUVRect matAndUVRect = new MB_MaterialAndUVRect(rms[i], new Rect(0f, 0f, 1f, 1f), new Rect(0f,0f,1f,1f), new Rect(0f,0f,1f,1f), new Rect(0f,0f,1f,1f), "");
                if (!mss.Contains(matAndUVRect))
                {
                    mss.Add(matAndUVRect);
                }
            }
        }
        if (rms.Length > 1) {
			tbr.prefabUVRects = new Rect[mss.Count];
            tbr.materials = ms = new Material[mss.Count];
            tbr.resultMaterials = new MB_MultiMaterial[mss.Count];
            for (int i = 0; i < mss.Count; i++){
                ms[i] = mss[i].material;
				tbr.prefabUVRects[i] = new Rect(0f,0f,1f,1f);
				tbr.resultMaterials[i] = new MB_MultiMaterial();
				List<Material> sourceMats = new List<Material>();
				sourceMats.Add (ms[i]);
				tbr.resultMaterials[i].sourceMaterials = sourceMats;
				tbr.resultMaterials[i].combinedMaterial = ms[i];
			}
			tbr.doMultiMaterial = true;
		} else {
			tbr.doMultiMaterial = false;
			tbr.prefabUVRects = new Rect[]{new Rect(0f,0f,1f,1f)};
            tbr.materials = ms = new Material[] { mss[0].material };
            tbr.resultMaterial = mss[0].material;
            tbr.resultMaterials = new MB_MultiMaterial[] { new MB_MultiMaterial() };
            List<Material> sourceMats = new List<Material>();
            sourceMats.Add(ms[0]);
            tbr.resultMaterials[0].sourceMaterials = sourceMats;
            tbr.resultMaterials[0].combinedMaterial = mss[0].material;
        }
        tbr.materialsAndUVRects = mss.ToArray();
        tbr.fixOutOfBoundsUVs = false;
        return tbr;
	}
	
    public bool ContainsMaterial(Material m)
    {
		if (materialsAndUVRects.Length == 0) {
			materialsAndUVRects = new MB_MaterialAndUVRect[materials.Length];
			for (int i = 0; i < materialsAndUVRects.Length; i++){
				materialsAndUVRects[i] = new MB_MaterialAndUVRect(materials[i], prefabUVRects[i],new Rect(0f,0f,1f,1f), new Rect(0f, 0f, 1f, 1f),new Rect(0f,0f,1f,1f),"");
			}
		}
        for (int i = 0; i < materialsAndUVRects.Length; i++)
        {
            if (materialsAndUVRects[i].material == m){
                return true;
            }
        }
        return false;
    }


	public string GetDescription(){
		StringBuilder sb = new StringBuilder();
		sb.Append("Shaders:\n");
		HashSet<Shader> shaders = new HashSet<Shader>();
		if (materialsAndUVRects != null){
			for (int i = 0; i < materialsAndUVRects.Length; i++){
                if (materialsAndUVRects[i].material != null)
                {
                    shaders.Add(materialsAndUVRects[i].material.shader);
                }	
			}
		}
		
		foreach(Shader m in shaders){
			sb.Append("  ").Append(m.name).AppendLine();
		}
		sb.Append("Materials:\n");
		if (materialsAndUVRects != null){
			for (int i = 0; i < materialsAndUVRects.Length; i++){
                if (materialsAndUVRects[i].material != null)
                {
                    sb.Append("  ").Append(materialsAndUVRects[i].material.name).AppendLine();
                }
			}
		}
		return sb.ToString();
	}

    public class Material2AtlasRectangleMapper
    {
        MB2_TextureBakeResults tbr;
        //bool allMatsAreUnique;
        int[] numTimesMatAppearsInAtlas;
        MB_MaterialAndUVRect[] matsAndSrcUVRect;
        //Rect[] uvRectInAtlas;

        public Material2AtlasRectangleMapper(MB2_TextureBakeResults res)
        {
            tbr = res;
            matsAndSrcUVRect = res.materialsAndUVRects;

            //backward compatibility. this may be an old TextureBakeResult which has no materialsAndUVRects if so then build it now
            if (matsAndSrcUVRect == null || matsAndSrcUVRect.Length == 0)
            {
                matsAndSrcUVRect = new MB_MaterialAndUVRect[res.materials.Length];
                for (int i = 0; i < res.materials.Length; i++)
                {
                    matsAndSrcUVRect[i] = new MB_MaterialAndUVRect(res.materials[i], res.prefabUVRects[i],new Rect(0f,0f,1f,1f), new Rect(0f, 0f, 1f, 1f), new Rect(0f,0f,1f,1f), "");
                }
                res.materialsAndUVRects = matsAndSrcUVRect;
            }

            //count the number of times a material appears in the atlas. used for fast lookup
            numTimesMatAppearsInAtlas = new int[matsAndSrcUVRect.Length];
            for (int i = 0; i < matsAndSrcUVRect.Length; i++)
            {
                if (numTimesMatAppearsInAtlas[i] > 1)
                {
                    continue;
                }
                int count = 1;
                for (int j = i + 1; j < matsAndSrcUVRect.Length; j++)
                {
                    if (matsAndSrcUVRect[i].material == matsAndSrcUVRect[j].material)
                    {
                        count++;
                    }
                }
                numTimesMatAppearsInAtlas[i] = count;
                if (count > 1)
                {
                    //allMatsAreUnique = false;
                    for (int j = i + 1; j < matsAndSrcUVRect.Length; j++)
                    {
                        if (matsAndSrcUVRect[i].material == matsAndSrcUVRect[j].material)
                        {
                            numTimesMatAppearsInAtlas[j] = count;
                        }
                    }
                }
            }
            
        }

        //a material can appear more than once in an atlas if using fixOutOfBoundsUVs.
        //in this case need to use the UV rect of the mesh to find the correct rectangle.
        public bool TryMapMaterialToUVRect(Material mat, Mesh m, int submeshIdx, MB3_MeshCombinerSingle.MeshChannelsCache meshChannelCache, Dictionary<int, MB_Utility.MeshAnalysisResult[]> meshAnalysisCache,
                                              out Rect rectInAtlas, 
                                              //out Rect subrectInAtlasMatTiling,
                                              out Rect encapsulatingRect,
                                              out Rect sourceMaterialTilingOut,
                                              ref String errorMsg,
                                              MB2_LogLevel logLevel)
        {
			if (tbr.materialsAndUVRects.Length == 0 && tbr.materials.Length > 0) {
				errorMsg = "The 'Material Bake Result' needs to be re-baked to be compatible with this version of Mesh Baker. Please re-bake using the MB3_TextureBaker.";
				rectInAtlas = new Rect();
                //subrectInAtlasMatTiling = new Rect();
                encapsulatingRect = new Rect();
                    sourceMaterialTilingOut = new Rect();
				return false;
			}
            if (mat == null)
            {
                rectInAtlas = new Rect();
                //subrectInAtlasMatTiling = new Rect();
                encapsulatingRect = new Rect();
                sourceMaterialTilingOut = new Rect();
                errorMsg = String.Format("Mesh {0} Had no material on submesh {1} cannot map to a material in the atlas", m.name, submeshIdx);
                return false;
            }
            if (submeshIdx >= m.subMeshCount)
            {
                errorMsg = "Submesh index is greater than the number of submeshes";
                rectInAtlas = new Rect();
                //subrectInAtlasMatTiling = new Rect();
                encapsulatingRect = new Rect();
                sourceMaterialTilingOut = new Rect();
                return false;
            }
            
            //find the first index of this material
            int idx = -1;
            for (int i = 0; i < matsAndSrcUVRect.Length; i++)
            {
                if (mat == matsAndSrcUVRect[i].material)
                {
                    idx = i;
                    break;
                }
            }
            // if couldn't find material
            if (idx == -1)
            {
                rectInAtlas = new Rect();
                //subrectInAtlasMatTiling = new Rect();
                encapsulatingRect = new Rect();
                sourceMaterialTilingOut = new Rect();
                errorMsg = String.Format("Material {0} could not be found in the Material Bake Result", mat.name);
                return false;
            }

            if (!tbr.fixOutOfBoundsUVs)
            {
                if (numTimesMatAppearsInAtlas[idx] != 1)
                {
                    Debug.LogError("There is a problem with this TextureBakeResults. FixOutOfBoundsUVs is false and a material appears more than once.");
                }
                rectInAtlas = matsAndSrcUVRect[idx].atlasRect;
                //subrectInAtlasMatTiling = matsAndSrcUVRect[idx].atlasSubrectMaterialOnly;
                encapsulatingRect = matsAndSrcUVRect[idx].samplingEncapsulatinRect;
                sourceMaterialTilingOut = matsAndSrcUVRect[idx].sourceMaterialTiling;
                return true;
            } else {
                //todo what if no UVs
                //Find UV rect in source mesh

                MB_Utility.MeshAnalysisResult[] mar;
                if (!meshAnalysisCache.TryGetValue(m.GetInstanceID(), out mar))
                {
                    mar = new MB_Utility.MeshAnalysisResult[m.subMeshCount];
                    for (int j = 0; j < m.subMeshCount; j++)
                    {
                        Vector2[] uvss = meshChannelCache.GetUv0Raw(m);
                        MB_Utility.hasOutOfBoundsUVs(uvss, m, ref mar[j], j);
                    }
                    meshAnalysisCache.Add(m.GetInstanceID(), mar);
                }

                //this could be a mesh that was not used in the texture baking that has huge UV tiling too big for the rect that was baked
                //find a record that has an atlas uvRect capable of containing this
                bool found = false;
                if (logLevel >= MB2_LogLevel.trace)
                {
                    Debug.LogWarning(String.Format("Trying to find a rectangle in atlas capable of holding tiled sampling rect for mesh {0} using material {1}",m,mat));
                }
                for (int i = idx; i < matsAndSrcUVRect.Length; i++)
                {
                    if (matsAndSrcUVRect[i].material == mat)
                    {
                        //calculate what the UV rect in the atlas would be if we combined the material tiling of this rect with the UV tiling of this submesh
                        Rect potentialRect = new Rect(); 
                        Rect uvR = mar[submeshIdx].uvRect;
                        Rect matR = matsAndSrcUVRect[i].sourceMaterialTiling;
                        // test to see if this would fit in what was baked in the atlas
                        Rect rr = matsAndSrcUVRect[i].samplingEncapsulatinRect;
                        MB3_UVTransformUtility.Canonicalize(ref rr, 0,0);

                        potentialRect = MB3_UVTransformUtility.CombineTransforms(ref uvR, ref matR);
                        if (logLevel >= MB2_LogLevel.trace)
                        {
                            Debug.Log("uvR=" + uvR.ToString("f5") + " matR=" + matR.ToString("f5") + "Potential Rect " + potentialRect.ToString("f5") + " encapsulating=" + rr.ToString("f5"));
                        }
                        MB3_UVTransformUtility.Canonicalize(ref potentialRect,rr.x,rr.y);
                        if (logLevel >= MB2_LogLevel.trace)
                        {
                            Debug.Log("Potential Rect Cannonical " + potentialRect.ToString("f5") + " encapsulating=" + rr.ToString("f5"));
                        }

                        if (MB3_UVTransformUtility.RectContains(ref rr, ref potentialRect)) {
                            if (logLevel >= MB2_LogLevel.trace)
                            {
                                Debug.Log("Found rect in atlas capable of containing tiled sampling rect for mesh " + m + " at idx=" + i);
                            }
                            idx = i;
                            found = true;
                            break;
                        }
                    }
                }
                if (found)
                {
                    rectInAtlas = matsAndSrcUVRect[idx].atlasRect;
                    //subrectInAtlasMatTiling = matsAndSrcUVRect[idx].atlasSubrectMaterialOnly;
                    encapsulatingRect = matsAndSrcUVRect[idx].samplingEncapsulatinRect;
                    sourceMaterialTilingOut = matsAndSrcUVRect[idx].sourceMaterialTiling;
                    return true;
                } else
                {
                    rectInAtlas = new Rect();
                    //subrectInAtlasMatTiling = new Rect();
                    encapsulatingRect = new Rect();
                    sourceMaterialTilingOut = new Rect();
                    errorMsg = String.Format("Could not find a tiled rectangle in the atlas capable of containing the uv and material tiling on mesh {0}", m.name);
                    return false;
                }
            }
        }
    }
}