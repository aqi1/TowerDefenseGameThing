using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheater : MonoBehaviour
{
    [SerializeField] private WorldState worldState;
    [SerializeField] private AudioSource soundBox;
    [SerializeField] private AudioClip realTuffGuy;

    public void EvalCheat(string cheat)
    {
        if (cheat.Equals("greedisgood"))
        {
            CheatMoney();
        }
        else if (cheat.Equals("gaben"))
        {
            CheatLife();
        }
    }

    private void CheatMoney()
    {
        worldState.AddPlayerMoney(40f);
        soundBox.PlayOneShot(realTuffGuy);
    }

    private void CheatLife()
    {
        worldState.HealPlayer(5);
        soundBox.PlayOneShot(realTuffGuy);
    }
}
