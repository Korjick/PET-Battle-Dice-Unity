using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateTowerButton : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private int lvlCost, lvlCostIncrease;
    [SerializeField] private Text lvlCostLabel, nameLabel;
    [SerializeField] private Text lvlLabel, lvlCircleLabel;
    [SerializeField] private LeanButton upgrade;

    private int lvl;

    public Image Image { get { return image; } set { image = value; } }
    public int Lvl { get { return lvl; } set { lvl = value; } }
    public int LvlCost { get { return lvlCost; } set { lvlCost = value; } }
    public int LvlCostIncrease { get { return lvlCostIncrease; } }
    public Text LvlCostLabel { get { return lvlCostLabel; } set { lvlCostLabel = value; } }
    public Text LvlLabel { get { return lvlLabel; } set { lvlLabel = value; } }
    public Text LvlCircleLabel { get { return lvlCircleLabel; } set { lvlCircleLabel = value; } }
    public Text NameLabel { get { return nameLabel; } }
    public LeanButton Upgrade { get { return upgrade; } }

    public void Awake()
    {
        lvl = 1;
    }
}
