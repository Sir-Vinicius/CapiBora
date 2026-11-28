using UnityEngine;

public class Billboard : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
        //private Camera cam;
        void Start()
        {
            //cam = Camera.main;
        }

        void LateUpdate()
        {
            //Vector3 direction = cam.transform.position - transform.position;
            //transform.forward = -direction;
    
            transform.rotation = Quaternion.identity;
        }
    
}

