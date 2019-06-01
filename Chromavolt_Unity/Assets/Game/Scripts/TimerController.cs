using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour {

    public static float time_secs;

    public void Reset() {
        time_secs = 0.0f;
        GetComponent<Text>().text = "00.00";
    }

    // Use this for initialization
    void Start () {
        Reset();
	}

    // Only called while game is active (player still alive)
    //Timer will stop counting once player has died/been shattered
    void FixedUpdate () {

        time_secs += Time.deltaTime;
        GetComponent<Text>().text = time_secs.ToString(format: "0#.##");

	}
}
