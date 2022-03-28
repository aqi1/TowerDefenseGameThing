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
    [SerializeField] private ParticleSystem muzzleFlash1;
    [SerializeField] private ParticleSystem muzzleFlash2;
    [SerializeField] private GameObject towerHead;
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
            //laser.enabled = false;
            beamer.shooting = false;
            return;
        }

        if(tower.towerType != TOWER_TYPE.TESLA && target)
            LockOnTarget();

        ShootTarget();
    }
    private void LockOnTarget()
    {
        // rotate tower but only on y-axis
        Vector3 relativePos = target.transform.position - fireLocation.position;
        Quaternion LookAtRotation = Quaternion.LookRotation(relativePos);
        Quaternion LookAtRotationY = Quaternion.Euler(transform.rotation.eulerAngles.x, LookAtRotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        towerHead.transform.rotation = LookAtRotationY;
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
                default:

                    break;
            }

            // play attack sound
            if(audioSource && !audioSource.isPlaying)
                audioSource.Play();

            nextShootTime = Time.time + 1 / tower.shootRate;
        }
    }

    private void ShootGun()  // hits increase fire damage taken on target
    {
        muzzleFlash1.Play();
        muzzleFlash2.Play();
        Enemy enemy = target.GetComponent<Enemy>();
        enemy.fireDamageMultiplier += 0.08f * (1 + tower.upgradeLevel / 4);
        enemy.TakeDamage(tower.towerDamage * tower.upgradeLevel);

        worldState.bulletsFired += 1;
    }

    private void ShootLaser() // hits slow the target
    {
        Enemy enemy = target.GetComponent<Enemy>();

        beamer.startLoc = fireLocation.position;
        beamer.endLoc = target.transform.position;

        beamer.shooting = true;
        enemy.TakeDamage(tower.towerDamage * tower.upgradeLevel);
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

        enemy.TakeDamage(tower.towerDamage * tower.upgradeLevel * enemy.fireDamageMultiplier);
        Destroy(groundFire, 10);

        worldState.flamesSpread += 1;
    }
}
