using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeController : MonoBehaviour {
    public Slider volumeSounds;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            volumeSounds.value = PlayerPrefs.GetFloat("Volume", volumeSounds.value);
        }
    }
    private void Update()
    {
        AudioListener.volume = volumeSounds.value;
    }
    public void SetVolume()
    {
        AudioListener.volume = volumeSounds.value;
        PlayerPrefs.SetFloat("Volume", volumeSounds.value);
        PlayerPrefs.Save();
    }
}
