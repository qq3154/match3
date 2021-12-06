using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenuRender : MonoBehaviour
{
    [SerializeField] GameObject PlayableLevel;
    [SerializeField] GameObject UnplayableLevel;
    

    // Start is called before the first frame update
    void Start()
    {        
        for (int i = 1; i <= 16; i++)
        {
            GameObject item;
            if (i <= PlayerPrefs.GetInt("CurrentLevel"))
            {
                item = Instantiate(PlayableLevel, this.transform, false);
                item.transform.GetChild(1).gameObject.GetComponent<StarRender>().SetStar(i);
                item.transform.GetChild(0).gameObject.GetComponent<Text>().text = i.ToString();

            }
            else
            {
                item = Instantiate(UnplayableLevel, this.transform, false);
            }

            item.gameObject.name = i.ToString();

            Vector3 pos = new Vector3(-280 + ((i - 1) % 4) * 180, 390 - 180 * ((i - 1) / 4), 0);
            item.gameObject.GetComponent<RectTransform>().anchoredPosition = pos;

        }
    }
    
    

}
