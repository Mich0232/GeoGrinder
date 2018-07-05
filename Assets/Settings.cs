namespace Moitho.Utils
{
    using UnityEngine;

    public static class Settings 
    {
        public static string API_KEY {
            private set { }
            get { return "AIzaSyBw6oFYnxCBZSoRVRC4uATQ0sr2RzZijDo"; }
        }

        public static int GPS_TIMEOUT_LIMIT {
            private set { }
            get { return 20; }
        }

        public static int TIMEOUT_LIMIT {
            private set { }
            get { return GPS_TIMEOUT_LIMIT + 1; }
        }

        public static float DISTANCE_TO_UPDATE
        {
            private set { }
            get { return 5f; }
        }

        public static char SPAWN_POINT_KEY {
            get { return 's'; }
            private set { }
        }

        public static string ASSETS_PATH
        {
            private set { }
            get
            {
                if(Application.platform == RuntimePlatform.Android)
                {
                    return "jar:file://" + Application.dataPath + "!/assets";
                }
                else if(Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    return Application.dataPath + "/Raw";
                }
                else
                {
                    return Application.dataPath + "/StreamingAssets";
                }
            }
        }
    }
}
