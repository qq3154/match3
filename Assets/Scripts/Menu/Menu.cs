using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public float OpenSize;
    public float CloseSize;

    Image menu;
    bool isOpen =false;  

    Coroutine _currentCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        menu = this.GetComponent<Image>();
        menu.GetComponent<RectTransform>().localScale = new Vector3(CloseSize, CloseSize, 1);      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenMenu()
    {      
        if (!isOpen)
        {
            SwipeController.s_swipeAble = false;
            Debug.Log("Open");                              
            if(_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
            }               
            _currentCoroutine = StartCoroutine(IE_Open(OpenSize));                                             
        }
        else
        {
            Debug.Log("Close");
            SwipeController.s_swipeAble = true;
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
            }
            _currentCoroutine = StartCoroutine(IE_Open(CloseSize));  
        }
        
        isOpen = !isOpen;        
    }
    

    IEnumerator IE_Open(float size)
    {
      
        float timeOfTravel = 1f;
        float currentTime = 0;
        float normalizedValue;
        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.deltaTime;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time 
            menu.GetComponent<RectTransform>().localScale = Vector3.Lerp(menu.GetComponent<RectTransform>().localScale, new Vector3(size, size, 1f), normalizedValue);
            yield return null;
        }
       
    }
   
 
}
