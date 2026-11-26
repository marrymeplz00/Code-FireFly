using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillUpgradeUI : MonoBehaviour
{
    [Header("Manager")]
    public SkillUpgradeManager upgradeManager;

    [Header("Upgrade Point")]
    public TMP_Text upgradePointText;

    [Header("Level Colors")]
    public Color unlockedColor = Color.white;
    public Color lockedColor = Color.gray;

    [Header("Normal Attack")]
    public Button normalLv1Button;
    public Button normalLv2Button;
    public Button normalLv3Button;

    [Header("Skill E")]
    public Button eLv1Button;
    public Button eLv2Button;
    public Button eLv3Button;

    [Header("Skill W")]
    public Button wLv1Button;
    public Button wLv2Button;
    public Button wLv3Button;

    [Header("Skill R")]
    public Button rLv1Button;
    public Button rLv2Button;
    public Button rLv3Button;

    private void Start()
    {
        UpdateUI();
    }

    // Upgrade Normal
    public void UpgradeNormal()
    {
        if (upgradeManager != null && upgradeManager.UpgradeNormalAttack())
        {
            UpdateUI();
        }
    }

    // Upgrade E
    public void UpgradeE()
    {
        if (upgradeManager != null && upgradeManager.UpgradeE())
        {
            UpdateUI();
        }
    }

    // Upgrade W
    public void UpgradeW()
    {
        if (upgradeManager != null && upgradeManager.UpgradeW())
        {
            UpdateUI();
        }
    }

    // Upgrade R
    public void UpgradeR()
    {
        if (upgradeManager != null && upgradeManager.UpgradeR())
        {
            UpdateUI();
        }
    }

    // Update UI
    public void UpdateUI()
    {
        if (upgradeManager == null || upgradeManager.playerControl == null)
        {
            return;
        }

        PlayerControl player = upgradeManager.playerControl;

        UpdateLevelButtons(
            normalLv1Button,
            normalLv2Button,
            normalLv3Button,
            player.attackLevel
        );

        UpdateLevelButtons(
            eLv1Button,
            eLv2Button,
            eLv3Button,
            player.eLevel
        );

        UpdateLevelButtons(
            wLv1Button,
            wLv2Button,
            wLv3Button,
            player.wLevel
        );

        UpdateLevelButtons(
            rLv1Button,
            rLv2Button,
            rLv3Button,
            player.rLevel
        );

        if (upgradePointText != null)
        {
            upgradePointText.text = "Upgrade Point : " + upgradeManager.upgradePoints;
        }
    }

    // Level Buttons
    private void UpdateLevelButtons(
        Button lv1,
        Button lv2,
        Button lv3,
        int currentLevel)
    {
        if (lv1 == null || lv2 == null || lv3 == null)
            return;

        bool canUpgrade = upgradeManager.upgradePoints > 0;

        // สี
        SetButtonColor(lv1, currentLevel >= 1);
        SetButtonColor(lv2, currentLevel >= 2);
        SetButtonColor(lv3, currentLevel >= 3);

        // ปุ่ม Lv.1 เป็น Level เริ่มต้นจึงกดไม่ได้
        lv1.interactable = false;

        // อัปได้ทีละ Level
        lv2.interactable = currentLevel == 1 && canUpgrade;

        lv3.interactable = currentLevel == 2 && canUpgrade;
    }

    // Button Color
    private void SetButtonColor(Button button, bool unlocked)
    {
        if (button == null)
            return;

        Color color = unlocked ? unlockedColor : lockedColor;

        // ป้องกัน Button เปลี่ยนสีเอง
        ColorBlock colors = button.colors;

        colors.normalColor = color;
        colors.highlightedColor = color;
        colors.pressedColor = color;
        colors.selectedColor = color;
        colors.disabledColor = color;

        button.colors = colors;

        // เปลี่ยน Graphic โดยตรง
        if (button.targetGraphic != null)
        {
            button.targetGraphic.color = color;
        }
    }
}