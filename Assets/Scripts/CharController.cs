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
    [SerializeField]
    private Transform m_rotationTargetHead;
    [SerializeField]
    private Transform m_rotationTargetBody;

    public static CharacterController unityController;
    public static CharController instance;
    public Vector3 move;
    Vector3 _axis;

    Animator anim;
    private const float _DEAD_ZONE = 0.1f;
    public float v, h, speedRot;
    Vector3 orientationFix = new Vector3(90, 0, 0);
    Vector3 orientationFixHead = new Vector3(110, 0, 0);
    private Quaternion m_currentHeadRotation;
    private Quaternion m_currentBodyRotation;
    [SerializeField] private float m_rotationMultiply = 0.5f;
    

    void Awake() {
        instance = this;
        unityController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        _axis = transform.InverseTransformDirection(transform.up);
        this.m_currentHeadRotation = headChild.transform.localRotation;
        this.m_currentBodyRotation = child.transform.localRotation;
    }

    void Update() {
        // if (Camera.main == null) return;

        // GetInput();
        // CharMotor.instance.UpdateMotor(move);
       // CharMotor.instance.Moving(move);
        // GeroyAnimation();
    }

    void LateUpdate() {
        RecalcTargetRotation();
        LerpBody();
    }


    private Transform _GetLimitedCameraTransform() {

        float angleCamera = Quaternion.Angle(transform.rotation, Camera.main.transform.rotation);
        float angleLeftLimit = Quaternion.Angle(transform.rotation, m_leftRotationLimit.rotation);
        float angleRighrLimit = Quaternion.Angle(transform.rotation, m_rightRotationLimit.rotation);

        if( angleCamera < angleLeftLimit && angleCamera < angleRighrLimit ) {
            return Camera.main.transform;
        }

        float angleCameraToLeftLimit = Quaternion.Angle(Camera.main.transform.rotation, m_rightRotationLimit.rotation);
        float angleCameraToRightLimit = Quaternion.Angle(Camera.main.transform.rotation, m_leftRotationLimit.rotation);

        if ( angleCameraToLeftLimit < angleCameraToRightLimit ) {
            return m_rightRotationLimit;
        } else {
            return m_leftRotationLimit;
        }
    }
    void RecalcTargetRotation() {


        Transform cameraLimitedTransform = _GetLimitedCameraTransform();

        Vector3 uphead = transform.InverseTransformDirection(cameraLimitedTransform.position - m_rotationTargetHead.transform.position);
        uphead.y = 0;
        Quaternion rotationhead = Quaternion.LookRotation(_axis, uphead) * Quaternion.Euler(orientationFixHead);
        m_rotationTargetHead.transform.localRotation = rotationhead;

        Vector3 upbody = transform.InverseTransformDirection(cameraLimitedTransform.position  - m_rotationTargetBody.transform.position);
        Quaternion rotationbody = Quaternion.LookRotation(_axis,upbody) * Quaternion.Euler(orientationFix);
        m_rotationTargetBody.transform.localRotation = rotationbody;

    }
    private void LerpBody() {
        m_currentHeadRotation = _GetPartOfRotation( m_currentHeadRotation, m_rotationTargetHead.localRotation );
        m_currentBodyRotation = _GetPartOfRotation( m_currentBodyRotation, m_rotationTargetBody.localRotation );
        headChild.transform.localRotation = m_currentHeadRotation;
        child.transform.localRotation = m_currentBodyRotation;

    }
    private Quaternion _GetPartOfRotation( Quaternion from, Quaternion to ) {
        return Quaternion.Slerp( from, to, m_rotationMultiply * Time.deltaTime );
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
