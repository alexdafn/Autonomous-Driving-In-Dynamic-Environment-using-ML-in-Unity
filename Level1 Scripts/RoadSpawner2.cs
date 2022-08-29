using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSpawner2 : MonoBehaviour
{
    public List<GameObject> roads;
    private float offset = 40f;

    // Start is called before the first frame update
    void Start()
    {
        // Corrects the order of the elements in the list if needed
        /* Compile error, OrderBy issue
        if (roads != null && roads.Count > 0)
        {
            roads = roads.OrderBy(r => r.transform.position.z).ToList();
        } */
    }

    // Moves the first road behind, after the third road in front
    public void MoveRoad()
    {
        GameObject movedRoad = roads[0];
        roads.Remove(movedRoad);
        float newZ = roads[roads.Count-1].transform.position.z + offset;
        movedRoad.transform.position = new Vector3 (0,0, newZ);
        roads.Add(movedRoad);
    }
    

}
//////LATHOS TOPOTHETHSH NEOY DROMOY