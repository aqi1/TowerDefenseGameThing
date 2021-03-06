using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class WorldState : MonoBehaviour
{
    [SerializeField] private int playerLives = 10;

    [SerializeField] private GameObject gameOverPanels;
    [SerializeField] private GameObject gameOverObject;
    [SerializeField] private GameObject winObject;

    [SerializeField] private GameObject menuBackground;
    [SerializeField] private GameObject menuParent;
    [SerializeField] private GameObject menuSettings;
    [SerializeField] private GameObject menuConfirm;
    [SerializeField] private GameObject menuCheat;
    [SerializeField] private Button menuButton;

    [SerializeField] private Text gameOverStats;
    [SerializeField] private Text gameOverLore;
    [SerializeField] private Text playerMoneyText;
    [SerializeField] private Text playerLivesText;
    [SerializeField] private Text waveNumberText;
    [SerializeField] private Text enemiesRemainingText;

    [SerializeField] private GameObject towerSelectionPanel;
    [SerializeField] private GameObject towerUpgradePanel;
    [SerializeField] private GameObject selectionBox;
    [SerializeField] private GameObject explosion;

    [SerializeField] private GameObject sounds;
    [SerializeField] private GameObject jukeBox;
    [SerializeField] private AudioClip endMusic;
    private SoundController soundController;
    private AudioSource musicSource;

    [SerializeField] private GameObject enemySpawnerObject;
    private EnemySpawner enemySpawnerScript;
    private int enemiesAtm;
    private int waveAtm;
    private GameObject[] towers;
    private GameObject[] beams;
    private GameObject[] buildLocations;

    // game stats
    public uint defensesBuilt = 0;
    public uint defensesUpgraded = 0;
    public uint bulletsFired = 0;
    public uint flamesSpread = 0;
    public uint beamsProjected = 0;
    public uint ordnanceDetonated = 0;
    public uint missilesLaunched = 0;
    public uint casualtiesInflicted = 0;

    private bool isGameOver = false;
    private bool playedWinSound = false;

    [SerializeField] private float playerMoney = 10f;

    void Start()
    {
        soundController = sounds.GetComponent<SoundController>();
        musicSource = jukeBox.GetComponent<AudioSource>();
        enemySpawnerScript = enemySpawnerObject.GetComponent<EnemySpawner>();
        soundController.PlaySound(2); // game start sound
    }

    void Update()
    {
        enemiesAtm = enemySpawnerScript.GetEnemiesAlive();
        waveAtm = enemySpawnerScript.GetWaveNumber();

        // update UI
        playerLivesText.text = "HP: " + playerLives;
        waveNumberText.text = "Wave: " + waveAtm;
        enemiesRemainingText.text = "Hostiles: " + enemiesAtm;
        playerMoneyText.text = "Money: " + playerMoney.ToString("n2");

        // wave win condition
        if (waveAtm > 0 && enemiesAtm <= 0 && !isGameOver)
        {
            winObject.SetActive(true);
            if (!soundController.IsPlayingIndex(1) && !playedWinSound)
            {
                soundController.PlaySound(1);
                playedWinSound = true;
            }
        }
        else
        {
            winObject.SetActive(false);
            playedWinSound = false;
        }

        // TODO: keybinds and UI should be moved to a new class
        if (Input.GetMouseButtonDown(1)) // right click to "deselect" everything
        {
            ClearUI();
            TooltipSystem.Hide();
        }
        if (Input.GetKeyDown(KeyCode.Escape)) // open game menu
        {
            if (!menuParent.activeSelf)
                menuButton.onClick.Invoke();
            else if (menuParent.activeSelf)
            {
                soundController.PlaySound(8);
                menuBackground.SetActive(false);
                menuSettings.SetActive(false);
                menuConfirm.SetActive(false);
                menuParent.SetActive(false);
                menuCheat.SetActive(false);
            }
            TooltipSystem.Hide();
        }

        // lose condition
        if (playerLives <= 0 && !isGameOver)
            GameOver();
        if (isGameOver) // in game over state, set money to 0 repeatedly
            playerMoney = 0.0f;
    }

    public void DamagePlayer()
    {
        if(playerLives > 0)
            playerLives -= 1;

        if (!soundController.IsPlayingIndex(4) && !isGameOver)
            soundController.PlaySound(4);
    }

    public void DamagePlayer(int damage)
    {
        if (playerLives > 0 && playerLives >= damage)
            playerLives -= damage;
        else
            playerLives = 0;

        if (!soundController.IsPlayingIndex(4) && !isGameOver)
            soundController.PlaySound(4);
    }

    public void HealPlayer()
    {
        if (playerLives <= 0 || isGameOver)
            return;

        playerLives += 1;

        if (!soundController.IsPlayingIndex(7))
            soundController.PlaySound(7);
    }

    // for cheat code. No sound effect.
    public void HealPlayer(int a)
    {
        if (playerLives <= 0 || isGameOver)
            return;

        playerLives += a;
    }

    public void AddPlayerMoney(float a)
    {
        playerMoney += a;
    }

    public void SubtractPlayerMoney(float a)
    {
        playerMoney -= a;
    }

    public float GetPlayerMoney()
    {
        return playerMoney;
    }

    public void BootToMenu()
    {
        SceneManager.LoadScene(0); // 0 is main menu
    }

    public void KillPlayer()
    {
        playerLives = 0;
    }
    
    public bool IsGameOver()
    {
        return isGameOver;
    }

    private void ClearUI()
    {
        if(towerSelectionPanel)
            towerSelectionPanel.SetActive(false);
        if(towerUpgradePanel)
            towerUpgradePanel.SetActive(false);
        if(selectionBox)
        {
            selectionBox.GetComponent<SelectionBox>().SetTower(null);
            selectionBox.SetActive(false);
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        soundController.StopSound(1); // stop sound: enemy unit destroyed
        soundController.StopSound(4); // stop sound: civ killed
        soundController.PlaySound(11); // sound: kaboom

        if (musicSource.isPlaying)
            musicSource.Stop();
        musicSource.PlayOneShot(endMusic); // end music

        if (waveAtm < 6)
            soundController.PlaySound(13); // sound: mission is a failure
        else
            soundController.PlaySound(3); // sound: battle control terminated

        ClearUI();

        // delete towers and lingering beams upon game over
        towers = GameObject.FindGameObjectsWithTag("Tower");
        beams = GameObject.FindGameObjectsWithTag("BeamTag");
        buildLocations = GameObject.FindGameObjectsWithTag("BuildSpot");

        for (int i = 0; i < towers.Length; i++)
        {
            defensesBuilt += 1;
            Instantiate(explosion, towers[i].transform.position, Quaternion.identity);
            towers[i].SetActive(false);
        }

        for (int i = 0; i < beams.Length; i++)
        {
            beams[i].SetActive(false);
        }

        for (int i = 0; i < buildLocations.Length; i++)
        {
            buildLocations[i].GetComponent<TowerGenerator>().hasTower = false;
            buildLocations[i].GetComponent<TowerGenerator>().UnhideIcon();
        }

        // display end panel
        gameOverStats.text = "\n\n";

        if (waveAtm == 0)
        {
            gameOverStats.text += "0\n";
        }
        else
        {
            gameOverStats.text += (waveAtm - 1) + "\n";
        }

        gameOverStats.text += defensesBuilt + "\n"
                            + defensesUpgraded + "\n\n"
                            + bulletsFired + "\n"
                            + beamsProjected + "\n"
                            + flamesSpread + "\n"
                            + missilesLaunched + "\n"
                            + ordnanceDetonated + "\n\n"
                            + casualtiesInflicted + "\n"
                            + "1";

        gameOverLore.text = "## NECROLOGUE ##\n\n";
        if (waveAtm > 15)
        {
            gameOverLore.text += "Your unrelenting soul burns brightly as a beacon of hope parallel to that of the"
                            + " God-Emperor Himself. You have ascended to the highest pantheons of sainthood, and your name is"
                            + " bellowed in warcries from the arctic wastes in the north to the urban battlegrounds of Nova Terra."
                            + " In death, you have attained immortality, a fate that lesser beings may only dream of.";
        }
        else if (waveAtm > 10)
        {
            gameOverLore.text += "Your unwavering sacrifice has bought sufficient time to rally the citizenry for a final stand"
                            + " against the endless horde. Inspired by your actions, the people are animated with fresh ardor."
                            + " The God-Emperor protects, always and forever, for He is our shield and we are His children.";
        }
        else if (waveAtm > 5)
        {
            gameOverLore.text += "Your sacrifice, though valiant, left the enemy unperturbed. Soon, Novo Terra will be mere"
                            + " radioactive ash. There will be no surrender, nor fear. All will be washed away"
                            + " by the divine wind.";
        }
        else
        {
            gameOverLore.text += "You have failed in your sole duty and have shamed His name.\n\nThe punishment for your"
                            + " incompetence is death.";
        }

        gameOverPanels.SetActive(true);
    }
}
