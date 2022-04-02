using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerGenerator : MonoBehaviour
{
    [SerializeField] private GameObject towerSelectionPanel;
    [SerializeField] private GameObject towerUpgradePanel;
    [SerializeField] private GameObject selectionBox;
    private SelectionBox selectionBoxScript;
    public GameObject towerObject;
    public bool hasTower = false;

    void Awake()
    {
        towerSelectionPanel = GameObject.Find("TowerSelectionPanel");
        towerUpgradePanel = GameObject.Find("TowerUpgradePanel");
        selectionBox = GameObject.Find("SelectionBox");
        selectionBoxScript = selectionBox.GetComponent<SelectionBox>();
    }


    void Start()
    {
        towerUpgradePanel.SetActive(false);
        towerSelectionPanel.SetActive(false);
        selectionBox.SetActive(false);
        selectionBoxScript.SetTower(null);
    }

    public void SelectTower()
    {
        towerUpgradePanel.SetActive(false);
        towerSelectionPanel.SetActive(false);

        towerUpgradePanel.GetComponent<TowerSelector>().towerLocation = gameObject;
        towerSelectionPanel.GetComponent<TowerSelector>().towerLocation = gameObject;

        // selection box outline should be drawn in 2D screen space
        Vector3 selectionPosition = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        selectionBox.transform.position = selectionPosition;
        selectionBox.SetActive(true);

        if (hasTower)
        {
            if (towerObject.GetComponent<Tower>().upgradeLevel < 4) // filled spot
            {
                towerUpgradePanel.SetActive(true);
                towerUpgradePanel.GetComponent<TowerSelector>().towerObject = towerObject;
            }
            selectionBoxScript.SetTower(towerObject.GetComponent<Tower>());
            HideIcon();
        }
        else // empty spot
        {
            towerUpgradePanel.SetActive(false);
            towerSelectionPanel.SetActive(true);
            selectionBoxScript.SetTower(null);
        }
    }

    public void HideIcon()
    {
        gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
    }

    public void UnhideIcon()
    {
        gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.79f);
    }
}
