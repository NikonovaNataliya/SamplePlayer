using UnityEngine;
using System.Collections;

public class RotateBody : MonoBehaviour {

    Transform cam;

    [SerializeField]
    GameObject child;
    [SerializeField]
    GameObject headChild;
    Vector3 _axis;
    public Vector3 orientationFix = new Vector3(90, 0, 0);
    public Vector3 orientationFixHead = new Vector3(110, 0, 0);
    Vector3 startCam;
    CharacterController controller;
    Animator anim;
    //public Transform sphere;
    GameObject bot;
    float h, v;
    bool triggBot;
    Vector3 direction;
    public float speed, speedSideward, speedRun, speedRot;

    void Start() {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        // bot = GameObject.FindGameObjectWithTag("Bot");
        cam = Camera.main.transform;
        startCam = cam.transform.eulerAngles;
        _axis = transform.InverseTransformDirection(transform.up);
    }
    void LateUpdate() {

        ChildRot();
        Move();

    }
    void FixedUpdate() {

        GeroyAnimation();

    }

    void Update() {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
    }
    void ChildRot() {

        float angle = Quaternion.Angle(transform.rotation, cam.transform.rotation);
        float cc = cam.rotation.eulerAngles.y;
        // Debug.Log("YYYYYYYYYYYYY" + cc);

        if ((angle < 40 && 180 - cc > 0 && cc < 320) || (angle < 40 && (180 - cc) < 0 && cc > 40)) {
            Vector3 up = transform.InverseTransformDirection(cam.transform.position - headChild.transform.position);
            Quaternion rotation = Quaternion.LookRotation(_axis, up) * Quaternion.Euler(orientationFixHead);
            headChild.transform.localRotation = rotation;

        }

        if (angle < 90 && angle > 40) {
            Vector3 different = Quaternion.FromToRotation(cam.transform.position, headChild.transform.position).eulerAngles;
            Debug.Log("different : " + different);
            Vector3 different1 = different.normalized;
            Debug.Log("differentLocal :" + different1);
            Vector3 up = transform.InverseTransformDirection(cam.transform.position  - child.transform.position);
            Debug.Log(up);
            Vector3 camUp = transform.InverseTransformDirection(cam.transform.position);
            Quaternion rotation = Quaternion.LookRotation(_axis, up) * Quaternion.Euler(orientationFix);
            child.transform.localRotation = rotation;

        }

        //else {
        //    Vector3 ch = new Vector3(0, child.transform.localRotation.eulerAngles.y, 0);
        //    Vector3 tr = new Vector3(0, transform.rotation.eulerAngles.y, 0);
        //    Quaternion rotation = Quaternion.FromToRotation(ch, tr);
        //    child.transform.localRotation = Quaternion.Slerp(child.transform.localRotation, rotation, speedRot * Time.deltaTime);
        //}



        //    Vector3 tr = transform.rotation.eulerAngles;
        //   // Debug.Log("transform : " + tr);
        //    Vector3 childTr = child.transform.rotation.eulerAngles;
        //   // Debug.Log("child :" + childTr);
        //    float angle = Quaternion.Angle(transform.rotation, cam.transform.rotation);
        //    Debug.Log(angle);

        //    Quaternion Q = Quaternion.FromToRotation(transform.forward, cam.forward);
        //    if (angle < 50)
        //         child.transform.localRotation = Quaternion.Euler(0, angle, 0);

        //        //  child.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.up);
        //        // else if (angle > 310)
    }
    /*
        void ChildRot() {
            Quaternion q = Quaternion.FromToRotation(transform.forward, cam.forward);
            Vector3 rot = q.eulerAngles;
            rot = child.transform.InverseTransformDirection(rot);
           // Vector3 deltaAngle = ;
            Debug.Log(rot);
            if (rot.y < 50 || rot.y > 310)
                child.transform.localRotation = transform.rotation * q;


           // else if (rot.y < 90 || rot.y > 270) {
          //      child.transform.localEulerAngles = rot.y < 180 ? new Vector3(0, 30, 0) : new Vector3(0, 310, 0);
          //      headChild.transform.localRotation = q * transform.rotation;
          //  }

           // else if (rot.y > 90 || rot.y < 270) {
               // currentChild = child.transform.rotation.eulerAngles;
              //  currentChild = new Vector3(child.transform.rotation.eulerAngles.x, 0,child.transform.rotation.eulerAngles.z);
              //  child.transform.localRotation = transform.localRotation;


              //  child.transform.rotation = Quaternion.Slerp(child.transform.rotation, Quaternion.LookRotation(transform.up -
               //                                                       child.transform.position), Time.deltaTime * speedRot);



                //if (h != 0 || v != 0) {
                //    child.transform.rotation = transform.rotation;
                //}
          //  }

        }

    */

    void Move() {
        //вектор направления движения
        direction = new Vector3(h, 0, v);
        direction = cam.TransformDirection(direction);
        direction = new Vector3(direction.x, 0, direction.z);
        //  Debug.Log(direction);
        Vector3 dir = transform.rotation.eulerAngles;
        // Debug.Log("////////////////" + dir);

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
        if ((v != 0 && h == 0) || (v != 0 && h != 0))
            controller.Move(direction * speed * Time.deltaTime);
        if (h != 0 && v == 0)
            controller.Move(direction * speedSideward * Time.deltaTime);

        if (Input.GetKey(KeyCode.X))
            controller.Move(direction * speedRun * Time.deltaTime);


    }

    void GeroyAnimation() {

        anim.SetFloat("Vertical", v, 0.01f, Time.fixedDeltaTime);
        anim.SetFloat("Horizontal", h, 0.01f, Time.fixedDeltaTime);
        if (h != 0 || v != 0)
            anim.SetBool("Move", true);
        else anim.SetBool("Move", false);

        //if (v == 0 && h != 0) {
        //    anim.SetBool("Sideward", true);
        //    anim.SetBool("Move", false);
        //}
        //if (v != 0 && h == 0) {
        //    anim.SetBool("Sideward", false);
        //    anim.SetBool("Move", true);
        //}
        //if (v == 0 && h == 0) {
        //    anim.SetBool("Sideward", false);
        //    anim.SetBool("Move", false);
        //}
        //if (v != 0 && h != 0) {
        //    anim.SetBool("Sideward", false);
        //    anim.SetBool("Move", true);
        //}

        if (Input.GetMouseButton(0)) {
            anim.SetBool("Attack", true);
        }
        else anim.SetBool("Attack", false);

        if (Input.GetKey(KeyCode.X)) {
            anim.SetBool("Run", true);
        }
        else anim.SetBool("Run", false);
    }
    //    void OnAnimatorIK() {
    //        if(triggBot == true){
    //          anim.SetLookAtWeight(0.5f);
    //          anim.SetLookAtPosition(bot.transform.position );
    //}

    //       // anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
    //       // anim.SetIKPosition(AvatarIKGoal.LeftFoot, transform.position + transform.forward);
    //    }

    //    void OnTriggerStay(Collider col){
    //        if(col == GameObject.FindGameObjectWithTag("Bot"))
    //           triggBot = true;
    //        Debug.Log("!!!");
    //}
}
