using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public int enemiesAlive = 0;

    public int round = 0;
    public int kill = 0;

    public GameObject[] spawnPoints;

    public GameObject enemyPrefab;

    public GameObject pauseMenu;

    public TextMeshProUGUI roundNum;
    public TextMeshProUGUI roundsSurvived;
    public TextMeshProUGUI killCount;
    public TextMeshProUGUI totalCount;
    public GameObject endScreen;

    public Animator blackScreenAnimator;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        //healthNum.text = "Health " + player.health.ToString();
        if (enemiesAlive == 0) {
            round++;
            NextWave(round);
            roundNum.text = "Round " + round.ToString();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Pause();
        }

        killCount.text = "Total Kill Count : " + kill.ToString();
    }

    //If there is no enemy left, Increase round number and summon more enemies
    public void NextWave(int round) {
        for (int i = 0; i < round; i++) {
            GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject enemySpawned = Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
            enemySpawned.GetComponent<EnemyManager>().gameManager = GetComponent<GameManager>();
            enemiesAlive++;
        }
    }

    //Get game over screen when health reaches 0
    public void EndGame() {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        endScreen.SetActive(true);
        roundsSurvived.text = round.ToString();
        totalCount.text = kill.ToString();
    }

    //Reload scenes and variables to its default
    public void ReplayGame() {
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
        round = 0;
        kill = 0;
    }

    //Animate Main menu transition
    public void MainMenu() {
        Time.timeScale = 1; 
        AudioListener.volume = 1;
        blackScreenAnimator.SetTrigger("FadeIn");
        Invoke("LoadMainMenuScene", .4f);
    }

    //Load Main Menu Scene
    void LoadMainMenuScene() {
        SceneManager.LoadScene(0);
    }

    //Activate Pause Screen
    public void Pause() {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        AudioListener.volume = 0;
    }


    //Deactivate Pause Screen
    public void UnPause() {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        AudioListener.volume = 1;
    }
}
