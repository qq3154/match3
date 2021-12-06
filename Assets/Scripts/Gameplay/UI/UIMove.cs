using UnityEngine;
using System.Collections;
using Observer;
using UnityEngine.UI;

public class UIMove : MonoBehaviour
{

    private void Awake()
    {
        this.RegisterListener(EventID.OnMoveSuccessful, (param) => DecreaseMoveCount());
    }

    // Use this for initialization
    void Start()
    {
        this.GetComponent<Text>().text = PlayerConfig.instance.moveCount.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void DecreaseMoveCount()
    {
        PlayerConfig.instance.moveCount--;
        this.GetComponent<Text>().text = PlayerConfig.instance.moveCount.ToString();
    }
}
