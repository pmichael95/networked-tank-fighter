using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnPoints : NetworkBehaviour
{
    #region ABOUT
    /*
     * Spawn points are where the 2 player tanks can spawn.
     * We have spawn point A and B.
     * Based on what's taken, we spawn in another position.
     * First one is chosen at random, other is the last one left of the two.
     */
    #endregion

    #region VARIABLES
    [Tooltip("Possible spawn positions. Needs to be set through Inspector.")]
    public GameObject[] spawnPositions;
    // Boolean flags to check if the spawn points are taken or not.
    public bool spawnATaken = false;
    public bool spawnBTaken = false;
    #endregion

    /// <summary>
    /// Verifies which spawn positions are taken or not.
    /// Then, returns a transform of the free one, or random if first time.
    /// </summary>
    /// <returns>Transform to spawn the tank at.</returns>
    public Transform GetFreeSpawnPoint()
    {
        if (!spawnATaken && !spawnBTaken)
        {
            // First time we assign a spot point
            int randomPos = Random.Range(0, 2);
            Debug.Log("randomPos = " + randomPos);
            if (randomPos == 0)
            {
                spawnATaken = true;
                return spawnPositions[0].transform;
            }
            else
            {
                spawnBTaken = true;
                return spawnPositions[1].transform;
            }
        }
        else if (spawnATaken && spawnBTaken)
        {
            return spawnPositions[0].transform;
        }
        else
        {
            if (spawnATaken && !spawnBTaken)
            {
                spawnBTaken = true;
                return spawnPositions[1].transform;
            }
            else
            {
                spawnATaken = true;
                return spawnPositions[0].transform;
            }
        }
    }
}
