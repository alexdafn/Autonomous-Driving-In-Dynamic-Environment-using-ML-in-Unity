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


    // Initialization of values, on every episode
    private RoadSpawnerManager roadSpawnerManager; 

    public GameObject blueCarObject;
    Rigidbody carRigidBody;

    // Checker for giving reward IF a roadp art has been passed OR  penalty if sideroad has been stepped
    int roadsStraightPassedCounterAgent=0;
    int roadsTurnPassedCounterAgent=0;

    public float balanceStabilizerAgent=-0.5f;
    // Used on training to check current reward
    public float scoreDisplay=0;
    // UIScript access on timer
    public float episodeTimer;
    // UIScript access on roadCounter 
    public float CAroadCounterText;
    // UIScript access on DifficultySelector 
    public float CAdifficultySelectorText;
    
    public float accelerationReward;
    public float brakingPenalty;
    public float stackedTime;// If the car stands almost still for a certain time, reset the episode
    public bool stackedTimeFlag;

    // DestroyObjectsInScene when episode begins
    private GameObject[] tempDestroy;

    // Have10MaxRoads()
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

    // Training Initialization, getting access on the important components
    public override void Initialize()
    {
        carRigidBody = blueCarObject.GetComponent<Rigidbody>();
        carRigidBody.centerOfMass= new Vector3(0,balanceStabilizerAgent,0);

        roadSpawner = GameObject.FindObjectOfType<RoadSpawner>();
        roadSpawnerManager = GameObject.FindObjectOfType<RoadSpawnerManager>();
    }

    // Collects the observations as the agents plays the episode
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(carRigidBody.velocity.magnitude); // speed
        //sensor.AddObservation(transform.position); 
        sensor.AddObservation(transform.rotation.y); // car's angle on y
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        // move forward or brake
        float acceleration_brakingAction = vectorAction[0];

        // steer left or right
        float steeringAction = vectorAction[1];

        // move forward
        if(acceleration_brakingAction==1)
        { 
            blueCar.MakeCarAction(acceleration_brakingAction);
            // Speed limitation
            if(carRigidBody.velocity.magnitude>15)
            {
                accelerationReward=0;
            }else if(carRigidBody.velocity.magnitude>1)
            {
                accelerationReward=carRigidBody.velocity.magnitude/(330); // Reward parameter
            }
            AddReward(accelerationReward);
        }

        // break
        if(acceleration_brakingAction==2)
        {
            blueCar.MakeCarAction(acceleration_brakingAction+2);

            if(carRigidBody.velocity.magnitude==0)
            {
               brakingPenalty=-1; 
            }else if(carRigidBody.velocity.magnitude>1)
            {
                brakingPenalty=1/(carRigidBody.velocity.magnitude*240);// Penalty parameter
            }
             AddReward(-brakingPenalty);
        }

        // Do Nothing for acceleration/brake
        if(acceleration_brakingAction==0){blueCar.MakeCarAction(acceleration_brakingAction);AddReward(-0.00001f);}

        // steer left
        if(steeringAction==1)
        {
            blueCar.MakeCarAction(steeringAction+1);
            
            // If steps on any side, do:
            if(blueCar.leftSidePenaltyFlag){AddReward(-0.003f);} // straight road piece
            if(blueCar.rightSidePenaltyFlag){AddReward(0.005f);} // straight road piece

            if(blueCar.leftTurnSidePenaltyFlag){AddReward(-0.003f);} // turn
            if(blueCar.rightTurnSidePenaltyFlag){AddReward(0.005f);} // turn
        }
        // steer right
        if(steeringAction==2)
        {
            blueCar.MakeCarAction(steeringAction+1);

            // If steps on any side, do:
            if(blueCar.leftSidePenaltyFlag){AddReward(0.005f);} // straight road piece
            if(blueCar.rightSidePenaltyFlag){AddReward(-0.003f);} // straight road piece

            if(blueCar.leftTurnSidePenaltyFlag){AddReward(0.005f);} // turn
            if(blueCar.rightTurnSidePenaltyFlag){AddReward(-0.003f);} // turn
        }
        // DONOTHING
        if(steeringAction==0){blueCar.MakeCarAction(steeringAction+5);}

        // Penalty when car steps on the side
        if(blueCar.leftSidePenaltyFlag || blueCar.rightSidePenaltyFlag || blueCar.leftTurnSidePenaltyFlag || blueCar.rightTurnSidePenaltyFlag)
        {
            AddReward(-0.005f);
        }
        
        // Penalty when car is not moving, restarts the episode
        if(blueCar.difference<0.001455)
        {
            stackedTimeFlag=true;
            if(stackedTime>20)
            {
                SetReward(-1f);
                EndEpisode();
            }
        }else{stackedTimeFlag=false;}

        // If a straight road piece has been passed by
        if(roadsStraightPassedCounterAgent<blueCar.roadsStraightPassedCounter)
        {
            HaveMax10Roads();// Algorithm for road deletion on the run
            roadsStraightPassedCounterAgent=blueCar.roadsStraightPassedCounter;
        }

        // If a turn road piece has been passed by
        if(roadsTurnPassedCounterAgent<blueCar.roadsTurnPassedCounter)
        {
            roadsTurnPassedCounterAgent=blueCar.roadsTurnPassedCounter;
        }
        
        // Ends the episode 
        if(blueCar.episodeDoneFlag)
        {
            AddReward(-10f);
            EndEpisode();
        }

        // UIScript for displaying the score while training
        scoreDisplay= GetCumulativeReward();
    }
    
    // Used for testing on training with keyboard control
    public override void Heuristic(float[] actionsOut)
    {
        bool accelerationKey = Input.GetKey(KeyCode.UpArrow);
        bool brakeKey = Input.GetKey(KeyCode.DownArrow);
        bool leftSteeringKey = Input.GetKey(KeyCode.LeftArrow);
        bool rightSteeringKey = Input.GetKey(KeyCode.RightArrow);

        if(accelerationKey)
        {
            actionsOut[0]= 1; // 1= ACCELERATE
            accelerationKey=false;
        }else if (brakeKey)
        {  
            actionsOut[0]=2; // 2= BRAKE
            brakeKey=false;
        }else if(!accelerationKey && !brakeKey)
        {
            actionsOut[0]=0; // 0= DO NOTHING
            
        }
    
        if(leftSteeringKey)
        {
            actionsOut[1]=1; // 1= LEFT
            leftSteeringKey=false;
        }else if(rightSteeringKey)
        {
            actionsOut[1]=2; // 2= RIGHT
            rightSteeringKey=false;
        }else if(!leftSteeringKey && !rightSteeringKey)
        {
            actionsOut[1]=0; // 0= DO NOTHING
        }    
    }

    // Episode Initialization 
    public override void OnEpisodeBegin()
    {
        print("The episode has begun");

        DestroyObjectsInScene(); // Destroy all previous road pieces

        roadSpawner.nextSpawnPoint= new Vector3(0,0,-15); // starting spawn point
        roadSpawner.turnRoadFlag=0; // proper turn spawning

        // Initialize RoadSpawnerManager CLASS values
        roadSpawnerManager.counter = 0;
        roadSpawnerManager.roadSelector=1;
        roadSpawnerManager.rotationSelector=0;
        roadSpawnerManager.difficultySelector=5;

        StartingRoadPieces(); // Create new starting roadPieces

        // Initialize CarController CLASS values
        blueCar.episodeDoneFlag= false;
        blueCar.transform.position = new Vector3(0,0.5f,0);
        blueCar.RandomReposition();

        blueCar.roadsStraightPassedCounter=0;// Counter if a TriggerSpot has been passed by
        blueCar.roadsTurnPassedCounter=0;// Counter if a TriggerSpot has been passed by
        
        roadsStraightPassedCounterAgent=0;// Counter if a TriggerSpot has been passed by
        roadsTurnPassedCounterAgent=0;// Counter if a TriggerSpot has been passed by

        carRigidBody.velocity= new Vector3(0,0,0);// Car starts without previous speed

        roadsDestroyCounter=0;// Have10MaxRoads()
        episodeTimer=0; // reset timer
        stackedTime=0;  // reset timer 
    }
    
    // Destroys everything when episode ends
    private void DestroyObjectsInScene()
    {
        // Saved on a matrix and then delete
        tempDestroy= GameObject.FindGameObjectsWithTag("RoadPiece");
        foreach(GameObject tempD in tempDestroy){Destroy(tempD);}
    }
    
    // First road pieces
    private void StartingRoadPieces()
    {   
        roadSpawner.SpawnRoadPiece(0,0);
        roadSpawner.SpawnRoadPiece(1,0);
        roadSpawner.SpawnRoadPiece(1,0);
        roadSpawner.SpawnRoadPiece(1,0);
        roadSpawner.SpawnRoadPiece(1,0);
    }

    // Deletes the last road piece while car is moving forward
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