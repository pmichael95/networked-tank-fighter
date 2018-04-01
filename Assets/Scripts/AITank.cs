using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AITank : NetworkBehaviour {

    #region ABOUT
    /**
     * The AI enemy tanks.
     **/
    #endregion

    #region VARIABLES
    [Tooltip("The prefab for the tank's projectile when firing.")]
    public GameObject projectilePrefab;

    // The instantiated projectile that we shot
    private GameObject mProjectile;
    // 'This' character script
    public Character mCharacter;
    // Rotation speed multiplier
    private const float ROT_SPEED = 45.0f;
    private float fireRate = 2.5f; // 1s longer than regular tanks 
    private float lastShot = 0.0f;
    #endregion

    /// <summary>
    /// Acquires the character component of the AI tanks.
    /// This is used to assign targets, and determine if we can move or not (if in range to shoot).
    /// </summary>
    void Start()
    {
        mCharacter = GetComponent<Character>();
    }

    /// <summary>
    /// Checks if we've got Line of Sight, and within range, to shoot a player tank.
    /// </summary>
    void Update()
    {
        if (!hasAuthority) return;

        // Acquire the target from Character
        GameObject target = mCharacter.target;
        if (target)
        {
            if (Vector3.Distance(this.transform.position, target.transform.position) < 5.0f)
            {
                if (!Physics.Linecast(this.transform.position, target.transform.position))
                {
                    // Since we hit a player, initiate fire, but also freeze the AI Tank to shoot in place
                    mCharacter.canMove = false;
                    CmdSpawnProjectile();
                }
                else
                {
                    mCharacter.canMove = true;
                }
            }
            else
            {
                mCharacter.canMove = true;
            }
        }
        else
        {
            mCharacter.canMove = true;
        }
    }

    /// <summary>
    /// Spawns a projectile on the server.
    /// </summary>
    [Command]
    private void CmdSpawnProjectile()
    {
        if (Time.time > fireRate + lastShot)
        {
            mProjectile = GameObject.Instantiate(projectilePrefab);
            mProjectile.transform.position = this.transform.position + this.transform.forward; // Offset by adding transform.forward so it won't hit the firing tan
            mProjectile.GetComponent<Rigidbody>().velocity = this.transform.TransformDirection(Vector3.forward * 10.0f);

            // Spawn it on server as well
            NetworkServer.Spawn(mProjectile);
            lastShot = Time.time;
        }
    }
}
