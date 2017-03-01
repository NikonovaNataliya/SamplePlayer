using UnityEngine;
using System.Collections;

public class RotateBody : MonoBehaviour {

    [SerializeField]
    private GameObject child;
    [SerializeField]
    private GameObject headChild;
    [SerializeField]
    private Transform m_leftRotationLimit;
    [SerializeField]
    private Transform m_rightRotationLimit;

    public Vector3 orientationFix = new Vector3(90, 0, 0);
    public Vector3 orientationFixHead = new Vector3(110, 0, 0);
    private Vector3 _axis;

    CharacterController controller;
    Animator anim;
    Transform cam;
    Vector3 direction;
    float h, v;
    public float speed, speedSideward, speedRun, speedRot;
    //public Transform sphere;
    //GameObject bot;
    //bool triggBot;


    void Start() {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        // bot = GameObject.FindGameObjectWithTag("Bot");
        cam = Camera.main.transform;
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

    private Transform _GetLimitedCameraTransform() {

        float angleCamera = Quaternion.Angle(transform.rotation, cam.transform.rotation);

        float angleLeftLimit = Quaternion.Angle(transform.rotation, m_leftRotationLimit.rotation);
        float angleRighrLimit = Quaternion.Angle(transform.rotation, m_rightRotationLimit.rotation);
        Debug.Log(angleCamera);
        if (angleCamera < angleLeftLimit && angleCamera < angleRighrLimit) {
            return cam.transform;
        }

        float angleCameraToLeftLimit = Quaternion.Angle(cam.transform.rotation, m_rightRotationLimit.rotation);
        float angleCameraToRightLimit = Quaternion.Angle(cam.transform.rotation, m_leftRotationLimit.rotation);

        if (angleCameraToLeftLimit < angleCameraToRightLimit) {
            return m_rightRotationLimit;
        }
        else {
            return m_leftRotationLimit;
        }
    }
    void ChildRot() {

        Transform cameraLimitedTransform = _GetLimitedCameraTransform();

        Vector3 up = transform.InverseTransformDirection(cameraLimitedTransform.position - headChild.transform.position);

        Quaternion rotation = Quaternion.LookRotation(_axis, up) * Quaternion.Euler(orientationFixHead);
        headChild.transform.localRotation = rotation;

        Vector3 UP = transform.InverseTransformDirection(cameraLimitedTransform.position - child.transform.position);
        Quaternion rotate = Quaternion.LookRotation(_axis, UP) * Quaternion.Euler(orientationFix);

        child.transform.localRotation = rotate;
    }

    void Move() {
        //вектор направления движения
        direction = new Vector3(h, 0, v);
        direction = cam.TransformDirection(direction);
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
        //if (h > 0 && v == 0) {
        //    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.right, direction),
        //                                                                                     speedRot * Time.deltaTime);
        //}
        //if (h < 0 && v == 0) {
        //    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.left, direction),
        //                                                                                     speedRot * Time.deltaTime);
        //}
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
