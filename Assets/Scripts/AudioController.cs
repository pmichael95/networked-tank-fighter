using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    #region ABOUT
    /**
     * A simple audio slider for volume adjustments.
     **/
    #endregion

    /// <summary>
    /// Sets the volume to the value of the slider.
    /// </summary>
    public void SetVolume(Slider s)
    {
        AudioListener.volume = s.value;
    }
}
