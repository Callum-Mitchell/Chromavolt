using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour {

    public int lifetime = 200;
    private int remainingLifetime;

	// Use this for initialization
	void Awake () {
        remainingLifetime = lifetime;
	}
	
	// FixedUpdate is called once per deltatime
	void FixedUpdate () {
        if (remainingLifetime <= 0) {
            Destroy(gameObject);
        }
        remainingLifetime--;
	}
}
