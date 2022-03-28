using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Colorblind : MonoBehaviour
{
    [SerializeField] private Text waveText, enemyText, lifeText, moneyText, winText;
    [SerializeField] private Image backgroundPanel;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    public void ChangeColors(int a)
    {
        switch (a)
        {
            case 0:
                waveText.color = Color.red;
                enemyText.color = Color.red;
                lifeText.color = Color.red;
                moneyText.color = Color.red;
                winText.color = Color.red;
                backgroundPanel.color = new Color(0.87f, 0.87f, 0.87f, 1.0f);
                break;
            case 1:
            case 2:
                waveText.color = Color.blue;
                enemyText.color = Color.blue;
                lifeText.color = Color.blue;
                moneyText.color = Color.blue;
                winText.color = Color.yellow;
                backgroundPanel.color = new Color(0.87f, 0.87f, 0.87f, 1.0f);
                break;
            case 3:
                Color orange = new Color(1f, 0.5f, 0.1f);
                waveText.color = orange;
                enemyText.color = orange;
                lifeText.color = orange;
                moneyText.color = orange;
                winText.color = orange;
                backgroundPanel.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
                break;
            case 4:
                Color greyish = new Color(0.20f, 0.20f, 0.20f, 1.0f);
                waveText.color = greyish;
                enemyText.color = greyish;
                lifeText.color = greyish;
                moneyText.color = greyish;
                winText.color = greyish;
                backgroundPanel.color = new Color(0.87f, 0.87f, 0.87f, 1.0f);
                break; 
            default:
                break;
        }
    }
}
