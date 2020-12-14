using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Improve : MonoBehaviour
{
    [SerializeField] Image sprite;
    [SerializeField] Text name, desc, criticalDamage;
    [SerializeField] Text attack, speed, richment, magic;
    [SerializeField] Text updateCost, level, moneyCount, improveMoneyCount;
    [SerializeField] Text attackInc, speedInc, richmentInc, magicInc, criticalDamageInc;

    private int id;

    public int ID { get { return id; } set { id = value; } }
    public float AttackInc { get; set; }
    public float SpeedInc { get; set; }
    public float RichmentInc { get; set; }
    public float MagicInc { get; set; }

    public void Init()
    {
        sprite.gameObject.GetComponent<Animator>().runtimeAnimatorController = 
            DiceCollector.instance.Dices[id].GetComponent<Animator>().runtimeAnimatorController;
        name.text = DiceCollector.instance.Dices[id].name.Split('(')[0];

        level.text = PlayerPrefs.GetInt("Dice" + id + "Level", 1).ToString() + " / 15";
        moneyCount.text = PlayerPrefs.GetInt("MoneyCount", 0).ToString();
        improveMoneyCount.text = PlayerPrefs.GetInt("MoneyCount", 0).ToString();

        attack.text = ((int)PlayerPrefs.GetFloat("Dice" + id + "AttackDamage", DiceCollector.instance.Dices[id].GetComponent<Dice>().AttackingDamage)).ToString();
        speed.text = PlayerPrefs.GetFloat("Dice" + id + "AttackSpeed", DiceCollector.instance.Dices[id].GetComponent<Dice>().TimeBetweenAttack).ToString();
        richment.text = PlayerPrefs.GetFloat("Dice" + id + "AwardMultiplier", DiceCollector.instance.Dices[id].GetComponent<Dice>().AwardMultiplier).ToString();
        magic.text = ((int)PlayerPrefs.GetFloat("Dice" + id + "MagicDamage", DiceCollector.instance.Dices[id].GetComponent<Dice>().MagicDamage)).ToString();

        updateCost.text = PlayerPrefs.GetInt("Dice" + id + "UpdateCost", 5).ToString();

        attackInc.text = "+ " + AttackInc.ToString();
        speedInc.text = SpeedInc.ToString();
        richmentInc.text = "+ " + RichmentInc.ToString();
        magicInc.text = "+ " + MagicInc.ToString();

        criticalDamage.text = PlayerPrefs.GetInt("CriticalDamage", 100).ToString();
        criticalDamageInc.text = "+ " + PlayerPrefs.GetInt("Dice" + id + "CriticalMaxCount", 1).ToString();
    }

    public void ImproveDice()
    {
        if (PlayerPrefs.GetInt("MoneyCount", 0) >= PlayerPrefs.GetInt("Dice" + id + "UpdateCost", 5))
        {
            PlayerPrefs.SetInt("Dice" + id + "Level", PlayerPrefs.GetInt("Dice" + id + "Level", 1) + 1);

            PlayerPrefs.SetInt("MoneyCount", PlayerPrefs.GetInt("MoneyCount", 0) - PlayerPrefs.GetInt("Dice" + id + "UpdateCost", 5));
            PlayerPrefs.SetInt("Dice" + id + "UpdateCost", PlayerPrefs.GetInt("Dice" + id + "UpdateCost", 5) * 2);

            PlayerPrefs.SetInt("Dice" + id + "CriticalCount", PlayerPrefs.GetInt("Dice" + id + "CriticalCount", 0) + 1);
            PlayerPrefs.SetInt("CriticalDamage", PlayerPrefs.GetInt("CriticalDamage", 100) 
                + PlayerPrefs.GetInt("Dice" + id + "CriticalMaxCount", 1));
            if (PlayerPrefs.GetInt("Dice" + id + "CriticalMaxCount", 1) == PlayerPrefs.GetInt("Dice" + id + "CriticalCount", 0))
            {
                PlayerPrefs.SetInt("Dice" + id + "CriticalMaxCount", PlayerPrefs.GetInt("Dice" + id + "CriticalMaxCount", 1) + 1);
                PlayerPrefs.SetInt("Dice" + id + "CriticalCount", 0);
            }

            PlayerPrefs.SetFloat("Dice" + id + "AttackDamage", DiceCollector.instance.Dices[id].GetComponent<Dice>().AttackingDamage + DiceCollector.instance.Dices[id].GetComponent<Dice>().AttackInc);
            PlayerPrefs.SetFloat("Dice" + id + "AttackSpeed", DiceCollector.instance.Dices[id].GetComponent<Dice>().TimeBetweenAttack + DiceCollector.instance.Dices[id].GetComponent<Dice>().SpeedInc);
            PlayerPrefs.SetFloat("Dice" + id + "AwardMultiplier", DiceCollector.instance.Dices[id].GetComponent<Dice>().AwardMultiplier + DiceCollector.instance.Dices[id].GetComponent<Dice>().RichmentInc);
            PlayerPrefs.SetFloat("Dice" + id + "MagicDamage", DiceCollector.instance.Dices[id].GetComponent<Dice>().MagicDamage + DiceCollector.instance.Dices[id].GetComponent<Dice>().MagicInc);
        }

        PlayerPrefs.Save();
        Init();
    }
}
