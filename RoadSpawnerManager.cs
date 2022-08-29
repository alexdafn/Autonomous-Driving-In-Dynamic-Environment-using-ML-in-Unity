using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSpawnerManager : MonoBehaviour
{
    private RoadSpawner roadSpawner;

    public int counter = 0;
    public int roadSelector=1;
    public int rotationSelector=0;
    public int difficultySelector=5;
    // Start is called before the first frame update
    public void Start()
    {
        roadSpawner = GetComponent<RoadSpawner>();
        roadSpawner = GameObject.FindObjectOfType<RoadSpawner>();
    }

    

    public void SpawnTriggerEntered()
    {
        //Difficulty Selection according to the roads that have been spawned
        if (counter>30 && counter<=60){ 
            difficultySelector=4;
            Debug.Log("counter is " + counter +"AND difficulty is "+ difficultySelector);
        }else if(counter>60 && counter<=90){
            difficultySelector=3;
            Debug.Log("counter is " + counter +"AND difficulty is "+ difficultySelector);
        }else if(counter>90){
            difficultySelector=2;
            Debug.Log("counter is " + counter +"AND difficulty is "+ difficultySelector);
        }else{
            Debug.Log("counter is " + counter +"AND STARTING difficulty is "+ difficultySelector);
        }

        //roadSpawner.SpawnRoadPiece(1,0);

        
        //Road Spawning
        if (counter%difficultySelector!=0){
            roadSelector=1;
            //roadSpawner.SpawnRoadPiece(1,0);
            roadSpawner.SpawnRoadPiece(roadSelector,rotationSelector);
            roadSpawner.maxRoadsActiveCounter++;
           
            counter++;
        }else if (counter%difficultySelector==0){
            //roadSelector=rnd.Next(1,4);
            roadSelector=Random.Range(1,4);
            //roadSpawner.SpawnRoadPiece(3,0);
            roadSpawner.SpawnRoadPiece(roadSelector,rotationSelector);
            if(roadSelector==2){
                rotationSelector-=90;
            }else if (roadSelector==3){
                rotationSelector+=90;
            }
            roadSpawner.maxRoadsActiveCounter++;
            counter++;
        }  /*
        else if (counter>5 && counter<10){
            roadSpawner.SpawnRoadPiece(1,90);
            counter++;
        }else if(counter==10){
            roadSpawner.SpawnRoadPiece(3,90);
            counter++;
        }else{
            roadSpawner.SpawnRoadPiece(1,180);
            counter++;
        }*/
        //if (1) then 
       // Destroy(gameObject, 2);
    }
      /*
    public void SpawnTriggerEntered()
    {
        if (counter<5){
            //roadSpawner.SpawnRoadPiece(1,0);
            roadSpawner.SpawnRoadPiece(roadSelector,rotationSelector);
            counter++;
        }else if (counter==5){
            roadSpawner.SpawnRoadPiece(3,0);
            counter++;
        }
        else if (counter>5 && counter<10){
            roadSpawner.SpawnRoadPiece(1,90);
            counter++;
        }else if(counter==10){
            roadSpawner.SpawnRoadPiece(3,90);
            counter++;
        }else{
            roadSpawner.SpawnRoadPiece(1,180);
            counter++;
        }
    }*/
}
