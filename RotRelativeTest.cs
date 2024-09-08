using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotTest : MonoBehaviour
{
	public GameObject a;
	public GameObject b;
	
    // Start is called before the first frame update
    void Start()
    {
	    Quaternion r = a.transform.rotation;
	    Quaternion q = b.transform.rotation;
	    
	    Quaternion k = q * Quaternion.Inverse(r);
	    
	    Vector3 dir = k.eulerAngles;
	    
	    Debug.Log("angle=" + dir.x + " " + dir.y + " " + dir.z);
    
	    Vector3 d = k * Vector3.forward;
	    
	    Debug.Log("vec=" + d.x + " " + d.y + " " + d.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
