using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionBox : MonoBehaviour
{

    private Tower towerScript;
    void Update()
    {
        if (towerScript)
        {
            GetComponentsInChildren<Text>()[0].text = "KILLS: " + towerScript.killCount;
        }
    }

    public void SetTower(Tower a)
    {
        towerScript = a;

        if(!a)
            GetComponentsInChildren<Text>()[0].text = "";
    }
}
