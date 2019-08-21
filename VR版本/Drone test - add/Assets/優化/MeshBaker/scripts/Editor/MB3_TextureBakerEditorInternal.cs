//----------------------------------------------
//            MeshBaker
// Copyright © 2011-2012 Ian Deane
//---------------------------------------------- 
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DigitalOpus.MB.Core;

using UnityEditor;

namespace DigitalOpus.MB.Core{
	public class MB3_TextureBakerEditorInternal{
		//add option to exclude skinned mesh renderer and mesh renderer in filter
		//example scenes for multi material
		
		private static GUIContent insertContent = new GUIContent("+", "add a material");
		private static GUIContent deleteContent = new GUIContent("-", "delete a material");
		private static GUILayoutOption buttonWidth = GUILayout.MaxWidth(20f);
        public static GUIContent noneContent = new GUIContent("");

        //private SerializedObject textureBaker;
		private SerializedProperty logLevel, textureBakeResults, maxTilingBakeSize, maxAtlasSize, 
            doMultiMaterial, fixOutOfBoundsUVs, resultMaterial, resultMaterials, atlasPadding, 
            resizePowerOfTwoTextures, customShaderProperties, objsToMesh, texturePackingAlgorithm, 
            forcePowerOfTwoAtlas, considerNonTextureProperties, sortOrderAxis;
		
		bool resultMaterialsFoldout = true;
		bool showInstructions = false;
		bool showContainsReport = true;

        GUIStyle multipleMaterialBackgroundStyle = new GUIStyle();
        GUIStyle multipleMaterialBackgroundStyleDarker = new GUIStyle();
        GUIStyle editorBoxBackgroundStyle = new GUIStyle();


        Texture2D multipleMaterialBackgroundColor;
        Texture2D multipleMaterialBackgroundColorDarker;
        Texture2D editorBoxBackgroundColor;

        Color buttonColor = new Color(.8f,.8f,1f,1f);

        private static GUIContent
            createPrefabAndMaterialLabelContent = new GUIContent("Create Empty Assets For Combined Material", "Creates a material asset and a 'MB2_TextureBakeResult' asset. You should set the shader on the material. Mesh Baker uses the Texture properties on the material to decide what atlases need to be created. The MB2_TextureBakeResult asset should be used in the 'Material Bake Result' field."),
            logLevelContent = new GUIContent("Log Level"),
			openToolsWindowLabelContent = new GUIContent("Open Tools For Adding Objects", "Use these tools to find out what can be combined, discover possible problems with meshes, and quickly add objects."),
			fixOutOfBoundsGUIContent = new GUIContent("Consider Mesh UVs", "(Was called 'fix out of bounds UVs') The textures will be sampled based on mesh uv rectangle as well as material tiling. This can have two effects:\n\n" + 
                                                        "1) If the mesh only uses a small rectangle of it's source material (atlas). Only that small rectangle will be baked into the atlas.\n\n" +
                                                        "2) If the mesh has uvs outside the 0,1 range (tiling) then this tiling will be baked into the atlas."),
			resizePowerOfTwoGUIContent = new GUIContent("Resize Power-Of-Two Textures", "Shrinks textures so they have a clear border of width 'Atlas Padding' around them. Improves texture packing efficiency."),
			customShaderPropertyNamesGUIContent = new GUIContent("Custom Shader Propert Names", "Mesh Baker has a list of common texture properties that it looks for in shaders to generate atlases. Custom shaders may have texture properties not on this list. Add them here and Meshbaker will generate atlases for them."),
			combinedMaterialsGUIContent = new GUIContent("Combined Materials", "Use the +/- buttons to add multiple combined materials. You will also need to specify which materials on the source objects map to each combined material."),
			maxTilingBakeSizeGUIContent = new GUIContent("Max Tiling Bake Size","This is the maximum size tiling textures will be baked to."),
			maxAtlasSizeGUIContent = new GUIContent("Max Atlas Size","This is the maximum size of the atlas. If the atlas is larger than this textures being added will be shrunk."),

			objectsToCombineGUIContent = new GUIContent("Objects To Be Combined","These can be prefabs or scene objects. They must be game objects with Renderer components, not the parent objects. Materials on these objects will baked into the combined material(s)"),
			textureBakeResultsGUIContent = new GUIContent("Material Bake Result","This asset contains a mapping of materials to UV rectangles in the atlases. It is needed to create combined meshes or adjust meshes so they can use the combined material(s). Create it using 'Create Empty Assets For Combined Material'. Drag it to the 'Material Bake Result' field to use it."),
			texturePackingAgorithmGUIContent = new GUIContent("Texture Packer", "Unity's PackTextures: Atlases are always a power of two. Can crash when trying to generate large atlases. \n\n "+
			                                                  "Mesh Baker Texture Packer: Atlases will be most efficient size and shape (not limited to a power of two). More robust for large atlases. \n\n"+
			                                                  "Mesh Baker Texture Packer Fast: Same as Mesh Baker Texture Packer but creates atlases on the graphics card using RenderTextures instead of the CPU. Source textures can be compressed. Requires Unity 5 or Unity 4 Pro. May not be pixel perfect."),
			configMultiMatFromObjsContent = new GUIContent("Build Source To Combined Mapping From \n Objects To Be Combined", "This will group the materials on your source objects by shader and create one source to combined mapping for each shader found. For example if combining trees then all the materials with the same bark shader will be grouped togther and all the materials with the same leaf material will be grouped together. You can adjust the results afterwards. \n\nIf fix out-of-bounds UVs is NOT checked then submeshes with UVs outside 0,0..1,1 will be mapped to their own submesh regardless of shader."),
			forcePowerOfTwoAtlasContent = new GUIContent("Force Power-Of-Two Atlas","Forces atlas x and y dimensions to be powers of two with aspect ratio 1:1,1:2 or 2:1. Unity recommends textures be a power of two for everything but GUI textures."),
            considerNonTexturePropertiesContent = new GUIContent("Blend Non-Texture Properties","Will blend non-texture properties such as _Color, _Glossiness with the textures. Objects with different non-texture property values will be copied into different parts of the atlas even if they use the same textures. This feature requires that TextureBlenders " +
                                                            "exist for the result material shader. It is easy to extend Mesh Baker by writing custom TextureBlenders. Default TextureBlenders exist for: \n" +
                                                             "  - Standard \n" +
                                                             "  - Diffuse \n" +
                                                             "  - Bump Diffuse\n"),
            gc_SortAlongAxis = new GUIContent("SortAlongAxis", "Transparent materials often require that triangles be rendered in a certain order. This will sort Game Objects along the specified axis. Triangles will be added to the combined mesh in this order.");




    [MenuItem("GameObject/Create Other/Mesh Baker/Material Only Baker")]
		public static void CreateNewTextureBaker(){
			MB3_TextureBaker[] mbs = (MB3_TextureBaker[]) Editor.FindObjectsOfType(typeof(MB3_TextureBaker));
	    	Regex regex = new Regex(@"(\d+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
			int largest = 0;
			try{
				for (int i = 0; i < mbs.Length; i++){
					Match match = regex.Match(mbs[i].name);
					if (match.Success){
						int val = Convert.ToInt32(match.Groups[1].Value);
						if (val >= largest)
							largest = val + 1;
					}
				}
			} catch(Exception e){
				if (e == null) e = null; //Do nothing supress compiler warning
			}
			GameObject nmb = new GameObject("MaterialBaker" + largest);
			nmb.transform.position = Vector3.zero;
			nmb.AddComponent<MB3_MeshBakerGrouper>();
			MB3_TextureBaker tb = nmb.AddComponent<MB3_TextureBaker>();
			tb.packingAlgorithm = MB2_PackingAlgorithmEnum.MeshBakerTexturePacker;
		}
	
		void _init(SerializedObject textureBaker) {
            //textureBaker = new SerializedObject(target);
            logLevel = textureBaker.FindProperty("LOG_LEVEL");
			doMultiMaterial = textureBaker.FindProperty("_doMultiMaterial");
			fixOutOfBoundsUVs = textureBaker.FindProperty("_fixOutOfBoundsUVs");
			resultMaterial = textureBaker.FindProperty("_resultMaterial");
			resultMaterials = textureBaker.FindProperty("resultMaterials");
			atlasPadding = textureBaker.FindProperty("_atlasPadding");
			resizePowerOfTwoTextures = textureBaker.FindProperty("_resizePowerOfTwoTextures");
			customShaderProperties= textureBaker.FindProperty("_customShaderProperties");
			objsToMesh = textureBaker.FindProperty("objsToMesh");
			maxTilingBakeSize = textureBaker.FindProperty("_maxTilingBakeSize");
			maxAtlasSize = textureBaker.FindProperty("_maxAtlasSize");
			textureBakeResults = textureBaker.FindProperty("_textureBakeResults");
			texturePackingAlgorithm = textureBaker.FindProperty("_packingAlgorithm");
			forcePowerOfTwoAtlas = textureBaker.FindProperty("_meshBakerTexturePackerForcePowerOfTwo");
            considerNonTextureProperties = textureBaker.FindProperty("_considerNonTextureProperties");
            sortOrderAxis = textureBaker.FindProperty("sortAxis");
        }	

        public void OnEnable(SerializedObject textureBaker)
        {
            _init(textureBaker);
            bool isPro = EditorGUIUtility.isProSkin;
            Color backgroundColor = isPro
                ? new Color32(35, 35, 35, 255)
                : new Color32(174, 174, 174, 255);
            multipleMaterialBackgroundColor = MB3_MeshBakerEditorFunctions.MakeTex(8, 8, backgroundColor);
            backgroundColor = isPro
                ? new Color32(50, 50, 50, 255)
                : new Color32(160, 160, 160, 255);
            multipleMaterialBackgroundColorDarker = MB3_MeshBakerEditorFunctions.MakeTex(8, 8, backgroundColor);
            backgroundColor = isPro
                ? new Color32(35, 35, 35, 255)
                : new Color32(174, 174, 174, 255);
            editorBoxBackgroundColor = MB3_MeshBakerEditorFunctions.MakeTex(8,8,backgroundColor);
            multipleMaterialBackgroundStyle.normal.background = multipleMaterialBackgroundColor;
            multipleMaterialBackgroundStyleDarker.normal.background = multipleMaterialBackgroundColorDarker;
            editorBoxBackgroundStyle.normal.background = editorBoxBackgroundColor;
            editorBoxBackgroundStyle.border = new RectOffset(0, 0, 0, 0);
            editorBoxBackgroundStyle.margin = new RectOffset(5, 5, 5, 5);
            editorBoxBackgroundStyle.padding = new RectOffset(10, 10, 10, 10);

        }

        public void OnDisable()
        {
            if (multipleMaterialBackgroundColor != null) GameObject.DestroyImmediate(multipleMaterialBackgroundColor);
            if (multipleMaterialBackgroundColorDarker != null)  GameObject.DestroyImmediate(multipleMaterialBackgroundColorDarker);
            if (editorBoxBackgroundColor != null) GameObject.DestroyImmediate(editorBoxBackgroundColor);
        }

        public void DrawGUI(SerializedObject textureBaker, MB3_TextureBaker momm, System.Type editorWindow)
        {
            if (textureBaker == null)
            {
                return;
            }
            textureBaker.Update();

            showInstructions = EditorGUILayout.Foldout(showInstructions, "Instructions:");
            if (showInstructions)
            {
                EditorGUILayout.HelpBox("1. Add scene objects or prefabs to combine. For best results these should use the same shader as result material.\n\n" +
                                        "2. Create Empty Assets For Combined Material(s)\n\n" +
                                        "3. Check that shader on result material(s) are correct.\n\n" +
                                        "4. Bake materials into combined material(s).\n\n" +
                                        "5. Look at warnings/errors in console. Decide if action needs to be taken.\n\n" +
                                        "6. You are now ready to build combined meshs or adjust meshes to use the combined material(s).", UnityEditor.MessageType.None);

            }
            //mom.LOG_LEVEL = (MB2_LogLevel) EditorGUILayout.EnumPopup("Log Level", mom.LOG_LEVEL);
            EditorGUILayout.PropertyField(logLevel, logLevelContent);

            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical(editorBoxBackgroundStyle);
            EditorGUILayout.LabelField("Objects To Be Combined", EditorStyles.boldLabel);
            if (GUILayout.Button(openToolsWindowLabelContent))
            {
                MB3_MeshBakerEditorWindowInterface mmWin = (MB3_MeshBakerEditorWindowInterface)EditorWindow.GetWindow(editorWindow);
                mmWin.target = (MB3_MeshBakerRoot)momm;
            }
            EditorGUILayout.PropertyField(objsToMesh, objectsToCombineGUIContent, true);
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select Objects In Scene"))
            {
                Selection.objects = momm.GetObjectsToCombine().ToArray();
                if (momm.GetObjectsToCombine().Count > 0)
                {
                    SceneView.lastActiveSceneView.pivot = momm.GetObjectsToCombine()[0].transform.position;
                }
                
            }
            if (GUILayout.Button(gc_SortAlongAxis))
            {
                MB3_MeshBakerRoot.ZSortObjects sorter = new MB3_MeshBakerRoot.ZSortObjects();
                sorter.sortAxis = sortOrderAxis.vector3Value;
                sorter.SortByDistanceAlongAxis(momm.GetObjectsToCombine());
            }
            EditorGUILayout.PropertyField(sortOrderAxis, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
            if (GUILayout.Button(createPrefabAndMaterialLabelContent))
            {
                string newPrefabPath = EditorUtility.SaveFilePanelInProject("Asset name", "", "asset", "Enter a name for the baked texture results");
                if (newPrefabPath != null)
                {
                    CreateCombinedMaterialAssets(momm, newPrefabPath);
                }
            }
            EditorGUILayout.PropertyField(textureBakeResults, textureBakeResultsGUIContent);
            if (textureBakeResults.objectReferenceValue != null)
            {
                showContainsReport = EditorGUILayout.Foldout(showContainsReport, "Shaders & Materials Contained");
                if (showContainsReport)
                {
                    EditorGUILayout.HelpBox(((MB2_TextureBakeResults)textureBakeResults.objectReferenceValue).GetDescription(), MessageType.Info);
                }
            }
            EditorGUILayout.PropertyField(doMultiMaterial, new GUIContent("Multiple Combined Materials"));

            if (momm.doMultiMaterial)
            {
                EditorGUILayout.BeginVertical(multipleMaterialBackgroundStyle);
                EditorGUILayout.LabelField("Source Material To Combined Mapping", EditorStyles.boldLabel);
                if (GUILayout.Button(configMultiMatFromObjsContent))
                {
                    ConfigureMutiMaterialsFromObjsToCombine(momm, resultMaterials, textureBaker);
                }
                EditorGUILayout.BeginHorizontal();
                resultMaterialsFoldout = EditorGUILayout.Foldout(resultMaterialsFoldout, combinedMaterialsGUIContent);

                if (GUILayout.Button(insertContent, EditorStyles.miniButtonLeft, buttonWidth))
                {
                    if (resultMaterials.arraySize == 0)
                    {
                        momm.resultMaterials = new MB_MultiMaterial[1];
                    }
                    else
                    {
                        resultMaterials.InsertArrayElementAtIndex(resultMaterials.arraySize - 1);
                    }
                }
                if (GUILayout.Button(deleteContent, EditorStyles.miniButtonRight, buttonWidth))
                {
                    resultMaterials.DeleteArrayElementAtIndex(resultMaterials.arraySize - 1);
                }
                EditorGUILayout.EndHorizontal();
                if (resultMaterialsFoldout)
                {
                    for (int i = 0; i < resultMaterials.arraySize; i++)
                    {
                        EditorGUILayout.Separator();
                        if (i % 2 == 1)
                        {
                            EditorGUILayout.BeginVertical(multipleMaterialBackgroundStyle);
                        }
                        else
                        {
                            EditorGUILayout.BeginVertical(multipleMaterialBackgroundStyleDarker);
                        }
                        string s = "";
                        if (i < momm.resultMaterials.Length && momm.resultMaterials[i] != null && momm.resultMaterials[i].combinedMaterial != null) s = momm.resultMaterials[i].combinedMaterial.shader.ToString();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("---------- submesh:" + i + " " + s, EditorStyles.boldLabel);
                        if (GUILayout.Button(deleteContent, EditorStyles.miniButtonRight, buttonWidth))
                        {
                            resultMaterials.DeleteArrayElementAtIndex(i);
                        }
                        EditorGUILayout.EndHorizontal();
                        if (i < resultMaterials.arraySize)
                        {
                            EditorGUILayout.Separator();
                            SerializedProperty resMat = resultMaterials.GetArrayElementAtIndex(i);
                            EditorGUILayout.PropertyField(resMat.FindPropertyRelative("combinedMaterial"));
                            SerializedProperty sourceMats = resMat.FindPropertyRelative("sourceMaterials");
                            EditorGUILayout.PropertyField(sourceMats, true);
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.PropertyField(resultMaterial, new GUIContent("Combined Mesh Material"));
            }

            int labelWidth = 200;
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical(editorBoxBackgroundStyle);
            EditorGUILayout.LabelField("Material Bake Options", EditorStyles.boldLabel);
            DrawPropertyFieldWithLabelWidth(atlasPadding, new GUIContent("Atlas Padding"), labelWidth);
            DrawPropertyFieldWithLabelWidth(maxAtlasSize, maxAtlasSizeGUIContent, labelWidth);
            DrawPropertyFieldWithLabelWidth(resizePowerOfTwoTextures, resizePowerOfTwoGUIContent, labelWidth);
            DrawPropertyFieldWithLabelWidth(maxTilingBakeSize, maxTilingBakeSizeGUIContent, labelWidth);
            //EditorGUILayout.PropertyField(fixOutOfBoundsUVs,fixOutOfBoundsGUIContent);
            DrawPropertyFieldWithLabelWidth(fixOutOfBoundsUVs, fixOutOfBoundsGUIContent, labelWidth);
            if (texturePackingAlgorithm.intValue == (int)MB2_PackingAlgorithmEnum.MeshBakerTexturePacker ||
                texturePackingAlgorithm.intValue == (int)MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Fast)
            {
                DrawPropertyFieldWithLabelWidth(forcePowerOfTwoAtlas, forcePowerOfTwoAtlasContent, labelWidth);
            }
            DrawPropertyFieldWithLabelWidth(considerNonTextureProperties, considerNonTexturePropertiesContent, labelWidth);
            if (texturePackingAlgorithm.intValue == (int)MB2_PackingAlgorithmEnum.UnitysPackTextures)
            {
                EditorGUILayout.HelpBox("Unity's texture packer has memory problems and frequently crashes the editor.", MessageType.Warning);
            }
            EditorGUILayout.PropertyField(texturePackingAlgorithm, texturePackingAgorithmGUIContent);
            EditorGUILayout.PropertyField(customShaderProperties, customShaderPropertyNamesGUIContent, true);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();
            Color oldColor = GUI.backgroundColor;
            GUI.color = buttonColor;
            if (GUILayout.Button("Bake Materials Into Combined Material"))
            {
                momm.CreateAtlases(updateProgressBar, true, new MB3_EditorMethods());
                EditorUtility.ClearProgressBar();
                if (momm.textureBakeResults != null) EditorUtility.SetDirty(momm.textureBakeResults);
            }
            GUI.backgroundColor = oldColor;
            textureBaker.ApplyModifiedProperties();
            if (GUI.changed)
            {
                textureBaker.SetIsDifferentCacheDirty();
            }
		}



        public void DrawPropertyFieldWithLabelWidth(SerializedProperty prop, GUIContent content, int labelWidth)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(content, GUILayout.Width(labelWidth));
            EditorGUILayout.PropertyField(prop, noneContent);
            EditorGUILayout.EndHorizontal();
        }
        	
		public void updateProgressBar(string msg, float progress){
			EditorUtility.DisplayProgressBar("Combining Meshes", msg, progress);
		}
		
		public static void ConfigureMutiMaterialsFromObjsToCombine(MB3_TextureBaker mom, SerializedProperty resultMaterials, SerializedObject textureBaker){
			if (mom.GetObjectsToCombine().Count == 0){
				Debug.LogError("You need to add some objects to combine before building the multi material list.");
				return;
			}
			if (resultMaterials.arraySize > 0){
				Debug.LogError("You already have some source to combined material mappings configured. You must remove these before doing this operation.");
				return;			
			}
			if (mom.textureBakeResults == null){
				Debug.LogError("Material Bake Result asset must be set before using this operation.");
				return;
			}
			Dictionary<Shader,List<Material>> shader2Material_map = new Dictionary<Shader, List<Material>>();
			Dictionary<Material,Mesh> obUVobject2material_map = new Dictionary<Material,Mesh>();
			
			//validate that the objects to be combined are valid
			for (int i = 0; i < mom.GetObjectsToCombine().Count; i++){
				GameObject go = mom.GetObjectsToCombine()[i];
				if (go == null) {
					Debug.LogError("Null object in list of objects to combine at position " + i);
					return;
				}
				Renderer r = go.GetComponent<Renderer>();
				if (r == null || (!(r is MeshRenderer) && !(r is SkinnedMeshRenderer))){
					Debug.LogError("GameObject at position " + i + " in list of objects to combine did not have a renderer");
					return;
				}
				if (r.sharedMaterial == null){
					Debug.LogError("GameObject at position " + i + " in list of objects to combine has a null material");
					return;						
				}			
			}
			
			//first pass put any meshes with obUVs on their own submesh if not fixing OB uvs
			if (!mom.fixOutOfBoundsUVs){
				for (int i = 0; i < mom.GetObjectsToCombine().Count; i++){
					GameObject go = mom.GetObjectsToCombine()[i];
					Mesh m = MB_Utility.GetMesh(go);
					MB_Utility.MeshAnalysisResult dummyMar = new MB_Utility.MeshAnalysisResult();
					Renderer r = go.GetComponent<Renderer>();	
					for (int j = 0; j < r.sharedMaterials.Length; j++){
						if (MB_Utility.hasOutOfBoundsUVs(m, ref dummyMar, j)){
							if (!obUVobject2material_map.ContainsKey(r.sharedMaterials[j])){
								Debug.LogWarning("Object " + go + " submesh " + j + " uses UVs outside the range 0,0..1,1 to generate tiling. This object has been mapped to its own submesh in the combined mesh. It can share a submesh with other objects that use different materials if you use the fix out of bounds UVs feature which will bake the tiling");
								obUVobject2material_map.Add(r.sharedMaterials[j],m);
							}
						}
					}
				}
			}
			
			//second pass  put other materials without OB uvs in a shader to material map
			for (int i = 0; i < mom.GetObjectsToCombine().Count; i++){
				Renderer r = mom.GetObjectsToCombine()[i].GetComponent<Renderer>();
				for (int j = 0; j < r.sharedMaterials.Length; j++){
					if (!obUVobject2material_map.ContainsKey(r.sharedMaterials[j])) {
						if (r.sharedMaterials[j] == null) continue;
						List<Material> matsThatUseShader = null;
						if (!shader2Material_map.TryGetValue(r.sharedMaterials[j].shader, out matsThatUseShader)){
							matsThatUseShader = new List<Material>();
							shader2Material_map.Add(r.sharedMaterials[j].shader, matsThatUseShader);
						}
						if (!matsThatUseShader.Contains(r.sharedMaterials[j])) matsThatUseShader.Add(r.sharedMaterials[j]);
					}
				}			
			}
			
			if (shader2Material_map.Count == 0 && obUVobject2material_map.Count == 0) Debug.LogError("Found no materials in list of objects to combine");
			mom.resultMaterials = new MB_MultiMaterial[shader2Material_map.Count + obUVobject2material_map.Count];
			string pth = AssetDatabase.GetAssetPath(mom.textureBakeResults);
			string baseName = Path.GetFileNameWithoutExtension(pth);
			string folderPath = pth.Substring(0,pth.Length - baseName.Length - 6);		
			int k = 0;
			foreach(Shader sh in shader2Material_map.Keys){ 
				List<Material> matsThatUse = shader2Material_map[sh];
				MB_MultiMaterial mm = mom.resultMaterials[k] = new MB_MultiMaterial();
				mm.sourceMaterials = matsThatUse;
				string matName = folderPath +  baseName + "-mat" + k + ".mat";
				Material newMat = new Material(Shader.Find("Diffuse"));
				if (matsThatUse.Count > 0 && matsThatUse[0] != null){
					MB3_TextureBaker.ConfigureNewMaterialToMatchOld(newMat, matsThatUse[0]);
				}
				AssetDatabase.CreateAsset(newMat, matName);
				mm.combinedMaterial = (Material) AssetDatabase.LoadAssetAtPath(matName,typeof(Material));
				k++;
			}
			foreach(Material m in obUVobject2material_map.Keys){
				MB_MultiMaterial mm = mom.resultMaterials[k] = new MB_MultiMaterial();
				mm.sourceMaterials = new List<Material>();
				mm.sourceMaterials.Add(m);
				string matName = folderPath +  baseName + "-mat" + k + ".mat";
				Material newMat = new Material(Shader.Find("Diffuse"));
				MB3_TextureBaker.ConfigureNewMaterialToMatchOld(newMat,m);
				AssetDatabase.CreateAsset(newMat, matName);
				mm.combinedMaterial = (Material) AssetDatabase.LoadAssetAtPath(matName,typeof(Material));
				k++;
			}
			textureBaker.UpdateIfDirtyOrScript();
		}
	
	public static void CreateCombinedMaterialAssets(MB3_TextureBaker target, string pth){
		MB3_TextureBaker mom = (MB3_TextureBaker) target;
		string baseName = Path.GetFileNameWithoutExtension(pth);
		if (baseName == null || baseName.Length == 0) return;
		string folderPath = pth.Substring(0,pth.Length - baseName.Length - 6);
		
		List<string> matNames = new List<string>();
		if (mom.doMultiMaterial){
			for (int i = 0; i < mom.resultMaterials.Length; i++){
				matNames.Add( folderPath +  baseName + "-mat" + i + ".mat" );
				AssetDatabase.CreateAsset(new Material(Shader.Find("Diffuse")), matNames[i]);
				mom.resultMaterials[i].combinedMaterial = (Material) AssetDatabase.LoadAssetAtPath(matNames[i],typeof(Material));
			}
		}else{
			matNames.Add( folderPath +  baseName + "-mat.mat" );
			Material newMat = null;
			if (mom.GetObjectsToCombine().Count > 0 && mom.GetObjectsToCombine()[0] != null){
				Renderer r = mom.GetObjectsToCombine()[0].GetComponent<Renderer>();
				if (r == null){
					Debug.LogWarning("Object " + mom.GetObjectsToCombine()[0] + " does not have a Renderer on it.");
				} else {
					if (r.sharedMaterial != null){
						newMat = new Material(r.sharedMaterial);
						//newMat.shader = r.sharedMaterial.shader;					
						MB3_TextureBaker.ConfigureNewMaterialToMatchOld(newMat,r.sharedMaterial);
					}
				}
			} else {
				Debug.Log("If you add objects to be combined before creating the Combined Material Assets. Then Mesh Baker will create a result material that is a duplicate of the material on the first object to be combined. This saves time configuring the shader.");	
			}
			if (newMat == null){ 
				newMat = new Material(Shader.Find("Diffuse"));
			}
			AssetDatabase.CreateAsset(newMat, matNames[0]);
			mom.resultMaterial = (Material) AssetDatabase.LoadAssetAtPath(matNames[0],typeof(Material));
		}
		//create the MB2_TextureBakeResults
		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<MB2_TextureBakeResults>(),pth);
		mom.textureBakeResults = (MB2_TextureBakeResults) AssetDatabase.LoadAssetAtPath(pth, typeof(MB2_TextureBakeResults));
		AssetDatabase.Refresh();
	}	
	
	}
	

}