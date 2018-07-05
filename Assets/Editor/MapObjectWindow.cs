using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Moitho.Map;
using Moitho.Serialization;

public class MapObjectWindow : EditorWindow {

    Vector2 latLon = new Vector2(0, 0);
    MapPointObject prefab;
    string object_name = "New Map Point";
    string filename = "New File";
    string filename_to_save = "";


    [MenuItem("Window/MapObjects")]
	public static void ShowWindow()
    {
        GetWindow<MapObjectWindow>("Map Objects");
    }


    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Create new File");
        filename = EditorGUILayout.TextField("Filename", filename);
        if (GUILayout.Button("Create"))
        {
            XMLManager.Instance.CreateNewFile(filename);
        }

        EditorGUILayout.LabelField("Add new MapPoint");
        latLon = EditorGUILayout.Vector2Field("Lat Lon", latLon);
        prefab = (MapPointObject)EditorGUILayout.ObjectField(prefab, typeof(MapPointObject), false);
        object_name = EditorGUILayout.TextField("Object name", object_name);

        if(GUILayout.Button("Add"))
        {
            if (latLon != Vector2.zero && prefab != null)
                XMLManager.Instance.AddMapPointToDB(new MapPointEntry(prefab.OBJECT_ID, latLon, object_name));
        }

        EditorGUILayout.LabelField("Save to file");
        filename_to_save = EditorGUILayout.TextField("Save File", filename_to_save);
        if (GUILayout.Button("Save"))
        {
            if(filename_to_save != "")
                XMLManager.Instance.SaveFile(filename_to_save);
        }
            EditorGUILayout.EndVertical();
    }
}
