using UnityEngine;
using DG.Tweening;

public class Menu : MonoBehaviour
{
    [SerializeField] private Transform shopPanel;
    [SerializeField] private Transform settingsPanel;
    //[SerializeField] private Transform statsPanel;
    [SerializeField] private Transform exitPanel;
    
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float activationDelay = 0.1f;
    
    private Vector3 visiblePosition = new Vector3(970, 0, 0);
    private Vector3 hiddenPosition = new Vector3(1490, 0, 0);

    private Transform[] allPanels;

    private void Awake()
    {
        // Инициализируем массив всех панелей
        allPanels = new Transform[] { shopPanel, settingsPanel, exitPanel };
    }

    // Функция для закрытия всех панелей
    public void CloseAllPanels()
    {
        foreach (Transform panel in allPanels)
        {
            if (panel.gameObject.activeSelf)
            {
                TogglePanel(panel, false);
            }
        }
    }

    private void CloseAllOtherPanels(Transform currentPanel)
    {
        foreach (Transform panel in allPanels)
        {
            if (panel != currentPanel && panel.gameObject.activeSelf)
            {
                TogglePanel(panel, false);
            }
        }
    }

    public void TogglePanel(Transform panel, bool show)
    {
        if (show)
        {
            // Закрываем все другие панели перед открытием текущей
            CloseAllOtherPanels(panel);
            
            // Активируем панель перед анимацией
            panel.gameObject.SetActive(true);
            
            // Анимация появления
            panel.DOMoveX(visiblePosition.x, animationDuration)
                .From(hiddenPosition)
                .SetEase(Ease.OutBack);
        }
        else
        {
            // Анимация исчезновения
            panel.DOMoveX(hiddenPosition.x, animationDuration)
                .From(visiblePosition)
                .SetEase(Ease.InBack)
                .OnComplete(() => 
                {
                    // Деактивируем панель после анимации с небольшой задержкой
                    DOVirtual.DelayedCall(activationDelay, delegate 
                    {
                        panel.gameObject.SetActive(false);
                    });
                });
        }
    }

    // Методы для кнопок
    public void ToggleShop(bool show) 
    {
        TogglePanel(shopPanel, show);
    }

    public void ToggleSettings(bool show) 
    {
        TogglePanel(settingsPanel, show);
    }

    public void ToggleStats(bool show) 
    {
        //TogglePanel(statsPanel, show);
    }

    public void ToggleExit(bool show) 
    {
        TogglePanel(exitPanel, show);
    }
}