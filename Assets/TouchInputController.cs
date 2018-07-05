namespace Moitho.Controllers
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;


    public class TouchInputController : MonoBehaviour {

        public LayerMask TouchInput;
        public Camera mainCam;

	
	    // Update is called once per frame
	    void Update () {

        #if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
                RaycastHit _hit;

                if(Physics.Raycast(ray, out _hit, 200f, TouchInput))
                {
                    _hit.collider.gameObject.SendMessage("OnTouchUp", SendMessageOptions.DontRequireReceiver);
                }
            }
        #endif
            if (Input.touchCount > 0)
            {
               
                foreach(Touch touch in Input.touches)
                {
                    Vector3 scrPoint = new Vector3(touch.position.x, touch.position.y, 0);
                    Ray ray = mainCam.ScreenPointToRay(scrPoint);
                    RaycastHit _hit;

                    if(Physics.Raycast(ray, out _hit, TouchInput))
                    {
                        GameObject target = _hit.collider.gameObject;

                        if(touch.phase == TouchPhase.Ended)
                        {
                            target.SendMessage("OnTouchUp", SendMessageOptions.DontRequireReceiver);
                        }
                    }
                }
            }

	    }
    }

}
