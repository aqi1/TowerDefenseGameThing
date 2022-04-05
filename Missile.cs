using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] private float missileSpeed;
    [SerializeField] private GameObject smallExplosion;
    private Tower parentTowerScript; // pass in from parent
    private GameObject target; // pass in from parent (no target changing allowed mid-flight)
    private Vector3 targetLocation; // last location of target
    private bool detonated;

    // Start is called before the first frame update
    void Start()
    {
        detonated = false;
        Destroy(gameObject, 30); // if no detonation in 30 sec, it probably went off the rails
    }

    // Update is called once per frame
    void Update()
    {
        if(!detonated)
            GuideToTarget();
    }

    public void SetTarget(GameObject enemyTarget)
    {
        target = enemyTarget;
        targetLocation = target.transform.position;
    }

    public void SetParentTower(Tower t)
    {
        parentTowerScript = t;
    }

    private void GuideToTarget()
    {
        if(target)
            targetLocation = target.transform.position;

        transform.position += (targetLocation - transform.position).normalized * Time.deltaTime * missileSpeed;
        Quaternion rot = Quaternion.LookRotation((targetLocation - transform.position).normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, 10f);

        if (Vector3.Distance(transform.position, targetLocation) <= 0.2f)
            Explode();
    }

    private void Explode()
    {
        detonated = true;
        GameObject explosion;
        Collider[] colliders;

        gameObject.GetComponent<AudioSource>().Play();

        explosion = Instantiate(smallExplosion, transform.position, Quaternion.identity) as GameObject;
        colliders = Physics.OverlapSphere(transform.position, 1.15f);
        
        Destroy(explosion, 2); // delete explosion effect after 2 sec

        foreach (var hitCollider in colliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                hitCollider.GetComponent<Enemy>().TakeDamage((parentTowerScript.towerDamage * parentTowerScript.upgradeLevel), parentTowerScript);
            }
        }

        // hide missile
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }

        Destroy(gameObject, 3); // delete missile after 3 sec so the audio has time to play
    }
}
