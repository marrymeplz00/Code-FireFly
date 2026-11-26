using UnityEngine;

public class SkillUpgradeManager : MonoBehaviour
{
    [Header("Player")]
    public PlayerControl playerControl;

    [Header("Upgrade Point")]
    public int upgradePoints = 0;

    // Add Upgrade Point
    public void AddUpgradePoints(int amount)
    {
        upgradePoints += amount;
    }

    // Normal Attack
    public bool UpgradeNormalAttack()
    {
        if (upgradePoints <= 0)
            return false;

        if (playerControl.attackLevel >= 3)
            return false;

        playerControl.attackLevel++;
        upgradePoints--;

        return true;
    }

    // Skill E
    public bool UpgradeE()
    {
        if (upgradePoints <= 0)
            return false;

        if (playerControl.eLevel >= 3)
            return false;

        playerControl.eLevel++;
        upgradePoints--;

        return true;
    }

    // Skill W
    public bool UpgradeW()
    {
        if (upgradePoints <= 0)
            return false;

        if (playerControl.wLevel >= 3)
            return false;

        playerControl.wLevel++;
        upgradePoints--;

        return true;
    }

    // Skill R
    public bool UpgradeR()
    {
        if (upgradePoints <= 0)
            return false;

        if (playerControl.rLevel >= 3)
            return false;

        playerControl.rLevel++;
        upgradePoints--;

        return true;
    }
}