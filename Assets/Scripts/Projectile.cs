using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(AudioSource))]
public class Projectile : NetworkBehaviour
{
    #region ABOUT
    /**
     * Handles primarily the projectile's collision, and plays appropriate audio.
     **/
    #endregion

    #region VARIABLES
    [Tooltip("Whether or not this projectile does increased damage.")]
    public bool hasDMGBoost = false;
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
        else if (col.gameObject.tag == "Enemy")
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
        if (hasDMGBoost)
        {
            // Since we have the DMG boost powerup, we deal double damage.
            mTank.playerHealth -= 40;
        }
        else
        {
            // Regular damage without powerup.
            mTank.playerHealth -= 20;
        }
    }
}
