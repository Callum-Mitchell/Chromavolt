using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralWallGlow : MonoBehaviour {

    private int glowLoopDuration = 30;
    private int glowLoopMidpoint = 15;
    private int glowLoopTime = 0;
    private Color glowStartColor;
    private Color glowMidColor;
    private Material glowMaterial;

	// Use this for initialization
	void Start () {
        glowLoopTime = 0;
        glowStartColor = new Color(0.6f, 0f, 0f);
        glowMidColor = new Color(0.74f, 0f, 0f);
        glowMaterial = GetComponent<Renderer>().material;
        glowMaterial.color = glowStartColor;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Color glowColor;
        glowLoopTime++;
		if(glowLoopTime > glowLoopDuration)
        {
            glowLoopTime = 0;
        }
        if(glowLoopTime >= glowLoopMidpoint)
        {
            glowColor = glowMidColor - (glowMidColor - glowStartColor) * (glowLoopTime - glowLoopMidpoint) / (glowLoopDuration - glowLoopMidpoint);
        }
        else
        {
            glowColor = glowStartColor + (glowMidColor - glowStartColor) * glowLoopTime / glowLoopMidpoint;
        }

        glowMaterial.color = glowColor;
    }
}
