using UnityEngine;
using System.Collections;

public class xxxMoving : MonoBehaviour {

    CharacterController controller;
    Animator anim;
    public Transform sphere;
    public float speed, speedSideward, speedRun, speedRot;
    Vector3 direction;
    float h, v;
    bool trig = false;

	void Start () {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
	}

    void FixedUpdate() {
        Move();
        GeroyAnimation();
    }

    void Update () {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
	}

    void Move() {
        //вектор направления движения
        direction = new Vector3(h, 0, v);
        direction = Camera.main.transform.TransformDirection(direction);
        direction = new Vector3(direction.x, 0, direction.z);
        //разворот в сторону движения
 
        if (v > 0) {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction),
                                                                                 speedRot * Time.deltaTime);
        }
        if (v < 0) {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.back, direction),
                                                                                             speedRot * Time.deltaTime);
        }
        if (h > 0 && v == 0) {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.right, direction),
                                                                                             speedRot * Time.deltaTime);
        }
        if (h < 0 && v == 0) {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.left, direction),
                                                                                             speedRot * Time.deltaTime);
        }
      //  if ((v != 0 && h == 0) || (v != 0 && h != 0))
            controller.Move(direction * speed * Time.deltaTime);
      //  if (h != 0 )
       //     controller.Move(direction * speedSideward * Time.deltaTime);
        if (Input.GetKey(KeyCode.X))
            controller.Move(direction * speedRun * Time.deltaTime);
            
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

        if (Input.GetMouseButton(0)) {
            anim.SetBool("Attack", true);
        }
        else anim.SetBool("Attack", false);

        if (Input.GetKey(KeyCode.X)) {
            anim.SetBool("Run", true);
        }
        else anim.SetBool("Run", false);
    }
    void OnAnimatorIK() {
        if (trig) {
            anim.SetLookAtWeight(0.5f);
            anim.SetLookAtPosition(sphere.position);
        }
          //  anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
          //  anim.SetIKPosition(AvatarIKGoal.RightHand, sphere.position);

    }
     void OnTriggerStay(Collider col) {
        if (col.tag == "Bot") {
            trig = true;
        }
    }
    void OnTriggerExit(Collider col) {
        trig = false;
    }
}
