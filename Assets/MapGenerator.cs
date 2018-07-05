namespace Moitho.Map
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using Moitho.Utils;
    using Moitho.Controllers;
    using System;

    public class MapGenerator : MonoBehaviour {

        public Material tileMaterial;
        public GameObject tilePrefab;
        public Vector2Int mapSize = new Vector2Int(3, 3);


        public bool useGPS = true;
        private float latitude;
        private float longitude;
        private int zoom = 16;
        private int mapHeight = 1280;
        private int mapWidth = 1280;
        public MapType mapType;
        private int scale = 1;

        private GameObject mapParent;

        private float factor_lon = 0.01372f;
        private float factor_lat = 0.00881f;
        private int tile_step = 10;

        private float X_Min_Bound;
        private float X_Max_Bound;

        private float Y_Min_Bound;
        private float Y_Max_Bound;

        public Vector2 HorizontalBounds
        {
            private set { }
            get { return new Vector2(X_Min_Bound, X_Max_Bound); }
        }

        public Vector2 VerticalBounds
        {
            private set { }
            get { return new Vector2(Y_Min_Bound, Y_Max_Bound); }
        }

        private MapStatus mapStatus = MapStatus.empty;
        public bool IsMapReady
        {
            private set { }
            get { return mapStatus == MapStatus.ready;}
        }

        private Vector2 centerLatLon;
        public Vector3 MapCenterWorldScale;

        //private List<TileData> tiles = new List<TileData>();
        private List<TileData> activeTiles = new List<TileData>();
        private List<TileData> deactivatedTiles = new List<TileData>();

        public Vector3 LatLonToWorldPos(Vector2 latlon)
        {
            float lat_pos = ((centerLatLon.x - latlon.x) / factor_lat) * tile_step;
            float lon_pos = ((centerLatLon.y - latlon.y) / factor_lon) * tile_step;

            return new Vector3(lon_pos, 0, lat_pos);
        }

        private void SetUpLatLon()
        {
            if(useGPS)
            {
                Vector2 pos = GPS.Instance.Location;
                latitude = pos.x;
                longitude = pos.y;
                centerLatLon = pos;
            }
        }

        private IEnumerator WaitForGPS()
        {
            int timeout = Settings.TIMEOUT_LIMIT;
            while(!GPS.Instance.IsWorking && timeout > 0)
            {
                yield return new WaitForSecondsRealtime(1f);
                timeout--;
            }

            if(timeout > 0 || GPS.Instance.IsWorking)
            {
                SetUpLatLon();
                StartCoroutine(RenderTileMap(latitude, longitude));
            }
            
            yield break;
        }

        public void UpdateMap(Vector3 playerPos)
        {
            MapCenterWorldScale = TileDetector.Instance.CurrentTile;
            List<TileData> tilesToRemove = new List<TileData>();
            float limit = (tile_step * Mathf.Sqrt(2));
            // Deactivate tile that are away
            foreach(TileData tileData in activeTiles)
            {
                if(Mathf.Abs(Vector3.Distance(tileData.worldPos, MapCenterWorldScale)) > limit)
                {
                    tileData.Deactivate();
                    deactivatedTiles.Add(tileData);
                    tilesToRemove.Add(tileData);
                }
            }

            foreach(TileData tlData in tilesToRemove)
            {
                activeTiles.RemoveAll(x => x.Tile == tlData.Tile);
            }

            List<Vector3> missingPos = CheckIfNearTilesAreCached();
            StartCoroutine(RenderTileByMissingPos(missingPos));
           
        }

        private void Start()
        {
            InitMapParent();
            StartCoroutine(WaitForGPS());
        }

        private void InitMapParent()
        {
            mapParent = new GameObject("TileMap");
            MapCenterWorldScale = Vector3.zero;
        }

        private IEnumerator RenderTileMap(float startLat, float startLon)
        {
            mapStatus = MapStatus.rendering;
            SetUpLatLon();
            // Parent
            
            for(int j = -1; j < (mapSize.x - 1); j++)
            {
                float lat_offset = factor_lat * j;
                for(int i = -1; i < (mapSize.y - 1);i++)
                {
                    
                    float lon_offset = factor_lon * i;  
                    
                    float lat = startLat + lat_offset;
                    float lon = startLon + lon_offset;
                    
                    WWW request = new WWW(GetURLForLatLon(lat, lon));

                    yield return request;

                    int x = tile_step * -i;
                    int y = tile_step * -j;
                    CreateNewTileAt(x, y, request.texture, new Vector2(lat, lon), lat_offset, lon_offset);
                }      
            }
            UpdateBounds();
            mapStatus = MapStatus.ready;
            StartCoroutine(MapPointController.Instance.SpawnMapObjects());

        }

        private void UpdateBounds()
        {
            Vector3 initBounds = activeTiles[0].worldPos;
            X_Min_Bound = initBounds.x;
            X_Max_Bound = initBounds.x;
            Y_Min_Bound = initBounds.z;
            Y_Max_Bound = initBounds.z;
            float side_factor = tile_step / 2f;
            foreach(TileData tile in activeTiles)
            {
                float x = tile.worldPos.x;
                float y = tile.worldPos.z;
                if(x > 0)
                {
                    if (x + side_factor > X_Max_Bound)
                        X_Max_Bound = x + side_factor;
                }
                else
                {
                    if (x - side_factor < X_Min_Bound)
                        X_Min_Bound = x - side_factor;
                }

                if(y > 0)
                {
                    if (y + side_factor > Y_Max_Bound)
                        Y_Max_Bound = y + side_factor;
                }
                else
                {
                    if (y - side_factor < Y_Min_Bound)
                        Y_Min_Bound = y - side_factor;
                }
            }
            

        }

        private void CreateNewTileAt(int x, int y, Texture mapImage, Vector2 latlon, float lat_offset, float lon_offset)
        {
            GameObject newTile = Instantiate(tilePrefab, new Vector3(x, 0, y), Quaternion.identity, mapParent.transform);
            newTile.name = "Tile_" + x + "_" + y;
            Material material = new Material(tileMaterial);
            material.mainTexture = mapImage;
            newTile.GetComponent<Renderer>().material = material;

            TileData tileData = new TileData(newTile, material, new Vector3(x, 0, y), latlon, lat_offset, lon_offset, true);

            activeTiles.Add(tileData);
        }

        private string GetURLForLatLon(float lat, float lon)
        {
            return "https://maps.googleapis.com/maps/api/staticmap?center=" + lat + "," + lon + "&zoom=" + zoom + "&size=" + mapWidth + "x" + mapHeight + "&scale=" + scale + "&maptype=" + mapType + "&key=" + Settings.API_KEY;
        }

        private List<Vector3> CheckIfNearTilesAreCached()
        {
            Vector3 mapCenter = MapCenterWorldScale;
            List<Vector3> notFoundTilesPosition = new List<Vector3>();
            float limit = (tile_step * Mathf.Sqrt(2));
            for(int i = -1;i<2;i++)
            {
                for(int j = -1; j<2;j++)
                {
                    int x = tile_step * -i;
                    int y = tile_step * -j;
                    Vector3 testPos = new Vector3((int)mapCenter.x + x, 0, (int)mapCenter.z + y);
                    if(!activeTiles.Exists(test => test.worldPos == testPos))
                    {
                        if(deactivatedTiles.Exists(test => test.worldPos == testPos))
                        {
                            TileData tile = deactivatedTiles.Find(test => test.worldPos == testPos);                     
                            tile.Activate();
                            activeTiles.Add(tile);
                            deactivatedTiles.Remove(tile);
                        }
                        else
                        {
                            notFoundTilesPosition.Add(testPos);
                        }                      
                    }
                }
            }
            return notFoundTilesPosition;
        }

        // TODO: Performance upgrade
        private IEnumerator RenderTileByMissingPos(List<Vector3> missingTiles)
        {
            TileData centerTile = GetTileDataByWorldPos(MapCenterWorldScale);
            foreach(Vector3 pos in missingTiles)
            {
                Vector3 posDiff = ((centerTile.worldPos - pos)/tile_step);
                float lat = centerTile.latLon.x + (posDiff.z * factor_lat);
                float lon = centerTile.latLon.y + (posDiff.x * factor_lon);

                WWW request = new WWW(GetURLForLatLon(lat, lon));

                yield return request;

                CreateNewTileAt((int)pos.x, (int)pos.z, request.texture, new Vector2(lat, lon), (posDiff.x * factor_lat), (posDiff.z * factor_lon));
            }
            UpdateBounds();
            StartCoroutine(MapPointController.Instance.SpawnMapObjects());
            yield break;
        }

        private TileData GetTileDataByWorldPos(Vector3 worldPos)
        {
            if (activeTiles.Exists(test => test.worldPos == worldPos))
                return activeTiles.Find(test => test.worldPos == worldPos);
            return new TileData();
        }
    }

    struct TileData
    {
        public GameObject Tile;
        public Material image;
        public Vector3 worldPos;
        public Vector2 latLon;
        public float lat_offset;
        public float lon_offset;
        public bool isActive;

        public TileData(GameObject tile, Material image, Vector3 worldPos, Vector2 latLon, float lat_offset, float lon_offset, bool active)
        {
            this.Tile = tile;
            this.image = image;
            this.worldPos = worldPos;
            this.latLon = latLon;
            this.lat_offset = lat_offset;
            this.lon_offset = lon_offset;
            this.isActive = active;
        }

        public void Deactivate()
        {
            Tile.SetActive(false);
            isActive = false;
        }

        public void Activate()
        {

             Tile.SetActive(true);
             isActive = true;
            
        }
    }

    public enum MapType
    {
        roadmap,
        satelite,
        hybrid,
        terrain
    }

    enum MapStatus
    {
        empty,
        rendering,
        ready
    }
}

