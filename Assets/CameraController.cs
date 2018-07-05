namespace Moitho.Controllers
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CameraController : MonoBehaviour {

        public static CameraController Instance;

        [Tooltip("Delay between camera position update")]
        [Range(0.5f, 3f)]
        public float cameraUpdatedFrequency = 1f;
        private Transform player;
        private bool playerObjIsSet = false;

        private Vector3 oldPos;
    
        private void Awake()
        {
            Instance = this;
        }

        public void StartToFollowPlayer(GameObject player)
        {
            this.player = player.transform;
            playerObjIsSet = true;
            StartCoroutine(FollowPlayer());
        }

        private IEnumerator FollowPlayer()
        {
            while (playerObjIsSet) {
                yield return new WaitForSecondsRealtime(0.01f);
                    Vector3 newPos = new Vector3(player.position.x, 0, player.position.z);
                    if (newPos != oldPos)
                        transform.position = newPos;
            }
        }

    }

}
