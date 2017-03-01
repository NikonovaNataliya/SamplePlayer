using UnityEngine;
using System.Collections;

public class CharTPSCamera : MonoBehaviour {

    public static CharTPSCamera instance;
    public Transform target;
    public float Distance { get; set; }

    private const float _DISTANCE_MIN = 4f;
    private const float _DISTANCE_START = _DISTANCE_MIN;
    private const float _DISTANCE_MAX = 12f;
    private const float _DISTANCE_SMOOTH = 0.05f;
    private const float _X_SMOOTH = 0.05f;
    private const float _Y_SMOOTH = 0.1f;

    private float _velDistance;
    private float _velX;
    private float _velY;
    private float _velZ;
    private float _mouseX;
    private float _mouseY;
    private float _desireDistance;
    private Vector3 _desirePosition;
    private Vector3 _position;

    private const float _X_MOUSE_SENSITIVITY = 5f;
    private const float _Y_MOUSE_SENSITIVITY = 5f;
    private const float _MOUSE_WHEEL_SENSITIVITY = 5f;
    private const float _DEAD_ZONE = 0.01f;
    private const float _Y_MIN_LIMIT = 0f;
    private const float _Y_MAX_LIMIT = 20f;

    void Awake() {
        instance = this;
    }

    void Start() {
        Distance = Mathf.Clamp(Distance, _DISTANCE_MIN, _DISTANCE_MAX);
        Reset();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate() {
        if (target == null) return;

        PlayerInput();
        CalcDesirePosition();
        UpdatePosition();
    }

    void PlayerInput() {
       // if (Input.GetMouseButton(1)) {
            _mouseX += Input.GetAxis("Mouse X") * _X_MOUSE_SENSITIVITY;
            _mouseY -= Input.GetAxis("Mouse Y") * _Y_MOUSE_SENSITIVITY;
      //  }
        _mouseY = Utils.ClampAngle(_mouseY, _Y_MIN_LIMIT, _Y_MAX_LIMIT);
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll < -_DEAD_ZONE || scroll> _DEAD_ZONE) {
            _desireDistance = Mathf.Clamp(Distance - scroll * _MOUSE_WHEEL_SENSITIVITY, _DISTANCE_MIN, _DISTANCE_MAX);
        }
    }

    void CalcDesirePosition() {
        Distance = Mathf.SmoothDamp(Distance, _desireDistance, ref _velDistance, _DISTANCE_SMOOTH);
        _desirePosition = CalcPosition(_mouseY, _mouseX, Distance);
    }

    Vector3 CalcPosition(float rotx, float roty, float distance) {
        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(rotx, roty, 0);
        return target.position + rotation * direction;
    }

    public void Reset() {
        _mouseX = 0f;
        _mouseY = 10f;
        Distance = _DISTANCE_START;
        _desireDistance = Distance;
    }
    void UpdatePosition() {
        float posX = Mathf.SmoothDamp(_position.x, _desirePosition.x, ref _velX, _X_SMOOTH);
        float posY = Mathf.SmoothDamp(_position.y, _desirePosition.y, ref _velY, _Y_SMOOTH);
        float posZ = Mathf.SmoothDamp(_position.z, _desirePosition.z, ref _velZ, _X_SMOOTH);

        _position = new Vector3(posX, posY, posZ);
        transform.position = _position;
        transform.LookAt(target);
    }

     static public void GetCamera() {

        GameObject tempCamera;
        GameObject targetTemp;
        CharTPSCamera myCamera;

        if (Camera.main != null) tempCamera = Camera.main.gameObject;
        else {
            tempCamera = new GameObject("Main Camera");
            tempCamera.AddComponent<Camera>();
            tempCamera.tag = "Main Camera";
        }
        tempCamera.AddComponent<CharTPSCamera>();
        myCamera = tempCamera.GetComponent<CharTPSCamera>();
        targetTemp = GameObject.Find("targetLookAt");

        if(targetTemp == null) {
            targetTemp = new GameObject("targetLookAt");
            targetTemp.transform.position = Vector3.zero;
        }
        myCamera.target = targetTemp.transform;
    }
}

internal static class Utils {
    public static float ClampAngle(float angle, float min, float max) {
        do {
            if (angle < -360) angle += 360;
            if (angle > 360) angle -= 360;
        }
        while (angle < -360 || angle > 360);
        return Mathf.Clamp(angle, min, max);
    }
}
