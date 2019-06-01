using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelRankingText : MonoBehaviour {

    private float nextRankingTime;
    private string currentRankingText = "";

    [System.Serializable]
    public struct ranking {
        public string text;
        public float timeBenchmark;
    }
    public ranking[] rankingSet = new ranking[6];

    private int rankingCount; //Total number of rankings in the level (TODO: decide if this will be constant across all levels)
    private int currentRankingNumber;

    void UpdateRanking() {

        //Set the next time goal to progress another ranking
        if (currentRankingNumber < rankingCount - 1) {
            nextRankingTime = rankingSet[currentRankingNumber + 1].timeBenchmark;
        }
        else {
            nextRankingTime = 9.9f * (10 ^ 100);
        }
        //Update the ranking text
        if (currentRankingNumber < rankingCount) {
            currentRankingText = rankingSet[currentRankingNumber].text;
        }
        else {
            currentRankingText = "No Ranking";
        }

        GetComponent<Text>().text = currentRankingText;

        //Increase the player's ranking by 1
        currentRankingNumber++;
    }

    public void Reset() {

        rankingCount = rankingSet.Length;
        currentRankingNumber = 0;

        //Initialization of ranking time benchmarks
        UpdateRanking();

        GetComponent<Text>().text = currentRankingText;
    }
    // Use this for initialization
    void Start () {
        Reset();
    }

    // Update is called once per frame
    void Update () {

        if(TimerController.time_secs >= nextRankingTime) {
            UpdateRanking();
        }
    }
}
