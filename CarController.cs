using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random=UnityEngine.Random;


public class CarController : MonoBehaviour
{
    // Input for movement
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    // Making a reference for an instance of class RoadSpawnerManager
    private RoadSpawnerManager roadSpawnerManager;

    private int actionNumberGenerator;
    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float brakeForce;
    private bool isBraking;
    
    // Initial driving values
    [SerializeField] private float motorForce = 1100f;
    public float currentBrakeForce = 2500f;
    public float maxSteerAngle = 120f;

    // Change in the center of the mass so the car doesn't flip
    public float balanceStabilizer = -0.9f; 
    public Rigidbody rb; //Reference on car's rigid body

    // Random position
    public float Radius = 0.8f;

    // MakeCarAction()
    public float actionNumber;

    // PositionCalculation()
    public float difference; // totalDifference;

    // RandomReposition()
    int randomRotationNumber;

    public bool episodeDoneFlag; 

    public bool leftTurnSidePenaltyFlag;// when car steps on the side of the road(TURN)
    public bool rightTurnSidePenaltyFlag;// when car steps on the side of the road(TURN)
    public bool leftSidePenaltyFlag;// when car steps on the side of the road(STRAIGHT)
    public bool rightSidePenaltyFlag;// when car steps on the side of the road(STRAIGHT)
    public bool timerStackedFlag; // used to check the time that the car doens't move

    public int roadsStraightPassedCounter=0;
    public int roadsTurnPassedCounter=0;

    // Getting access on the wheels movement
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    // Getting access on the wheels position in Unity 3D space system
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    void Start()
    {
        // Getting a reference for an instance of class RoadSpawnerManager
        roadSpawnerManager = GameObject.FindObjectOfType<RoadSpawnerManager>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Most functions used before the creation of the CarAgent Script, for movement testing
        //GetInput();
        //HandleMotor();
        //HandleSteering();
        UpdateWheels();
        //ApplyBraking();
    }

    private void LateUpdate()
    {
        PositionCalculation();
    }

    // Not in use because of CarAgentScript, used in previous version
    public void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBraking = Input.GetKey(KeyCode.Space);
        if(isBraking){
            currentBrakeForce = 3000;
        }else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)){
            currentBrakeForce = 0;
        }
    }

    // Not in use because of MakeCarAction(), used in previous version
    public void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = 1 * motorForce ;// 2.3f;
        frontRightWheelCollider.motorTorque = 1 * motorForce ;// 2.3f;
    }

    public void ApplyBraking()
    {
        currentBrakeForce=2500;
        frontRightWheelCollider.brakeTorque = currentBrakeForce;
        frontLeftWheelCollider.brakeTorque = currentBrakeForce;
        rearRightWheelCollider.brakeTorque = currentBrakeForce;
        rearLeftWheelCollider.brakeTorque = currentBrakeForce;
    }

    // Not in use because of MakeCarAction(), used in previous version
    public void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput / 2.3f;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    // Realistic wheel movement update
    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    // Realistic wheel movement update
    private void UpdateSingleWheel(WheelCollider WheeleCollider, Transform WheeleTransform)
    {
        Vector3 pos;
        Quaternion rot;
        WheeleCollider.GetWorldPose(out pos, out rot);
        WheeleTransform.rotation = rot;
        WheeleTransform.position = pos;
    }

    //Connection with CarAgentScript, when an action is chosen
    public void MakeCarAction(float actionNumber)
    {
        if(actionNumber==1){ //forward
            print("Action Taken: Go forward");
            currentBrakeForce = 0;
            frontRightWheelCollider.brakeTorque = currentBrakeForce;
            frontLeftWheelCollider.brakeTorque = currentBrakeForce;
            rearRightWheelCollider.brakeTorque = currentBrakeForce;
            rearLeftWheelCollider.brakeTorque = currentBrakeForce;
            frontLeftWheelCollider.motorTorque = 1 * motorForce ;
            frontRightWheelCollider.motorTorque = 1 * motorForce ;

        }else if(actionNumber==2){ //left
            print("Action Taken: Steer left");
            currentSteerAngle = maxSteerAngle * -1 / 2.3f;
            frontLeftWheelCollider.steerAngle = currentSteerAngle;
            frontRightWheelCollider.steerAngle = currentSteerAngle;

            currentBrakeForce = 0;
            frontRightWheelCollider.brakeTorque = currentBrakeForce;
            frontLeftWheelCollider.brakeTorque = currentBrakeForce;
            rearRightWheelCollider.brakeTorque = currentBrakeForce;
            rearLeftWheelCollider.brakeTorque = currentBrakeForce;

        }else if(actionNumber==3){ //right
            print("Action Taken: Steer Right");
            currentSteerAngle = maxSteerAngle * 1 / 2.3f;
            frontLeftWheelCollider.steerAngle = currentSteerAngle;
            frontRightWheelCollider.steerAngle = currentSteerAngle;

            currentBrakeForce = 0;
            frontRightWheelCollider.brakeTorque = currentBrakeForce;
            frontLeftWheelCollider.brakeTorque = currentBrakeForce;
            rearRightWheelCollider.brakeTorque = currentBrakeForce;
            rearLeftWheelCollider.brakeTorque = currentBrakeForce;
        }else if(actionNumber==4){ //brake
            print("Action Taken: Brake");
            ApplyBraking();
            
        }else if(actionNumber==0) //0 FOR ACCELERATION-BRAKING
        {
            print("did nothing for speed");
            frontLeftWheelCollider.motorTorque = 1 * 0 ;
            frontRightWheelCollider.motorTorque = 1 * 0 ;
            //return;
        }else if (actionNumber==5) //0 FOR STEERING
        {
            print("did nothing for steering");
            currentSteerAngle = 0* 1 / 2.3f;
            frontLeftWheelCollider.steerAngle = currentSteerAngle;
            frontRightWheelCollider.steerAngle = currentSteerAngle;
        }
    }

    // Random reposition around the initial position
    public void RandomReposition()
    {
        transform.position = Random.insideUnitCircle*Radius;
        
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation = Quaternion.Euler(0,0,0);
        randomRotationNumber=Random.Range(-40,40);
        transform.Rotate(0,randomRotationNumber,0);
    }

    /*
    private void CarHasFallen()
    {
        if(transform.position.y<-10)
        {
            episodeDoneFlag = true;
        }
    }*/

    // Used to check if the car is almost standing still
    public void PositionCalculation()
    {
        difference = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.y) + Mathf.Abs(rb.velocity.z);
    }

    // Collision Handling
    private void OnTriggerEnter(Collider other) 
    {
        // if a straight road piece has been passed by
        if((other.tag == "TriggerSpot"))
        {
            roadSpawnerManager.SpawnTriggerEntered();
            roadsStraightPassedCounter++;
        }
        
        // if a turn road piece has been passed by
        if(other.tag == "TurnTriggerSpot")
        {
            roadSpawnerManager.SpawnTriggerEntered();
            roadsTurnPassedCounter++;

        }

        // if an environmental object gets hit, ends the episode of training
        if((other.tag == "Tree") || (other.tag == "House") || (other.tag == "Lamp") || (other.tag == "GreenFloor"))
        {
            episodeDoneFlag = true;
        }

        // Check if the car steps on the side of the road
        if(other.tag =="LeftSide"){leftSidePenaltyFlag=true;timerStackedFlag=true;}
        if(other.tag =="RightSide"){rightSidePenaltyFlag=true;timerStackedFlag=true;}

        if(other.tag =="LeftTurn"){leftTurnSidePenaltyFlag=true;timerStackedFlag=true;}
        if(other.tag =="RightTurn"){rightTurnSidePenaltyFlag=true;timerStackedFlag=true;}
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the car exits the side of the road
        if(other.tag =="LeftSide"){leftSidePenaltyFlag=false;timerStackedFlag=false;}
        if(other.tag =="RightSide"){rightSidePenaltyFlag=false;timerStackedFlag=false;}

        if(other.tag =="LeftTurn"){leftTurnSidePenaltyFlag=false;timerStackedFlag=false;}
        if(other.tag =="RightTurn"){rightTurnSidePenaltyFlag=false;timerStackedFlag=false;}
    }
}
