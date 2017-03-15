using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAnimation : MonoBehaviour {

    Animator anim;
    int ind;
    float v, h;
    public bool isFall = true;

    CharMotor charMotor;
    HashID nameHashState;
    AnimatorStateInfo stateInfo;

    void Start () {
        anim = GetComponent<Animator>();
        charMotor = GetComponent<CharMotor>();
        nameHashState = GetComponent<HashID>();
    }

	void Update () {
        GeroyAnimation();
        v = GetComponent<CharController>().vert;
        h = GetComponent<CharController>().horiz;

    }

    void GeroyAnimation() {

        anim.SetFloat("Vertical", v, 0.01f, Time.fixedDeltaTime);
        anim.SetFloat("Horizontal", h, 0.01f, Time.fixedDeltaTime);

        if (v == 0 && h != 0) {
            anim.SetBool("Sideward", true);
            anim.SetBool("Move", false);
        }
        if (v != 0 && h == 0) {
            anim.SetBool("Sideward", false);
            anim.SetBool("Move", true);
        }
        if (v == 0 && h == 0) {
            anim.SetBool("Sideward", false);
            anim.SetBool("Move", false);
        }
        if (v != 0 && h != 0) {
            anim.SetBool("Sideward", false);
            anim.SetBool("Move", true);
        }
        ind = Random.Range(0, 7);
        if (Input.GetMouseButton(0)) {
            anim.SetFloat("Attack", ind);
            anim.SetBool("Attack1", true);
        }
        else anim.SetBool("Attack1", false);

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButton(1)) {
            anim.SetBool("RightKick", true);
            anim.SetBool("Attack1", false);
        }

        else anim.SetBool("RightKick", false);

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButton(0)) {
            anim.SetBool("LeftKick", true);
            anim.SetBool("Attack1", false);
        }

        else anim.SetBool("LeftKick", false);

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.X) && Input.GetKeyDown(KeyCode.LeftControl))
            anim.SetBool("Trip", true);
        else anim.SetBool("Trip", false);

        if (Input.GetKey(KeyCode.X)) {
            anim.SetBool("Run", true);
        }
        else anim.SetBool("Run", false);

        int layerMaskWalls = 1 << 9;
        Debug.DrawRay(transform.position + transform.up, transform.forward * 0.6f, Color.red);
 

        if (Physics.Raycast(transform.position + transform.up, transform.forward, 0.85f,layerMaskWalls) 
                                     && charMotor._horizontalVelocity > 5.0f) {
            anim.SetBool("Fall", true);
        }
        else {
            anim.SetBool("Fall", false);
        }
        if (stateInfo.fullPathHash == nameHashState.idleState) {// я хотела определить, находится ли он в состоянии "упал"
            isFall = true;                                      // чтобы по булю останавливать его в CharController
            Debug.Log("//////////////////");                    // но - не получается
        }

        if (Input.GetKey(KeyCode.Space)) {
            anim.SetBool("Jump", true);
        }        
        else anim.SetBool("Jump", false);
    }
}
