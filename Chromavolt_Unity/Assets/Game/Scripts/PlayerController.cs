using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.LightningBolt;

public class PlayerController : MonoBehaviour {

    private Transform PlayerTransform;
    private static Vector3 playerTargetEulerAngles; //by having target rotation explicitly valued in script,
    //problems arising from auto-conversion between equivalent EulerAngles can be avoided.
    private Vector3 playerAnglesStatePositive;
    private Vector3 playerAnglesStateNegative;

    private float fl_playerSpeed;
    public float fl_speedMultiplier = 1.0f; //Player's horizontal speed varies by level - this adjusts it
    private const float fl_LEAN_MULTIPLIER = 0; //adjusts how much the player leans into a turn

    private int int_flipFrames = 0;

    private const int int_FLIP_DURATION = 7; //The number of FixedUpdates over which a player's state swap will occur

    //TODO: clarify the differences between these two values
    private bool bl_isFlipping = false;
    private bool bl_isFlipAxisInUse = false;

    private bool bl_isPositiveState = true;

    //Child objects
    //Visual effects
    [System.Serializable]
    public struct LightningRenderer
    {
        public GameObject[] orbSurrounders;
        public GameObject[] orbCrossers;
        public Color positiveStateColor;
        public Color negativeStateColor;
    }

    public LightningRenderer lightning;

    IEnumerator flipPlayer(bool bl_isTargetStatePositive) {

        //Temporarily make the orb-crossing bolts invisible
        foreach (GameObject bolt in lightning.orbCrossers)
        {
            LineRenderer lightningRenderer = bolt.GetComponent<LineRenderer>();
            lightningRenderer.material = new Material(lightningRenderer.material);
            bolt.GetComponent<LightningBoltScript>().updateRenderColors(Color.clear, Color.clear);
            bolt.GetComponent<LightningBoltScript>().UpdateFromMaterialChange();

            //lightningRenderer.startColor = new Color(0, 0, 0, 0);
            //lightningRenderer.endColor = new Color(0, 0, 0, 0);
            //Gradient gradient = new Gradient();
            //gradient.SetKeys(
            //    new GradientColorKey[] { new GradientColorKey(Color.clear, 0.0f), new GradientColorKey(Color.clear, 1.0f) },
            //    new GradientAlphaKey[] { new GradientAlphaKey(0, 0.0f), new GradientAlphaKey(0, 1.0f) }
            //);
            //lightningRenderer.colorGradient = gradient;
        }

        for (int_flipFrames = 0; int_flipFrames < int_FLIP_DURATION; int_flipFrames++) {

            playerTargetEulerAngles = new Vector3(playerTargetEulerAngles.x, playerTargetEulerAngles.y + 180.0f / int_FLIP_DURATION, playerTargetEulerAngles.z);
            float flipProportion = ((float)int_flipFrames / int_FLIP_DURATION);
            Color intermediateColor;
            if (bl_isTargetStatePositive)
            {
                intermediateColor = lightning.negativeStateColor + (lightning.positiveStateColor - lightning.negativeStateColor) / flipProportion;
            }
            else
            {
                intermediateColor = lightning.positiveStateColor + (lightning.negativeStateColor - lightning.positiveStateColor) / flipProportion;
            }
            foreach (GameObject bolt in lightning.orbSurrounders)
            {
                LineRenderer lightningRenderer = bolt.GetComponent<LineRenderer>();
                lightningRenderer.material = new Material(lightningRenderer.material);
                bolt.GetComponent<LightningBoltScript>().updateRenderColors(intermediateColor, intermediateColor);
                bolt.GetComponent<LightningBoltScript>().UpdateFromMaterialChange();
            }

            yield return new WaitForFixedUpdate();
        }

        if (bl_isPositiveState)
        {
            playerTargetEulerAngles = playerAnglesStatePositive;
            //Restore orb-crossing bolt colors to new state
            foreach (GameObject bolt in lightning.orbCrossers)
            {
                LineRenderer lightningRenderer = bolt.GetComponent<LineRenderer>();
                lightningRenderer.material = new Material(lightningRenderer.material);
                bolt.GetComponent<LightningBoltScript>().updateRenderColors(lightning.positiveStateColor, lightning.positiveStateColor);
                bolt.GetComponent<LightningBoltScript>().UpdateFromMaterialChange();
                bolt.transform.position = new Vector3(bolt.transform.position.x, bolt.transform.position.y, -bolt.transform.position.z);
                //lightningRenderer.startColor = lightning.positiveStateColor;
                //lightningRenderer.endColor = lightning.positiveStateColor;
            }

        }
        else
        {
            playerTargetEulerAngles = playerAnglesStateNegative;
            foreach (GameObject bolt in lightning.orbCrossers)
            {
                LineRenderer lightningRenderer = bolt.GetComponent<LineRenderer>();
                lightningRenderer.material = new Material(lightningRenderer.material);
                bolt.GetComponent<LightningBoltScript>().updateRenderColors(lightning.negativeStateColor, lightning.negativeStateColor);
                bolt.GetComponent<LightningBoltScript>().UpdateFromMaterialChange();
                bolt.transform.position = new Vector3(bolt.transform.position.x, bolt.transform.position.y, -bolt.transform.position.z);
                //lightningRenderer.startColor = lightning.negativeStateColor;
                //lightningRenderer.endColor = lightning.negativeStateColor;
            }
        }
        int_flipFrames = 0;
        bl_isFlipping = false;
        yield break; //ends the coroutine
    }

    private void OnTriggerEnter(Collider other) {
        string colliderTag = other.gameObject.tag;
        if ((colliderTag == "NeutralWall") || (colliderTag == "PositiveWall" && !bl_isPositiveState) || (colliderTag == "NegativeWall" && bl_isPositiveState)) {
            //call shatter sequence/loss conditions here
            Debug.Log("Shattered");
            LevelManager.gameIsActive = false;
        }
    }

    //Reset is called upon (re)starting a level
    public void Reset() {
        fl_playerSpeed = 0;

        PlayerTransform = transform; //gameObject.GetComponent<Transform>();
        playerTargetEulerAngles = PlayerTransform.eulerAngles;
        PlayerTransform.eulerAngles = playerTargetEulerAngles;
        playerAnglesStatePositive = playerTargetEulerAngles;
        playerAnglesStateNegative = playerTargetEulerAngles + new Vector3(0, 180, 0);

        bl_isFlipping = false;
        bl_isFlipAxisInUse = false;

        bl_isPositiveState = true;
        int_flipFrames = 0;

    }

    public void updateStateColors()
    {
        lightning.positiveStateColor = LevelManager.positiveStateColor;
        lightning.negativeStateColor = LevelManager.negativeStateColor;
        if (!bl_isFlipping)
        {
            Color lightningSetColor = bl_isPositiveState ? lightning.positiveStateColor : lightning.negativeStateColor;
            foreach (GameObject bolt in lightning.orbSurrounders)
            {
                LineRenderer lightningRenderer = bolt.GetComponent<LineRenderer>();
                lightningRenderer.material = new Material(lightningRenderer.material);
                bolt.GetComponent<LightningBoltScript>().updateRenderColors(lightningSetColor, lightningSetColor);
                bolt.GetComponent<LightningBoltScript>().UpdateFromMaterialChange();
            }
            foreach (GameObject bolt in lightning.orbCrossers)
            {
                LineRenderer lightningRenderer = bolt.GetComponent<LineRenderer>();
                lightningRenderer.material = new Material(lightningRenderer.material);
                bolt.GetComponent<LightningBoltScript>().updateRenderColors(lightningSetColor, lightningSetColor);
                bolt.GetComponent<LightningBoltScript>().UpdateFromMaterialChange();
            }

        }
    }

    // Use this for initialization
    void Start () {
        Reset();
    }

    /* Update is called once per frame rendered, no matter what.
     *Can be used while player is dead
     */
    void Update () {
        if(Input.GetButtonDown("Reset")) {
            LevelManager.isResettingLevel = true;
        }
    }

    // FixedUpdate is repeatedly called only while player is alive
    void FixedUpdate () {

        /*This code section calls the coroutine to flip to the opposite state
         * whenever the flip axis key is hit. It does not allow for
         * calls to the flip function while a flip is already occuring*/
        if (Input.GetAxisRaw("Flip") != 0)
        {
            if (bl_isFlipAxisInUse == false)
            {
                if (bl_isFlipping == false)
                {

                    if (gameObject.tag == "Player")
                    {
                        bl_isPositiveState = !bl_isPositiveState;
                        bl_isFlipping = true;
                    }

                    StartCoroutine(flipPlayer(bl_isPositiveState));
                }

                bl_isFlipAxisInUse = true;
            }
        }
        if (Input.GetAxisRaw("Flip") == 0)
        {
            bl_isFlipAxisInUse = false;
        }

        //resets the player's world rotation to 0
        PlayerTransform.eulerAngles = new Vector3(0, 0, 0);

        //Applies player movement
        fl_playerSpeed = Input.GetAxis("Horizontal");
        PlayerTransform.Translate(new Vector3(fl_playerSpeed * fl_speedMultiplier, 0, 0), Space.World);

        //Finally, performs rotation around each global axis separately, in order (x, z, y)
        //Again, this helps prevent issues with different Euler angle conventions and equivalencies
        PlayerTransform.RotateAround(PlayerTransform.position, new Vector3(1, 0, 0), playerTargetEulerAngles.x);
        PlayerTransform.RotateAround(PlayerTransform.position, new Vector3(0, 0, 1), playerTargetEulerAngles.z);
        PlayerTransform.RotateAround(PlayerTransform.position, new Vector3(0, 1, 0), playerTargetEulerAngles.y);

        updateStateColors();
    }
}
