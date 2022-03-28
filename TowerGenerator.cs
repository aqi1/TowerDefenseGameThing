using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGenerator : MonoBehaviour
{
    [SerializeField] private GameObject towerSelectionPanel;
    [SerializeField] private GameObject towerUpgradePanel;
    [SerializeField] private GameObject selectionBox;
    public GameObject tower;
    public bool hasTower = false;

    void Awake()
    {
        towerSelectionPanel = GameObject.Find("TowerSelectionPanel");
        towerUpgradePanel = GameObject.Find("TowerUpgradePanel");
        selectionBox = GameObject.Find("SelectionBox");
    }

    void Start()
    {
        towerUpgradePanel.SetActive(false);
        towerSelectionPanel.SetActive(false);
        selectionBox.SetActive(false);
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

        if (hasTower && tower.GetComponent<Tower>().upgradeLevel < 4) // filled spot
        {
            towerUpgradePanel.SetActive(true);
            towerUpgradePanel.GetComponent<TowerSelector>().tower = tower;
        }
        else if (!hasTower) // empty spot
        {
            towerUpgradePanel.SetActive(false);
            towerSelectionPanel.SetActive(true);
        }
    }
}
