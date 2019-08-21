using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using EasyRoads3D;
using EasyRoads3DEditor;
public class EasyRoadsEditorMenu : ScriptableObject {







[MenuItem( "GameObject/Create Other/EasyRoads3D/New Object" )]
public static void  CreateEasyRoads3DObject ()
{

Terrain[] terrains = (Terrain[]) FindObjectsOfType(typeof(Terrain));
if(terrains.Length == 0){
EditorUtility.DisplayDialog("Alert", "No Terrain objects found! EasyRoads3D objects requires a terrain object to interact with. Please create a Terrain object first", "Close");
return;
}



if(NewEasyRoads3D.instance == null){
NewEasyRoads3D window = (NewEasyRoads3D)ScriptableObject.CreateInstance(typeof(NewEasyRoads3D));
window.ShowUtility();
}



}
[MenuItem( "GameObject/Create Other/EasyRoads3D/Back Up/Terrain Height Data" )]
public static void  GetTerrain ()
{
if(GetEasyRoads3DObjects()){

OQOOCDQCDO.OQDOOOODOD(Selection.activeGameObject);
}else{
EditorUtility.DisplayDialog("Alert", "No EasyRoads3D objects found! Terrain functions cannot be accessed!", "Close");
}
}
[MenuItem( "GameObject/Create Other/EasyRoads3D/Restore/Terrain Height Data" )]
public static void  SetTerrain ()
{
if(GetEasyRoads3DObjects()){

OQOOCDQCDO.OCDDOOQQOC(Selection.activeGameObject);
}else{
EditorUtility.DisplayDialog("Alert", "No EasyRoads3D objects found! Terrain functions cannot be accessed!", "Close");
}
}
[MenuItem( "GameObject/Create Other/EasyRoads3D/Back Up/Terrain Splatmap Data" )]
public static void  OOQDCOCCOO()
{
if(GetEasyRoads3DObjects()){

OOODCCCCOD.OOQDCOCCOO(Selection.activeGameObject);
}else{
EditorUtility.DisplayDialog("Alert", "No EasyRoads3D objects found! Terrain functions cannot be accessed!", "Close");
}
}
[MenuItem( "GameObject/Create Other/EasyRoads3D/Restore/Terrain Splatmap Data" )]
public static void  OCCQOODCDQ ()
{
if(GetEasyRoads3DObjects()){
string path = "";
if(EditorUtility.DisplayDialog("Road Splatmap", "Would you like to merge the terrain splatmap(s) with a road splatmap?", "Yes", "No")){
path = EditorUtility.OpenFilePanel("Select png road splatmap texture", "", "png");
}


OOODCCCCOD.OOOQODDCQC(true, 100, 4, path, Selection.activeGameObject);
}else{
EditorUtility.DisplayDialog("Alert", "No EasyRoads3D objects found! Terrain functions cannot be accessed!", "Close");
}
}
[MenuItem( "GameObject/Create Other/EasyRoads3D/Back Up/Terrain Vegetation Data" )]
public static void  OOOCDCCOOD()
{
if(GetEasyRoads3DObjects()){

OQOOCDQCDO.OOOCDCCOOD(Selection.activeGameObject, null, "");
}else{
EditorUtility.DisplayDialog("Alert", "No EasyRoads3D objects found! Terrain functions cannot be accessed!", "Close");
}
}
[MenuItem( "GameObject/Create Other/EasyRoads3D/Back Up/All Terrain Data" )]
public static void  GetAllData()
{
if(GetEasyRoads3DObjects()){

OQOOCDQCDO.OQDOOOODOD(Selection.activeGameObject);
OOODCCCCOD.OOQDCOCCOO(Selection.activeGameObject);
OQOOCDQCDO.OOOCDCCOOD(Selection.activeGameObject, null,"");
}else{
EditorUtility.DisplayDialog("Alert", "No EasyRoads3D objects found! Terrain functions cannot be accessed!", "Close");
}
}
[MenuItem( "GameObject/Create Other/EasyRoads3D/Restore/Terrain Vegetation Data" )]
public static void  OODDOQQQOC()
{
if(GetEasyRoads3DObjects()){

OQOOCDQCDO.OODDOQQQOC(Selection.activeGameObject);
}else{
EditorUtility.DisplayDialog("Alert", "No EasyRoads3D objects found! Terrain functions cannot be accessed!", "Close");
}
}
[MenuItem( "GameObject/Create Other/EasyRoads3D/Restore/All Terrain Data" )]
public static void  RestoreAllData()
{
if(GetEasyRoads3DObjects()){

OQOOCDQCDO.OCDDOOQQOC(Selection.activeGameObject);
OOODCCCCOD.OOOQODDCQC(true, 100, 4, "", Selection.activeGameObject);
OQOOCDQCDO.OODDOQQQOC(Selection.activeGameObject);

}else{
EditorUtility.DisplayDialog("Alert", "No EasyRoads3D objects found! Terrain functions cannot be accessed!", "Close");
}


}

[MenuItem ("GameObject/Create Other/EasyRoads3D/Side Objects/Object Manager")]
static void ShowObjectManager ()
{

if(RoadObjectScript.erInit == ""){
RoadObjectScript[] scripts = (RoadObjectScript[])FindObjectsOfType(typeof(RoadObjectScript));
if(scripts != null) Selection.activeGameObject = scripts[0].gameObject;


}
if(ObjectManager.instance == null){

if(Terrain.activeTerrain != null)ODODOOOCQC.terrainTrees = OQQOQDQOQD.OQQDDDDQOO();
ObjectManager window =(ObjectManager)ScriptableObject.CreateInstance(typeof(ObjectManager));
window.ShowUtility();
}
}


[MenuItem( "GameObject/Create Other/EasyRoads3D/Build EasyRoads3D Objects" )]
public static void  FinalizeRoads ()
{

bool destroyTerrainScript = true;
if(EditorUtility.DisplayDialog("Build EasyRoads3D Objects", "This process includes destroying all EasyRoads3D control objects. Did you make a backup of the Scene? Do you want to continue?\n\nDepending on the number of EasyRoads3D objects in the Scene and used side objects, this process may take a while. Please be patient. ", "Yes", "No")){
RoadObjectScript[] scripts = (RoadObjectScript[])FindObjectsOfType(typeof(RoadObjectScript));
foreach (RoadObjectScript script in scripts) {
bool renderflag = true;
bool renderAlreadyDone = false;
int num = 0;
if(!script.displayRoad){
num = EditorUtility.DisplayDialogComplex ("Disabled EasyRoads3D Object Detected:", script.gameObject.name + " is currently not displayed.\n\nWould you like to activate and finalize this object, destroy this object or skip it in this finalize procedure?", "Finalize", "Destroy", "Skip");
if(num == 0){
script.displayRoad = true;
if(script.OQQDCODQCD == null){
List<ODODDQQO> arr = OQOQQQDDOD.OOOQQDOOCQ(false);
script.OQODDOQDDC(arr, OQOQQQDDOD.OCDODCQODQ(arr), OQOQQQDDOD.OCDOCQODDC(arr));
}
script.OQQDCODQCD.OOCCCOCCDD(script.displayRoad, script.OOQCCQCQOD);
}
if(num == 1){

renderflag = false;
}
if(num == 2){
renderflag = false;
destroyTerrainScript = false;
}
}
if(script.transform != null && renderflag && !script.OQQDQQCCDQ){
if(script.OQQDCODQCD == null){
List<ODODDQQO> arr = OQOQQQDDOD.OOOQQDOOCQ(false);
script.OQODDOQDDC(arr, OQOQQQDDOD.OCDODCQODQ(arr), OQOQQQDDOD.OCDOCQODDC(arr));
}

if(RoadObjectScript.erInit == ""){
RoadObjectScript.erInit = OCQOQDDQQD.ODCOCOQCQO(RoadObjectScript.version); 
OQQOQDQOQD.erInit = RoadObjectScript.erInit;
}

if(script.OQQDCODQCD == null){
script.OQQDQQOCQC(script.transform, null, null, null);
}
OQQOQDQOQD.ODOCODOQCO = true;
if(!script.OQQDQQCCDQ){
script.geoResolution = 0.5f;
script.OQOCDQQODC();
if(script.objectType < 2) OQDCQOCDCQ(script);
script.OQQDCODQCD.terrainRendered = true;
script.OOOQOQQQDD();



}
if(script.displayRoad && script.objectType < 2){

if(script.objectType == 1){

SetWaterScript(script);
}
script.OQQDCODQCD.road.transform.parent = null;
script.OQQDCODQCD.road.layer = 0;
script.OQQDCODQCD.road.name = script.gameObject.name;
}
else if(script.OQQDCODQCD.road != null)DestroyImmediate(script.OQQDCODQCD.road);



bool flag = false;
for(int i=0;i<script.ODODQQOD.Length;i++){
if(script.ODODQQOD[i]){
flag = true;
break;
}
}
if(flag){
OODCCCCDCC.OCDQCCCDCO(script);
}
foreach(Transform child in script.transform){
if(child.name == "Side Objects"){
child.name = script.gameObject.name + " - Side Objects ";
child.parent = null;
}
}
}else if(script.OQQDQQCCDQ){
renderAlreadyDone = true;
destroyTerrainScript = false;
}
if((script.displayRoad || num != 2) && !renderAlreadyDone)DestroyImmediate(script.gameObject);
}

if(destroyTerrainScript){
EasyRoads3DTerrainID[] terrainscripts = (EasyRoads3DTerrainID[])FindObjectsOfType(typeof(EasyRoads3DTerrainID));
foreach (EasyRoads3DTerrainID script in terrainscripts) {
DestroyImmediate(script);
}
}
}
}

public static bool GetEasyRoads3DObjects(){
RoadObjectScript[] scripts = (RoadObjectScript[])FindObjectsOfType(typeof(RoadObjectScript));
bool flag = false;
foreach (RoadObjectScript script in scripts) {
if(script.OQQDCODQCD == null){

List<ODODDQQO> arr = OQOQQQDDOD.OOOQQDOOCQ(false);
script.OQQDQQOCQC(script.transform, arr, OQOQQQDDOD.OCDODCQODQ(arr), OQOQQQDDOD.OCDOCQODDC(arr));


}
flag = true;
}
return flag;
}

static private void OQDCQOCDCQ(RoadObjectScript target){
EditorUtility.DisplayProgressBar("Build EasyRoads3D Object - " + target.gameObject.name,"Initializing", 0);

RoadObjectScript[] scripts = (RoadObjectScript[])FindObjectsOfType(typeof(RoadObjectScript));
List<Transform> rObj = new List<Transform>();


#if UNITY_4_3

#else
Undo.RegisterUndo(OOODCCCCOD.terrain.terrainData, "EasyRoads3D Terrain leveling");
#endif
foreach(RoadObjectScript script in scripts) {
if(script.transform != target.transform) rObj.Add(script.transform);
}
if(target.ODODQOQO == null){
target.ODODQOQO = target.OQQDCODQCD.ODOCCQQDCO();
target.ODODQOQOInt = target.OQQDCODQCD.OOOCOQCQQC();
}
target.OOQDDOQCDQ(0.5f, true, false);

List<tPoint> hitODDDDQQQDC = target.OQQDCODQCD.OOCOOQOCQD(Vector3.zero, target.raise, target.obj, target.OOQDOOQQ, rObj, target.handleVegetation);
List<Vector3> changeArr = new List<Vector3>();
float stepsf = Mathf.Floor(hitODDDDQQQDC.Count / 10);
int steps = Mathf.RoundToInt(stepsf);
for(int i = 0; i < 10;i++){
changeArr = target.OQQDCODQCD.OCCDODQCOO(hitODDDDQQQDC, i * steps, steps, changeArr);
EditorUtility.DisplayProgressBar("Build EasyRoads3D Object - " + target.gameObject.name,"Updating Terrain", i * 10);
}

changeArr = target.OQQDCODQCD.OCCDODQCOO(hitODDDDQQQDC, 10 * steps, hitODDDDQQQDC.Count - (10 * steps), changeArr);
target.OQQDCODQCD.ODDOOQCQQQ(changeArr, rObj);

target.OOOQOQQQDD();
EditorUtility.ClearProgressBar();

}
private static void SetWaterScript(RoadObjectScript target){
for(int i = 0; i < target.OQDCQCQDDC.Length; i++){
if(target.OQQDCODQCD.road.GetComponent(target.OQDCQCQDDC[i]) != null && i != target.selectedWaterScript)DestroyImmediate(target.OQQDCODQCD.road.GetComponent(target.OQDCQCQDDC[i]));
}
if(target.OQDCQCQDDC[0] != "None Available!"  && target.OQQDCODQCD.road.GetComponent(target.OQDCQCQDDC[target.selectedWaterScript]) == null){
UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(target.OQQDCODQCD.road, "Assets/EasyRoads3D/Editor/EasyRoadsEditorMenu.cs (309,1)", target.OQDCQCQDDC[target.selectedWaterScript]);

}
}
public static Vector3 ReadFile(string file)
{
Vector3 pos = Vector3.zero;
if(File.Exists(file)){
StreamReader streamReader = File.OpenText(file);
string line = streamReader.ReadLine();
line = line.Replace(",",".");
string[] lines = line.Split("\n"[0]);
string[] arr = lines[0].Split("|"[0]);
float.TryParse(arr[0],System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out pos.x);
float.TryParse(arr[1],System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out pos.y);
float.TryParse(arr[2],System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out pos.z);
}
return pos;
}
}
