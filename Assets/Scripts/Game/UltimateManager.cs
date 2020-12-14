using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UltimateManager : MonoBehaviour
{
    public static UltimateManager instance;

    private bool blockLava, blockToxic, blockWater;

    public void Awake()
    {
        if (instance != null) Destroy(gameObject);

        instance = this;
        blockLava = blockToxic = blockWater = false;
    }

    public void Ultimate(Ammo curAmmo, Enemy enemy)
    {
        switch (curAmmo.Type)
        {
            case TypeOfAmmo.Fire:
                FireAttack(enemy, curAmmo);
                break;
            case TypeOfAmmo.Light:
                LightAttack(enemy, curAmmo);
                break;
            case TypeOfAmmo.Lava:
                StartCoroutine(LavaAttack(enemy, curAmmo));
                break;
            case TypeOfAmmo.Water:
                StartCoroutine(WaterAttack(enemy, curAmmo));
                break;
            case TypeOfAmmo.Magic:
                MagicAttack(enemy, curAmmo);
                break;
            case TypeOfAmmo.Toxic:
                StartCoroutine(ToxicAttack(enemy, curAmmo));
                break;
        }
    }

    private IEnumerator ToxicAttack(Enemy enemy, Ammo curAmmo)
    {
        if (!blockToxic)
        {
            blockToxic = true;
            float sec = 2.5f + (PlayerPrefs.GetInt("Dice" + curAmmo.ID + "Level", 1) - 1) * 0.04f;
            float percentage = 1 - (5f + (PlayerPrefs.GetInt("Dice" + curAmmo.ID + "Level", 1) - 1)) / 100f;

            float curSpeedMult = enemy.SpeedMultiplier;
            enemy.SpeedMultiplier = percentage * curSpeedMult;
            yield return new WaitForSeconds(sec);
            enemy.SpeedMultiplier = curSpeedMult;
            blockToxic = false;
        }
    }

    private void MagicAttack(Enemy enemy, Ammo ammo)
    {
        enemy.SelfHit(ammo.MagicDamage, ammo.AwardMultiplier);
    }

    private void FireAttack(Enemy enemy, Ammo ammo)
    {
        int idx;
        if ((idx = GameManager.instance.ListOfEnemies.IndexOf(enemy)) < GameManager.instance.ListOfEnemies.Count - 1)
        {
            GameManager.instance.ListOfEnemies[idx + 1].SelfHit(ammo.MagicDamage, ammo.AwardMultiplier);
        }
        if ((idx = GameManager.instance.ListOfEnemies.IndexOf(enemy)) < GameManager.instance.ListOfEnemies.Count - 2)
        {
            GameManager.instance.ListOfEnemies[idx + 2].SelfHit(ammo.MagicDamage, ammo.AwardMultiplier);
        }
    }

    private void LightAttack(Enemy enemy, Ammo ammo)
    {
        int idx;
        if ((idx = GameManager.instance.ListOfEnemies.IndexOf(enemy)) < GameManager.instance.ListOfEnemies.Count - 1)
        {
            GameManager.instance.ListOfEnemies[idx + 1].SelfHit(ammo.AttackingDamage, ammo.AwardMultiplier);
        }
        if ((idx = GameManager.instance.ListOfEnemies.IndexOf(enemy)) < GameManager.instance.ListOfEnemies.Count - 2)
        {
            GameManager.instance.ListOfEnemies[idx + 2].SelfHit(Mathf.RoundToInt(ammo.AttackingDamage * .35f), ammo.AwardMultiplier);
        }
    }

    private IEnumerator LavaAttack(Enemy enemy, Ammo ammo)
    {
        if (!blockLava)
        {
            blockLava = true;
            float curSpeedMult = enemy.SpeedMultiplier;
            enemy.SpeedMultiplier = 0;
            yield return new WaitForSeconds(4f);
            enemy.SpeedMultiplier = curSpeedMult;
            blockLava = false;
        }
    }

    private IEnumerator WaterAttack(Enemy enemy, Ammo curAmmo)
    {
        if (!blockWater)
        {
            blockWater = true;
            float curSpeedMult = enemy.SpeedMultiplier;
            enemy.SpeedMultiplier = 0.97f * curSpeedMult;
            yield return new WaitForSeconds(1f);
            enemy.SpeedMultiplier = curSpeedMult;
            blockWater = false;
        }
    }
}
