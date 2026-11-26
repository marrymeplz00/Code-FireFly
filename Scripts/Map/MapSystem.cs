using UnityEngine;

public class MapSystem : MonoBehaviour
{
    [Header("Map UI")]
    public GameObject mapUI;

    [Header("Map Panels")]
    public GameObject localMapPanel;
    public GameObject worldMapPanel;

    [Header("Mini Map")]
    public GameObject miniMap;

    [Header("Pause Menu")]
    public GameObject pauseMenu;

    [Header("Gameplay UI")]
    public GameObject gameplayUI;

    private bool isMapOpen = false;

    void Start()
    {
        // ปิด Map ตอนเริ่มเกม
        mapUI.SetActive(false);

        // Local Map เป็นค่าเริ่มต้น
        localMapPanel.SetActive(true);
        worldMapPanel.SetActive(false);

        // MiniMap แสดงตอนเล่นเกม
        miniMap.SetActive(true);

        // Pause Menu ปิด
        pauseMenu.SetActive(false);

        // Gameplay UI แสดง
        if (gameplayUI != null)
        {
            gameplayUI.SetActive(true);
        }

        Time.timeScale = 1f;
    }

    void Update()
    {
        // กด M เปิด/ปิด Map
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMap();
        }
    }

    public void ToggleMap()
    {
        if (isMapOpen)
        {
            CloseMap();
        }
        else
        {
            OpenMap();
        }
    }

    public void OpenMap()
    {
        isMapOpen = true;
        // เปิด Gameplay UI ก่อน
        if (gameplayUI != null)
        {
            gameplayUI.SetActive(true);
        }

        // เปิด Map
        mapUI.SetActive(true);

        // ปิด Pause Menu
        pauseMenu.SetActive(false);

        // ซ่อน MiniMap
        miniMap.SetActive(false);

        // เปิด Local Map
        localMapPanel.SetActive(true);
        worldMapPanel.SetActive(false);

        // เกมหยุด
        Time.timeScale = 0f;
    }

    // เปิด Map จากปุ่มใน Pause Menu
    public void OpenMapFromPause()
    {
        isMapOpen = true;

        // เปิด Gameplay UI
        if (gameplayUI != null)
        {
            gameplayUI.SetActive(true);
        }

        // เปิด Map 
        mapUI.SetActive(true);

        // ปิด Pause Menu
        pauseMenu.SetActive(false);

        // ซ่อน MiniMap
        miniMap.SetActive(false);

        // เปิด Local Map
        localMapPanel.SetActive(true);
        worldMapPanel.SetActive(false);

        
        // เกมหยุด
        Time.timeScale = 0f;
    }

    // ปิด Map
    public void CloseMap()
    {
        isMapOpen = false;

        // ปิด Map
        mapUI.SetActive(false);

        // แสดง MiniMap
        miniMap.SetActive(true);

        // Gameplay UI กลับมา
        if (gameplayUI != null)
        {
            gameplayUI.SetActive(true);
        }

        // เกมเล่นต่อ
        Time.timeScale = 1f;
    }

    // Local Map
    public void ShowLocalMap()
    {
        localMapPanel.SetActive(true);
        worldMapPanel.SetActive(false);
    }

    // World Map
    public void ShowWorldMap()
    {
        localMapPanel.SetActive(false);
        worldMapPanel.SetActive(true);
    }
}