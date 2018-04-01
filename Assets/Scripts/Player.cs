using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    #region ABOUT
    /**
     * Tracks important information for each player in the networked environment.
     **/
    #endregion

    #region VARIABLES
    [Tooltip("The player's tank object prefab that is created on launch of the networked game.")]
    public GameObject playerTankPrefab;
    private GameObject mTank;
    private SpawnPoints mSpawnPoints; // The spawn points available to us
    #endregion

    /// <summary>
    /// Spawns a player tank prefab when a player connects.
    /// </summary>
	void Start () {
        if (! isLocalPlayer) return;

        // Command the server to spawn a tank
        CmdSpawnTank();
	}
	
	/// <summary>
	/// Ensures local player authority.
	/// </summary>
	void Update () {
		// Update runs on every computer whether or not they own this particular player
        if (! isLocalPlayer) return;
	}

    /// <summary>
    /// Spawns a tank prefab for a connected player.
    /// </summary>
    [Command]
    private void CmdSpawnTank()
    {
        // Acquire the spawnPoints object
        mSpawnPoints = GameObject.Find("SpawnPoints").GetComponent<SpawnPoints>();
        // Acquire a spawn position
        Transform spawnPos = mSpawnPoints.GetFreeSpawnPoint();
        // On server so spawn tank for the connected player
        GameObject tank = GameObject.Instantiate(playerTankPrefab);
        // Set the tank's transform to the spawnPos
        tank.transform.position = spawnPos.position;
        tank.transform.rotation = spawnPos.rotation;
        // Then set the player's tank to it
        mTank = tank;

        // Spawn the tank with authority
        NetworkServer.SpawnWithClientAuthority(tank, connectionToClient);
    }

    /// <summary>
    /// When we disconnect from the server, destroy the player.
    /// </summary>
    /// <param name="player">The networked player.</param>
    private void OnPlayerDisconnected(NetworkPlayer player)
    {
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
        NetworkServer.Destroy(mTank);
    }
}
