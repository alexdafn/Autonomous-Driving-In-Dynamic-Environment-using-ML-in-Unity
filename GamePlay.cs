using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//isws na diagrafei
//using System.Windows.Forms;

public class GamePlay : MonoBehaviour
{
    //PositionCalculation()
    private Vector3 previousPosition, currentPosition, differenceVector;
    private float difference, totalDifference;
    //private Transform blueCar;
    public CarController blueCar;

    //Scores
    public float score;
    private float rewardScore;
    private float penaltyScore=0;

    //random desicion
    //public int actionNumberGameplay;

    //154level initialization
    private RoadSpawner roadSpawner;

    // Start is called before the first frame update
    void Start()
    {
        //ResetLevel();
        //98blueCar = GameObject.Find("Free_Racing_Car_Blue").transform;
        //blueCar = GameObject.Find("Free_Racing_Car_Blue");

        //154level initialization
        roadSpawner = GetComponent<RoadSpawner>();
        roadSpawner = GameObject.FindObjectOfType<RoadSpawner>();
        //LevelInitialization();
    }

    // Update is called once per frame
    void Update()
    {
        //Reward();
        //Penalty();
        //if(/*synthhkh done*/){
            //ResetLevel();
        //}
        ///RandomAction();

        if(blueCar.episodeDoneFlag){IsDone();}//done episode
        //CallRandomAction(); // exw energh kai thn RandomAction() sthn FixedUpdate CarContoller
        
    }

    private void FixedUpdate()
    {
       //98 previousPosition = blueCar.transform.position;
        //print(previousPosition);
        CallRandomAction();
    }
    private void LateUpdate()
    {
        //98PositionCalculation();
        Reward();
        Penalty();
        ScoreCalculation();
    }   


    /*98
    public float PositionCalculation()
    {
        currentPosition= blueCar.transform.position;
        differenceVector = currentPosition-previousPosition;
        difference = Mathf.Abs(differenceVector.x) + Mathf.Abs(differenceVector.y) + Mathf.Abs(differenceVector.z);
        totalDifference+=difference;
        print(difference); // to metro ths taxythtas poy kineitai to amaksi
        print(totalDifference); // h synolikh apostash poy exei dianysh to amaksi
        return totalDifference;


    }98*/

    private void Reward()
    {
       rewardScore+=blueCar.difference;
    }

    private void Penalty()
    {
        //penaltyScore=blueCar.penaltyCounter*(-5000);
        //lathos, pleon exei 2 diaforetikes gia eftheia kai strofh
    }

    /*
    private void ResetLevel()
    {

    }*/

    private void ScoreCalculation()
    {
        score = rewardScore+ penaltyScore;
        //print("THE SCORE IS"+ score);
    }

    private void LevelInitialization()
    {
        roadSpawner.SpawnRoadPiece(0,0);
        roadSpawner.SpawnRoadPiece(1,0);
        roadSpawner.SpawnRoadPiece(1,0);
        roadSpawner.SpawnRoadPiece(1,0);
        roadSpawner.SpawnRoadPiece(1,0);
        roadSpawner.SpawnRoadPiece(1,0);
        roadSpawner.SpawnRoadPiece(1,0);
        //blueCar.RandomReposition();
    }

    private void CallRandomAction( )
    {
        blueCar.actionNumber = Random.Range(0,4);
        print(blueCar.actionNumber);
        blueCar.MakeCarAction(blueCar.actionNumber); // na to dw
        //print("OLA KALA");
    }

    //na to ftiaksw
    private void Step(int action)
    {
        //blueCar.actionNumber= action;
    }

    private void IsDone()
    {
        //Reset the level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /*
    private bool ReturnKey(int number)
    {
        if(number==0 || number==1){
            return  Input.GetKeyDown(KeyCode.Space);
        }
    } */
}
