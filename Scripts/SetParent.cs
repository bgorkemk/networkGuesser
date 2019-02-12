using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetParent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		GameObject obj = GameObject.Find("Canvas");
		gameObject.transform.SetParent(obj.transform);
    }
   
}
