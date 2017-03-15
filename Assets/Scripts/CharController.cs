using UnityEngine;

class CharController : MonoBehaviour
{
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

    public float vert, horiz;
    private bool _isControlable = true;
    public bool IsControllable
    {
        get { return _isControlable; }
        set
        {
            SendMessage("_SwitchControl", value, SendMessageOptions.DontRequireReceiver);
            _isControlable = value;
        }
    }

    //Результат - вектор движения.
    public Vector3 move;

    private Vector3 _axis;
    //Размер мертвой зоны
    private const float _DEAD_ZONE = 0.1f;

    Vector3 orientationFix = new Vector3(90, 0, 0);
    Vector3 orientationFixHead = new Vector3(110, 0, 0);
    private Quaternion m_currentHeadRotation;
    private Quaternion m_currentBodyRotation;
    [SerializeField]
    private float m_rotationMultiply = 0.5f;

    void Awake()
    {
        //Просим у класса камеры найти камеру в сцене
        CharTPSCamera.GetCamera();
        _axis = transform.InverseTransformDirection(transform.up);
        this.m_currentHeadRotation = headChild.transform.localRotation;
        this.m_currentBodyRotation = child.transform.localRotation;
    }

    void Update()
    {
        //Если камеры нет - ничего не делаем
        if (Camera.main == null) return;
        if (!IsControllable) return;

        _GetHandleInput();
        //Обрабатываем введенные игроком данные
        _GetMoveInput();
        //Говорим CharMotor, что пора двигаться
                                                          //здесь я собиралась применить bool isFall
        if (!Input.GetKey(KeyCode.F)) {
            SendMessage("_DidMove", move, SendMessageOptions.DontRequireReceiver);
        }

                                          // но, даже если бы булька работала, на "бег" все равно не распространяется.
                                          // останавливается только при ходьбе
                                                            


        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate() {
        RecalcTargetRotation();
        LerpBody();
    }

    private void _GetMoveInput()
    {
        //На сколько сместились по "вертикали" (т.е. вперед/назад)
        vert = Input.GetAxis("Vertical");
        //На сколько сместились по "горизонтали" (т.е. влево/вправо)
        horiz = Input.GetAxis("Horizontal");

        //Обнуляем вектор движения.
        move = Vector3.zero;

        //Если смещение по "вертикали" вышло из мертвой зоны
        if (vert > _DEAD_ZONE || vert < -_DEAD_ZONE)
            //Прибавляем к вектору движения это смещение
            move += new Vector3(0, 0, vert);
        //Если смещение по "горизонтали" вышло из мертвой зоны
        if (horiz > _DEAD_ZONE || horiz < -_DEAD_ZONE)
            //Прибавляем к вектору движения это смещение
            move += new Vector3(horiz, 0, 0);
        if (Input.GetKey(KeyCode.F)) {
            move = Vector3.zero;
        }
    }

    private void _GetHandleInput()
    {
        //Если клавиша jump не нажата - ничего не делаем
        if (!Input.GetButton("Jump")) return;
        SendMessage("_DidJump", SendMessageOptions.DontRequireReceiver);
    }

    private Transform _GetLimitedCameraTransform() {

        float angleCamera = Quaternion.Angle(transform.rotation, Camera.main.transform.rotation);
        float angleLeftLimit = Quaternion.Angle(transform.rotation, m_leftRotationLimit.rotation);
        float angleRighrLimit = Quaternion.Angle(transform.rotation, m_rightRotationLimit.rotation);

        if (angleCamera < angleLeftLimit && angleCamera < angleRighrLimit) {
            return Camera.main.transform;
        }

        float angleCameraToLeftLimit = Quaternion.Angle(Camera.main.transform.rotation, m_rightRotationLimit.rotation);
        float angleCameraToRightLimit = Quaternion.Angle(Camera.main.transform.rotation, m_leftRotationLimit.rotation);

        if (angleCameraToLeftLimit < angleCameraToRightLimit) {
            return m_rightRotationLimit;
        }
        else {
            return m_leftRotationLimit;
        }
    }
    void RecalcTargetRotation() {


        Transform cameraLimitedTransform = _GetLimitedCameraTransform();

        Vector3 uphead = transform.InverseTransformDirection(cameraLimitedTransform.position - m_rotationTargetHead.transform.position);
        uphead.y = 0;
        Quaternion rotationhead = Quaternion.LookRotation(_axis, uphead) * Quaternion.Euler(orientationFixHead);
        m_rotationTargetHead.transform.localRotation = rotationhead;

        Vector3 upbody = transform.InverseTransformDirection(cameraLimitedTransform.position - m_rotationTargetBody.transform.position);
        Quaternion rotationbody = Quaternion.LookRotation(_axis, upbody) * Quaternion.Euler(orientationFix);
        m_rotationTargetBody.transform.localRotation = rotationbody;

    }
    private void LerpBody() {
        m_currentHeadRotation = _GetPartOfRotation(m_currentHeadRotation, m_rotationTargetHead.localRotation);
        m_currentBodyRotation = _GetPartOfRotation(m_currentBodyRotation, m_rotationTargetBody.localRotation);
        headChild.transform.localRotation = m_currentHeadRotation;
        child.transform.localRotation = m_currentBodyRotation;

    }
    private Quaternion _GetPartOfRotation(Quaternion from, Quaternion to) {
        return Quaternion.Slerp(from, to, m_rotationMultiply * Time.deltaTime);
    }
}
