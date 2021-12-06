using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeAdjustor : MonoBehaviour
{
    public Mixer mixer;

    [SerializeField] GameObject slider;
    [SerializeField] float volumeValue;
    

    // Start is called before the first frame update
    void Start()
    {
        GetVolumeValue(mixer);
    }

    public void GetVolumeValue(Mixer mixer)
    {
        switch (mixer)
        {
            case Mixer.Master:
                slider.GetComponent<Slider>().value = AudioManager.instance.VolumeAllValue();
                break;

            case Mixer.BGM:
                slider.GetComponent<Slider>().value = AudioManager.instance.VolumeBGMValue();
                break;
            case Mixer.SFX:
                slider.GetComponent<Slider>().value = AudioManager.instance.VolumeSFXValue();
                break;
            default:
                break;
        }
       
    }

    public void MainVolumeAdjust()
    {
        volumeValue = slider.GetComponent<Slider>().value;
        AudioManager.instance.AdjustVolumeAll(volumeValue);
    }

    public void BGMVolumeAdjust()
    {
        volumeValue = slider.GetComponent<Slider>().value;
        AudioManager.instance.AdjustVolumeBGM(volumeValue);
    }

    public void SFXVolumeAdjust()
    {
        volumeValue = slider.GetComponent<Slider>().value;
        AudioManager.instance.AdjustVolumeSFX(volumeValue);
    }

    public enum Mixer
    {
        Master = 0,
        BGM = 1,
        SFX = 2,
    }
}
