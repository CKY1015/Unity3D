﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DigitalOpus.MB.Core;

public class MB3_MeshBakerGrouper : MonoBehaviour {
    public enum ClusterType
    {
        none,
        grid,
        pie,
        agglomerative,
    }
    public MB3_MeshBakerGrouperCore grouper;
    public ClusterType clusterType = ClusterType.none;
    public GrouperData data = new GrouperData();

	//these are for getting a resonable bounds in which to draw gizmos.
	[HideInInspector] public Bounds sourceObjectBounds = new Bounds(Vector3.zero,Vector3.one);

	void OnDrawGizmosSelected(){
		if (grouper == null)
        {
            grouper = CreateGrouper(clusterType, data);
        }
        if (grouper.d == null)
        {
            grouper.d = data;
        }
        grouper.DrawGizmos(sourceObjectBounds);
	}

    public MB3_MeshBakerGrouperCore CreateGrouper(ClusterType t, GrouperData data)
    {
        if (t == ClusterType.grid) grouper = new MB3_MeshBakerGrouperGrid(data);
        if (t == ClusterType.pie) grouper = new MB3_MeshBakerGrouperPie(data);
        if (t == ClusterType.agglomerative)
        {
            MB3_TextureBaker tb = GetComponent<MB3_TextureBaker>();
            List<GameObject> gos;
            if (tb != null)
            {
                gos = tb.GetObjectsToCombine();
            } else
            {
                gos = new List<GameObject>(); 
            }
            grouper = new MB3_MeshBakerGrouperCluster(data,gos);
        }
        if (t == ClusterType.none) grouper = new MB3_MeshBakerGrouperNone(data);
        return grouper;
    }
}

namespace DigitalOpus.MB.Core {
//all properties go here so that settings are remembered as user switches between cluster types
[Serializable]
public class GrouperData
{
    public bool clusterOnLMIndex;
    public bool clusterByLODLevel;
    public Vector3 origin;
    //Normally these properties would be in the subclasses but putting them here makes writing the inspector much easier
    //for grid
    public Vector3 cellSize;
    //for pie
    public int pieNumSegments = 4;
    public Vector3 pieAxis = Vector3.up;
    //for clustering
    public int height = 1;
    public float maxDistBetweenClusters = 1f;

}

[Serializable]
public abstract class MB3_MeshBakerGrouperCore
{

    public GrouperData d; 

    public abstract Dictionary<string, List<Renderer>> FilterIntoGroups(List<GameObject> selection);
    public abstract void DrawGizmos(Bounds sourceObjectBounds);
    public void DoClustering(MB3_TextureBaker tb)
    {
        //todo warn for no objects and no material bake result
        Dictionary<string, List<Renderer>> cell2objs = FilterIntoGroups(tb.GetObjectsToCombine());

        Debug.Log("Found " + cell2objs.Count + " cells with Renderers. Creating bakers.");
        if (d.clusterOnLMIndex)
        {
            Dictionary<string, List<Renderer>> cell2objsNew = new Dictionary<string, List<Renderer>>();
            foreach (string key in cell2objs.Keys)
            {
                List<Renderer> gaws = cell2objs[key];
                Dictionary<int, List<Renderer>> idx2objs = GroupByLightmapIndex(gaws);
                foreach (int keyIdx in idx2objs.Keys)
                {
                    string keyNew = key + "-LM-" + keyIdx;
                    cell2objsNew.Add(keyNew, idx2objs[keyIdx]);
                }
            }
            cell2objs = cell2objsNew;
        }
        if (d.clusterByLODLevel)
        {
            //visit each cell
            //visit each renderer
            //check if that renderer is a child of an LOD group
            //      visit each LOD level check if this renderer is in that list.
            //      if not add it to LOD0 for that cell
            //      otherwise add it to LODX for that cell creating LODs as necessary
#if UNITY_4_1 || UNITY_4_3 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8
			Debug.LogError("Cluster by LOD level not supported");
#else
            Dictionary<string, List<Renderer>> cell2objsNew = new Dictionary<string, List<Renderer>>();
            foreach (string key in cell2objs.Keys)
            {
                List<Renderer> gaws = cell2objs[key];
                foreach(Renderer r in gaws)
                    {
                        if (r == null) continue;
                        bool foundInLOD = false;
                        LODGroup lodg = r.GetComponentInParent<LODGroup>();
                        if (lodg != null)
                        {
                            LOD[] lods = lodg.GetLODs();
                            for (int i = 0; i < lods.Length; i++)
                            {
                                LOD lod = lods[i];
                                if (Array.Find<Renderer>(lod.renderers, x => x == r) != null)
                                {
                                    foundInLOD = true;
                                    List<Renderer> rs;
                                    string newKey = String.Format("{0}_LOD{1}", key, i);
                                    if (!cell2objsNew.TryGetValue(newKey,out rs)){
                                        rs = new List<Renderer>();
                                        cell2objsNew.Add(newKey, rs);
                                    }
                                    if (!rs.Contains(r)) rs.Add(r);
                                }
                            } 
                        }
                        if (!foundInLOD)
                        {
                            List<Renderer> rs;
                            string newKey = String.Format("{0}_LOD0", key);
                            if (!cell2objsNew.TryGetValue(newKey, out rs))
                            {
                                rs = new List<Renderer>();
                                cell2objsNew.Add(newKey, rs);
                            }
                            if (!rs.Contains(r)) rs.Add(r);
                        }
                    }
            }
            cell2objs = cell2objsNew;
#endif
        }
        foreach (string key in cell2objs.Keys)
        {
            List<Renderer> gaws = cell2objs[key];
            if (gaws.Count > 1)
            {
                AddMeshBaker(tb, key, gaws);
            }
        }
    }

    Dictionary<int, List<Renderer>> GroupByLightmapIndex(List<Renderer> gaws)
    {
        Dictionary<int, List<Renderer>> idx2objs = new Dictionary<int, List<Renderer>>();
        for (int i = 0; i < gaws.Count; i++)
        {
            List<Renderer> objs = null;
            if (idx2objs.ContainsKey(gaws[i].lightmapIndex))
            {
                objs = idx2objs[gaws[i].lightmapIndex];
            }
            else {
                objs = new List<Renderer>();
                idx2objs.Add(gaws[i].lightmapIndex, objs);
            }
            objs.Add(gaws[i]);
        }
        return idx2objs;
    }

    void AddMeshBaker(MB3_TextureBaker tb, string key, List<Renderer> gaws)
    {
        int numVerts = 0;
        for (int i = 0; i < gaws.Count; i++)
        {
            Mesh m = MB_Utility.GetMesh(gaws[i].gameObject);
            if (m != null)
                numVerts += m.vertexCount;
        }

        GameObject nmb = new GameObject("MeshBaker-" + key);
        nmb.transform.position = Vector3.zero;
        MB3_MeshBakerCommon newMeshBaker;
        if (numVerts >= 65535)
        {
            newMeshBaker = nmb.AddComponent<MB3_MultiMeshBaker>();
            newMeshBaker.useObjsToMeshFromTexBaker = false;
        }
        else {
            newMeshBaker = nmb.AddComponent<MB3_MeshBaker>();
            newMeshBaker.useObjsToMeshFromTexBaker = false;
        }
        newMeshBaker.textureBakeResults = tb.textureBakeResults;
        newMeshBaker.transform.parent = tb.transform;
        for (int i = 0; i < gaws.Count; i++)
        {
            newMeshBaker.GetObjectsToCombine().Add(gaws[i].gameObject);
        }
    }
}

[Serializable]
public class MB3_MeshBakerGrouperNone : MB3_MeshBakerGrouperCore
{
    public MB3_MeshBakerGrouperNone(GrouperData d)
    {
        this.d = d;
    }

    public override Dictionary<string, List<Renderer>> FilterIntoGroups(List<GameObject> selection)
    {
        Debug.Log("Filtering into groups none");

        Dictionary<string, List<Renderer>> cell2objs = new Dictionary<string, List<Renderer>>();

        List<Renderer> rs = new List<Renderer>();
        for (int i = 0; i < selection.Count; i++)
        {
                if (selection[i] != null)
                {
                    rs.Add(selection[i].GetComponent<Renderer>());
                }
        }

        cell2objs.Add("MeshBaker", rs);
        return cell2objs;
    }

    public override void DrawGizmos(Bounds sourceObjectBounds)
    {

    }
}

[Serializable]
public class MB3_MeshBakerGrouperGrid : MB3_MeshBakerGrouperCore
{
    public MB3_MeshBakerGrouperGrid(GrouperData d)
    {
        this.d = d;
    }

    public override Dictionary<string, List<Renderer>> FilterIntoGroups(List<GameObject> selection)
    {
        Dictionary<string, List<Renderer>> cell2objs = new Dictionary<string, List<Renderer>>();
        if (d.cellSize.x <= 0f || d.cellSize.y <= 0f || d.cellSize.z <= 0f)
        {
            Debug.LogError("cellSize x,y,z must all be greater than zero.");
            return cell2objs;
        }

        Debug.Log("Collecting renderers in each cell");
        foreach (GameObject t in selection)
        {
            if (t == null)
                {
                    continue;
                }
            GameObject go = t;
            Renderer mr = go.GetComponent<Renderer>();
            if (mr is MeshRenderer || mr is SkinnedMeshRenderer)
            {
                //get the cell this gameObject is in
                Vector3 gridVector = mr.transform.position;
                gridVector.x = Mathf.Floor((gridVector.x - d.origin.x) / d.cellSize.x) * d.cellSize.x;
                gridVector.y = Mathf.Floor((gridVector.y - d.origin.y) / d.cellSize.y) * d.cellSize.y;
                gridVector.z = Mathf.Floor((gridVector.z - d.origin.z) / d.cellSize.z) * d.cellSize.z;
                List<Renderer> objs = null;
                string gridVectorStr = gridVector.ToString();
                if (cell2objs.ContainsKey(gridVectorStr))
                {
                    objs = cell2objs[gridVectorStr];
                }
                else {
                    objs = new List<Renderer>();
                    cell2objs.Add(gridVectorStr, objs);
                }
                if (!objs.Contains(mr))
                {
                    objs.Add(mr);
                }
            }
        }
        return cell2objs;
    }

    public override void DrawGizmos(Bounds sourceObjectBounds)
    {
        Vector3 cs = d.cellSize;
        if (cs.x <= .00001f || cs.y <= .00001f || cs.z <= .00001f) return;
        Vector3 p = sourceObjectBounds.center - sourceObjectBounds.extents;
        Vector3 offset = d.origin;
        offset.x = offset.x % cs.x;
        offset.y = offset.y % cs.y;
        offset.z = offset.z % cs.z;
        //snap p to closest cell center
        Vector3 start;
        p.x = Mathf.Round((p.x) / cs.x) * cs.x + offset.x;
        p.y = Mathf.Round((p.y) / cs.y) * cs.y + offset.y;
        p.z = Mathf.Round((p.z) / cs.z) * cs.z + offset.z;
        if (p.x > sourceObjectBounds.center.x - sourceObjectBounds.extents.x) p.x = p.x - cs.x;
        if (p.y > sourceObjectBounds.center.y - sourceObjectBounds.extents.y) p.y = p.y - cs.y;
        if (p.z > sourceObjectBounds.center.z - sourceObjectBounds.extents.z) p.z = p.z - cs.z;
        start = p;
        int numcells = Mathf.CeilToInt(sourceObjectBounds.size.x / cs.x + sourceObjectBounds.size.y / cs.y + sourceObjectBounds.size.z / cs.z);
        if (numcells > 200)
        {
            Gizmos.DrawWireCube(d.origin + cs / 2f, cs);
        }
        else {
            for (; p.x < sourceObjectBounds.center.x + sourceObjectBounds.extents.x; p.x += cs.x)
            {
                p.y = start.y;
                for (; p.y < sourceObjectBounds.center.y + sourceObjectBounds.extents.y; p.y += cs.y)
                {
                    p.z = start.z;
                    for (; p.z < sourceObjectBounds.center.z + sourceObjectBounds.extents.z; p.z += cs.z)
                    {
                        Gizmos.DrawWireCube(p + cs / 2f, cs);
                    }
                }
            }
        }
    }
}

[Serializable]
public class MB3_MeshBakerGrouperPie : MB3_MeshBakerGrouperCore
{
    public MB3_MeshBakerGrouperPie(GrouperData data)
    {
        d = data;
    }

    public override Dictionary<string, List<Renderer>> FilterIntoGroups(List<GameObject> selection)
    {
        Dictionary<string, List<Renderer>> cell2objs = new Dictionary<string, List<Renderer>>();
        if (d.pieNumSegments == 0)
        {
            Debug.LogError("pieNumSegments must be greater than zero.");
            return cell2objs;
        }

        if (d.pieAxis.magnitude <= .000001f)
        {
            Debug.LogError("Pie axis must have length greater than zero.");
            return cell2objs;
        }

        d.pieAxis.Normalize();
        Quaternion pieAxis2yIsUp = Quaternion.FromToRotation(d.pieAxis, Vector3.up);

        Debug.Log("Collecting renderers in each cell");
        foreach (GameObject t in selection)
        {
                if (t == null)
                {
                    continue;
                }
            GameObject go = t;
            Renderer mr = go.GetComponent<Renderer>();
            if (mr is MeshRenderer || mr is SkinnedMeshRenderer)
            {
                //get the cell this gameObject is in
                Vector3 origin2obj = mr.transform.position - d.origin;
                origin2obj.Normalize();
                origin2obj = pieAxis2yIsUp * origin2obj;

                float d_aboutY = 0f;
                if (Mathf.Abs(origin2obj.x) < 10e-5f && Mathf.Abs(origin2obj.z) < 10e-5f)
                {
                    d_aboutY = 0f;
                }
                else {
                    d_aboutY = Mathf.Atan2(origin2obj.z, origin2obj.x) * Mathf.Rad2Deg;
                    if (d_aboutY < 0f) d_aboutY = 360f + d_aboutY;
                }

                int segment = Mathf.FloorToInt(d_aboutY / 360f * d.pieNumSegments);

                List<Renderer> objs = null;
                string segStr = "seg_" + segment;
                if (cell2objs.ContainsKey(segStr))
                {
                    objs = cell2objs[segStr];
                }
                else {
                    objs = new List<Renderer>();
                    cell2objs.Add(segStr, objs);
                }
                if (!objs.Contains(mr))
                {
                    objs.Add(mr);
                }
            }
        }
        return cell2objs;
    }

    public override void DrawGizmos(Bounds sourceObjectBounds)
    {
        if (d.pieAxis.magnitude < .1f) return;
        if (d.pieNumSegments < 1) return;
        float rad = sourceObjectBounds.extents.magnitude;
        DrawCircle(d.pieAxis, d.origin, rad, 24);
        Quaternion yIsUp2PieAxis = Quaternion.FromToRotation(Vector3.up, d.pieAxis);
        Quaternion rStep = Quaternion.AngleAxis(180f / d.pieNumSegments, Vector3.up);
        Vector3 r = rStep * Vector3.forward;
        for (int i = 0; i < d.pieNumSegments; i++)
        {
            Vector3 rr = yIsUp2PieAxis * r;
            Gizmos.DrawLine(d.origin, d.origin + rr * rad);
            r = rStep * r;
            r = rStep * r;
        }
    }

    public static void DrawCircle(Vector3 axis, Vector3 center, float radius, int subdiv)
    {
        Quaternion q = Quaternion.AngleAxis(360 / subdiv, axis);
        Vector3 r = new Vector3(axis.y, -axis.x, axis.z); //should be perpendicular to axis
        r.Normalize();
        r *= radius;
        for (int i = 0; i < subdiv + 1; i++)
        {
            Vector3 r2 = q * r;
            Gizmos.DrawLine(center + r, center + r2);
            r = r2;
        }
    }
}


[Serializable]
public class MB3_MeshBakerGrouperKMeans : MB3_MeshBakerGrouperCore
{
    public int numClusters = 4;
    public Vector3[] clusterCenters = new Vector3[0];
    public float[] clusterSizes = new float[0];

    public MB3_MeshBakerGrouperKMeans(GrouperData data)
    {
        d = data;
    }

    public override Dictionary<string, List<Renderer>> FilterIntoGroups(List<GameObject> selection)
    {
        Dictionary<string, List<Renderer>> cell2objs = new Dictionary<string, List<Renderer>>();
        List<GameObject> validObjs = new List<GameObject>();
        int numClusters = 20;
        foreach (GameObject t in selection)
        {
                if (t == null)
                {
                    continue;
                }
            GameObject go = t;
            Renderer mr = go.GetComponent<Renderer>();
            if (mr is MeshRenderer || mr is SkinnedMeshRenderer)
            {
                //get the cell this gameObject is in
                validObjs.Add(go);
            }
        }
        if (validObjs.Count > 0 && numClusters > 0 && numClusters < validObjs.Count)
        {
            MB3_KMeansClustering kmc = new MB3_KMeansClustering(validObjs, numClusters);
            kmc.Cluster();
            clusterCenters = new Vector3[numClusters];
            clusterSizes = new float[numClusters];
            for (int i = 0; i < numClusters; i++)
            {
                List<Renderer> lr = kmc.GetCluster(i, out clusterCenters[i], out clusterSizes[i]);
                if (lr.Count > 0)
                {
                    cell2objs.Add("Cluster_" + i, lr);
                }
            }
        }
        else
        {
            //todo error messages
        }
        return cell2objs;
    }

    public override void DrawGizmos(Bounds sceneObjectBounds)
    {
        if (clusterCenters != null && clusterSizes != null && clusterCenters.Length == clusterSizes.Length)
        {
            for (int i = 0; i < clusterSizes.Length; i++)
            {
                Gizmos.DrawWireSphere(clusterCenters[i], clusterSizes[i]);
            }
        }
    }
}

    [Serializable]
    public class MB3_MeshBakerGrouperCluster : MB3_MeshBakerGrouperCore
    {

        public MB3_AgglomerativeClustering cluster;
        float _lastMaxDistBetweenClusters;
        public float _ObjsExtents; //extents
        List<MB3_AgglomerativeClustering.ClusterNode> _clustersToDraw = new List<MB3_AgglomerativeClustering.ClusterNode>();
        float[] _radii;
        
        public MB3_MeshBakerGrouperCluster(GrouperData data, List<GameObject> gos)
        {
            d = data;
            //BuildClusters(gos);
        }

        public override Dictionary<string, List<Renderer>> FilterIntoGroups(List<GameObject> selection)
        {
            Dictionary<string, List<Renderer>> cell2objs = new Dictionary<string, List<Renderer>>();
            for (int i = 0; i < _clustersToDraw.Count; i++)
            {
                MB3_AgglomerativeClustering.ClusterNode node = _clustersToDraw[i];
                List<Renderer> rrs = new List<Renderer>();
                for (int j = 0; j < node.leafs.Length; j++)
                {
                    Renderer r = cluster.clusters[node.leafs[j]].leaf.go.GetComponent<Renderer>();
                    if (r is MeshRenderer || r is SkinnedMeshRenderer)
                    {
                        rrs.Add(r);
                    }
                }
                if (rrs.Count > 1)
                {
                    cell2objs.Add("Cluster_" + i, rrs);
                }
            }
            return cell2objs;
        }

        public void BuildClusters(List<GameObject> gos, ProgressUpdateDelegate progFunc)
        {
            if (gos.Count == 0)
            {
                Debug.LogWarning("No objects to cluster");
                return;
            }
            if (cluster == null) cluster = new MB3_AgglomerativeClustering();
            List<MB3_AgglomerativeClustering.item_s> its = new List<MB3_AgglomerativeClustering.item_s>();
            for (int i = 0; i < gos.Count; i++)
            {
                if (gos[i] != null && its.Find(x => x.go == gos[i]) == null)
                {
                    MB3_AgglomerativeClustering.item_s ii = new MB3_AgglomerativeClustering.item_s();
                    ii.go = gos[i];
                    ii.coord = gos[i].transform.position;
                    its.Add(ii);
                }
            }
            cluster.items = its;
            //yield return cluster.agglomerate();
            cluster.agglomerate(progFunc);
            _BuildListOfClustersToDraw();
        }

        void _BuildListOfClustersToDraw(){
            _clustersToDraw.Clear();
            if (cluster.clusters == null)
            {
                return;
            }
            List<MB3_AgglomerativeClustering.ClusterNode> removeMe = new List<MB3_AgglomerativeClustering.ClusterNode>();
            float largest = 1f;
            for (int i = 0; i < cluster.clusters.Length; i++){
                MB3_AgglomerativeClustering.ClusterNode node = cluster.clusters[i];
                //don't draw clusters that were merged too far apart and only want leaf nodes
                if (node.distToMergedCentroid <= d.maxDistBetweenClusters && node.leaf == null){
                    _clustersToDraw.Add(node);
                }
                if (node.distToMergedCentroid > largest)
                {
                    largest = node.distToMergedCentroid;
                }
            }
            for (int i = 0; i < _clustersToDraw.Count; i++)
            {
                removeMe.Add(_clustersToDraw[i].cha);
                removeMe.Add(_clustersToDraw[i].chb);
            }

            for (int i = 0; i < removeMe.Count; i++){
                _clustersToDraw.Remove(removeMe[i]);
            }
            _radii = new float[_clustersToDraw.Count];
            
            for (int i = 0; i < _radii.Length; i++)
            {
                MB3_AgglomerativeClustering.ClusterNode n = _clustersToDraw[i];
                Bounds b = new Bounds(n.centroid, Vector3.one);
                for (int j = 0; j < n.leafs.Length; j++)
                {
                    Renderer r = cluster.clusters[n.leafs[j]].leaf.go.GetComponent<Renderer>();
                    if (r != null)
                    {
                        b.Encapsulate( r.bounds );
                    }
                }
                _radii[i] = b.extents.magnitude;
            }
            _ObjsExtents = largest + 1f;
            if (_ObjsExtents < 2f) _ObjsExtents = 2f;
        }
        
        public override void DrawGizmos(Bounds sceneObjectBounds)
        {
            if (cluster == null || cluster.clusters == null)
            {
                return;
            }
            if (_lastMaxDistBetweenClusters != d.maxDistBetweenClusters){
                _BuildListOfClustersToDraw();
                _lastMaxDistBetweenClusters = d.maxDistBetweenClusters;
            }            
            for (int i = 0; i < _clustersToDraw.Count; i++)
            {
                Gizmos.color = Color.white;
                MB3_AgglomerativeClustering.ClusterNode node = _clustersToDraw[i];
                Gizmos.DrawWireSphere(node.centroid, _radii[i]);
            }
        }
    }
}

