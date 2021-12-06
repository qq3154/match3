using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [SerializeField] AudioCommand audioCommand;
    // Start is called before the first frame update
    void Start()
    {
        audioCommand.Excute();
    }

    
}
