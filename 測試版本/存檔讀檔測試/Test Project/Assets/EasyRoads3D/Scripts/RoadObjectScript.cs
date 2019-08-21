using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System;
using EasyRoads3D;

public class RoadObjectScript : MonoBehaviour {
static public string version = "";
public int objectType = 0;
public bool displayRoad = true;
public float roadWidth = 5.0f;
public float indent = 3.0f;
public float surrounding = 5.0f;
public float raise = 1.0f;
public float raiseMarkers = 0.5f;
public bool OOQDOOQQ = false;
public bool renderRoad = true;
public bool beveledRoad = false;
public bool applySplatmap = false;
public int splatmapLayer = 4;
public bool autoUpdate = true;
public float geoResolution = 5.0f;
public int roadResolution = 1;
public float tuw =  15.0f;
public int splatmapSmoothLevel;
public float opacity = 1.0f;
public int expand = 0;
public int offsetX = 0;
public int offsetY = 0;
private Material surfaceMaterial;
public float surfaceOpacity = 1.0f;
public float smoothDistance = 1.0f;
public float smoothSurDistance = 3.0f;
private bool handleInsertFlag;
public bool handleVegetation = true;
public float OCCQQDCQDQ = 2.0f;
public float OQDCDOQQQQ = 1f;
public int materialType = 0;
String[] materialStrings;
public string uname;
public string email;
private MarkerScript[] mSc;

private bool OCQCQQQOOO;
private bool[] ODDQDOQQCC = null;
private bool[] OQDQQQQQDC = null;
public string[] OOOQDCCQOD;
public string[] ODODQOQO;
public int[] ODODQOQOInt;
public int OQCQODCDOC = -1;
public int OQDOODDQOC = -1;
static public GUISkin OCQODCOOQQ;
static public GUISkin ODCCDDDCDQ;
public bool OQCOQQQODO = false;
private Vector3 cPos;
private Vector3 ePos;
public bool OOCDOQQODQ;
static public Texture2D OCDOCCOCDO;
public int markers = 1;
public OQQOQDQOQD OQQDCODQCD;
private GameObject ODOQDQOO;
public bool OQQDQQCCDQ;
public bool doTerrain;
private Transform OCQOQDDQCO = null;
public GameObject[] OCQOQDDQCOs;
private static string OCDQDQDOQO = null;
public Transform obj;
private string OOCODDODCQ;
public static string erInit = "";
static public Transform ODOODODDQO;
private RoadObjectScript OQDOCCQOCD;
public bool flyby;


private Vector3 pos;
private float fl;
private float oldfl;
private bool OQCOOCQCQC;
private bool OCQCDCDCDO;
private bool ODOQQQCCDD;
public Transform OOQCCQCQOD;
public int OdQODQOD = 1;
public float OOQQQDOD = 0f;
public float OOQQQDODOffset = 0f;
public float OOQQQDODLength = 0f;
public bool ODODDDOO = false;
static public string[] ODOQDOQO;
static public string[] ODODOQQO; 
static public string[] ODODQOOQ;
public int ODQDOOQO = 0;
public string[] ODQQQQQO;  
public string[] ODODDQOO; 
public bool[] ODODQQOD; 
public int[] OOQQQOQO; 
public int ODOQOOQO = 0; 

public bool forceY = false;
public float yChange = 0f;
public float floorDepth = 2f;
public float waterLevel = 1.5f; 
public bool lockWaterLevel = true;
public float lastY = 0f;
public string distance = "0";
public string markerDisplayStr = "Hide Markers";
static public string[] objectStrings;
public string objectText = "Road";
public bool applyAnimation = false;
public float waveSize = 1.5f;
public float waveHeight = 0.15f;
public bool snapY = true;

private TextAnchor origAnchor;
public bool autoODODDQQO;
public Texture2D roadTexture;
public Texture2D roadMaterial;
public string[] OQDOOOQDQQ;
public string[] OQDCQCQDDC;
public int selectedWaterMaterial;
public int selectedWaterScript;
private bool doRestore = false;
public bool doFlyOver;
public static GameObject tracer;
public Camera goCam;
public float speed = 1f;
public float offset = 0f;
public bool camInit;
public GameObject customMesh = null;
static public bool disableFreeAlerts = true;
public bool multipleTerrains;
public bool editRestore = true;
public Material roadMaterialEdit;
static public int backupLocation = 0;
public string[] backupStrings = new string[2]{"Outside Assets folder path","Inside Assets folder path"};
public Vector3[] leftVecs = new Vector3[0];
public Vector3[] rightVecs = new Vector3[0];
public bool applyTangents = false;
public bool sosBuild = false;
public float splinePos = 0;
public float camHeight = 3;
public Vector3 splinePosV3 = Vector3.zero;
public bool iOS = false;
public void OQODDOQDDC(List<ODODDQQO> arr, String[] DOODQOQO, String[] OODDQOQO){

OQQDQQOCQC(transform, arr, DOODQOQO, OODDQOQO);
}
public void OOQQOQCQDC(MarkerScript markerScript){

OCQOQDDQCO = markerScript.transform;



List<GameObject> tmp = new List<GameObject>();
for(int i=0;i<OCQOQDDQCOs.Length;i++){
if(OCQOQDDQCOs[i] != markerScript.gameObject)tmp.Add(OCQOQDDQCOs[i]);
}




tmp.Add(markerScript.gameObject);
OCQOQDDQCOs = tmp.ToArray();
OCQOQDDQCO = markerScript.transform;

OQQDCODQCD.OQCOQCDODO(OCQOQDDQCO, OCQOQDDQCOs, markerScript.OCQOQQCDCQ, markerScript.OQCCQQCCQQ, OOQCCQCQOD, out markerScript.OCQOQDDQCOs, out markerScript.trperc, OCQOQDDQCOs);

OQDOODDQOC = -1;
}
public void OOCDDDDQQQ(MarkerScript markerScript){
if(markerScript.OQCCQQCCQQ != markerScript.ODOOQQOO || markerScript.OQCCQQCCQQ != markerScript.ODOOQQOO){
OQQDCODQCD.OQCOQCDODO(OCQOQDDQCO, OCQOQDDQCOs, markerScript.OCQOQQCDCQ, markerScript.OQCCQQCCQQ, OOQCCQCQOD, out markerScript.OCQOQDDQCOs, out markerScript.trperc, OCQOQDDQCOs);
markerScript.ODQDOQOO = markerScript.OCQOQQCDCQ;
markerScript.ODOOQQOO = markerScript.OQCCQQCCQQ;
}
if(OQDOCCQOCD.autoUpdate) OOQDDOQCDQ(OQDOCCQOCD.geoResolution, false, false);
}
public void ResetMaterials(MarkerScript markerScript){
if(OQQDCODQCD != null)OQQDCODQCD.OQCOQCDODO(OCQOQDDQCO, OCQOQDDQCOs, markerScript.OCQOQQCDCQ, markerScript.OQCCQQCCQQ, OOQCCQCQOD, out markerScript.OCQOQDDQCOs, out markerScript.trperc, OCQOQDDQCOs);
}
public void ODOODDCQDC(MarkerScript markerScript){
if(markerScript.OQCCQQCCQQ != markerScript.ODOOQQOO){
OQQDCODQCD.OQCOQCDODO(OCQOQDDQCO, OCQOQDDQCOs, markerScript.OCQOQQCDCQ, markerScript.OQCCQQCCQQ, OOQCCQCQOD, out markerScript.OCQOQDDQCOs, out markerScript.trperc, OCQOQDDQCOs);
markerScript.ODOOQQOO = markerScript.OQCCQQCCQQ;
}
OOQDDOQCDQ(OQDOCCQOCD.geoResolution, false, false);
}
private void ODCDQOOCOO(string ctrl, MarkerScript markerScript){
int i = 0;
foreach(Transform tr in markerScript.OCQOQDDQCOs){
MarkerScript wsScript = (MarkerScript) tr.GetComponent<MarkerScript>();
if(ctrl == "rs") wsScript.LeftSurrounding(markerScript.rs - markerScript.ODOQQOOO, markerScript.trperc[i]);
else if(ctrl == "ls") wsScript.RightSurrounding(markerScript.ls - markerScript.DODOQQOO, markerScript.trperc[i]);
else if(ctrl == "ri") wsScript.LeftIndent(markerScript.ri - markerScript.OOQOQQOO, markerScript.trperc[i]);
else if(ctrl == "li") wsScript.RightIndent(markerScript.li - markerScript.ODODQQOO, markerScript.trperc[i]);
else if(ctrl == "rt") wsScript.LeftTilting(markerScript.rt - markerScript.ODDQODOO, markerScript.trperc[i]);
else if(ctrl == "lt") wsScript.RightTilting(markerScript.lt - markerScript.ODDOQOQQ, markerScript.trperc[i]);
else if(ctrl == "floorDepth") wsScript.FloorDepth(markerScript.floorDepth - markerScript.oldFloorDepth, markerScript.trperc[i]);
i++;
}
}
public void OCOODQDDQO(){
if(markers > 1) OOQDDOQCDQ(OQDOCCQOCD.geoResolution, false, false);
}
public void OQQDQQOCQC(Transform tr, List<ODODDQQO> arr, String[] DOODQOQO, String[] OODDQOQO){
version = "2.5.4";
OCQODCOOQQ = (GUISkin)Resources.Load("ER3DSkin", typeof(GUISkin));


OCDOCCOCDO = (Texture2D)Resources.Load("ER3DLogo", typeof(Texture2D));
if(RoadObjectScript.objectStrings == null){
RoadObjectScript.objectStrings = new string[3];
RoadObjectScript.objectStrings[0] = "Road Object"; RoadObjectScript.objectStrings[1]="River Object";RoadObjectScript.objectStrings[2]="Procedural Mesh Object";
}
obj = tr;
OQQDCODQCD = new OQQOQDQOQD();
OQDOCCQOCD = obj.GetComponent<RoadObjectScript>();
foreach(Transform child in obj){
if(child.name == "Markers") OOQCCQCQOD = child;
}
OQQOQDQOQD.terrainList.Clear();
Terrain[] terrains = (Terrain[])FindObjectsOfType(typeof(Terrain));
foreach(Terrain terrain in terrains) {
Terrains t = new Terrains();
t.terrain = terrain;
if(!terrain.gameObject.GetComponent<EasyRoads3DTerrainID>()){
EasyRoads3DTerrainID terrainscript = (EasyRoads3DTerrainID)terrain.gameObject.AddComponent<EasyRoads3DTerrainID>();
string id = UnityEngine.Random.Range(100000000,999999999).ToString();
terrainscript.terrainid = id;
t.id = id;
}else{
t.id = terrain.gameObject.GetComponent<EasyRoads3DTerrainID>().terrainid;
}
OQQDCODQCD.OCOCDOQDQD(t);
}
OOODCCCCOD.OCOCDOQDQD();
if(roadMaterialEdit == null){
roadMaterialEdit = (Material)Resources.Load("materials/roadMaterialEdit", typeof(Material));
}
if(objectType == 0 && GameObject.Find(gameObject.name + "/road") == null){
GameObject road = new GameObject("road");
road.transform.parent = transform;
}

OQQDCODQCD.OQDDDQQDCO(obj, OCDQDQDOQO, OQDOCCQOCD.roadWidth, surfaceOpacity, out OOCDOQQODQ, out indent, applyAnimation, waveSize, waveHeight);
OQQDCODQCD.OQDCDOQQQQ = OQDCDOQQQQ;
OQQDCODQCD.OCCQQDCQDQ = OCCQQDCQDQ;
OQQDCODQCD.OdQODQOD = OdQODQOD + 1;
OQQDCODQCD.OOQQQDOD = OOQQQDOD;
OQQDCODQCD.OOQQQDODOffset = OOQQQDODOffset;
OQQDCODQCD.OOQQQDODLength = OOQQQDODLength;
OQQDCODQCD.objectType = objectType;
OQQDCODQCD.snapY = snapY;
OQQDCODQCD.terrainRendered = OQQDQQCCDQ;
OQQDCODQCD.handleVegetation = handleVegetation;
OQQDCODQCD.raise = raise;
OQQDCODQCD.roadResolution = roadResolution;
OQQDCODQCD.multipleTerrains = multipleTerrains;
OQQDCODQCD.editRestore = editRestore;
OQQDCODQCD.roadMaterialEdit = roadMaterialEdit;
OQQDCODQCD.renderRoad = renderRoad;
OQQDCODQCD.iOS = iOS;
if(backupLocation == 0)OOQOOOOOOO.backupFolder = "/EasyRoads3D";
else OOQOOOOOOO.backupFolder =  "/Assets/EasyRoads3D/backups";

ODODQOQO = OQQDCODQCD.ODOCCQQDCO();
ODODQOQOInt = OQQDCODQCD.OOOCOQCQQC();


if(OQQDQQCCDQ){




doRestore = true;
}


OQOCDQQODC();

if(arr != null || ODODQOOQ == null) ODDQQDDCDC(arr, DOODQOQO, OODDQOQO);


if(doRestore) return;
}
public void UpdateBackupFolder(){
}
public void OOOQQDOQQO(){
if(!ODODDDOO || objectType == 2){
if(ODDQDOQQCC != null){
for(int i = 0; i < ODDQDOQQCC.Length; i++){
ODDQDOQQCC[i] = false;
OQDQQQQQDC[i] = false;
}
}
}
}

public void OQDOQQCQCC(Vector3 pos){


if(!displayRoad){
displayRoad = true;
OQQDCODQCD.OOCCCOCCDD(displayRoad, OOQCCQCQOD);
}
pos.y += OQDOCCQOCD.raiseMarkers;
if(forceY && ODOQDQOO != null){
float dist = Vector3.Distance(pos, ODOQDQOO.transform.position);
pos.y = ODOQDQOO.transform.position.y + (yChange * (dist / 100f));
}else if(forceY && markers == 0) lastY = pos.y;
GameObject go = null;
if(ODOQDQOO != null) go = (GameObject)Instantiate(ODOQDQOO);
else go = (GameObject)Instantiate(Resources.Load("marker", typeof(GameObject)));
Transform newnode = go.transform;
newnode.position = pos;
newnode.parent = OOQCCQCQOD;
markers++;
string n;
if(markers < 10) n = "Marker000" + markers.ToString();
else if (markers < 100) n = "Marker00" + markers.ToString();
else n = "Marker0" + markers.ToString();
newnode.gameObject.name = n;
MarkerScript scr = newnode.GetComponent<MarkerScript>();
scr.OOCDOQQODQ = false;
scr.objectScript = obj.GetComponent<RoadObjectScript>();
if(ODOQDQOO == null){
scr.waterLevel = OQDOCCQOCD.waterLevel;
scr.floorDepth = OQDOCCQOCD.floorDepth;
scr.ri = OQDOCCQOCD.indent;
scr.li = OQDOCCQOCD.indent;
scr.rs = OQDOCCQOCD.surrounding;
scr.ls = OQDOCCQOCD.surrounding;
scr.tension = 0.5f;
if(objectType == 1){

pos.y -= waterLevel;
newnode.position = pos;
}
}
if(objectType == 2){
#if UNITY_3_5
if(scr.surface != null)scr.surface.gameObject.active = false;
#else
if(scr.surface != null)scr.surface.gameObject.SetActive(false);
#endif
}
ODOQDQOO = newnode.gameObject;
if(markers > 1){
OOQDDOQCDQ(OQDOCCQOCD.geoResolution, false, false);
if(materialType == 0){

OQQDCODQCD.OCDCCQOOOO(materialType);

}
}
}
public void OOQDDOQCDQ(float geo, bool renderMode, bool camMode){
OQQDCODQCD.OCOOQQQQQC.Clear();
int ii = 0;
OQQDDDCQCC k;
foreach(Transform child  in obj)
{
if(child.name == "Markers"){
foreach(Transform marker   in child) {
MarkerScript markerScript = marker.GetComponent<MarkerScript>();
markerScript.objectScript = obj.GetComponent<RoadObjectScript>();
if(!markerScript.OOCDOQQODQ) markerScript.OOCDOQQODQ = OQQDCODQCD.ODOCQCDQCQ(marker);
k  = new OQQDDDCQCC();
k.position = marker.position;
k.num = OQQDCODQCD.OCOOQQQQQC.Count;
k.object1 = marker;
k.object2 = markerScript.surface;
k.tension = markerScript.tension;
k.ri = markerScript.ri;
if(k.ri < 1)k.ri = 1f;
k.li =markerScript.li;
if(k.li < 1)k.li = 1f;
k.rt = markerScript.rt;
k.lt = markerScript.lt;
k.rs = markerScript.rs;
if(k.rs < 1)k.rs = 1f;
k.OCQCDOQDQO = markerScript.rs;
k.ls = markerScript.ls;
if(k.ls < 1)k.ls = 1f;
k.OCDOQQCCCD = markerScript.ls;
k.renderFlag = markerScript.bridgeObject;
k.OQOQQDQOQC = markerScript.distHeights;
k.newSegment = markerScript.newSegment;
k.tunnelFlag = markerScript.tunnelFlag;
k.floorDepth = markerScript.floorDepth;
k.waterLevel = waterLevel;
k.lockWaterLevel = markerScript.lockWaterLevel;
k.sharpCorner = markerScript.sharpCorner;
k.OQDOOCCQDO = OQQDCODQCD;
markerScript.markerNum = ii;
markerScript.distance = "-1";
markerScript.OQDCOQQQOQ = "-1";
OQQDCODQCD.OCOOQQQQQC.Add(k);
ii++;
}
}
}
distance = "-1";

OQQDCODQCD.OQDQQDDCQQ = OQDOCCQOCD.roadWidth;

OQQDCODQCD.OCOCCQCCQC(geo, obj, OQDOCCQOCD.OOQDOOQQ, renderMode, camMode, objectType);
if(OQQDCODQCD.leftVecs.Count > 0){
leftVecs = OQQDCODQCD.leftVecs.ToArray();
rightVecs = OQQDCODQCD.rightVecs.ToArray();
}
}
public void StartCam(){

OOQDDOQCDQ(0.5f, false, true);

}
public void OQOCDQQODC(){
int i = 0;
foreach(Transform child  in obj)
{
if(child.name == "Markers"){
i = 1;
string n;
foreach(Transform marker in child) {
if(i < 10) n = "Marker000" + i.ToString();
else if (i < 100) n = "Marker00" + i.ToString();
else n = "Marker0" + i.ToString();
marker.name = n;
ODOQDQOO = marker.gameObject;
i++;
}
}
}
markers = i - 1;

OOQDDOQCDQ(OQDOCCQOCD.geoResolution, false, false);
}
public List<Transform> RebuildObjs(){
RoadObjectScript[] scripts = (RoadObjectScript[])FindObjectsOfType(typeof(RoadObjectScript));
List<Transform> rObj = new List<Transform>();
foreach (RoadObjectScript script in scripts) {
if(script.transform != transform) rObj.Add(script.transform);
}
return rObj;
}
public void RestoreTerrain1(){

OOQDDOQCDQ(OQDOCCQOCD.geoResolution, false, false);
if(OQQDCODQCD != null) OQQDCODQCD.ODQDODDOCQ();
ODODDDOO = false;
}
public void OOOQOQQQDD(){
OQQDCODQCD.OOOQOQQQDD(OQDOCCQOCD.applySplatmap, OQDOCCQOCD.splatmapSmoothLevel, OQDOCCQOCD.renderRoad, OQDOCCQOCD.tuw, OQDOCCQOCD.roadResolution, OQDOCCQOCD.raise, OQDOCCQOCD.opacity, OQDOCCQOCD.expand, OQDOCCQOCD.offsetX, OQDOCCQOCD.offsetY, OQDOCCQOCD.beveledRoad, OQDOCCQOCD.splatmapLayer, OQDOCCQOCD.OdQODQOD, OOQQQDOD, OOQQQDODOffset, OOQQQDODLength);
}
public void OCCOQDDOQQ(){
OQQDCODQCD.OCCOQDDOQQ(OQDOCCQOCD.renderRoad, OQDOCCQOCD.tuw, OQDOCCQOCD.roadResolution, OQDOCCQOCD.raise, OQDOCCQOCD.beveledRoad, OQDOCCQOCD.OdQODQOD, OOQQQDOD, OOQQQDODOffset, OOQQQDODLength);
}
public void OOCOCODQDO(Vector3 pos, bool doInsert){


if(!displayRoad){
displayRoad = true;
OQQDCODQCD.OOCCCOCCDD(displayRoad, OOQCCQCQOD);
}

int first = -1;
int second = -1;
float dist1 = 10000;
float dist2 = 10000;
Vector3 newpos = pos;
OQQDDDCQCC k;
OQQDDDCQCC k1 = (OQQDDDCQCC)OQQDCODQCD.OCOOQQQQQC[0];
OQQDDDCQCC k2 = (OQQDDDCQCC)OQQDCODQCD.OCOOQQQQQC[1];

OQQDCODQCD.ODDOQOQOOD(pos, out first, out second, out dist1, out dist2, out k1, out k2, out newpos);
pos = newpos;
if(doInsert && first >= 0 && second >= 0){
if(OQDOCCQOCD.OOQDOOQQ && second == OQQDCODQCD.OCOOQQQQQC.Count - 1){
OQDOQQCQCC(pos);
}else{
k = (OQQDDDCQCC)OQQDCODQCD.OCOOQQQQQC[second];
string name = k.object1.name;
string n;
int j = second + 2;
for(int i = second; i < OQQDCODQCD.OCOOQQQQQC.Count - 1; i++){
k = (OQQDDDCQCC)OQQDCODQCD.OCOOQQQQQC[i];
if(j < 10) n = "Marker000" + j.ToString();
else if (j < 100) n = "Marker00" + j.ToString();
else n = "Marker0" + j.ToString();
k.object1.name = n;
j++;
}
k = (OQQDDDCQCC)OQQDCODQCD.OCOOQQQQQC[first];
Transform newnode = (Transform)Instantiate(k.object1.transform, pos, k.object1.rotation);
newnode.gameObject.name = name;
newnode.parent = OOQCCQCQOD;
MarkerScript scr = newnode.GetComponent<MarkerScript>();
scr.OOCDOQQODQ = false;
float	totalDist = dist1 + dist2;
float perc1 = dist1 / totalDist;
float paramDif = k1.ri - k2.ri;
scr.ri = k1.ri - (paramDif * perc1);
paramDif = k1.li - k2.li;
scr.li = k1.li - (paramDif * perc1);
paramDif = k1.rt - k2.rt;
scr.rt = k1.rt - (paramDif * perc1);
paramDif = k1.lt - k2.lt;
scr.lt = k1.lt - (paramDif * perc1);
paramDif = k1.rs - k2.rs;
scr.rs = k1.rs - (paramDif * perc1);
paramDif = k1.ls - k2.ls;
scr.ls = k1.ls - (paramDif * perc1);
OOQDDOQCDQ(OQDOCCQOCD.geoResolution, false, false);
if(materialType == 0)OQQDCODQCD.OCDCCQOOOO(materialType);
#if UNITY_3_5
if(objectType == 2) scr.surface.gameObject.active = false;
#else
if(objectType == 2) scr.surface.gameObject.SetActive(false);
#endif
}
}
OQOCDQQODC();
}
public void OQQDDOOQDC(){

DestroyImmediate(OQDOCCQOCD.OCQOQDDQCO.gameObject);
OCQOQDDQCO = null;
OQOCDQQODC();
}
public void OQDCDQOCDQ(){

OQQDCODQCD.OCQODOCDQO(12);

}

public List<SideObjectParams> OOODQCQOQD(){
List<SideObjectParams> param = new List<SideObjectParams>();
SideObjectParams sop;
foreach(Transform child in obj){
if(child.name == "Markers"){
foreach(Transform marker in child){
MarkerScript markerScript = marker.GetComponent<MarkerScript>();
sop  = new SideObjectParams();
sop.ODDGDOOO = markerScript.ODDGDOOO;
sop.ODDQOODO = markerScript.ODDQOODO;
sop.ODDQOOO = markerScript.ODDQOOO;
param.Add(sop);
}
}
}
return param;
}
public void OQCQODCCCD(){
List<string> arrNames = new List<string>();
List<int> arrInts = new List<int>();
List<string> arrIDs = new List<string>();

for(int i=0;i<ODODOQQO.Length;i++){
if(ODODQQOD[i] == true){
arrNames.Add(ODODQOOQ[i]);
arrIDs.Add(ODODOQQO[i]);
arrInts.Add(i);
}
}
ODODDQOO = arrNames.ToArray();
OOQQQOQO = arrInts.ToArray();
}
public void ODDQQDDCDC(List<ODODDQQO> arr, String[] DOODQOQO, String[] OODDQOQO){



bool saveSOs = false;
ODODOQQO = DOODQOQO;
ODODQOOQ = OODDQOQO;






List<MarkerScript> markerArray = new List<MarkerScript>();
if(obj == null)OQQDQQOCQC(transform, null, null, null);
foreach(Transform child  in obj) {
if(child.name == "Markers"){
foreach(Transform marker  in child) {
MarkerScript markerScript = marker.GetComponent<MarkerScript>();
markerScript.OQODQQDO.Clear();
markerScript.ODOQQQDO.Clear();
markerScript.OQQODQQOO.Clear();
markerScript.ODDOQQOO.Clear();
markerArray.Add(markerScript);
}
}
}
mSc = markerArray.ToArray();





List<bool> arBools = new List<bool>();



int counter1 = 0;
int counter2 = 0;

if(ODQQQQQO != null){

if(arr.Count == 0) return;



for(int i = 0; i < ODODOQQO.Length; i++){
ODODDQQO so = (ODODDQQO)arr[i];

for(int j = 0; j < ODQQQQQO.Length; j++){
if(ODODOQQO[i] == ODQQQQQO[j]){
counter1++;


if(ODODQQOD.Length > j ) arBools.Add(ODODQQOD[j]);
else arBools.Add(false);

foreach(MarkerScript scr  in mSc) {


int l = -1;
for(int k = 0; k < scr.ODDOOQDO.Length; k++){
if(so.id == scr.ODDOOQDO[k]){
l = k;
break;
}
}
if(l >= 0){
scr.OQODQQDO.Add(scr.ODDOOQDO[l]);
scr.ODOQQQDO.Add(scr.ODDGDOOO[l]);
scr.OQQODQQOO.Add(scr.ODDQOOO[l]);

if(so.sidewaysDistanceUpdate == 0 || (so.sidewaysDistanceUpdate == 2 && (float)scr.ODDQOODO[l] != so.oldSidwaysDistance)){
scr.ODDOQQOO.Add(scr.ODDQOODO[l]);

}else{
scr.ODDOQQOO.Add(so.splinePosition);

}




}else{
scr.OQODQQDO.Add(so.id);
scr.ODOQQQDO.Add(so.markerActive);
scr.OQQODQQOO.Add(true);
scr.ODDOQQOO.Add(so.splinePosition);
}

}
}
}
if(so.sidewaysDistanceUpdate != 0){



}
saveSOs = false;
}
}


for(int i = 0; i < ODODOQQO.Length; i++){
ODODDQQO so = (ODODDQQO)arr[i];
bool flag = false;
for(int j = 0; j < ODQQQQQO.Length; j++){

if(ODODOQQO[i] == ODQQQQQO[j])flag = true;
}
if(!flag){
counter2++;

arBools.Add(false);

foreach(MarkerScript scr  in mSc) {
scr.OQODQQDO.Add(so.id);
scr.ODOQQQDO.Add(so.markerActive);
scr.OQQODQQOO.Add(true);
scr.ODDOQQOO.Add(so.splinePosition);
}

}
}

ODODQQOD = arBools.ToArray();


ODQQQQQO = new String[ODODOQQO.Length];
ODODOQQO.CopyTo(ODQQQQQO,0);





List<int> arInt= new List<int>();
for(int i = 0; i < ODODQQOD.Length; i++){
if(ODODQQOD[i]) arInt.Add(i);
}
OOQQQOQO  = arInt.ToArray();


foreach(MarkerScript scr  in mSc) {
scr.ODDOOQDO = scr.OQODQQDO.ToArray();
scr.ODDGDOOO = scr.ODOQQQDO.ToArray();
scr.ODDQOOO = scr.OQQODQQOO.ToArray();
scr.ODDQOODO = scr.ODDOQQOO.ToArray();

}
if(saveSOs){

}



}
public bool CheckWaterHeights(){
if(OOODCCCCOD.terrain == null) return false;
bool flag = true;

float y = OOODCCCCOD.terrain.transform.position.y;
foreach(Transform child  in obj) {
if(child.name == "Markers"){
foreach(Transform marker  in child) {
//MarkerScript markerScript = marker.GetComponent<MarkerScript>();
if(marker.position.y - y <= 0.1f) flag = false;
}
}
}
return flag;
}
}
