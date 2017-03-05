using UnityEngine;
using System.Collections;

public class MoveBot : MonoBehaviour {

    Animator anim;
    UnityEngine.AI.NavMeshAgent agent;

    public Transform currentPosition;
    public float speed;
   // public float time, timer;

    void Start() {

        anim = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        Invoke("Moving", 17f);
    }

    void Moving() {
        BotPoints point = currentPosition.GetComponent<BotPoints>();
        currentPosition = point.GetNext();
        agent.destination = currentPosition.position;
        Invoke("Moving", 10f);
        anim.SetBool("Move", true);
    }
    void Update() {
        agent.speed = speed;
        // Moving();

        float dist = Vector3.Distance(currentPosition.position, transform.position);
        if (dist < 0.5f) {
            anim.SetBool("Move", false);
        }
        //if (currentPosition) {
        //    timer += Time.deltaTime;
        //    if (timer > time) {
        //        if (currentPosition == CheckPoints.Points[].Length - 1) currentPosition = 0;
        //        timer = 0;
        //    }
        //    anim.SetBool("Move", false);
        //}
    }
}
