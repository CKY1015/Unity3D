using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using EasyRoads3D;
public class OODCCCCDCC{

static public void OCDQCCCDCO(RoadObjectScript target){


OQOQQQDDOD.ODQOQDOCQQ(target.transform);

List<ODODDQQO> arr = OQOQQQDDOD.OOOQQDOOCQ(false);
target.ODDQQDDCDC(arr, OQOQQQDDOD.OCDODCQODQ(arr), OQOQQQDDOD.OCDOCQODDC(arr));
Transform mySideObject = OQOQCQDCQQ(target);
OOOODDOCQC(target.OQQDCODQCD, target.transform, target.OOODQCQOQD(), target.OOQDOOQQ, target.OOQQQOQO, target.raise, target, mySideObject);



target.ODODDDOO = true;

}
static public void OOOODDOCQC(OQQOQDQOQD OQQDCODQCD, Transform obj , List<SideObjectParams> param , bool OOQDOOQQ ,  int[] activeODODDQQO , float raise, RoadObjectScript target , Transform mySideObject){
List<OQQDDDCQCC> pnts  = target.OQQDCODQCD.OCOOQQQQQC;
List<ODODDQQO> arr  = OQOQQQDDOD.OOOQQDOOCQ(false);
for(int i = 0; i < activeODODDQQO.Length; i++){  
ODODDQQO so = (ODODDQQO)arr[activeODODDQQO[i]];

GameObject goi  = null;
if(so.OQOOQCQDQC != "") goi =  (GameObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(so.OQOOQCQDQC), typeof(GameObject));
GameObject OCQODCQODD = null;
if(so.OQOCCODODD != "") OCQODCQODD = (GameObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(so.OQOCCODODD), typeof(GameObject));
GameObject OOOQQDCQQC = null;
if(so.ODQDQDCDQQ != "") OOOQQDCQQC =  (GameObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(so.ODQDQDCDQQ), typeof(GameObject));
OQOQQQDDOD.OCDCCCOCCD(so, pnts, obj, OQQDCODQCD, param, OOQDOOQQ, activeODODDQQO[i], raise, goi, OCQODCQODD, OOOQQDCQQC);
if(so.terrainTree > 0){

if(EditorUtility.DisplayDialog("Side Objects", "Side Object " + so.name + " in " + target.gameObject.name + " includes an asset part of the terrain vegetation data.\n\nWould you like to add this side object to the terrain vegetation data?", "yes","no")){
foreach(Transform child in mySideObject){
if(child.gameObject.name == so.name){
OQOQQQDDOD.OCCCQODODO(activeODODDQQO[i], child);
MonoBehaviour.DestroyImmediate(child.gameObject);
break;
}
}
}
}
foreach(Transform child in mySideObject)if(child.gameObject.GetComponent(typeof(sideObjectScript)) != null) MonoBehaviour.DestroyImmediate(child.gameObject.GetComponent(typeof(sideObjectScript)));
}
}

static public void OCCCDDDQDO(sideObjectScript scr, int index, RoadObjectScript target, Transform go){
string n = go.gameObject.name;
Transform p = go.parent;

if(go != null){
MonoBehaviour.DestroyImmediate(go.gameObject);
}
List<ODODDQQO> arr = OQOQQQDDOD.OOOQQDOOCQ(false);
ODODDQQO so = (ODODDQQO)arr[index];

OCQQODDQDD(n, p, so, index, target);

GameObject goi  = null;
if(so.OQOOQCQDQC != "") goi =  (GameObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(so.OQOOQCQDQC), typeof(GameObject));
GameObject OCQODCQODD = null;
if(so.OQOCCODODD != "") OCQODCQODD = (GameObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(so.OQOCCODODD), typeof(GameObject));
GameObject OOOQQDCQQC = null;
if(so.ODQDQDCDQQ != "") OOOQQDCQQC =  (GameObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(so.ODQDQDCDQQ), typeof(GameObject));

OQOQQQDDOD.OCOQQQOCCC(target.OQQDCODQCD, target.transform, target.OOODQCQOQD(), target.OOQDOOQQ, index, target.raise, goi, OCQODCQODD, OOOQQDCQQC);
arr = null;
}

static public Transform OQOQCQDCQQ(RoadObjectScript target){

GameObject go  =  new GameObject("Side Objects");

go.transform.parent = target.transform;
List<ODODDQQO> arr = OQOQQQDDOD.OOOQQDOOCQ(false);
for(int i = 0; i < target.OOQQQOQO.Length; i++){  
ODODDQQO so = (ODODDQQO)arr[target.OOQQQOQO[i]];
OCQQODDQDD(so.name, go.transform, so, target.OOQQQOQO[i], target);
}
return go.transform;
}
static public void OCQQODDQDD(string objectname, Transform obj, ODODDQQO so, int index, RoadObjectScript target){



Transform rootObject = null;

foreach(Transform child1 in obj)
{
if(child1.name == objectname){
rootObject = child1;

if(so.textureGUID !=""){
MeshRenderer mr  = (MeshRenderer)rootObject.transform.GetComponent(typeof(MeshRenderer));
Material mat =  (Material)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(so.textureGUID), typeof(Material));
mr.material = mat;

}
}
}
if(rootObject == null){
GameObject go  =  new GameObject(objectname);
go.name = objectname;
go.transform.parent = obj;
rootObject = go.transform;

go.AddComponent(typeof(MeshFilter));
go.AddComponent(typeof(MeshRenderer));
go.AddComponent(typeof(MeshCollider));
go.AddComponent(typeof(sideObjectScript));
sideObjectScript scr = (sideObjectScript)go.GetComponent(typeof(sideObjectScript));
if(so.textureGUID !=""){
MeshRenderer mr = (MeshRenderer)go.GetComponent(typeof(MeshRenderer));
Material mat =  (Material)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(so.textureGUID), typeof(Material));
mr.material = mat;
scr.mat = mat;
}
scr.soIndex = index;
scr.soName = so.name;

scr.soAlign = int.Parse(so.align);
scr.soUVx = so.uvx;
scr.soUVy = so.uvy;
scr.m_distance = so.m_distance;
scr.objectType = so.objectType;
scr.weld = so.weld;
scr.combine = so.combine;
scr.OCCCDCOQDQ = so.OCCCDCOQDQ;
scr.m_go = so.OQOOQCQDQC;
if(so.OQOCCODODD != ""){
scr.OQOCCODODD = so.OQOCCODODD;

}
if(so.OQOCCODODD != ""){
scr.ODQDQDCDQQ = so.ODQDQDCDQQ;

}
scr.selectedRotation = so.selectedRotation;
scr.position = so.position;
scr.uvInt = so.uvType;
scr.randomObjects = so.randomObjects;
scr.childOrder = so.childOrder;
scr.sidewaysOffset = so.sidewaysOffset;
scr.density = so.density;
scr.OQDOCCQOCD = target;
scr.terrainTree = so.terrainTree;
scr.xPosition = so.xPosition;
scr.yPosition = so.yPosition;
scr.uvYRound = so.uvYRound;
scr.m_collider = so.collider;

}
}

}
