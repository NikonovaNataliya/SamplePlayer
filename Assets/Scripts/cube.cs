using UnityEngine;
using System.Collections;

public class cube : MonoBehaviour {
    public float speed;
    public Transform child;

	void Start () {
        
	}
	

	void Update () {
        Vector3 dir = transform.forward - child.transform.forward;
        //Quaternion rotation = Quaternion.LookRotation(dir);
        Quaternion rotation = Quaternion.FromToRotation(child.transform.forward, transform.forward);
        child.transform.localRotation = Quaternion.Lerp(child.transform.localRotation, rotation, speed * Time.deltaTime);

	}
}
