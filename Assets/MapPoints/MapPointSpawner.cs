using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPointSpawner : MonoBehaviour {

    private readonly List<Vector3> mapPointsCords = new List<Vector3>();

	public void AddCords(Vector3 cords)
    {
        mapPointsCords.Add(cords);
    }

    public List<Vector3> GetCords()
    {
        return mapPointsCords;
    }
}
