namespace Moitho.Controllers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Moitho.Map;
    using Moitho.Serialization;
    using UnityEngine;

    public class MapPointController : MonoBehaviour {

        public static MapPointController Instance;


        private MapGenerator map;
        private bool setupLock;
        public string sourceFilename = "map_points";
        public bool loadMapObjects = false;

        public List<MapPointObject> mapPointPrefabs = new List<MapPointObject>();
        private List<MapPointEntry> mapObjectsToSpawn;
        private GameObject mapObjectParent;

        // Use this for initialization
        void Start()
        {
            Instance = this;
            map = GameObject.FindGameObjectWithTag("GameController").GetComponent<Map.MapGenerator>();
            mapObjectParent = new GameObject("MapObjects");
            StartCoroutine(LoadObjectsFromFile());
        }

        private IEnumerator LoadObjectsFromFile()
        {
            while(!XMLManager.Instance.DataFileIsRead)
                yield return new WaitForSecondsRealtime(0.1f);

            //XMLManager.Instance.LoadFile(sourceFilename);
            mapObjectsToSpawn = XMLManager.Instance.GetMapPointObjects();
            yield break;
        }

        public IEnumerator SpawnMapObjects()
        {
            yield return new WaitForSecondsRealtime(1f);
            if (loadMapObjects)
            {
                Vector2 h_Bounds = map.HorizontalBounds;
                Vector2 v_Bounds = map.VerticalBounds;
                List<MapPointEntry> toRemove = new List<MapPointEntry>();

                foreach(MapPointEntry mapPoint in mapObjectsToSpawn)
                {
                    Vector3 objectPos = map.LatLonToWorldPos(mapPoint.latlon);

                    if (CheckIfInActiveArea(objectPos, h_Bounds, v_Bounds))
                    {
                        print("Spawning");
                        Vector3 spawnPos = map.LatLonToWorldPos(mapPoint.latlon);
                        spawnPos.y = 0.5f;
                        Instantiate(IDToMapPointPrefab(mapPoint.id), spawnPos, Quaternion.identity);
                        toRemove.Add(mapPoint);
                    }
                    else
                        print(mapPoint.label + " :Out of bounds");
                    
                }

                foreach(MapPointEntry point in toRemove)
                {
                    mapObjectsToSpawn.RemoveAll(x => x.label == point.label);
                }
                toRemove.Clear();
            }
        }

        private bool CheckIfInActiveArea(Vector3 objectPos, Vector2 horizontalBounds, Vector2 verticalBounds)
        {
            if (objectPos.x >= horizontalBounds.x && objectPos.x <= horizontalBounds.y)
                if (objectPos.z >= verticalBounds.x && objectPos.z <= horizontalBounds.y)
                    return true;
            return false;
        }

        private GameObject IDToMapPointPrefab(int ID)
        {
            if (mapPointPrefabs.Exists(x => x.OBJECT_ID == ID))
                return mapPointPrefabs.Find(x => x.OBJECT_ID == ID).prefab;
            return null;
        }
    }

}
