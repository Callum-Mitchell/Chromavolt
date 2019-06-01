using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    //universal trigger variables accessible to all other scripts
    public static bool gameIsActive = true;
    public static bool isResettingLevel = false;
    public static bool isSwitchingLevels = false;

    public int levelID = 1; //used only on "level" type scenes 

    public GameObject player;
    public GameObject leftPlayerClone;
    public GameObject rightPlayerClone;

    public GameObject timer;
    public GameObject levelRanking;
    public GameObject masterSpawner;

    //The type of the current level will determine which others to load, etcetera
    [HideInInspector]
    public enum sceneTypeList {
        TitleScreen = 0,
        LevelSelect = 1,
        Level = 2,
        GameOver = 3,
        Other = 4 //may add more types later if needed
    }
    public sceneTypeList sceneType = sceneTypeList.TitleScreen;

    IEnumerator PreloadScene(string sceneName) {

        AsyncOperation preloadSceneInBackground = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        preloadSceneInBackground.allowSceneActivation = false; //The asynchonous scene will be inactive by default

        while (!isSwitchingLevels) {
            yield return new WaitForFixedUpdate();
        }
        preloadSceneInBackground.allowSceneActivation = true;
        yield break;
    }

    public void unloadLevel(string sceneName) {

        //Check to prevent unloading the only active level
        if (SceneManager.GetActiveScene().name != sceneName) {

            if (SceneManager.GetSceneByName(sceneName).isLoaded) {
                SceneManager.UnloadSceneAsync(sceneName);
            }
        }
    }

    /* TODO: instead of actually reloading the current level, just reset 
     * all appropriate values and objects to the starting position
     * (that can still go here)
     */
    IEnumerator ResetCurrentLevel() {

        //This WaitForFixedUpdate yield gives other affected objects a chance to detect a reset via isResettingLevel
        yield return new WaitForFixedUpdate();

        player.GetComponent<PlayerController>().Reset();
        leftPlayerClone.GetComponent<PlayerController>().Reset();
        rightPlayerClone.GetComponent<PlayerController>().Reset();
        timer.GetComponent<TimerController>().Reset();
        levelRanking.GetComponent<LevelRankingText>().Reset();
        masterSpawner.GetComponent<MasterSpawner>().Reset();
        Reset();

        //SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNewLevel(string newLevelName) {
        Scene newLevel = SceneManager.GetSceneByName(newLevelName);
        Scene currentLevel = SceneManager.GetActiveScene();

        string currentLevelName = currentLevel.name;

        if (currentLevelName == newLevelName) { //Trying to reload the current level, or an identically named one - won't allow it
            return;
        }

        isSwitchingLevels = true;

        //load the new level if needed
        if (!newLevel.isLoaded) {
            SceneManager.LoadSceneAsync(newLevelName);
        }
        //Activate the new level
        SceneManager.SetActiveScene(newLevel);

        //Unload the current (previous) level
        unloadLevel(currentLevelName);
    }

    // TriggerLoss is called when the player is shattered or presses the appropriate key (//TODO: decide on that key)
    private void TriggerLoss() {
        Time.timeScale = 0; //All FixedUpdate functionality will be deactivated. 
    }

    private void Reset() {
        gameIsActive = true;
        isSwitchingLevels = false;
        isResettingLevel = false;
    }

    // Initialization. Will be called when a loaded scene is activated (preloaded scenes are designed to start out inactive)
    void Awake () {
        Reset();
    }
	
	void FixedUpdate () {
		if(!gameIsActive) {
            TriggerLoss();
        }
	}

    //Will continue to be called even after time has frozen
    void Update () {
        if(isResettingLevel) {
            Time.timeScale = 1f;
            gameIsActive = true;
            StartCoroutine(ResetCurrentLevel());
        }
    }
}
