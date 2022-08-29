using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    [Header("Component")]
    public Text timer;
    public Text score;
    public Text roadsPassed;
    public Text currentDifficulty;

    [Header("Timer Settings")]
    public float currentTime;
    public bool countDown;

    [Header("Limit Settings")]
    public bool hasLimit;
    public float timerLimit;

    public CarAgentScript scoreDisp;
    public RoadSpawnerManager roadManagerDisp;

    void Start()
    {
        //Getting the right reference
        roadManagerDisp = GameObject.FindObjectOfType<RoadSpawnerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        currentTime = countDown ? currentTime -= Time.deltaTime : currentTime += Time.deltaTime;
        
        if(hasLimit && ((countDown && currentTime<= timerLimit) || (!countDown && currentTime >= timerLimit)))
        {
            currentTime = timerLimit;
            SetTimerText();
            timer.color = Color.red;
            enabled = false;
        }
        
        
        SetTimerText();
        SetRoadCounterText();
        SetDifficultyText();
    }

    private void SetTimerText()
    {
        timer.text = currentTime.ToString("0.00");

    }

    private void SetScoreText()
    {
        float display = scoreDisp.scoreDisplay;
        score.text = display.ToString("0.000");
    }

    private void SetRoadCounterText()
    {
       roadsPassed.text=roadManagerDisp.counter.ToString("0.0");
    }

    private void SetDifficultyText()
    {
        currentDifficulty.text = roadManagerDisp.difficultySelector.ToString("0.0");
    }  
}
