using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrapAround : MonoBehaviour {

    private Transform player;
    public float leftBoundary;
    public float rightBoundary;
    private float wrapDistance;
	// Use this for initialization
	void Start () {
        player = transform;
        wrapDistance = (rightBoundary - leftBoundary);
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (player.position.x < leftBoundary) {
            player.Translate(new Vector3(wrapDistance, 0, 0), Space.World);
        }
        else if (player.position.x > rightBoundary) {
            player.Translate(new Vector3(-wrapDistance, 0, 0), Space.World);
        }

    }

}