using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* Desired functionality for this class:
 * 1. Spawn enemy waves as a whole
 * 2. Spawn individual elements of a wave at manually-set intervals
 * 3. Be able to add a horizontal position offset to the wave (based on "columns" of walls)
 * 4. Be able to add the same offset (random or controlled) to each individual wave element if desired
 * 5. Account for the wrap-around feature in both of these offsets
 * 6. Be able to randomly select between possible elements for a stage

 */
public class WaveSpawner : MonoBehaviour {

    //WaveElement can be used as the entire wave by making WaveObject contain the entire wave
    [System.Serializable]
    public struct WaveElement {
        public GameObject WaveObject;
        public Vector3 spawnPosition;
        public Vector3 spawnRotationEulerAngles;
        public int postSpawnDelay; //dictates how many FixedUpdate calls must pass after the element spawns before another can follow
        //public int[] possibleSpawnIDs; //The value of each spawn ID contained dictates when the element can appear, i.e. 1 = first object, 2 = second object
    }


    /* SpawnIDData is an important class for wave design. The length of the class instance array
     * determines the length of the wave in # of spawns (the index of this refers to the spawnID).
     * The values of ElementSet determine which elements of EnemyWaveSelection could possibly be
     * spawned for the given index of . None of the
     * elements of the second dimension should be negative nor exceed the size of
     * EnemyWaveSelection. */
    [System.Serializable]
    public class SpawnIDElementSelection {
        public int[] WaveElementSet;
    }
    public SpawnIDElementSelection[] WaveIDSetElementSelection;

    public WaveElement[] EnemyWaveSelection; //contains all the potenial elements the wave can spawn
    private WaveElement[] EnemyWave; //actual generated wave as a whole, with elements obtained from EnemyWaveSelection
    //private int PossibleWaveElementCount; //total number of possible elements to choose from
    private int WaveLength; //# of spawns before the wave ends 
    private int WaveDuration; //# of FixedUpdate calls before the wave ends

    private int currentSpawnID;
    private bool bl_callingEnabled;

    IEnumerator spawnElement(WaveElement nextElement)
    {
        int int_framesToNextCall = nextElement.postSpawnDelay;

        Instantiate(nextElement.WaveObject, nextElement.spawnPosition, Quaternion.Euler(nextElement.spawnRotationEulerAngles));

        while (int_framesToNextCall > 0) {
            int_framesToNextCall--;
            yield return new WaitForFixedUpdate();
        }

        currentSpawnID++;
        if (currentSpawnID >= WaveLength) {
            Destroy(gameObject);
        }

        bl_callingEnabled = true;

        yield break;
    }

    WaveElement SelectElement() {

        int possibleElementCount = WaveIDSetElementSelection[currentSpawnID].WaveElementSet.Length;
        int selectedElement = WaveIDSetElementSelection[currentSpawnID].WaveElementSet[Random.Range(0, possibleElementCount)];
        selectedElement = Mathf.RoundToInt(Mathf.Clamp(selectedElement, 0, EnemyWaveSelection.Length - 1));
        return EnemyWaveSelection[selectedElement];
    }

    // Use this for initialization
    void Start () {
		
        currentSpawnID = 0;
        bl_callingEnabled = false;
        WaveLength = WaveIDSetElementSelection.Length;
        EnemyWave = new WaveElement[WaveLength];
        WaveDuration = 0;

        while(currentSpawnID < WaveLength) {

            EnemyWave[currentSpawnID] = SelectElement();
            WaveDuration += EnemyWave[currentSpawnID].postSpawnDelay;
            currentSpawnID++;
        }

        //EnemyWave = EnemyWaveSelection; //OLD PLACEHOLDER - REMOVE

        MasterSpawner.int_framesUntilSpawn += WaveDuration; //adds duration to already-existing post-wave delay
        MasterSpawner.bl_isCallingWave = false;

        currentSpawnID = 0;
        bl_callingEnabled = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (LevelManager.isResettingLevel)
        {
            Destroy(gameObject);
        }

        if (bl_callingEnabled) {

            bl_callingEnabled = false;
            StartCoroutine(spawnElement(EnemyWave[currentSpawnID]));
        }
    }
}
