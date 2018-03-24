using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Tank : NetworkBehaviour
{
    #region ABOUT
    /**
     * The tank being controlled by a networked player.
     **/
    #endregion

    #region VARIABLES
    [Tooltip("The prefab for the tank's projectile when firing.")]
    public GameObject projectilePrefab;

    // --- POWERUP FLAGS --- //
    // These will identify if the tank has been powered up or not.
    public bool hasHPBoost { get; set; }
    public bool hasDMGBoost { get; set; }
    public bool hasFireRateBoost { get; set; }

    [SyncVar(hook = "OnTakeDamage")]
    public int playerHealth = 100; // By default 100HP
    // The instantiated projectile that we shot
    private GameObject mProjectile;
    // Rotation speed multiplier
    private const float ROT_SPEED = 30.0f;
    private float fireRate = 1.5f;
    private float lastShot = 0.0f;
    #endregion

    /// <summary>
	/// Moves this tank if we have authority.
	/// </summary>
	void Update () {
        if (! hasAuthority) return;

        // -- From here on, we have authority

        if (hasHPBoost)
        {
            CmdUpdatePlayerHP();
            hasHPBoost = false;
        }

        if (Input.GetKey(KeyCode.W))
        {
            // Move forward
            this.transform.Translate(0.0f, 0.0f, 0.025f);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            // Move backwards
            this.transform.Translate(0.0f, 0.0f, -0.025f);
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            // Rotate to the left
            this.transform.Rotate(0.0f, - (Time.deltaTime * ROT_SPEED), 0.0f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // Rotate to the right
            this.transform.Rotate(0.0f, Time.deltaTime * ROT_SPEED, 0.0f);
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Fire
            CmdSpawnProjectile();
        }
	}

    /// <summary>
    /// Spawns a projectile on the server.
    /// </summary>
    [Command]
    private void CmdSpawnProjectile()
    {
        if (Time.time > fireRate + lastShot && !hasFireRateBoost)
        {
            mProjectile = GameObject.Instantiate(projectilePrefab);
            mProjectile.transform.position = this.transform.position + this.transform.forward; // Offset by adding transform.forward so it won't hit the firing tank
            mProjectile.GetComponent<Rigidbody>().velocity = this.transform.TransformDirection(Vector3.forward * 10.0f);

            // Spawn it on server as well
            NetworkServer.Spawn(mProjectile);
            lastShot = Time.time;
        }
        else if (hasFireRateBoost)
        {
            mProjectile = GameObject.Instantiate(projectilePrefab);
            mProjectile.transform.position = this.transform.position + this.transform.forward; // Offset by adding transform.forward so it won't hit the firing tank
            mProjectile.GetComponent<Rigidbody>().velocity = this.transform.TransformDirection(Vector3.forward * 10.0f);

            // Spawn it on server as well
            NetworkServer.Spawn(mProjectile);
        }
    }

    /// <summary>
    /// When we have the HP Boost powerup, update HP to 200.
    /// </summary>
    [Command]
    private void CmdUpdatePlayerHP()
    {
        this.playerHealth = 200;
    }

    /// <summary>
    /// When we collide with a projectile, we are dealt damage.
    /// </summary>
    public void OnTakeDamage(int HP)
    {
        this.playerHealth = HP;
        if (this.playerHealth <= 0)
        {
            // We died
            NetworkServer.Destroy(this.gameObject);
        }
    }
}
