using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Colorblind : MonoBehaviour
{
    [SerializeField] private Text waveText, enemyText, lifeText, moneyText, winText;
    [SerializeField] private Image backgroundPanel, abilityPanel, damagePanel, rangePanel;
    private Color whiteRed = new Color(1.0f, 0.2f, 0.2f, 1.0f);
    private Color kindablue = new Color(0.63f, 0.75f, 0.95f, 1.0f);
    private Color greyish = new Color(1f, 1f, 1f, 1f);
    private Color halfEverything = new Color(0.5f, 0.5f, 0.5f, 1.0f);

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
                waveText.color = whiteRed;
                enemyText.color = whiteRed;
                lifeText.color = whiteRed;
                moneyText.color = whiteRed;
                winText.color = whiteRed;
                backgroundPanel.color = Color.white;
                abilityPanel.color = Color.white;
                damagePanel.color = Color.white;
                rangePanel.color = Color.white;
                break;
            case 1:
            case 2:
                waveText.color = kindablue;
                enemyText.color = kindablue;
                lifeText.color = kindablue;
                moneyText.color = kindablue;
                winText.color = kindablue;
                backgroundPanel.color = halfEverything;
                abilityPanel.color = halfEverything;
                damagePanel.color = halfEverything;
                rangePanel.color = halfEverything;
                break;
            case 3:
                Color orange = new Color(1f, 0.5f, 0.1f);
                waveText.color = orange;
                enemyText.color = orange;
                lifeText.color = orange;
                moneyText.color = orange;
                winText.color = orange;
                backgroundPanel.color = halfEverything;
                abilityPanel.color = halfEverything;
                damagePanel.color = halfEverything;
                rangePanel.color = halfEverything;
                break;
            case 4:
                
                waveText.color = greyish;
                enemyText.color = greyish;
                lifeText.color = greyish;
                moneyText.color = greyish;
                winText.color = greyish;
                backgroundPanel.color = halfEverything;
                abilityPanel.color = halfEverything;
                damagePanel.color = halfEverything;
                rangePanel.color = halfEverything;
                break; 
            default:
                break;
        }
    }
}
