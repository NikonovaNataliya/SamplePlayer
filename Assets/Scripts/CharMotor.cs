using UnityEngine;
using System.Collections;

public class CharMotor : MonoBehaviour {

    public static CharMotor instance;
    public float speed = 2.0f;
    public float speedRun = 4.0f;
    float vertical, horizontal;
    Vector3 direction;
    
    void Awake() {
        instance = this;
        vertical = GetComponent<CharController>().v;
        horizontal = GetComponent<CharController>().h;
    }
    public void UpdateMotor(Vector3 move) {
        _RotateChar(move);
        _ProcessMotion(move);
    }
    void _ProcessMotion(Vector3 moveVector) {
        moveVector = transform.TransformDirection(moveVector);
        Vector3.Normalize(moveVector);
        moveVector *= speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.X)) {                          // как сделать скорость бега?
            moveVector *= speedRun * Time.deltaTime;            // здесь она становится меньше
        }
        CharController.unityController.Move(moveVector);
                                                                      
    }
    void _RotateChar(Vector3 move) {
        //direction = new Vector3(horizontal, 0, vertical);
        //direction = Camera.main.transform.TransformDirection(direction);
        //direction = new Vector3(direction.x, 0, direction.z);
        if (move.x != 0 || move.z != 0) {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, transform.eulerAngles.z);

        }
        //if (vertical > 0) {
        //    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction),
        //                                                                         speedRun * Time.deltaTime);
        //}
        //if (vertical < 0) {
        //    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.back, direction),
        //                                                                                     speedRun * Time.deltaTime);
        //}
        //if (horizontal > 0 && vertical == 0) {
        //    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.right, direction),
        //                                                                                         speedRun * Time.deltaTime);
        //}
        //if (horizontal < 0 && vertical == 0) {
        //    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.left, direction),
        //                                                                                     speedRun * Time.deltaTime);
        //}

    }
}
