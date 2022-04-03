using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerSelector : MonoBehaviour
{
    //[SerializeField] GameObject towerPrefab;
    [SerializeField] private GameObject sounds;
    [SerializeField] private WorldState worldState;
    [SerializeField] private GameObject[] towers;
    [SerializeField] private GameObject debugtower;
    private SoundController soundController;
    public GameObject towerLocation;
    public GameObject towerObject = null;
    public Tower towerScript = null;
    private bool sortingByRange = false;

    private Vector3 upgradeScaleChange;

    private void Update()
    {
        CheckForKeybinds();
    }

    void Start()
    {
        upgradeScaleChange = new Vector3(0.067f, 0.067f, 0.067f);
        soundController = sounds.GetComponent<SoundController>();
    }

    public void AddTower(GameObject towerPre)
    {
        if (worldState.GetPlayerMoney() < towerPre.GetComponent<Tower>().purchaseCost) // insufficient funds
            soundController.PlaySound(5);
        else
        {
            soundController.PlaySound(12);
            soundController.PlaySound(0);
            worldState.SubtractPlayerMoney(1);

            towerObject = Instantiate(towerPre, towerLocation.transform.position, Quaternion.identity);

            towerLocation.GetComponent<TowerGenerator>().towerObject = towerObject;
            towerLocation.GetComponent<TowerGenerator>().hasTower = true;
            towerLocation.GetComponent<TowerGenerator>().SelectTower(); // show upgrade dialog
        }
    }

    public void UpgradeTower()
    {
        if (!towerObject)
        {
            Debug.Log("No tower to Upgrade()");
            return;
        }
        else
            towerScript = towerObject.GetComponent<Tower>();

        if (worldState.GetPlayerMoney() < towerScript.upgradeCost) // insufficient funds
            soundController.PlaySound(5);
        else if (towerScript.upgradeLevel < 4)
        {
            soundController.PlaySound(0);
            worldState.SubtractPlayerMoney(1);
            towerScript.upgradeLevel += 1;
            worldState.defensesUpgraded += 1;

            towerObject.transform.localScale += upgradeScaleChange;

            if (towerScript.upgradeLevel >= 4)
                towerLocation.GetComponent<TowerGenerator>().SelectTower(); // hide the panel at max upgrade level of 4
        }
    }

    public void SortingByRange()
    {
        sortingByRange = !sortingByRange;
    }

    // keybinds for towers. Need to make this prettier
    // TODO: move to a new keybinds class
    private void CheckForKeybinds()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (towerLocation.GetComponent<TowerGenerator>().hasTower)
            {
                UpgradeTower();
            }
            else
            {
                if(!sortingByRange)
                    AddTower(towers[0]);
                else
                    AddTower(towers[2]);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && !towerLocation.GetComponent<TowerGenerator>().hasTower)
        {
            if (!sortingByRange)
                AddTower(towers[1]);
            else
                AddTower(towers[3]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && !towerLocation.GetComponent<TowerGenerator>().hasTower)
        {
            if (!sortingByRange)
                AddTower(towers[2]);
            else
                AddTower(towers[1]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && !towerLocation.GetComponent<TowerGenerator>().hasTower)
        {
            if (!sortingByRange)
                AddTower(towers[3]);
            else
                AddTower(towers[0]);
        }
    }
}
