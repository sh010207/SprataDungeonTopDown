using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum UpgradeOption
{
    MaxHealth,
    AttackPower,
    Speed,
    Knockbak,
    AttackDelay,
    NumberOfProjectiles,
    COUNT
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private CharacterStat defalutStat;
    [SerializeField] private CharacterStat rangedStat;

    public Transform Player { get; private set; }
    public ObjectPool ObjectPool { get; private set; }
    public ParticleSystem EffectParticle;

    private HealthSystem playerHealth;

    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private Slider hpGaugeSlider;
    [SerializeField] private GameObject gameOverUI;

    [SerializeField] private int currentWaveIndex;
    private int currentSpawnCount = 0;
    private int waveSpawnCount = 0;
    private int waveSpawnPosCount = 0;

    public float spawnInterval = .5f;
    public List<GameObject> enemyPrefabs = new List<GameObject>();

    [SerializeField] private Transform spawnPositionsRoot;
    private List<Transform> spawnPositions = new List<Transform>(); 
    
    [SerializeField] private string playerTag = "Player";

    [SerializeField] private List<GameObject> rewards = new List<GameObject>();
    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;

        Player = GameObject.FindGameObjectWithTag(playerTag).transform;
        EffectParticle = GameObject.FindGameObjectWithTag("Particle").GetComponent<ParticleSystem>();

        ObjectPool = GetComponent<ObjectPool>();

        playerHealth = Player.GetComponent<HealthSystem>();
        playerHealth.OnDamage += UpdateHealthUI;
        playerHealth.OnHeal += UpdateHealthUI;
        playerHealth.OnDeath += GameOver;

        UpgradeStatInit();

        for(int i = 0; i < spawnPositionsRoot.childCount; i++)
        {
            spawnPositions.Add(spawnPositionsRoot.GetChild(i));
        }
    }

    private void UpgradeStatInit()
    {
        defalutStat.statsChangeType = StatsChangeType.Add;
        defalutStat.attackSO = Instantiate(defalutStat.attackSO);

        rangedStat.statsChangeType = StatsChangeType.Add;
        rangedStat.attackSO = Instantiate(rangedStat.attackSO);
    }

    private void Start()
    {
        StartCoroutine(StartNextWave());
    }

    IEnumerator StartNextWave()
    {
        while(true)
        {
            if (currentSpawnCount == 0)
            {
                UpdateWaveUI();
                
                yield return new WaitForSeconds(2f);

                ProcessWaveConditions();

                yield return StartCoroutine(SpawnEnemiesinWave());

                currentWaveIndex++;
            }
            
            yield return null;
        }
    }

    IEnumerator SpawnEnemiesinWave()
    {
        for(int i = 0; i < waveSpawnCount; i++)
        {
            int posidx  = Random.Range(0, spawnPositions.Count);
            for(int j = 0; j < waveSpawnPosCount; j++)
            {
                SpawnEnemyAyPosition(posidx);
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }
    private void SpawnEnemyAyPosition(int posidx)
    {
        int prefabidx = Random.Range(0, enemyPrefabs.Count);
        GameObject enemy = Instantiate(enemyPrefabs[prefabidx], spawnPositions[posidx].position, Quaternion.identity);
        enemy.GetComponent <CharacterStatHandler>().AddStatModifiers(defalutStat);
        enemy.GetComponent<CharacterStatHandler>().AddStatModifiers(rangedStat);

        enemy.GetComponent<HealthSystem>().OnDeath += OnEnemyDeath;
        currentSpawnCount++;

    }
    private void OnEnemyDeath()
    {
        currentSpawnCount--;        
    }
    private void ProcessWaveConditions()
    {
        // 20스테이지마다 이벤트가 발생
        if (currentWaveIndex % 20 == 0)
        {
            RandomUpgrade();
        }

        if (currentWaveIndex % 10 == 0)
        {
            IncreaseSpawnPositions();
        }

        if (currentWaveIndex % 5 == 0)
        {
            CreateReward();
        }

        if (currentWaveIndex % 3 == 0)
        {
            IncreaseWaveSpawnCount();
        }
    }
    private void IncreaseWaveSpawnCount()
    {
        waveSpawnCount++;
    }
    private void CreateReward()
    {
        int selectedRewardIndex = Random.Range(0,rewards.Count);
        int randomPositionindex = Random.Range(0, spawnPositions.Count);

        GameObject obj = rewards[selectedRewardIndex];
        Instantiate(obj, spawnPositions[randomPositionindex].position, Quaternion.identity);
    }
    private void IncreaseSpawnPositions()
    {
        waveSpawnPosCount = waveSpawnCount + 1 > spawnPositions.Count ? waveSpawnCount : waveSpawnCount + 1;
        waveSpawnCount = 0;
    }
    private void RandomUpgrade()
    {
        UpgradeOption option = (UpgradeOption)Random.Range(0, (int)UpgradeOption.COUNT);
        switch(option)
        {
            case UpgradeOption.MaxHealth:
                defalutStat.maxHealth += 2;
                break;

            case UpgradeOption.AttackPower:
                defalutStat.attackSO.power += 1;
                break;

            case UpgradeOption.Speed:
                defalutStat.speed += 0.1f;
                break;

            case UpgradeOption.Knockbak:
                defalutStat.attackSO.isOnKnockback = true;
                defalutStat.attackSO.knockbackPower += 1;
                defalutStat.attackSO.knockbackTime += 0.1f;
                break;

            case UpgradeOption.AttackDelay:
                defalutStat.attackSO.delay -= 0.05f;
                break;

            case UpgradeOption.NumberOfProjectiles:
                RangedAttackSO rangedAttackData = rangedStat.attackSO as RangedAttackSO;
                if (rangedAttackData != null) rangedAttackData.numberofProjectilesPerShot += 1;
                break;

            default:
                break;
        }
    }

    private void GameOver()
    {
        // UI켜주기
        gameOverUI.SetActive(true);
    }
    private void UpdateHealthUI()
    {
        hpGaugeSlider.value = playerHealth.CurrentHealth / playerHealth.MaxHealth;
    }
    
    private void UpdateWaveUI()
    {
        // waveText.text = (currentWaveIndex + 1).ToString();
        waveText.text = $"{currentWaveIndex + 1}";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}