using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteVolume : MonoBehaviour
{      

    public void MuteVolumeAll()
    {
        bool active = AudioManager.instance.volumeAdjustor.IsMuteAll();
        AudioManager.instance.ActiveVolumeAll(!active);
    }

    public void MuteVolumeBGM()
    {
        bool active = AudioManager.instance.volumeAdjustor.IsMuteBGM();
        AudioManager.instance.ActiveVolumeBGM(!active);
    }

    public void MuteVolumeSFX()
    {
        bool active = AudioManager.instance.volumeAdjustor.IsMuteSFX();
        AudioManager.instance.ActiveVolumeSFX(!active);
    }
}
