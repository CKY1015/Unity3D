using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Text;
using System;
using EasyRoads3D;
using EasyRoads3DEditor;
public class ObjectManager : EditorWindow
{

public static ObjectManager instance;
public string strVar;
public int intVar;
public GameObject goVar;
public ODODDQQO soVar;
public string[] strArr;
public List<ODODDQQO> sosArr;
public int returnType = 0;
public ObjectManager()
{

if( instance != null ){



}
instance = this;
title = "EasyRoads3D Object Manager";
position = new Rect((Screen.width - 550.0f) / 2.0f, (Screen.height - 400.0f) / 2.0f, 550.0f, 400.0f);
minSize = new Vector2(550.0f, 400.0f);
maxSize = new Vector2(550.0f, 400.0f);
ODODOOOCQC.OOCCOOQDCO();

}
public void OnDestroy(){
if(ProceduralObjectsEditor.instance != null){
ProceduralObjectsEditor.instance.Close();
}
ODODOOOCQC.newObjectFlag = false;
ODODOOOCQC.duplicateObjectFlag = false;
instance = null;
}
public static ObjectManager Instance{
get
{
if( instance == null ){
new ObjectManager();
}
return instance;
}
}
public void OnGUI()
{
strArr = null;
sosArr = null;
returnType = 0;
ODODOOOCQC.ODDQCDDQCQ(ref strVar, ref intVar, ref goVar, ref soVar, ref strArr, ref sosArr, ref returnType);


if(returnType == 2){






ProceduralObjectsEditor editor = null;
if(ProceduralObjectsEditor.instance == null){

editor = (ProceduralObjectsEditor)ScriptableObject.CreateInstance(typeof(ProceduralObjectsEditor));
}else{
editor = ProceduralObjectsEditor.instance;

}
editor.position = new Rect (editor.position.x, editor.position.y, 500, 400);

editor.title = strVar;



ODODDQQO so = soVar;


editor.DisplayNodes(intVar, so, goVar);
editor.Show();


}else if(returnType == 4){


List<ODODDQQO> arr = OQOQQQDDOD.OOOQQDOOCQ(false);
RoadObjectScript.ODODOQQO = OQOQQQDDOD.OCDODCQODQ(arr);
RoadObjectScript[] scripts = (RoadObjectScript[])FindObjectsOfType(typeof(RoadObjectScript));
foreach(RoadObjectScript scr in scripts){

scr.ODDQQDDCDC(arr, OQOQQQDDOD.OCDODCQODQ(arr), OQOQQQDDOD.OCDOCQODDC(arr));
}
if(ProceduralObjectsEditor.instance != null){
ProceduralObjectsEditor.instance.Close();
}
instance.Close();

}else if(returnType == 1){

SideObjectImporter ieditor = (SideObjectImporter)ScriptableObject.CreateInstance(typeof(SideObjectImporter));
SideObjectImporter.sideobjects =  strArr;
SideObjectImporter.flags = new bool[intVar];

SideObjectImporter.importedSos = sosArr;
ieditor.ShowUtility();


}else if(returnType == 3){

List<ODODDQQO> arr = OQOQQQDDOD.OOOQQDOOCQ(false);
RoadObjectScript.ODODOQQO = OQOQQQDDOD.OCDODCQODQ(arr);
RoadObjectScript[] scripts = (RoadObjectScript[])FindObjectsOfType(typeof(RoadObjectScript));
foreach(RoadObjectScript scr in scripts){

List<ODODDQQO>  arr1  = OQOQQQDDOD.OOOQQDOOCQ(false);
if(scr.OQQDCODQCD == null) {

scr.OQODDOQDDC(arr1, OQOQQQDDOD.OCDODCQODQ(arr1), OQOQQQDDOD.OCDOCQODDC(arr1));
}
scr.ODDQQDDCDC(arr1, OQOQQQDDOD.OCDODCQODQ(arr1), OQOQQQDDOD.OCDOCQODDC(arr1));
if(scr.OQQDQQCCDQ == true || scr.objectType == 2){
GameObject go = GameObject.Find(scr.gameObject.name+"/Side Objects/"+strVar);

if(go != null){

OODCCCCDCC.OCCCDDDQDO((sideObjectScript)go.GetComponent(typeof(sideObjectScript)), intVar, scr, go.transform);
}
}
}
}
}

}
