using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PowerUp : NetworkBehaviour
{
    #region ABOUT
    /**
     * Various power-ups will enhance the tank's fighting strength, health, etc.
     * This script handles the varying types of power-ups, and makes them blink.
     **/
    #endregion

    #region VARIABLES
    public enum PowerUpType { HPBoost, DMGBoost, FireRateBoost }
    [Tooltip("The type that 'this' powerup is of.")]
    public PowerUpType typeOfPowerUp;

    // The rate to have them blink
    private const float BLINK_RATE = 1.0f;
    private float lastBlink = 0.0f;
    private MeshRenderer mRenderer; // 'this' meshrenderer
    #endregion

    /// <summary>
    /// Fetches the MeshRenderer from the capsule.
    /// </summary>
	void Start () {
        mRenderer = this.GetComponent<MeshRenderer>();
	}
	
	/// <summary>
	/// If we have authority to, command the server to make it blink
	/// </summary>
	void Update () {
        if (!hasAuthority) return;

        CmdBlink();
	}

    /// <summary>
    /// Makes them blink every second.
    /// </summary>
    [Command]
    private void CmdBlink()
    {
        // This is on the server
        if (Time.time > BLINK_RATE + lastBlink)
        {
            if (mRenderer.material.color == Color.red) mRenderer.material.color = Color.white;
            else mRenderer.material.color = Color.red;

            lastBlink = Time.time;
        }
    }
}
