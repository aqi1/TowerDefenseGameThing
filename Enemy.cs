using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private WorldState worldState;
    [SerializeField] private float health;
    [SerializeField] private float speed = 3f;
    [SerializeField] private bool bossEnemy;
    private List<GameObject> enemyPath;
    private GameObject prev, next;
    private EnemySpawner enemySpawnerScript;
    private bool amIDead = false;
    private bool amISlowed = false;
    [SerializeField] public float fireDamageMultiplier = 1f;

    // Start is called before the first frame update
    void Start()
    {
        worldState = GameObject.FindGameObjectWithTag("GameController").GetComponent<WorldState>();
        enemySpawnerScript = GameObject.FindGameObjectWithTag("EnemySpawner").GetComponent<EnemySpawner>();

        if (enemySpawnerScript.GetWaveNumber() <= 5)
            health = health * (enemySpawnerScript.GetWaveNumber() / 1.7f);
        else if (enemySpawnerScript.GetWaveNumber() <= 10)
        {   // exponential health multiplier starting on wave 6
            health = Mathf.Pow(health * (enemySpawnerScript.GetWaveNumber() / 1.6f), 1.08f);
            if(!bossEnemy)
                speed = speed * 1.2f;
        }
        else if (enemySpawnerScript.GetWaveNumber() <= 15)
        {   // even more exponential health multiplier starting on wave 11
            health = Mathf.Pow(health * (enemySpawnerScript.GetWaveNumber() / 1.55f), 1.13f);
            if (!bossEnemy)
                speed = speed * 1.44f;
        }
        else if (enemySpawnerScript.GetWaveNumber() <= 20)
        {   // good luck
            health = Mathf.Pow(health * (enemySpawnerScript.GetWaveNumber() / 1.5f), 1.20f);
            if (!bossEnemy)
                speed = speed * 1.44f;
        }
        else
        {   // good luck
            health = Mathf.Pow(health * (enemySpawnerScript.GetWaveNumber() / 1.5f), 1.25f);
            if (!bossEnemy)
                speed = speed * 1.44f;
        }

        enemyPath = PathGenerator.GetPath();
        prev = enemyPath[0];

        transform.position = prev.transform.position;
    }

    // enemy pathing. FixedUpdate() runs at 50 fps, which means enemy movement updates at 50 fps.
    // This provides consistency, but might be bad practice, as someone running the game at 120 fps
    // and with a 120 Hz monitor would still only see enemies move 50 times / sec instead of 120,
    // thus not fully taking advantage of the hardware's potential. Might change this in the future.
    void FixedUpdate()
    {
        if (Convert.ToInt32(prev.name) + 1 < enemyPath.Count)
        {
            next = enemyPath[Convert.ToInt32(prev.name) + 1];

            transform.position += (next.transform.position - transform.position).normalized * Time.fixedDeltaTime * speed;
            Quaternion rot = Quaternion.LookRotation((next.transform.position - transform.position).normalized);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, 6f);

            // Debug.Log(Vector3.Distance(transform.position, next.transform.position));
            if (Vector3.Distance(transform.position, next.transform.position) <= 0.09f)
            {
                prev = next;
            }
        }
        else // reached end of path
        {
            Destroy(gameObject);

            if(!bossEnemy)
                worldState.GetComponent<WorldState>().DamagePlayer();
            else
                worldState.GetComponent<WorldState>().DamagePlayer(200); // if boss makes it to the end, insta-kill player

            enemySpawnerScript.EnemyKilled(); // decrements enemy count
        }
    }

    public void TakeDamage(float damage, Tower tower)
    {
        Tower lastTowerToHitMe = tower;

        if (health > 0)
        {
            // fire damage multiplier from flame turret
            if (lastTowerToHitMe && lastTowerToHitMe.towerType == TOWER_TYPE.FLAME)
            {
                if(!bossEnemy)
                    health = health - damage * fireDamageMultiplier;
                else
                    health = health - damage * (fireDamageMultiplier * 0.08f);
            }
            else
                health = health - damage;
        }
        if(health <= 0 && !amIDead)
        {
            amIDead = true;
            gameObject.GetComponent<AudioSource>().Play();
            gameObject.GetComponent<Animator>().SetTrigger("IsDead");
            gameObject.GetComponent<Animator>().SetInteger("DeadType", UnityEngine.Random.Range(0,3));

            // remove tag to stop being targeted
            gameObject.tag = "Untagged";

            // stop movement
            this.speed = 0;
            gameObject.GetComponent<Rigidbody>().freezeRotation = true;
            gameObject.GetComponent<CapsuleCollider>().enabled = false;

            // give player money
            if(!bossEnemy)
                worldState.AddPlayerMoney(0.25f);
            else
                worldState.AddPlayerMoney(25f);

            // increment kill counter
            worldState.casualtiesInflicted += 1;

            Destroy(gameObject, 5);
            enemySpawnerScript.EnemyKilled(); // decrements enemy count

            if(lastTowerToHitMe)
                lastTowerToHitMe.killCount += 1;
        }
    }

    public void HitByLaser(int slowEffectDivisor)
    {
        if (!amISlowed && !bossEnemy) // bosses are unslowable, otherwise it's too cheesy
            StartCoroutine(Slowed(slowEffectDivisor));
    }

    void OnTriggerStay(Collider other)
    {
        // stood in fire
        if (other.gameObject.tag == "aoeDamage")
        {
            if(!bossEnemy)
                TakeDamage(30f * fireDamageMultiplier * Time.fixedDeltaTime, other.gameObject.transform.parent.transform.parent.GetComponent<Tower>());
            else
                TakeDamage(30f * (fireDamageMultiplier * 0.08f) * Time.fixedDeltaTime, other.gameObject.transform.parent.transform.parent.GetComponent<Tower>());
        }
        else if (other.gameObject.tag == "aoeDamage2")
        {
            if (!bossEnemy)
                TakeDamage(45f * fireDamageMultiplier * Time.fixedDeltaTime, other.gameObject.transform.parent.transform.parent.GetComponent<Tower>());
            else
                TakeDamage(45f * (fireDamageMultiplier * 0.08f) * Time.fixedDeltaTime, other.gameObject.transform.parent.transform.parent.GetComponent<Tower>());
        }
        // stood in bombs
        else if (other.gameObject.tag == "bombDamage")
        {
            TakeDamage(1200f * Time.fixedDeltaTime, null);
        }
    }

    IEnumerator Slowed(int a)
    {
        amISlowed = true;
        speed = speed / a;
        yield return new WaitForSeconds(0.5f);
        speed = speed * a;
        amISlowed = false;
    }
}
