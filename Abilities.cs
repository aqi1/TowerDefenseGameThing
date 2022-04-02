using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour
{
    [SerializeField] private WorldState worldState;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private GameObject sounds;
    private SoundController soundController;
    private float reinforcementsCost = 15f;
    private float bombCost = 20f;
    private bool bombTargeting = false;

    // change cursor icon in targeting mode
    [SerializeField] private Texture2D[] cursorTexture;
    private Vector2 cursorHotspot;

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
            StartCoroutine(BombDrop());
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

            // change targeting crosshair based on colorblind setting
            int colorMode = PlayerPrefs.GetInt("colorblindMode", 0);
            ChangeCursor(colorMode);
        }
    }

    private void ChangeCursor(int colorMode)
    {
        switch (colorMode)
        {
            case 0:
                cursorHotspot = new Vector2(cursorTexture[0].width / 2, cursorTexture[0].height / 2);
                Cursor.SetCursor(cursorTexture[0], cursorHotspot, CursorMode.Auto);
                break;
            case 1:
            case 2:
            case 3:
                cursorHotspot = new Vector2(cursorTexture[1].width / 2, cursorTexture[1].height / 2);
                Cursor.SetCursor(cursorTexture[1], cursorHotspot, CursorMode.Auto);
                break;
            case 4:
                cursorHotspot = new Vector2(cursorTexture[2].width / 2, cursorTexture[2].height / 2);
                Cursor.SetCursor(cursorTexture[2], cursorHotspot, CursorMode.Auto);
                break;
            default:
                break;
        }
    }

    public void Reinforcements()
    {
        if (worldState.GetPlayerMoney() < reinforcementsCost) // insufficient funds
            soundController.PlaySound(5);
        else
        {
            worldState.SubtractPlayerMoney(reinforcementsCost);
            worldState.HealPlayer();
        }
    }

    private IEnumerator BombDrop()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            bombTargeting = false;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); // reset cursor to default icon

            soundController.PlaySound(9);

            yield return new WaitForSeconds(2);

            soundController.PlaySound(10);
            GameObject bombing = Instantiate(bombPrefab, hit.point, Quaternion.identity) as GameObject; // spawn explosions prefab
            Destroy(bombing, 7);
        }
    }
}
