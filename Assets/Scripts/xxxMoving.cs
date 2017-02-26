using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class xxxMoving : MonoBehaviour {

    CharacterController controller;
    Animator anim;
    public GameObject geroyPrefab;
    public Transform sphere;
    public float speed, speedSideward, speedRun, speedRot;
    public Slider slider;
    Vector3 direction, dir;
    Quaternion cameraCurrentRotation;
    float h, v;
    bool trig = false;
    public int health = 10;
    public Text text;
    public GameObject child;
    Transform cam;

    void Start () {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        text.enabled = false;
        child = this.gameObject.transform.GetChild(0).GetChild(0).GetChild(2).gameObject;

    }

    void Update() {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
    }

    void FixedUpdate() {
        Move();
        GeroyAnimation();
        GeroyHealth();
    }

     void GeroyHealth() {
        slider.value = health;
        if(health <= 0) {
            Death();
            GameObject.FindGameObjectWithTag("Bot").GetComponent<Animator>().Stop();
        }
    }
    void Death() {
        gameObject.SetActive(false);
        Instantiate(geroyPrefab, transform.position, transform.rotation);
        text.enabled = true;

    }


    void Move() {
        //вектор направления движения
        direction = new Vector3(h, 0, v);
        direction = Camera.main.transform.TransformDirection(direction);
        direction = new Vector3(direction.x, 0, direction.z);
        cameraCurrentRotation = Camera.main.transform.rotation;
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
            transform.rotation = Quaternion.Lerp(transform.rotation, cameraCurrentRotation, 
                                                                  speedRot * Time.deltaTime);
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
