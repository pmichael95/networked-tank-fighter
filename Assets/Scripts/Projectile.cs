using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Projectile : NetworkBehaviour
{
    #region ABOUT
    /**
     * Handles primarily the projectile's collision.
     **/
    #endregion

    void OnCollisionEnter(Collision col)
    {   
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("COLLIDED WITH PLAYER");
            // Deal damage on the server (which then calls the SyncVar)
            CmdDamagePlayer(col.gameObject);
            // Then destroy the projectile
            NetworkServer.Destroy(this.gameObject);
        }
        else if (col.gameObject.tag == "Obstacle")
        {
            NetworkServer.Destroy(col.gameObject);
            NetworkServer.Destroy(this.gameObject);
        }
        else if (col.gameObject.tag == "BoundingWalls")
        {
            NetworkServer.Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Handles dealing damage to a player.
    /// </summary>
    /// <param name="playerTank">The player's tank.</param>
    [Command]
    private void CmdDamagePlayer(GameObject playerTank)
    {
        playerTank.GetComponent<Tank>().playerHealth -= 20;
    }
}
