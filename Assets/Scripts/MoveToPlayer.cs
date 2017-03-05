using UnityEngine;
using System.Collections;

public class MoveToPlayer : MonoBehaviour {

    UnityEngine.AI.NavMeshAgent agent;
    Animator anim;
    public Transform player;
    public float speedRot;
    public float startMinDist, attackMinDist, attackDist;
    Vector3 startPosition, attackPosition;
    Vector3 attackPoint;

    void Start() {

        anim = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        startPosition = transform.position;
    }

    void Update() {

        attackPoint = player.position - (transform.rotation * Vector3.forward * attackDist);
        attackPosition = PositionCorretion(player.position, attackPoint);

        if (Vector3.Distance(startPosition, transform.position) <= startMinDist)
            anim.SetBool("Move", false);
        if (Vector3.Distance(attackPosition, transform.position) <= attackMinDist) {
            anim.SetBool("Move", false);
            anim.SetBool("Attack", true);
        }
        else 
            anim.SetBool("Attack", false);

    }
    Vector3 PositionCorretion(Vector3 target, Vector3 position) {
        Debug.DrawLine(target, position, Color.blue);
        RaycastHit hit;
        if (Physics.Linecast(target, position, out hit)) {
            float dist = Vector3.Distance(target, hit.point);
            position = target - (transform.rotation * Vector3.forward * dist);
        }
        return position;
    }

    void OnTriggerStay(Collider other) {
        if (other.gameObject.name == "GEROY") {
            agent.SetDestination(attackPosition);
            anim.SetBool("Move", true);
            Vector3 directon = player.position - transform.position;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, directon, speedRot * Time.deltaTime, 0);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }

    }
    void OnTriggerExit(Collider other) {

        agent.SetDestination(startPosition);
    }
}
