using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    public List<GameObject> roadPrefabs; // List on Unity Editor with possible road pieces
    public Vector3 nextSpawnPoint; //z=-15 in unity
    public int turnRoadFlag = 0 ;
    public int maxRoadsActiveCounter=0;
    int chance; 
    int houseDestroyFlag = 7;
    int treeDestroyFlag = 5;

    // Spawns a given road piece, then randomly, with chance deletes trees and houses
    public void SpawnRoadPiece(int roadIndex = 1, int rotationValue=0){
        GameObject temp = Instantiate(roadPrefabs[roadIndex], nextSpawnPoint, Quaternion.Euler(0,rotationValue,0));
        nextSpawnPoint = temp.transform.GetChild(1).transform.position; //moves nextSpawnPoint at the end of the last piece placed
       
       // Flags for the next road piece to be placed with the right angle
        if (roadIndex==2){
            turnRoadFlag=2; //left Turn
        }else if (roadIndex==3){
            turnRoadFlag=3; //right Turn
        }

        // Chance for something to be destroyed
        chance = Random.Range(0,101);
        if(roadIndex==1 && turnRoadFlag==0){
            if(chance<91){
                Destroy(temp.transform.GetChild(houseDestroyFlag).gameObject);
                houseDestroyFlag=8;
                if(chance%2==0){
                    Destroy(temp.transform.GetChild(houseDestroyFlag).gameObject);
                    houseDestroyFlag=7;
                }
            }
            if (chance<60){
                Destroy(temp.transform.GetChild(treeDestroyFlag).gameObject);
                treeDestroyFlag=5;
                if(chance%2==0){
                    Destroy(temp.transform.GetChild(treeDestroyFlag).gameObject);
                    treeDestroyFlag=6;
                }
            }
        }else if (roadIndex==1 && turnRoadFlag==2){ //left turn
            Destroy(temp.transform.GetChild(7).gameObject);
            if(chance<61){
                Destroy(temp.transform.GetChild(8).gameObject);
                Destroy(temp.transform.GetChild(treeDestroyFlag).gameObject);
                treeDestroyFlag=5;
                if(chance%2==0){
                    Destroy(temp.transform.GetChild(treeDestroyFlag).gameObject);
                    treeDestroyFlag=6;
                }
            }
            turnRoadFlag=0;
        }else if (roadIndex==1 && turnRoadFlag==3){ //right turn
            Destroy(temp.transform.GetChild(8).gameObject);
            if(chance<61){
                Destroy(temp.transform.GetChild(7).gameObject);
                Destroy(temp.transform.GetChild(treeDestroyFlag).gameObject);
                treeDestroyFlag=5;
                if(chance%2==0){
                    Destroy(temp.transform.GetChild(treeDestroyFlag).gameObject);
                    treeDestroyFlag=6;
                }
            }
            turnRoadFlag=0;
        }else if (roadIndex==2 || roadIndex==3){
            if (chance<15 || chance>85){
                Destroy(temp.transform.GetChild(9).gameObject);
            }
            if (chance<40){
                Destroy(temp.transform.GetChild(treeDestroyFlag+2).gameObject);
                treeDestroyFlag=5;
                if(chance%2==0){
                    Destroy(temp.transform.GetChild(treeDestroyFlag+2).gameObject);
                    treeDestroyFlag=6;
                }
            }
        }
    }
}
