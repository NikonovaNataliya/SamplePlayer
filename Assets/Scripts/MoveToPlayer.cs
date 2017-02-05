using UnityEngine;
using System.Collections;

public class MoveToPlayer : MonoBehaviour {

    public Transform player;
    public float speedRot;
    // public float speedMove;
    public float minDist, minDist2;
    NavMeshAgent agent;
    Animator anim;
    Vector3 startPosition, currentPosition, attackPosition;

    void Start() {

        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        startPosition = transform.position;
    }

    void Update() {

        currentPosition = transform.position;
        attackPosition = player.transform.position + new Vector3(-2, 0, 0);
        if (Vector3.Distance(startPosition, currentPosition) <= minDist)
            anim.SetBool("Move", false);
        if (Vector3.Distance(attackPosition, currentPosition) <= minDist2) {
            anim.SetBool("Move", false);
            anim.SetBool("Attack", true);
        }
        else anim.SetBool("Attack", false);

        Vector3 directon = player.position - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, directon, speedRot * Time.deltaTime, 0);
        Debug.DrawRay(transform.position, newDirection, Color.red);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }


    void OnTriggerStay(Collider other) {

        agent.SetDestination(attackPosition);
        anim.SetBool("Move", true);
    }
    void OnTriggerExit(Collider other) {

        agent.SetDestination(startPosition);
    }
}
