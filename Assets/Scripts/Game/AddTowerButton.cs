using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddTowerButton : MonoBehaviour
{
    [SerializeField] private int cost, diceCostIncrease;
    [SerializeField] private Text costLabel;

    private int tmpCost;

    public int Cost { get { return cost; } set { cost = value; } }
    public Text CostLabel { get { return costLabel; } set { costLabel = value; } }
    public int TmpCost { get { return tmpCost; } set { tmpCost = value; } }
    public int DiceCostIncrease { get { return diceCostIncrease; } }
}
