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
    #endregion

    /// <summary>
    /// Spawns a player tank prefab when a player connects.
    /// </summary>
	void Start () {
        if (! isLocalPlayer)
        {
            // This object belongs to another player
            return;
        }

        // Command the server to spawn a tank
        CmdSpawnTank();
	}
	
	// Update is called once per frame
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
        // On server so spawn tank for the connected player
        GameObject tank = GameObject.Instantiate(playerTankPrefab);
        mTank = tank;

        // Spawn the tank with authority
        NetworkServer.SpawnWithClientAuthority(tank, connectionToClient);
    }

    private void OnPlayerDisconnected(NetworkPlayer player)
    {
        NetworkServer.Destroy(mTank);
    }

    /// <summary>
    /// Moves 'this' (mTank) tank on the server.
    /// </summary>
    /*
    [Command]
    private void CmdMoveTank()
    {
        if (mTank == null) return;

        mTank.transform.Translate(0.0f, 0, 0.05f);
    }
    */
}
