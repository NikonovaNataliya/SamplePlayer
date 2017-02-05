using UnityEngine;
using System.Collections;

public class BotPoints : MonoBehaviour {

    public Transform[] Points;
    // можно назначать разные точки перемемещения для разных points
    // назначаем в инспекторе 
    public Transform GetNext() {
        return Points[Random.Range(0, Points.Length)];
    }
}
