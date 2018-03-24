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

    // 0: white, 1: red
    [SyncVar(hook="OnUpdateColor")] 
    private int meshColor = 0;
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
	/// At each frame, command the server to make it blink.
	/// </summary>
	void Update () {
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
            if (mRenderer.material.color == Color.red)
            {
                mRenderer.material.color = Color.white;
                meshColor = 0;
            }
            else
            {
                mRenderer.material.color = Color.red;
                meshColor = 1;
            }

            lastBlink = Time.time;
        }
    }

    /// <summary>
    /// When we update the color on the server side, update it on the client's as well.
    /// </summary>
    /// <param name="color">The color that it changed to.</param>
    private void OnUpdateColor(int color)
    {
        meshColor = color;
        if (meshColor == 0)
        {
            mRenderer.material.color = Color.white;
        }
        else
        {
            mRenderer.material.color = Color.red;
        }
    }

    /// <summary>
    /// When a powerup collides with a player, enhance them and destroy it.
    /// </summary>
    /// <param name="col">The collided player (tank) object.</param>
    void OnCollisionEnter(Collision col)
    {
        Tank mTank = col.gameObject.GetComponent<Tank>();
        if (mTank)
        {
            switch (typeOfPowerUp)
            {
                case PowerUpType.DMGBoost:
                    mTank.hasDMGBoost = true;
                    break;
                case PowerUpType.FireRateBoost:
                    mTank.hasFireRateBoost = true;
                    break;
                case PowerUpType.HPBoost:
                    mTank.hasHPBoost = true;
                    break;
                default:
                    Debug.LogError("ERRORR::UNKNOWN_POWERUP_TYPE_GIVEN");
                    break;
            }
            NetworkServer.Destroy(this.gameObject);
        }
    }
}
