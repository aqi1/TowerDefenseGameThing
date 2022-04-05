using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tower))]

public class TowerShooting : MonoBehaviour
{
    [SerializeField] private Transform fireLocation;
    [SerializeField] private GameObject flameGround;
    [SerializeField] private GameObject superFlameGround;
    [SerializeField] private ParticleSystem flamerFlash;
    [SerializeField] private ParticleSystem superFlamerFlash;
    [SerializeField] private GameObject smallExplosion;
    [SerializeField] private GameObject superExplosion;
    [SerializeField] private ParticleSystem muzzleFlash1;
    [SerializeField] private ParticleSystem muzzleFlash2;
    [SerializeField] private GameObject towerHead;
    [SerializeField] private GameObject missile;
    private SciFiBeamScript beamer;
    private AudioSource audioSource;
    private Tower tower;
    private GameObject target;
    private float nextShootTime = 0f;
    private WorldState worldState;

    // Start is called before the first frame update
    void Start()
    {
        worldState = GameObject.FindGameObjectWithTag("GameController").GetComponent<WorldState>();
        audioSource = gameObject.GetComponent<AudioSource>();
        tower = GetComponent<Tower>();
        if (tower.towerType == TOWER_TYPE.TESLA)
        {
            beamer = gameObject.GetComponent<SciFiBeamScript>();
        }
        InvokeRepeating("SelectTarget", 0, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (tower.towerType == TOWER_TYPE.TESLA && !target)
        {
            beamer.shooting = false;
            return;
        }

        if (target)
        {
            if (tower.towerType != TOWER_TYPE.TESLA)
                LockOnTarget();

            ShootTarget();
        }
    }
    private void LockOnTarget()
    {
        // rotate tower but only around y
        Vector3 relativePos = target.transform.position - fireLocation.position;
        Quaternion LookAtRotation = Quaternion.LookRotation(relativePos);
        towerHead.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, LookAtRotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    private void SelectTarget()
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        float minDist = Mathf.Infinity;
        int selectedEnemy = -1;
        for(int i = 0; i < allEnemies.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, allEnemies[i].transform.position);
            if(dist < minDist)
            {
                minDist = dist;
                selectedEnemy = i;
            }
        }

        if (selectedEnemy != -1 && minDist <= tower.towerRange)
            target = allEnemies[selectedEnemy];
        else
            target = null;
    }

    private void ShootTarget()
    {
        if(Time.time > nextShootTime && target)
        {
            switch (tower.towerType)
            {
                case TOWER_TYPE.GUN:
                    ShootGun();
                    break;
                case TOWER_TYPE.TESLA:
                    ShootLaser();
                    break;
                case TOWER_TYPE.FLAME:
                    ShootFlame();
                    break;
                case TOWER_TYPE.ARTILLERY:
                    ShootArtillery();
                    break;
                case TOWER_TYPE.MISSILE:
                    StartCoroutine(ShootMissile());
                    break;
                default:
                    break;
            }

            // play attack sound
            if(!audioSource.isPlaying && tower.towerType != TOWER_TYPE.MISSILE)
                audioSource.Play();

            // add some randomness to the attack rate
            nextShootTime = Time.time + (1 / Random.Range(tower.shootRate, tower.shootRate * 1.1f));
        }
    }

    private void ShootGun()  // hits increase fire damage taken on target
    {
        muzzleFlash1.Play();
        muzzleFlash2.Play();
        Enemy enemy = target.GetComponent<Enemy>();
        enemy.fireDamageMultiplier += 0.16f * (1 + tower.upgradeLevel / 4);
        enemy.TakeDamage(tower.towerDamage * tower.upgradeLevel, tower);

        worldState.bulletsFired += 1;
    }

    private void ShootLaser() // hits slow the target
    {
        Enemy enemy = target.GetComponent<Enemy>();

        beamer.startLoc = fireLocation.position;
        beamer.endLoc = target.transform.position;

        beamer.shooting = true;
        enemy.TakeDamage(tower.towerDamage * tower.upgradeLevel, tower);
        enemy.HitByLaser(tower.upgradeLevel + 2); // slow enemy by X/argument speed

        worldState.beamsProjected += 1;
    }

    private void ShootFlame() // hits spawn a ground effect
    {
        Enemy enemy = target.GetComponent<Enemy>();
        GameObject groundFire;

        if (tower.upgradeLevel < 4)
        {
            flamerFlash.Play();
            groundFire = Instantiate(flameGround, target.transform.position, Quaternion.identity) as GameObject; // spawn ground fire
        }
        else
        {
            superFlamerFlash.Play();
            groundFire = Instantiate(superFlameGround, target.transform.position, Quaternion.identity) as GameObject;
        }

        groundFire.transform.parent = gameObject.transform;
        enemy.TakeDamage(tower.towerDamage * tower.upgradeLevel * enemy.fireDamageMultiplier, tower);
        Destroy(groundFire, 10);

        worldState.flamesSpread += 1;
    }

    private void ShootArtillery()
    {
        muzzleFlash1.Play();
        GameObject explosion;
        Collider[] colliders;

        if (tower.upgradeLevel < 4)
        {
            explosion = Instantiate(smallExplosion, target.transform.position, Quaternion.identity) as GameObject;
            colliders = Physics.OverlapSphere(target.transform.position, 1.2f);
        }
        else
        {
            explosion = Instantiate(superExplosion, target.transform.position, Quaternion.identity) as GameObject;
            colliders = Physics.OverlapSphere(target.transform.position, 1.4f); // larger damage radius at max upgrade level
        }
        Destroy(explosion, 2);

        foreach(var hitCollider in colliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                // don't scale damage as much as other towers (radius was increased)
                hitCollider.GetComponent<Enemy>().TakeDamage(tower.towerDamage + (tower.towerDamage * (tower.upgradeLevel - 1)/ 1.5f), tower);
            }
        }

        worldState.ordnanceDetonated += 1;
    }

    private IEnumerator ShootMissile()
    {
        GameObject launchedMissile;
        int launchThisManyTimes = 2;
        // TODO: instantiate a missile

        for (int i = 0; i < launchThisManyTimes; i++)
        {
            audioSource.Play();
            if (target)
            {
                launchedMissile = Instantiate(missile, fireLocation.position, Quaternion.LookRotation(target.transform.position - fireLocation.position)) as GameObject;
                launchedMissile.GetComponent<Missile>().SetTarget(target);
                launchedMissile.GetComponent<Missile>().SetParentTower(tower);
                worldState.missilesLaunched += 1;
                yield return new WaitForSeconds(0.20f);
            }
            else
                break;
        }
    }
}
