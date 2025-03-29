using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class UniversalNightMode : MonoBehaviour
{
    // Группировка констант
    private const string PLAYER_PREFS_KEY = "NightMode";
    private const float TRANSITION_SPEED = 2f;
    private const float PARTIAL_INVERSION = 0.3f;

    [Header("Основные настройки")]
    [SerializeField] private KeyCode toggleKey = KeyCode.N;
    [SerializeField] private bool startInNightMode = false;
    
    [Header("Шейдерные эффекты")]
    [SerializeField] private Material invertMaterial;
    [Range(0, 1)] [SerializeField] private float maxInversion = 0.8f;
    
    [Header("UI Настройки")]
    [SerializeField] private Color nightTextColor = new Color(0.9f, 0.9f, 0.9f);
    [SerializeField] private Color dayTextColor = Color.black;
    [SerializeField] private Color nightBackgroundColor = new Color(0.15f, 0.15f, 0.15f);
    [SerializeField] private Color dayBackgroundColor = Color.white;
    
    [Header("Спрайты")]
    [SerializeField] private float spriteBrightnessModifier = -0.5f;

    // Кэшированные компоненты
    private bool isNightMode;
    private float currentInversion;
    private Camera mainCamera;
    private Dictionary<Graphic, Color> originalUIColors = new Dictionary<Graphic, Color>();
    private Dictionary<SpriteRenderer, Color> originalSpriteColors = new Dictionary<SpriteRenderer, Color>();

    void Awake()
    {
        mainCamera = GetComponent<Camera>();
        InitializeMaterial();
        CacheOriginalColors();
    }

    void Start()
    {
        LoadSettings();
        UpdateAllElements();
    }
    
    void Update()
    {
        HandleInput();
        UpdateTransition();
    }
    
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        ApplyShaderEffect(src, dest);
    }

    // Основные методы
    private void InitializeMaterial()
    {
        if (invertMaterial == null)
        {
            var shader = Shader.Find("Custom/InvertColors");
            if (shader != null) 
                invertMaterial = new Material(shader);
        }
    }

    private void CacheOriginalColors()
    {
        // Кэшируем только активные в начале игры элементы
        CacheUIElements();
        CacheSpriteRenderers();
    }

    private void CacheUIElements()
    {
        var allGraphics = FindObjectsOfType<Graphic>(true);
    }

    private void CacheSpriteRenderers()
    {
        var allRenderers = FindObjectsOfType<SpriteRenderer>(true);
    }

    private void LoadSettings()
    {
        isNightMode = PlayerPrefs.GetInt(PLAYER_PREFS_KEY, startInNightMode ? 1 : 0) == 1;
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleNightMode();
        }
    }

    private void UpdateTransition()
    {
        float target = isNightMode ? maxInversion : 0f;
        currentInversion = Mathf.MoveTowards(currentInversion, target, Time.deltaTime * TRANSITION_SPEED);
    }

    private void ApplyShaderEffect(RenderTexture src, RenderTexture dest)
    {
        if (currentInversion > 0 && invertMaterial != null)
        {
            invertMaterial.SetFloat("_Amount", currentInversion);
            Graphics.Blit(src, dest, invertMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }

    public void ToggleNightMode()
    {
        isNightMode = !isNightMode;
        UpdateAllElements();
        SaveSettings();
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt(PLAYER_PREFS_KEY, isNightMode ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void UpdateAllElements()
    {
        UpdateCameraBackground();
        UpdateUIElements();
        UpdateSprites();
    }

    private void UpdateCameraBackground()
    {
        mainCamera.backgroundColor = isNightMode ? nightBackgroundColor : dayBackgroundColor;
    }

    private void UpdateUIElements()
    {
        foreach (var item in originalUIColors)
        {
            if (item.Key == null) continue; // Проверка на уничтоженные объекты
            
            item.Key.color = isNightMode ? 
                InvertColor(item.Value, PARTIAL_INVERSION) : 
                item.Value;
        }
    }

    private void UpdateSprites()
    {
        foreach (var item in originalSpriteColors)
        {
            if (item.Key == null) continue;
            
            item.Key.color = isNightMode ? 
                AdjustBrightness(item.Value, spriteBrightnessModifier) : 
                item.Value;
        }
    }

    // Вспомогательные методы
    private Color InvertColor(Color color, float amount)
    {
        return Color.Lerp(
            color, 
            new Color(1 - color.r, 1 - color.g, 1 - color.b, color.a), 
            amount);
    }

    private Color AdjustBrightness(Color original, float modifier)
    {
        return new Color(
            Mathf.Clamp01(original.r + modifier),
            Mathf.Clamp01(original.g + modifier),
            Mathf.Clamp01(original.b + modifier),
            original.a);
    }
}