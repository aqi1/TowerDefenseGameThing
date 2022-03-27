using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSelector : MonoBehaviour
{
    //[SerializeField] GameObject towerPrefab;
    [SerializeField] private GameObject sounds;
    [SerializeField] private WorldState worldState;
    private SoundController soundController;
    public GameObject towerLocation;
    public GameObject tower = null;
    public Tower towerScript = null;

    private Vector3 upgradeScaleChange;

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
            soundController.PlaySound(0);
            worldState.SubtractPlayerMoney(1);

            tower = Instantiate(towerPre, towerLocation.transform.position, Quaternion.identity);

            towerLocation.GetComponent<TowerGenerator>().tower = tower;
            towerLocation.GetComponent<TowerGenerator>().hasTower = true;
            gameObject.SetActive(false);
        }
    }

    public void UpgradeTower()
    {
        if (!tower)
        {
            Debug.Log("No tower to Upgrade()");
            return;
        }
        else
            towerScript = tower.GetComponent<Tower>();

        if (worldState.GetPlayerMoney() < towerScript.upgradeCost) // insufficient funds
            soundController.PlaySound(5);
        else if (towerScript.upgradeLevel < 4)
        {
            if(!soundController.IsPlayingIndex(1))
                soundController.PlaySound(1);
            worldState.SubtractPlayerMoney(1);
            towerScript.upgradeLevel += 1;
            worldState.defensesUpgraded += 1;

            tower.transform.localScale += upgradeScaleChange;

            if (towerScript.upgradeLevel >= 4)
                towerLocation.GetComponent<TowerGenerator>().SelectTower(); // hide the panel at max upgrade level of 4
        }
    }
}
