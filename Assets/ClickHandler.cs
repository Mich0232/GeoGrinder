namespace Moitho.Map
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class ClickHandler : MonoBehaviour {

        public ParticleSystem particle;

        private void Awake()
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
        }

        void OnTouchUp()
        {
            print(gameObject.name + " Clicked");
            Instantiate(particle, transform.position, particle.transform.rotation, transform);                     
        }
    }

}
