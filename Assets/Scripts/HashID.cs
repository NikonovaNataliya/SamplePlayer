using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HashID : MonoBehaviour {

    public int idleState;
    public int moveState;
    public int runState;
    public int fallState;
    public int attackState;

    public int move;
    public int fall;
    public int attack;
    public int jump;

        void Awake() {
        idleState = Animator.StringToHash("Base Layer.metarig|POKOY");
        moveState = Animator.StringToHash("Base Layer.metarig|GO");
        runState = Animator.StringToHash("Base Layer.metarig|GO-BEG");
        fallState = Animator.StringToHash("Base Layer.metarig|FALL");
        attackState = Animator.StringToHash("Attack.metarig|ATAKA");

        move = Animator.StringToHash("Move");
        fall = Animator.StringToHash("Fall");
        attack = Animator.StringToHash("Attack");
        jump = Animator.StringToHash("Jump");
}
}
