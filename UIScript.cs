using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using TMPRO

public class UIScript : MonoBehaviour
{
    // Start is called before the first frame update
    //1public Text timer;
    //1public float time;
    //1float msec;
    //1float sec;
    //1float min;
    private GamePlay gamePlayObject;

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

    //score from gameplay
    public CarAgentScript scoreDisp;
    public RoadSpawnerManager roadManagerDisp;
    //public GameObject scoreDisplayObject;

    //PositionCalculation()
    /*
    private float totalMeters;
    private Vector3 previousPosition, currentPosition, differenceVector;
    private float totalDifference;
    private Transform blueCar;
    public float difference; */

    void Start()
    {
        //checking
        //1StartCoroutine("StopWatch");
        //blueCar = GameObject.Find("Free_Racing_Car_Blue").transform;
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
       // SetScoreText();
        SetRoadCounterText();
        SetDifficultyText();
    }

    /*
    private void FixedUpdate()
    {
        previousPosition = blueCar.transform.position;
    }

    /*
     private void LateUpdate()
    {
        PositionCalculation();
    }  
    /*1
    IEnumerator StopWatch()
    {
        while(true)
        {
            time += Time.deltaTime;
            msec = (int)(time-(int)time)*100;
            sec = (int)(time%60);
            min = (int)(time/60%60);
            
            timer.text = string.Format("{0:00}:{1:00}:{2:00}",min,sec,msec);
            //timer.text = string.Format("{1:00}",sec);

            yield return null;
        }
    }1*/

    /*54
    private void GetScore()
    {
        totalMeters = gamePlayObject.PositionCalculation();
    }54*/

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

    //attached into "Reset Level" Button
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    ///////EXTRAA///////
    

}
