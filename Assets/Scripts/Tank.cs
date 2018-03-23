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
	
	/// <summary>
	/// Moves this tank if we have authority.
	/// </summary>
	void Update () {
        if (! hasAuthority) return;

        // -- From here on, we have authority

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
        else if (Input.GetKey(KeyCode.A))
        {
            // Rotate to the left
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // Rotate to the right
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            // Fire
        }
	}
}
