using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    Image settingMenu;
    [SerializeField] Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        settingMenu = this.GetComponent<Image>();    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenCloseSettingMenu()
    {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
        canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);        
    }

    
}
