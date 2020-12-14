using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCollector : MonoBehaviour
{
    [SerializeField] private GameObject[] dices;

    public static DiceCollector instance;

    public GameObject[] Dices { get { return dices; } }

    public void Awake()
    {
        if (instance != null) Destroy(gameObject);
        instance = this;

        DontDestroyOnLoad(gameObject);
    }
}
