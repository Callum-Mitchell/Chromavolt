using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Desired functionality for this class:
 * 1. Move walls at customizable speeds vertically and horizontally (DONE)
 * 2. Allow for back-and-forth wall movement horizontally or vertically (DONE)
 * 3. Be able to toggle whether a wall is clamped onscreen (DONE)
 * 4. If wall is clamped on screen horizontally, but spawns offscreen,
 *    adjust position to onscreen similarly to wrap-around for player (DONE)
 * 5. If clamped, destroy object when it reaches a certain z threshold (DONE)
 * 6. If unclamped, destroy object after a set timeframe (DONE)

 */
public class WallMovement : MonoBehaviour {

    private Transform wall;
    public float zSpeed = -0.5f;
    public float xSpeed = 0.0f;

    public bool isScreenClamped = true; //set false for permanent objects

    //clamps used for back-and-forth movement
    public float min_xPos = -72f;
    public float max_xPos = 72f;
    public float min_zPos = -100f;
    public float max_zPos = 100f;

    public int wallLifetime = 600; //Total FixedUbdate calls before object is automatically destroyed
    private int wallAge = 0; //total frames passed since wall's inception

    //Called upon resetting a level. Will destroy the wall and call appropriate visual effects
    private void Reset() {

        //Call visual effects here (before destroying)
        Destroy(gameObject);

    }

    // Use this for initialization
    void Start () {

        wall = transform;
        wallAge = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if(LevelManager.isResettingLevel) {
            Reset();
        }

        if (wall.position.x < min_xPos || wall.position.x > max_xPos) {
            xSpeed = -xSpeed;
        }

        if (wall.position.z < min_zPos || wall.position.z > max_zPos)
        {
            zSpeed = -zSpeed;
        }

        wall.position += new Vector3(xSpeed, 0.0f, zSpeed);

        if (isScreenClamped) {

            if (wall.position.z <= -60) {
                Destroy(gameObject);
            }

            if (wall.position.x < -36f) {
                wall.position += new Vector3(72f, 0, 0);
            }
            else if (wall.position.x > 36f) {
                wall.position += new Vector3(-72f, 0, 0);
            }
        }
        else if(wallAge > wallLifetime) {
            Destroy(gameObject);
        }
    }
}
