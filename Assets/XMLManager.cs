namespace Moitho.Serialization
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Xml;
    using System.Xml.Serialization;
    using System.IO;
    using System;
    using Moitho.Utils;
    using UnityEngine.UI;

    public class XMLManager : MonoBehaviour  {

        public static XMLManager Instance;
        public Text debugText;

        private MapPointDataBase mapPointDB;

        private static string XML_DIR = "/XML/";

        public bool DataFileIsRead
        {
            get { return fileread; }
        }
        bool fileread = false;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {

             StartCoroutine(PrepareDataFile("map_points"));

        }

        public void LoadFile(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MapPointDataBase));
            FileStream stream = new FileStream(Application.dataPath + XML_DIR + filename + ".xml", FileMode.Open);
            mapPointDB = serializer.Deserialize(stream) as MapPointDataBase;
            stream.Close();
            fileread = true;
        }

        private IEnumerator PrepareDataFile(string filename)
        {
            debugText.text = Application.platform.ToString();

            if (Application.platform == RuntimePlatform.Android)
            {
                TextAsset textAsset = (TextAsset)Resources.Load(filename, typeof(TextAsset));
                XmlSerializer serializer = new XmlSerializer(typeof(MapPointDataBase));
                Stream stream = new MemoryStream(textAsset.bytes);
                StreamReader textReader = new StreamReader(stream);

                mapPointDB = (MapPointDataBase)serializer.Deserialize(textReader);
                stream.Dispose();
                fileread = true;
            }
            else
                LoadFile(filename);

            yield break;
        }

        public void CreateNewFile(string name)
        {
            FileStream stream = new FileStream(Application.dataPath + "/StreamingAssets/XML/" + name + ".xml", FileMode.Create);
            stream.Close();
        }

        public void SaveFile(string name)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MapPointDataBase));
            FileStream stream = new FileStream(Application.dataPath + "/StreamingAssets/XML/" + name + ".xml", FileMode.Open);
            serializer.Serialize(stream, mapPointDB);
            stream.Close();
        }

        public void AddMapPointToDB(MapPointEntry newItem)
        {
            mapPointDB.list.Add(newItem);
        }

        public List<MapPointEntry> GetMapPointObjects()
        {
            return mapPointDB.list;
        }

    }

    [System.Serializable]
    public class MapPointEntry
    {
        public int id;
        public Vector2 latlon;
        public string label;

        public MapPointEntry() { }

        public MapPointEntry(int id, Vector2 latlon, string label)
        {
            this.id = id;
            this.latlon = latlon;
            this.label = label;
        }
    }

    [System.Serializable]
    public class MapPointDataBase
    {
        public List<MapPointEntry> list = new List<MapPointEntry>();
    }

}

