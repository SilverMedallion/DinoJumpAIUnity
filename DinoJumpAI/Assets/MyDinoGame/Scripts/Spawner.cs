
using UnityEngine;

public class Spawner : MonoBehaviour
{

    //need to add an atribute to show up in the editor
    [System.Serializable]
    public struct SpawnableObject
    {
        public GameObject prefab;
        [Range(0f, 1f)]
        public float spawnChance;
    }

    //array of prefabs we can spawn
    public SpawnableObject[] objects;

    //will spawn a object randomly between 1 and 2 seconds
    public float minSpawnRate = 1f;
    public float maxSpawnRate = 2f;

    private void OnEnable()
    {
        //call spawn at a random time between the min and max spawn rate
        Invoke(nameof(Spawn), Random.Range(minSpawnRate, maxSpawnRate));
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
    private void Spawn()
    {
        //get random float between 0 and 1
        float spawnChance = Random.value;

        //loop for number of objects in array
        foreach(var obj in objects)
        {
            //check if the random spawn chance is less than the objects spawn chance then spawn that object
            if(spawnChance < obj.spawnChance)
            {
                //spawn a prefab of the obstacle to spawn at the position of the spawner
                GameObject obstacle = Instantiate(obj.prefab);
                //want it to be += because if we spawn the bird if we want it to be up in the air if we change the prefabs positition it will spawn it based on the prefabs position. so the bird at 0.5 Y for example
                obstacle.transform.position += transform.position;
                //once spawned break out the loop
                break;
                
            }
            //we do this because if the random value is not less than our highest value it will keep decreasing it until it is less the one of the other objects, with a chance it will never get lower and spawn nothing
            spawnChance -= obj.spawnChance;
        }

        //once object has been spawned spawn another object
        Invoke(nameof(Spawn), Random.Range(minSpawnRate, maxSpawnRate));

    }

}
