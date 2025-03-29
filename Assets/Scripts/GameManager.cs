using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public float initialGameSpeed = 5f;
    public float gameSpeedIncrease = 0.1f;
    public float gameSpeed { get; private set; }

    [Header("Elements")]
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hiscoreText;
    [SerializeField] private Button retryButton;
    [SerializeField] private TextMeshProUGUI coinCountText;
    [SerializeField] private GameObject menu;
    [SerializeField] private Animator animOpen;
    [SerializeField] private int allCoins;
    [SerializeField] private TextMeshProUGUI allCoinCountText;

    public UnityEvent<int> OnCoinsUpdated = new UnityEvent<int>();
    private int _coins;

    private Player player;
    private Spawner spawner;
    private float score;
    private float hiscore;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
        spawner = FindAnyObjectByType<Spawner>();

        OnCoinsUpdated.AddListener(UpdateCoinUI);
        
        NewGame();
    }

    public void NewGame()
    {
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();
        foreach (var obstacle in obstacles)
        {
            Destroy(obstacle.gameObject);
        }

        menu.SetActive(false);

        score = 0f;
        _coins = 0;
        gameSpeed = initialGameSpeed;
        enabled = true;

        player.gameObject.SetActive(true);
        spawner.gameObject.SetActive(true);

        gameOverText.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);

        UpdateAllCoins();
        UpdateHiscore();
        UpdateCoinUI(_coins);
        scoreText.text = Mathf.RoundToInt(score).ToString("D5");
    }

    public void AddCoin()
    {
        _coins++;
        OnCoinsUpdated.Invoke(_coins);

    }

    private void UpdateCoinUI(int coins)
    {
        coinCountText.text = coins.ToString("D5");
    }

    public void GameOver()
    {
        gameSpeed = 0f;
        enabled = false;

        player.gameObject.SetActive(false);
        spawner.gameObject.SetActive(false);

        gameOverText.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);

        UpdateAllCoins();
        UpdateHiscore();

        menu.SetActive(true);
        animOpen.SetTrigger("OpenMenu");
    }

    private void Update()
    {
        gameSpeed += gameSpeedIncrease * Time.deltaTime;
        score += gameSpeed * Time.deltaTime;
        scoreText.text = Mathf.RoundToInt(score).ToString("D5");
    }

    private void UpdateHiscore()
    {
        hiscore = PlayerPrefs.GetFloat("hiscore", 0);

        if (score > hiscore)
        {
            hiscore = score;
            PlayerPrefs.SetFloat("hiscore", hiscore);
        }

        hiscoreText.text = Mathf.FloorToInt(hiscore).ToString("D5");
    }

    private void UpdateAllCoins()
    {
        allCoins = PlayerPrefs.GetInt("allCoins", 0);

        allCoins += _coins;
        PlayerPrefs.SetInt("allCoins", allCoins);

        allCoinCountText.text = Mathf.FloorToInt(allCoins).ToString("D5");
    }
}