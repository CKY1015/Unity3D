@CustomEditor(EasyRoads3DTerrainID)
class TerrainEditorScript extends Editor
{
function OnSceneGUI()
{
if(Event.current.shift && RoadObjectScript.ODOODODDQO != null) Selection.activeGameObject = RoadObjectScript.ODOODODDQO.gameObject;
else RoadObjectScript.ODOODODDQO = null;
}
}
