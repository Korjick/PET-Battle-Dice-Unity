using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    Next, Play, GameOver, Win
}

[System.Serializable]
public class EnemiesPerSpawn
{
    public int[] enemiesID;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private int totalMoney, waveNumber, totalEscaped, score, totalKilled;
    private int roundSpawned, enemiesPerSpawn, totalEnemies;
    private GameState currentStatus;

    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private GameObject exit;
    [SerializeField] private GameObject[] wayPoints, enemies;
    [SerializeField] private EnemiesPerSpawn[] enemiesPerRound;
    [SerializeField] private int startMoney, gameMoney, lostMoney;
    [SerializeField] private Text totalMoneyLabel, wave, waveNotification;

    public int TotalEscaped { get { return totalEscaped; } set { totalEscaped = value; } }
    public int TotalKilled { get { return totalKilled; } set { totalKilled = value; } }
    public int TotalMoney { get { return totalMoney; } }

    public List<Enemy> ListOfEnemies { get; set; } = new List<Enemy>();
    public GameObject[] WayPoints { get { return wayPoints; } }
    public GameObject Exit { get { return exit; } }

    public bool Paused { get; set; }

    public void Awake()
    {
        if (instance != null) Destroy(gameObject);

        instance = this;
        currentStatus = GameState.Play;
    }

    public void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Paused = false;

        waveNumber = roundSpawned = totalKilled = 0;
        totalMoney = startMoney;
        totalMoneyLabel.text = totalMoney.ToString();
        enemiesPerSpawn = enemiesPerRound[waveNumber].enemiesID.Length;
        wave.text = "Wave " + (waveNumber + 1).ToString() + "/" + enemiesPerRound.Length;
        waveNotification.text = "WAVE " + (waveNumber + 1).ToString();
        LeanPulse.PulseAll("Notification");
        StartCoroutine(Spawn());

        totalEnemies = 0;
        for (int i = 0; i < enemiesPerRound.Length; i++) totalEnemies += enemiesPerRound[i].enemiesID.Length;
    }

    public void Update()
    {
        if (roundSpawned >= enemiesPerSpawn) currentStatus = GameState.Next;
        else if (roundSpawned < enemiesPerSpawn) currentStatus = GameState.Play;

        if(currentStatus == GameState.Next)
        {
            currentStatus = GameState.Play;
            waveNumber += 1;
            if (waveNumber < enemiesPerRound.Length)
            {
                wave.text = "Wave " + (waveNumber + 1).ToString() + "/" + enemiesPerRound.Length;
                waveNotification.text = "WAVE " + (waveNumber + 1).ToString();
                LeanPulse.PulseAll("Notification");
                enemiesPerSpawn = enemiesPerRound[waveNumber].enemiesID.Length;
                roundSpawned = 0;
                StartCoroutine(Spawn());
            }
        }
    }

    private IEnumerator Spawn()
    {
        if (enemiesPerSpawn > 0)
        {
            yield return new WaitForSecondsRealtime(1.5f);
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                while (Paused) yield return null; 
                GameObject curEnemy = Instantiate(enemies[enemiesPerRound[waveNumber].enemiesID[i]], spawnPoint.transform.parent);
                roundSpawned += 1;
                curEnemy.transform.position = spawnPoint.transform.position;
                curEnemy.GetComponent<Enemy>().ID = enemiesPerRound[waveNumber].enemiesID[i] + 1;
                if (i < enemiesPerSpawn - 1)
                {
                    yield return new WaitForSecondsRealtime(0.75f);
                }
            }
        }
    }

    public void SetCurrentGameState()
    {
        if (totalEscaped >= 3) currentStatus = GameState.GameOver;
        else if (totalKilled >= totalEnemies - totalEscaped) currentStatus = GameState.Win;
    }

    public void CheckCurrentGameState()
    {
        switch (currentStatus)
        {
            case GameState.GameOver:
                DestroyEnemies();
                Time.timeScale = 0;
                GameUIManager.instance.GameOverPanel.SetActive(true);
                score = lostMoney;
                GameUIManager.instance.GameOverScoreLabel.text = score.ToString();
                PlayerPrefs.SetInt("MoneyCount", PlayerPrefs.GetInt("MoneyCount", 0) + score);
                break;

            case GameState.Win:
                Time.timeScale = 0;
                GameUIManager.instance.AwardPanel.SetActive(true);

                for (int i = 0; i < 3 - totalEscaped; i++)
                    GameUIManager.instance.AwardPanel.transform.GetChild(2).GetChild(i).gameObject.SetActive(true);

                if (PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "StarCount", 0) < 3 - totalEscaped)
                {
                    PlayerPrefs.SetInt("StarCount",
                        PlayerPrefs.GetInt("StarCount", 0) - PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "StarCount", 0) + (3 - totalEscaped));
                    PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "StarCount", 3 - totalEscaped);
                }

                score = gameMoney - totalEscaped * 10;
                PlayerPrefs.SetInt("MoneyCount", PlayerPrefs.GetInt("MoneyCount", 0) + score);
                GameUIManager.instance.AwardScoreLabel.text = score.ToString();
                break;
        }
    }

    public void RegisterEnemy(Enemy enemy)
    {
        ListOfEnemies.Add(enemy);
    }

    public void UnRegisterEnemy(Enemy enemy)
    {
        ListOfEnemies.Remove(enemy);
        Destroy(enemy.gameObject);
    }

    public void DestroyEnemies()
    {
        Time.timeScale = 0;
        StopAllCoroutines();
        for (int i = 0; i < exit.transform.parent.childCount; i++)
            if (exit.transform.parent.GetChild(i).tag == "Enemy") Destroy(exit.transform.parent.GetChild(i).gameObject);
    }

    public void AddMoney(int amount)
    {
        totalMoney += amount;
        totalMoneyLabel.text = totalMoney.ToString();
    }

    public void SubtractMoney(int amount)
    {
        totalMoney -= amount;
        totalMoneyLabel.text = totalMoney.ToString();
    }
}
