using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random=UnityEngine.Random;


public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private RoadSpawnerManager roadSpawnerManager;

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float brakeForce;
    private bool isBraking;
    private int actionNumberGenerator;

    [SerializeField] private float motorForce = 1100f;
    public float currentBrakeForce = 2500f;
    public float maxSteerAngle = 120f;

    //Change in the center of the mass so the car doesn't flip
    public float balanceStabilizer = -0.9f; 
    public Rigidbody rb;

    //random position
    public float Radius = 0.8f;

    //MakeCarAction()
    public float actionNumber;

    //PositionCalculation()
    private Vector3 previousPosition, currentPosition, differenceVector;
    public float difference; // totalDifference;

    //RandomReposition()
    int randomRotationNumber;

    //Gia xrhsh se alles synarthseis
    public bool episodeDoneFlag; //isws na kanw gia kathe periptwsh, crashed, fallen, timeexpired
    
    public bool sidePenaltyFlag;
    public bool turnSidePenaltyFlag;

    public bool leftTurnSidePenaltyFlag;//για όταν πατάει πλαϊνό (ΣΤΡΟΦΗΣ)
    public bool rightTurnSidePenaltyFlag;//για όταν πατάει πλαϊνό (ΣΤΡΟΦΗΣ)
    public bool leftSidePenaltyFlag;//για όταν πατάει πλαϊνό (ΕΥΘΕΙΑΣ)
    public bool rightSidePenaltyFlag;//για όταν πατάει πλαϊνό (ΕΥΘΕΙΑΣ)
    public bool timerStackedFlag; //για όταν είναι ακίνητο για πολύ ώρα το αμάξι

    public int penaltyStraightSideCounter;
    public int penaltyTurnSideCounter;

    //gia xrhsh sthn antamoivh toy praktora
    public int roadsStraightPassedCounter=0;
    public int roadsTurnPassedCounter=0;


    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    void Start()
    {
        roadSpawnerManager = GameObject.FindObjectOfType<RoadSpawnerManager>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //GetInput();
        //HandleMotor();
        //HandleSteering();
        UpdateWheels();
        //ApplyBraking();

        previousPosition = transform.position;
    }

    private void LateUpdate()
    {
        PositionCalculation();
    }

    //Not in use because of CarAgentScript
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

    public void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput / 2.3f;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

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
        if(actionNumber==1){//forward
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
        }else if (actionNumber==5) //0 FIR STEERING
        {
            print("did nothing for steering");
            currentSteerAngle = 0* 1 / 2.3f;
            frontLeftWheelCollider.steerAngle = currentSteerAngle;
            frontRightWheelCollider.steerAngle = currentSteerAngle;
        }
    }

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

    public void PositionCalculation()
    {
        difference = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.y) + Mathf.Abs(rb.velocity.z);
    }

    // Collision Handling
    private void OnTriggerEnter(Collider other) 
    {
        if((other.tag == "TriggerSpot"))
        {
            roadSpawnerManager.SpawnTriggerEntered();
            roadsStraightPassedCounter++;
        }

        if(other.tag == "TurnTriggerSpot")
        {
            roadSpawnerManager.SpawnTriggerEntered();
            roadsTurnPassedCounter++;

        }

        if((other.tag == "Tree") || (other.tag == "House") || (other.tag == "Lamp") || (other.tag == "GreenFloor"))
        {
            episodeDoneFlag = true;
        }

        /*
        if(other.tag == "SideRoad")
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            penaltyStraightSideCounter++;
            sidePenaltyFlag=true;
        }

        if(other.tag == "TurnSideRoad")
        {
            penaltyTurnSideCounter++;
            turnSidePenaltyFlag=true;
        }*/

        if(other.tag =="LeftSide"){leftSidePenaltyFlag=true;timerStackedFlag=true;}
        if(other.tag =="RightSide"){rightSidePenaltyFlag=true;timerStackedFlag=true;}

        if(other.tag =="LeftTurn"){leftTurnSidePenaltyFlag=true;timerStackedFlag=true;}
        if(other.tag =="RightTurn"){rightTurnSidePenaltyFlag=true;timerStackedFlag=true;}
    }
    private void OnTriggerExit(Collider other)
    {
        /*
        if(other.tag == "SideRoad")
        {
            sidePenaltyFlag=false;
        }
        if(other.tag == "TurnSideRoad")
        {
            turnSidePenaltyFlag=false;
        }*/

        if(other.tag =="LeftSide"){leftSidePenaltyFlag=false;timerStackedFlag=false;}
        if(other.tag =="RightSide"){rightSidePenaltyFlag=false;timerStackedFlag=false;}

        if(other.tag =="LeftTurn"){leftTurnSidePenaltyFlag=false;timerStackedFlag=false;}
        if(other.tag =="RightTurn"){rightTurnSidePenaltyFlag=false;timerStackedFlag=false;}
    }
}
