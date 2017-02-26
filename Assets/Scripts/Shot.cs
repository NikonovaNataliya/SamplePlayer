using UnityEngine;
using System.Collections;

public class Shot : MonoBehaviour {


    public void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player") {

            other.GetComponent<xxxMoving>().health -= 1;
        }
    }
}
