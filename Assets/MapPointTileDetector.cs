using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPointTileDetector : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Tile"))
        {
            gameObject.transform.parent.SetParent(other.gameObject.transform);
            Destroy(gameObject, 1f);
        }
    }
}
