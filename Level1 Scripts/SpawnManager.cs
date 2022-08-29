using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    RoadSpawner2 roadSpawner;
    // Start is called before the first frame update
    void Start()
    {
        roadSpawner = GetComponent<RoadSpawner2>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnTriggerEntered()
    {
        roadSpawner.MoveRoad();
    }
}
