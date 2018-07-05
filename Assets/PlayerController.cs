namespace Moitho.Controllers
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Moitho.Utils;
    using System;

    public class PlayerController : MonoBehaviour {

        public GameObject playerPrefab;
        public float clickRange = 1f;

        private Map.MapGenerator map;

        private bool setupLock = false;

        private GameObject player;
        private bool stopUpdate = false;
        private bool playerPosUpdating = false;
        private ParticleSystem particles;

	    // Use this for initialization
	    void Start () {
            map = GameObject.FindGameObjectWithTag("GameController").GetComponent<Map.MapGenerator>();
	    }

        private void Update()
        {
            if (map.IsMapReady && !setupLock)
            {
                setupLock = true;
                StartCoroutine(PlayerSetup());
            }
                
        }

        private IEnumerator PlayerSetup()
        {
            Vector3 spawnPos = map.LatLonToWorldPos(GPS.Instance.Location);
            
            player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
            particles = player.GetComponentInChildren<ParticleSystem>();
            ParticleSystem.ShapeModule shapeModule = particles.shape;
            shapeModule.radius = clickRange;
            CameraController.Instance.StartToFollowPlayer(player);

            StartCoroutine(UpdatePlayerPosition());
            yield break;
        }

        private IEnumerator UpdatePlayerPosition()
        {
            float updateTime = GPS.Instance.locationUpdateFrequency;
            Vector3 oldPos = player.transform.position;
            while(!stopUpdate)
            {
                yield return new WaitForSecondsRealtime(updateTime);
                Vector3 newPos = map.LatLonToWorldPos(GPS.Instance.Location);
                if (newPos != oldPos && !playerPosUpdating)
                    StartCoroutine(MovePlayer(newPos, 3f));
            }
        }

        private IEnumerator MovePlayer(Vector3 newPosition, float time)
        {
            playerPosUpdating = true;
            float elapsedTime = 0;
            Vector3 startingPos = player.transform.position;
            while (elapsedTime < time)
            {
                player.transform.position = Vector3.Lerp(startingPos, newPosition, (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            playerPosUpdating = false;
            NeedToUpdateMap();
            yield break;
        }

        private void NeedToUpdateMap()
        {
            if (Mathf.Abs(Vector3.Distance(player.transform.position, map.MapCenterWorldScale)) > Settings.DISTANCE_TO_UPDATE)
            {
                map.UpdateMap(player.transform.position);
            }
                
        }
    }
}

