using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AIProjectile : NetworkBehaviour {
    #region ABOUT
    /**
     * The AI's fired projectile. Can only damage players or destroys walls.
     **/
    #endregion

    #region VARIABLES
    [Tooltip("Whether or not this projectile does increased damage.")]
    public AudioClip mClip; // The clip attached to the audio source
    #endregion

    /// <summary>
    /// When colliding, handle three cases.
    /// Case 1: Collided with another networked player, so deal damage, and play audio.
    /// Case 2: Collided with a destructible wall bit, play audio and destroy it.
    /// Case 3: Collided with bounding walls, just destroy this.
    /// </summary>
    /// <param name="col">The collided object the projectile hit.</param>
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            AudioSource.PlayClipAtPoint(mClip, this.transform.position);
            // Deal damage on the server (which then calls the SyncVar)
            CmdDamagePlayer(col.gameObject);
            // Then destroy the projectile
            NetworkServer.Destroy(this.gameObject);
        }
        else if (col.gameObject.tag == "Obstacle")
        {
            AudioSource.PlayClipAtPoint(mClip, this.transform.position);
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
        Tank mTank = playerTank.GetComponent<Tank>();
        // Regular damage without powerup.
        mTank.playerHealth -= 20;
    }
}
