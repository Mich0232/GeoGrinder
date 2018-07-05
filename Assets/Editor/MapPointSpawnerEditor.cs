using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Moitho.Utils;

[CustomEditor(typeof(MapPointSpawner))]
public class MapPointSpawnerEditor : Editor {

    private MapPointSpawner pointSpawner;

    public void OnEnable()
    {
        pointSpawner = (MapPointSpawner)target;
        SceneView.onSceneGUIDelegate = UpdateSpawner;
        
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginVertical();
        foreach(Vector3 cord in pointSpawner.GetCords())
        {
            GUILayout.Label(cord.ToString());
        }
        GUILayout.EndVertical();

        SceneView.RepaintAll();
    }

    void UpdateSpawner(SceneView sceneView)
    {
        Event e = Event.current;

        if (e.isKey && e.character == Settings.SPAWN_POINT_KEY)
        {
            //Debug.Log("S - Pressed");
            RaycastHit _hit;
            Ray ray = Camera.current.ScreenPointToRay(e.mousePosition);
            Physics.Raycast(ray, out _hit);
            Debug.Log(_hit.point);
            pointSpawner.AddCords(_hit.point);
        }
    }
}
