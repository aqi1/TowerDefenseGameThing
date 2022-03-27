using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour
{
    [SerializeField] private WorldState worldState;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private GameObject sounds;
    private SoundController soundController;
    private float bombCost = 30f;
    private bool bombTargeting = false;

    // Start is called before the first frame update
    void Start()
    {
        soundController = sounds.GetComponent<SoundController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bombTargeting && Input.GetMouseButtonDown(0))
        {
            BombDrop();
            bombTargeting = false;
        }
    }

    public void BombingRun()
    {
        if (worldState.GetPlayerMoney() < bombCost) // insufficient funds
            soundController.PlaySound(5);
        else
        {
            soundController.PlaySound(6);
            worldState.SubtractPlayerMoney(bombCost);
            bombTargeting = true;
        }
    }

    private void BombDrop()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            GameObject bombing = Instantiate(bombPrefab, hit.point, Quaternion.identity) as GameObject;
            Destroy(bombing, 7);
        }
    }
}
