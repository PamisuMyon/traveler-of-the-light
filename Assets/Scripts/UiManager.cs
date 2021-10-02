using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour 
{

    public static UiManager Instance { get; private set; }

    public Slider lpSlider;
    public Color selectedTextColor;

    GameObject gameUi;
    GameObject endUi;
    GameObject menuUi;
    GameObject menuPanel;
    GameObject levelSelectionPanel;
    Text fullScreenText;

    void Start()
    {
        Instance = this;

        gameUi = transform.Find("GameUi").gameObject;
        menuUi = transform.Find("MenuUi").gameObject;
        endUi = transform.Find("EndUi").gameObject;
        lpSlider = transform.Find("GameUi/LpSlider").GetComponent<Slider>();

        menuPanel = transform.Find("MenuUi/MenuPanel").gameObject;
        levelSelectionPanel = transform.Find("MenuUi/LevelSelectionPanel").gameObject;
        fullScreenText = transform.Find("MenuUi/MenuPanel/FullScreen/Text").GetComponent<Text>();
        Button[] levelButtons = levelSelectionPanel.GetComponentsInChildren<Button>();
        var curScene = SceneManager.GetActiveScene().buildIndex;
        for (int i = 0; i < levelButtons.Length; i++)
        {
            var button = levelButtons[i];
            if (i == curScene)
                button.GetComponentInChildren<Text>().color = selectedTextColor;
            int index = i;
            button.onClick.AddListener(() => 
            {
                Time.timeScale = 1;
                GameManager.Instance.LoadLevel(index);
            });
        }

        ShowGame();
        UpdateFullScreenText(Screen.fullScreen);
    }

    public void ShowGame()
    {
        gameUi.SetActive(true);
        endUi.SetActive(false);
    }

    public void ShowEnd()
    {
        gameUi.SetActive(false);
        endUi.SetActive(true);
    }

    public void ToggleMenu()
    {
        if (levelSelectionPanel.activeInHierarchy)
        {
            levelSelectionPanel.SetActive(false);
            menuPanel.SetActive(true);
        }
        else if (menuPanel.activeInHierarchy)
        {
            menuPanel.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void OnMenuButton()
    {
        if (levelSelectionPanel.activeInHierarchy)
        {
            levelSelectionPanel.SetActive(false);
        }
        else if (menuPanel.activeInHierarchy)
        {
            menuPanel.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            menuPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void OpenLevelSelection()
    {
        levelSelectionPanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        GameManager.Instance.RestartCurrent();
    }

    public void ToggleFullScreen()
    {
        GameManager.Instance.ToggleFullScreen();
    }

    public void UpdateFullScreenText(bool isFullScreen)
    {
        if (isFullScreen)
            fullScreenText.text = "Exit Full Screen";
        else
            fullScreenText.text = "Enter Full Screen";
    }

}