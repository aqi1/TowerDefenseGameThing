using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TOWER_TYPE { GUN, MISSILE, TESLA, FLAME, TOWER_COUNT }

public class Tower : MonoBehaviour
{
    public TOWER_TYPE towerType = TOWER_TYPE.TESLA;
    public float shootRate = 50f;
    public float towerDamage = 10f;
    public float towerRange = 5f;
    public int upgradeLevel = 1;

    public float purchaseCost = 1f;
    public float upgradeCost = 1f;
}
