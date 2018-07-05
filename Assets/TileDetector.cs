namespace Moitho.Utils
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Moitho.Map;

    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class TileDetector : MonoBehaviour {

        public static TileDetector Instance;

        public Vector3 CurrentTile;

        private void Start()
        {
            Instance = this;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.CompareTag("Tile"))
            {
                CurrentTile = other.gameObject.transform.position;
            }
        }

    }

}
