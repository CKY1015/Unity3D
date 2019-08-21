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


[CustomEditor(typeof(MB3_MeshBakerGrouper))]
public class MB3_MeshBakerGrouperEditor : Editor
{

    long lastBoundsCheckRefreshTime = 0;

    static GUIContent gc_ClusterType = new GUIContent("Cluster Type", "The scene will be divided cells. Meshes in each cell will be grouped into a single mesh baker");
    static GUIContent gc_GridOrigin = new GUIContent("Origin", "The scene will be divided into of cells. Meshes in each cell will be grouped into a single baker. This sets the origin for the clustering.");
    static GUIContent gc_CellSize = new GUIContent("Cell Size", "The scene will be divided into a grid of cells. Meshes in each cell will be grouped into a single baker. This sets the size of the cells.");
    static GUIContent gc_ClusterOnLMIndex = new GUIContent("Group By Lightmap Index", "Meshes sharing a lightmap index will be grouped together.");
    static GUIContent gc_NumSegements = new GUIContent("Num Pie Segments", "Number of segments/slices in the pie.");
    static GUIContent gc_PieAxis = new GUIContent("Pie Axis", "Scene will be divided into segments about this axis.");
    static GUIContent gc_ClusterByLODLevel = new GUIContent("Cluster By LOD Level", "A baker will be created for each LOD level.");
    static GUIContent gc_ClusterDistance = new GUIContent("Max Distance", "Source meshes closer than this value will be grouped into clusters.");


    private SerializedObject grouper;
    private SerializedProperty clusterType, gridOrigin, cellSize, clusterOnLMIndex, numSegments, pieAxis, clusterByLODLevel, clusterDistance;

    public void OnEnable()
    {
        lastBoundsCheckRefreshTime = 0;
        grouper = new SerializedObject(target);
        SerializedProperty d = grouper.FindProperty("data");
        clusterType = grouper.FindProperty("clusterType");
        gridOrigin = d.FindPropertyRelative("origin");
        cellSize = d.FindPropertyRelative("cellSize");
        clusterOnLMIndex = d.FindPropertyRelative("clusterOnLMIndex");
        clusterByLODLevel = d.FindPropertyRelative("clusterByLODLevel");
        numSegments = d.FindPropertyRelative("pieNumSegments");
        pieAxis = d.FindPropertyRelative("pieAxis");
       // clusterTreeHeight = d.FindPropertyRelative("height");
        clusterDistance = d.FindPropertyRelative("maxDistBetweenClusters");
    }

    public override void OnInspectorGUI()
    {
        MB3_MeshBakerGrouper tbg = (MB3_MeshBakerGrouper)target;
        MB3_TextureBaker tb = ((MB3_MeshBakerGrouper)target).GetComponent<MB3_TextureBaker>();
        DrawGrouperInspector();
        if (GUILayout.Button("Generate Mesh Bakers"))
        {
            if (tb == null)
            {
                Debug.LogError("There must be an MB3_TextureBaker attached to this game object.");
                return;
            }
            if (tb.GetObjectsToCombine().Count == 0)
            {
                Debug.LogError("The MB3_MeshBakerGrouper creates clusters based on the objects to combine in the MB3_TextureBaker component. There were no objects in this list.");
                return;
            }
            if (tb.transform.childCount > 0)
            {
                Debug.LogWarning("This MB3_TextureBaker had some existing child objects. You may want to delete these before 'Generating Mesh Bakers' since your source objects may be included in the List Of Objects To Combine of multiple MeshBaker objects.");
            }
            if (tb != null)
            {
                ((MB3_MeshBakerGrouper)target).grouper.DoClustering(tb);
            }
            else {
                Debug.LogError("MB3_MeshBakerGrouper needs to be attached to an MB3_TextureBaker");
            }
        }
        if (GUILayout.Button("Bake All Child MeshBakers"))
        {
            try
            {
                MB3_MeshBakerCommon[] mBakers = tbg.GetComponentsInChildren<MB3_MeshBakerCommon>();
                for (int i = 0; i < mBakers.Length; i++)
                {
                    //if (mBakers[i].textureBakeResults != null)
                    //{
                    //MB3_MeshBakerEditorFunctions.BakeIntoCombined(mBakers[i]);
                        MB3_MeshBakerEditorInternal.bake(mBakers[i]);
                    //} else
                    //{
                    //    Debug.LogError("Material Bake Resul was not set for baker: " + mBakers[i]);
                    //}
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
        string buttonTextEnableRenderers = "Disable Renderers On All Child MeshBaker Source Objs";
        bool enableRenderers = false;
        MB3_MeshBakerCommon bc = tbg.GetComponentInChildren<MB3_MeshBakerCommon>();
        if (bc != null && bc.GetObjectsToCombine().Count > 0)
        {
            GameObject go = bc.GetObjectsToCombine()[0];
            if (go != null && go.GetComponent<Renderer>() != null && go.GetComponent<Renderer>().enabled == false)
            {
                buttonTextEnableRenderers = "Enable Renderers On All Child MeshBaker Source Objs";
                enableRenderers = true;
            }
        }
        if (GUILayout.Button(buttonTextEnableRenderers))
        {
            try
            {
                MB3_MeshBakerCommon[] mBakers = tbg.GetComponentsInChildren<MB3_MeshBakerCommon>();
                for (int i = 0; i < mBakers.Length; i++)
                {
                    mBakers[i].EnableDisableSourceObjectRenderers(enableRenderers);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
        if (DateTime.UtcNow.Ticks - lastBoundsCheckRefreshTime > 10000000 && tb != null)
        {
            List<GameObject> gos = tb.GetObjectsToCombine();
            Bounds b = new Bounds(Vector3.zero, Vector3.one);
            if (gos.Count > 0 && gos[0] != null && gos[0].GetComponent<Renderer>() != null)
            {
                b = gos[0].GetComponent<Renderer>().bounds;
            }
            for (int i = 0; i < gos.Count; i++)
            {
                if (gos[i] != null && gos[i].GetComponent<Renderer>() != null)
                {
                    b.Encapsulate(gos[i].GetComponent<Renderer>().bounds);
                }
            }
            tbg.sourceObjectBounds = b;
            lastBoundsCheckRefreshTime = DateTime.UtcNow.Ticks;
        }
        grouper.ApplyModifiedProperties();
    }

    public void DrawGrouperInspector()
    {
        EditorGUILayout.HelpBox("This component helps you group meshes that are close together so they can be combined together." +
                            " It generates multiple MB3_MeshBaker objects from the List Of Objects to be combined in the MB3_TextureBaker component." +
                            " Objects that are close together will be grouped together and added to a new child MB3_MeshBaker object.\n\n" +
                            " TIP: Try the new agglomerative cluster type. It's awsome!", MessageType.Info);
        MB3_MeshBakerGrouper grouper = (MB3_MeshBakerGrouper)target;
        
        EditorGUILayout.PropertyField(clusterType, gc_ClusterType);
        MB3_MeshBakerGrouper.ClusterType gg = (MB3_MeshBakerGrouper.ClusterType)clusterType.enumValueIndex;
        if ((gg == MB3_MeshBakerGrouper.ClusterType.none && !(grouper.grouper is MB3_MeshBakerGrouperNone)) ||
            (gg == MB3_MeshBakerGrouper.ClusterType.grid && !(grouper.grouper is MB3_MeshBakerGrouperGrid)) ||
            (gg == MB3_MeshBakerGrouper.ClusterType.pie && !(grouper.grouper is MB3_MeshBakerGrouperPie)) ||
            (gg == MB3_MeshBakerGrouper.ClusterType.agglomerative && !(grouper.grouper is MB3_MeshBakerGrouperCluster))
            )
        {
            grouper.CreateGrouper(gg, grouper.data);
            grouper.clusterType = gg;
        }
        if (clusterType.enumValueIndex == (int) MB3_MeshBakerGrouper.ClusterType.grid)
        {
            EditorGUILayout.PropertyField(gridOrigin, gc_GridOrigin);
            EditorGUILayout.PropertyField(cellSize, gc_CellSize);
        }
        else if (clusterType.enumValueIndex == (int) MB3_MeshBakerGrouper.ClusterType.pie)
        {
            EditorGUILayout.PropertyField(gridOrigin, gc_GridOrigin);
            EditorGUILayout.PropertyField(numSegments, gc_NumSegements);
            EditorGUILayout.PropertyField(pieAxis, gc_PieAxis);
        }
        else if (clusterType.enumValueIndex == (int) MB3_MeshBakerGrouper.ClusterType.agglomerative)
        {
            float dist = clusterDistance.floatValue;
            float maxDist = 100f;
            MB3_MeshBakerGrouperCluster cl = null;
            if (grouper.grouper is MB3_MeshBakerGrouperCluster)
            {
                cl = (MB3_MeshBakerGrouperCluster)grouper.grouper;
                maxDist = cl._ObjsExtents;
            }
            dist = EditorGUILayout.Slider(gc_ClusterDistance, dist, 1f, maxDist);
            clusterDistance.floatValue = dist;
            string btnName = "Refresh Clusters";
            if (cl.cluster == null || cl.cluster.clusters == null || cl.cluster.clusters.Length == 0)
            {
                btnName = "Click To Build Clusters";
            }
            if (GUILayout.Button(btnName))
            {
                if (grouper.grouper is MB3_MeshBakerGrouperCluster)
                {
                    MB3_MeshBakerGrouperCluster cg = (MB3_MeshBakerGrouperCluster)grouper.grouper;
                    MB3_TextureBaker tb = grouper.GetComponent<MB3_TextureBaker>();
                    if (tb != null)
                    {
                        //MB3_EditorMethods em = new MB3_EditorMethods();
                        cg.BuildClusters(tb.GetObjectsToCombine(), updateProgressBar);
                        EditorUtility.ClearProgressBar();
                    }
                }
            }
        }
        EditorGUILayout.PropertyField(clusterOnLMIndex, gc_ClusterOnLMIndex);
        EditorGUILayout.PropertyField(clusterByLODLevel, gc_ClusterByLODLevel);
    }

    public void updateProgressBar(string msg, float progress)
    {
        EditorUtility.DisplayProgressBar("Combining Meshes", msg, progress);
    }
}
