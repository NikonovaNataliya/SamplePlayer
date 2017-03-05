using UnityEngine;
using System.Collections;

public class CharController : MonoBehaviour {

    [SerializeField]
    private Transform m_leftRotationLimit;
    [SerializeField]
    private Transform m_rightRotationLimit;
    [SerializeField]
    private GameObject child;
    [SerializeField]
    private GameObject headChild;

    public static CharacterController unityController;
    public static CharController instance;
    public Vector3 move;
    Vector3 _axis;

    Animator anim;
    private const float _DEAD_ZONE = 0.1f;
    public float v, h, speedRot;
    Vector3 orientationFix = new Vector3(90, 0, 0);
    Vector3 orientationFixHead = new Vector3(110, 0, 0);

    void Awake() {
        instance = this;
        unityController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        _axis = transform.InverseTransformDirection(transform.up);
    }

    void Update() {
        if (Camera.main == null) return;

        GetInput();
        CharMotor.instance.UpdateMotor(move);
       // CharMotor.instance.Moving(move);
        GeroyAnimation();
    }

    void LateUpdate() {

        ChildRot();
    }


    private Transform _GetLimitedCameraTransform() {

        float angleCamera = Quaternion.Angle(transform.rotation, Camera.main.transform.rotation);
        float angleLeftLimit = Quaternion.Angle(transform.rotation, m_leftRotationLimit.rotation);
        float angleRighrLimit = Quaternion.Angle(transform.rotation, m_rightRotationLimit.rotation);

        if (angleCamera < angleLeftLimit && angleCamera < angleRighrLimit) {
            return Camera.main.transform;
        }
        else {
            //return _StartTransform();
            return child.transform;
        }

        //float angleCameraToLeftLimit = Quaternion.Angle(Camera.main.transform.rotation, m_rightRotationLimit.rotation);
        //float angleCameraToRightLimit = Quaternion.Angle(Camera.main.transform.rotation, m_leftRotationLimit.rotation);

        //if (angleCameraToLeftLimit < angleCameraToRightLimit) {
        //    return m_rightRotationLimit;
        //}
        //else {
        //    return m_leftRotationLimit;
        //}
    }
    void ChildRot() {                            // нужно сделать, чтобы поворот в исходную позицию происходил плавно


        Transform cameraLimitedTransform = _GetLimitedCameraTransform();

        Vector3 up = transform.InverseTransformDirection(cameraLimitedTransform.position - headChild.transform.position);
        Quaternion rotation = Quaternion.LookRotation(_axis, up) * Quaternion.Euler(orientationFixHead);
        headChild.transform.localRotation = rotation;

        Vector3 UP = transform.InverseTransformDirection(cameraLimitedTransform.position - child.transform.position);
        Quaternion rotate = Quaternion.LookRotation(_axis, UP) * Quaternion.Euler(orientationFix);
        child.transform.localRotation = rotate;

        Quaternion rot = Quaternion.FromToRotation(cameraLimitedTransform.forward, transform.forward);
        child.transform.localRotation = Quaternion.Slerp(child.transform.localRotation, rot, speedRot * Time.deltaTime);
    }


    //private Transform _StartTransform() {
    //    Quaternion rotation = Quaternion.FromToRotation(child.transform.forward, transform.forward);

    //    child.transform.localRotation = Quaternion.Lerp(child.transform.localRotation, rotation, speedRot * Time.deltaTime);

    //    return child.transform;
    //}


    void GetInput() {
        CharMotor chM = CharMotor.instance;
         v = Input.GetAxis("Vertical");
         h = Input.GetAxis("Horizontal");
        move = Vector3.zero;
        if (v > _DEAD_ZONE || v < -_DEAD_ZONE)
            move += new Vector3(0, 0, v);
        if (h > _DEAD_ZONE || h < _DEAD_ZONE)
            move += new Vector3(h, 0, 0);
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
}
