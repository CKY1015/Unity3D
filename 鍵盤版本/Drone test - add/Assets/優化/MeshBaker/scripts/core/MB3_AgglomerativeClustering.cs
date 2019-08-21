using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

namespace DigitalOpus.MB.Core
{
    [Serializable]
    public class MB3_AgglomerativeClustering
    {

        public List<item_s> items = new List<item_s>();

        public ClusterNode[] clusters;

        [Serializable]
        public class ClusterNode
        {
            public item_s leaf;
            public ClusterNode cha;
            public ClusterNode chb;
            public int height; /* height of node from the bottom */
            public float distToMergedCentroid;
            public Vector3 centroid; /* centroid of this cluster */
            public int[] leafs; /* indexes of root clusters merged */
            public int idx; //index in clusters list

            public ClusterNode(item_s ii, int index)
            {
                leaf = ii;
                idx = index;
                leafs = new int[1];
                leafs[0] = index;
                centroid = ii.coord;
                height = 0;
            }

            public ClusterNode(ClusterNode a, ClusterNode b, int index, int h, float dist, ClusterNode[] clusters)
            {
                cha = a;
                chb = b;
                idx = index;
                leafs = new int[a.leafs.Length + b.leafs.Length];
                Array.Copy(a.leafs, leafs, a.leafs.Length);
                Array.Copy(b.leafs, 0, leafs, a.leafs.Length, b.leafs.Length);
                Vector3 c = Vector3.zero;
                for (int i = 0; i < leafs.Length; i++)
                {
                    c += clusters[leafs[i]].centroid;
                }
                centroid = c / leafs.Length;
                height = h;
                distToMergedCentroid = dist;
            }
        };


        [Serializable]
        public class item_s
        {
            public GameObject go;
            public Vector3 coord; /* coordinate of the input data point */
        };

        float euclidean_distance(Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b);
        }

        //public IEnumerator agglomerate()
        public void agglomerate(ProgressUpdateDelegate progFunc)
        {
            if (items.Count <= 1)
            {
                clusters = new ClusterNode[0];
                return;
                //yield break;
            }
            clusters = new ClusterNode[items.Count * 2 - 1];
            for (int i = 0; i < items.Count; i++)
            {
                clusters[i] = new ClusterNode(items[i], i);
            }

            float[][] distances = new float[items.Count * 2 - 1][];
            for (int i = 0; i < distances.Length; i++)
            {
                distances[i] = new float[items.Count * 2 - 1];
            }

            int numClussters = items.Count;
            List<ClusterNode> unclustered = new List<ClusterNode>();
            for (int i = 0; i < numClussters; i++)
            {
                unclustered.Add(clusters[i]);
                for (int j = 0; j < numClussters; j++)
                {
                    distances[i][j] = euclidean_distance(clusters[i].centroid, clusters[j].centroid);
                }
            }

            int height = 0;
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Start();
            while (unclustered.Count > 1)
            {
                height++;
                //find closest pair
                float min = 10e15f;
                int a, b;
                a = b = -1;
                for (int i = 0; i < unclustered.Count; i++)
                {
                    for (int j = 0; j < unclustered.Count; j++)
                    {
                        if (i != j)
                        {
                            int idxa = unclustered[i].idx;
                            int idxb = unclustered[j].idx;
                            float newDist = distances[idxa][idxb];
                            if (newDist < min)
                            {
                                min = newDist;
                                a = idxa;
                                b = idxb;
                            }
                        }
                    }
                }
                //make a new cluster with pair as children set merge height
                numClussters++;
                ClusterNode cn = new ClusterNode(clusters[a], clusters[b], numClussters - 1, height, min, clusters);
                //remove children from unclustered
                unclustered.Remove(clusters[a]);
                unclustered.Remove(clusters[b]);
                //add new cluster to unclustered
                clusters[numClussters - 1] = cn;
                unclustered.Add(cn);
                //update new clusteres distance
                for (int i = 0; i < numClussters - 1; i++)
                {
                    distances[numClussters - 1][i] = euclidean_distance(clusters[numClussters - 1].centroid, clusters[i].centroid);
                    distances[i][numClussters - 1] = euclidean_distance(clusters[i].centroid, clusters[numClussters - 1].centroid);
                }
                //if (timer.Interval > .2f)
                //{
                //    yield return null;
                //    timer.Start();
                //}
                if (progFunc != null) progFunc("Creating clusters:", ((float)(items.Count - unclustered.Count)) / items.Count);
            }
        }

        public int TestRun(List<GameObject> gos)
        {
            List<item_s> its = new List<item_s>();
            for (int i = 0; i < gos.Count; i++)
            {
                item_s ii = new item_s();
                ii.go = gos[i];
                ii.coord = gos[i].transform.position;
                its.Add(ii);
            }
            items = its;
            if (items.Count > 0)
            {
                agglomerate(null);
            }
            return 0;
        }


        //------

    }
}
