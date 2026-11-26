using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingMenu;
    [SerializeField] private GameObject generalPanel;
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private GameObject audioPanel;

    [Header("Gameplay UI")]
    [SerializeField] private GameObject gameplayUI;

    [Header("Upgrade Menu")]
    [SerializeField] private GameObject upgradeMenu;

    [Header("Credits Menu")]
    [SerializeField] private GameObject creditsMenu;

    private bool isPaused = false;

    private void Start()
    {
        pauseMenu.SetActive(false);

        if (settingMenu != null)
            settingMenu.SetActive(false);

        if (upgradeMenu != null)
            upgradeMenu.SetActive(false);

        if (creditsMenu != null)
            creditsMenu.SetActive(false);

        if (gameplayUI != null)
            gameplayUI.SetActive(true);

        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // Pause
    public void Pause()
    {
        isPaused = true;

        pauseMenu.SetActive(true);

        if (settingMenu != null)
            settingMenu.SetActive(false);

        if (upgradeMenu != null)
            upgradeMenu.SetActive(false);

        if (creditsMenu != null)
            creditsMenu.SetActive(false);

        if (gameplayUI != null)
            gameplayUI.SetActive(false);

        Time.timeScale = 0f;
    }

    // Resume
    public void Resume()
    {
        isPaused = false;

        pauseMenu.SetActive(false);

        if (settingMenu != null)
            settingMenu.SetActive(false);

        if (upgradeMenu != null)
            upgradeMenu.SetActive(false);

        if (creditsMenu != null)
            creditsMenu.SetActive(false);

        if (gameplayUI != null)
            gameplayUI.SetActive(true);

        Time.timeScale = 1f;
    }

    // Upgrade

    public void OpenUpgradeMenu()
    {
        isPaused = true;

        pauseMenu.SetActive(false);

        if (settingMenu != null)
            settingMenu.SetActive(false);

        if (creditsMenu != null)
            creditsMenu.SetActive(false);

        if (upgradeMenu != null)
            upgradeMenu.SetActive(true);

        if (gameplayUI != null)
            gameplayUI.SetActive(false);

        Time.timeScale = 0f;
    }

    public void CloseUpgradeMenu()
    {
        isPaused = true;

        if (upgradeMenu != null)
            upgradeMenu.SetActive(false);

        pauseMenu.SetActive(true);

        Time.timeScale = 0f;
    }

    // Credits
    public void OpenCredits()
    {
        Debug.Log("OpenCredits ถูกเรียก");

        isPaused = true;

        pauseMenu.SetActive(false);

        if (settingMenu != null)
            settingMenu.SetActive(false);

        if (upgradeMenu != null)
            upgradeMenu.SetActive(false);

        if (creditsMenu != null)
        {
            creditsMenu.SetActive(true);
            Debug.Log("เปิด CreditsMenu แล้ว");
        }
        else
        {
            Debug.LogError("ยังไม่ได้ใส่ Credits Menu ใน Inspector!");
        }

        if (gameplayUI != null)
            gameplayUI.SetActive(false);

        Time.timeScale = 0f;
    }

    // Close Credits
    public void CloseCredits()
    {
        isPaused = true;

        if (creditsMenu != null)
            creditsMenu.SetActive(false);

        pauseMenu.SetActive(true);

        Time.timeScale = 0f;
    }

    // Settings
    public void ShowGeneral()
    {
        generalPanel.SetActive(true);
        controlPanel.SetActive(false);
        audioPanel.SetActive(false);
    }

    public void ShowControl()
    {
        generalPanel.SetActive(false);
        controlPanel.SetActive(true);
        audioPanel.SetActive(false);
    }

    public void ShowAudio()
    {
        generalPanel.SetActive(false);
        controlPanel.SetActive(false);
        audioPanel.SetActive(true);
    }

    public void OpenSetting()
    {
        settingMenu.SetActive(true);
        pauseMenu.SetActive(false);

        ShowGeneral();
    }

    // Scene
    public void Exit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void Resumer()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Map1");
    }
}