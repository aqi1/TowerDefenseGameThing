using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGenerator : MonoBehaviour
{
    [SerializeField] private GameObject towerSelectionPanel;
    [SerializeField] private GameObject towerUpgradePanel;
    public GameObject tower;
    public bool hasTower = false;

    void Awake()
    {
        towerSelectionPanel = GameObject.Find("TowerSelectionPanel");
        towerUpgradePanel = GameObject.Find("TowerUpgradePanel");
    }

    void Start()
    {
        towerUpgradePanel.SetActive(false);
        towerSelectionPanel.SetActive(false);
    }

    public void SelectTower()
    {
        towerUpgradePanel.GetComponent<TowerSelector>().towerLocation = gameObject;
        towerSelectionPanel.GetComponent<TowerSelector>().towerLocation = gameObject;

        if (hasTower && tower.GetComponent<Tower>().upgradeLevel < 4)
        {
            towerUpgradePanel.SetActive(true);
            towerUpgradePanel.GetComponent<TowerSelector>().tower = tower;
            towerSelectionPanel.SetActive(false);
        }
        else if (!hasTower)
        {
            towerUpgradePanel.SetActive(false);
            towerSelectionPanel.SetActive(true);
        }
        else
        {
            towerUpgradePanel.SetActive(false);
            towerSelectionPanel.SetActive(false);
        }
    }
}
