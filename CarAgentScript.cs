using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Text;
using System.IO;
using UnityEngine.UI;

using System.Linq;

using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class CarAgentScript : Agent
{
    // Getting the car Reference
    public CarController blueCar;
    // Getting the road creation reference
    private RoadSpawner roadSpawner;

    //gia katallhlh axrikopoihsh twn timwn ths se kathe episodeio
    private RoadSpawnerManager roadSpawnerManager; 

    public GameObject blueCarObject;
    Rigidbody carRigidBody;

    //checker for giving reward IF a roadpart has been passed OR  penalty if sideroad has been stepped
    int roadsStraightPassedCounterAgent=0;
    int roadsTurnPassedCounterAgent=0;

    public float balanceStabilizerAgent=-0.5f;
    //Δίνεται στην UIScript για να φαίνεται κάτω δεξιά στην οθόνη
    public float scoreDisplay=0;
    //UIScript access on timer
    public float episodeTimer;
    //UIScript access on roadCounter 
    public float CAroadCounterText;
    //UIScript access on DifficultySelector 
    public float CAdifficultySelectorText;
    
    public float accelerationReward;
    public float brakingPenalty;
    public float sideSteppingPenalty;
    public float stackedTime;// για όταν είναι ακίνητο το αμάξι για κάποια ώρα να γίνεται reset
    public bool stackedTimeFlag;

    //DestroyObjectsInScene when episode begins
    private GameObject[] tempDestroy;

    //DestroyObjectsInScene //have10MaxRoads
    private GameObject[] tempDestroyOngoing;
    public float roadsDestroyCounter;

    void Update()
    {
        episodeTimer += Time.deltaTime;
        if(stackedTimeFlag)
        {stackedTime+= Time.deltaTime;}else{stackedTime=0;}
        CAroadCounterText=roadSpawnerManager.counter;
        CAdifficultySelectorText=roadSpawnerManager.difficultySelector;

    }

    public override void Initialize()
    {
        carRigidBody = blueCarObject.GetComponent<Rigidbody>();
        carRigidBody.centerOfMass= new Vector3(0,balanceStabilizerAgent,0);

        roadSpawner = GameObject.FindObjectOfType<RoadSpawner>();
        roadSpawnerManager = GameObject.FindObjectOfType<RoadSpawnerManager>();
    }

    //Collects the observations as the agents plays the episode
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(carRigidBody.velocity.magnitude);
        //sensor.AddObservation(transform.position); 
        sensor.AddObservation(transform.rotation.y);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        
        //move forward or brake
        float acceleration_brakingAction = vectorAction[0];

        //steer left or right
        float steeringAction = vectorAction[1];

        //move forward
        if(acceleration_brakingAction==1)
        { 
            blueCar.MakeCarAction(acceleration_brakingAction);
            //Speed limitation
            if(carRigidBody.velocity.magnitude>15)
            {
                accelerationReward=0;
            }else if(carRigidBody.velocity.magnitude>1)
            {
                accelerationReward=carRigidBody.velocity.magnitude/(330); //Reward parameter
            }
            AddReward(accelerationReward);
        }
        //break
        if(acceleration_brakingAction==2)
        {
            blueCar.MakeCarAction(acceleration_brakingAction+2);

            if(carRigidBody.velocity.magnitude==0)
            {
               brakingPenalty=-1; 
            }else if(carRigidBody.velocity.magnitude>1)
            {
                brakingPenalty=1/(carRigidBody.velocity.magnitude*240);//Penalty parameter
            }
             AddReward(-brakingPenalty);
        }
        //Do Nothing for acceleration/brake
        if(acceleration_brakingAction==0){blueCar.MakeCarAction(acceleration_brakingAction);AddReward(-0.00001f);}

        //steer left
        if(steeringAction==1)
        {
            blueCar.MakeCarAction(steeringAction+1);
            
            // If steps on any side, do:
            if(blueCar.leftSidePenaltyFlag){AddReward(-0.003f);} //για ευθεία
            if(blueCar.rightSidePenaltyFlag){AddReward(0.005f);} //για ευθεία

            if(blueCar.leftTurnSidePenaltyFlag){AddReward(-0.003f);} //για στροφη
            if(blueCar.rightTurnSidePenaltyFlag){AddReward(0.005f);} //για στροφη
        }
        //steer right
        if(steeringAction==2)
        {
            blueCar.MakeCarAction(steeringAction+1);

            // If steps on any side, do:
            if(blueCar.leftSidePenaltyFlag){AddReward(0.005f);} //για ευθεία
            if(blueCar.rightSidePenaltyFlag){AddReward(-0.003f);} //για ευθεία

            if(blueCar.leftTurnSidePenaltyFlag){AddReward(0.005f);} //για στροφη
            if(blueCar.rightTurnSidePenaltyFlag){AddReward(-0.003f);} //για στροφη
        }
        //DONOTHING
        if(steeringAction==0){blueCar.MakeCarAction(steeringAction+5);}

        

        //Ποινή όταν πατάει τα πλαϊνά
        if(blueCar.leftSidePenaltyFlag || blueCar.rightSidePenaltyFlag || blueCar.leftTurnSidePenaltyFlag || blueCar.rightTurnSidePenaltyFlag)
        {
            AddReward(-0.005f);
        }
        
        //Να τιμωρείται όταν μένει ακίνητο, και να ξανα ξεκινάει το επεισόδιο
        if(blueCar.difference<0.001455)
        {
            stackedTimeFlag=true;
            if(stackedTime>20)
            {
                SetReward(-1f);
                EndEpisode();
            }
        }else{stackedTimeFlag=false;}

        //If a straight road piece has been passed by
        if(roadsStraightPassedCounterAgent<blueCar.roadsStraightPassedCounter)
        {
            HaveMax10Roads();// Για διαγραφή των δρόμων καθώς προχωράει
            roadsStraightPassedCounterAgent=blueCar.roadsStraightPassedCounter;
        }

        //If a turn road piece has been passed by
        if(roadsTurnPassedCounterAgent<blueCar.roadsTurnPassedCounter)
        {
            roadsTurnPassedCounterAgent=blueCar.roadsTurnPassedCounter;
        }
        
        //Ends the episode 
        if(blueCar.episodeDoneFlag)
        {
            AddReward(-10f);
            EndEpisode();
        }

        //την χρησιμοποιεί η UIScript για να εμφανίσει στην οθόνη το reward του επεισοδίου
        scoreDisplay= GetCumulativeReward();
    }
    
    public override void Heuristic(float[] actionsOut)
    {
        bool accelerationKey = Input.GetKey(KeyCode.UpArrow);
        bool brakeKey = Input.GetKey(KeyCode.DownArrow);
        bool leftSteeringKey = Input.GetKey(KeyCode.LeftArrow);
        bool rightSteeringKey = Input.GetKey(KeyCode.RightArrow);

        if(accelerationKey)
        {
            actionsOut[0]= 1; //1= ACCELERATE
            accelerationKey=false;
        }else if (brakeKey)
        {  
            actionsOut[0]=2; //2= BRAKE
            brakeKey=false;
        }else if(!accelerationKey && !brakeKey)
        {
            actionsOut[0]=0; //0= DO NOTHING
            
        }
    
        if(leftSteeringKey)
        {
            actionsOut[1]=1; //1= LEFT
            leftSteeringKey=false;
        }else if(rightSteeringKey)
        {
            actionsOut[1]=2; //2= RIGHT
            rightSteeringKey=false;
        }else if(!leftSteeringKey && !rightSteeringKey)
        {
            actionsOut[1]=0; //0= DO NOTHING
        }    
    }

    public override void OnEpisodeBegin()
    {
        print("The episode has begun");

        DestroyObjectsInScene(); //destroy all previous roadPieces

        roadSpawner.nextSpawnPoint= new Vector3(0,0,-15); //starting spawn point
        roadSpawner.turnRoadFlag=0; //proper turn spawning

        //initialize RoadSpawnerManager CLASS values
        roadSpawnerManager.counter = 0;
        roadSpawnerManager.roadSelector=1;
        roadSpawnerManager.rotationSelector=0;
        roadSpawnerManager.difficultySelector=5;

        StartingRoadPieces(); //create new starting roadPieces

        //initialize CarController CLASS values
        blueCar.episodeDoneFlag= false;
        blueCar.transform.position = new Vector3(0,0.5f,0);
        blueCar.RandomReposition();

        blueCar.roadsStraightPassedCounter=0;//counter if a TriggerSpot has been passed by
        blueCar.roadsTurnPassedCounter=0;//counter if a TriggerSpot has been passed by
        
        roadsStraightPassedCounterAgent=0;//counter if a TriggerSpot has been passed by
        roadsTurnPassedCounterAgent=0;//counter if a TriggerSpot has been passed by

        carRigidBody.velocity= new Vector3(0,0,0);//gia na ksekinaei to amaksi xwris thn prohgoymenh taxythta

        roadsDestroyCounter=0;//Have10MaxRoads()
        episodeTimer=0;//reset το χρονόμετρο
        stackedTime=0; //reset το timer για όταν κολάει στα πλαϊνά
        scoreDisplay=0;
    }
    
    private void DestroyObjectsInScene()
    {
        //pinakas kai ola mazi destroy
        tempDestroy= GameObject.FindGameObjectsWithTag("RoadPiece");
        foreach(GameObject tempD in tempDestroy){Destroy(tempD);}
    }
    
    private void StartingRoadPieces()
    {   
        roadSpawner.SpawnRoadPiece(0,0);
        roadSpawner.SpawnRoadPiece(1,0);
        roadSpawner.SpawnRoadPiece(1,0);
        roadSpawner.SpawnRoadPiece(1,0);
        roadSpawner.SpawnRoadPiece(1,0);
    }

    private void HaveMax10Roads()
    {
        tempDestroyOngoing= GameObject.FindGameObjectsWithTag("RoadPiece");
        if(tempDestroyOngoing.Length>7)
        {
            Destroy(tempDestroyOngoing[0]);
            Destroy(tempDestroyOngoing[tempDestroyOngoing.Length]);
        }
    }
}