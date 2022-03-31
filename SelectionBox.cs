using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionBox : MonoBehaviour
{

    public Tower tower;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (tower)
        {
            GetComponentsInChildren<Text>()[0].text = "KILLS: " + tower.killCount;
        }
        else
        {
            GetComponentsInChildren<Text>()[0].text = "";
        }
    }
}
