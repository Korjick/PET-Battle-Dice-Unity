using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfAmmo
{
    Fire, Light, Water, Wind, Lava, Magic, Toxic, Electric
}

public class Ammo : MonoBehaviour
{
    [SerializeField] private TypeOfAmmo type;
    [SerializeField] private AudioSource pew;

    private int attackingDamage, magicDamage;
    private float attackingDamageMultiplier, awardMultiplier;
    private Enemy target;

    public int AttackingDamage { set { attackingDamage = value; } get { return attackingDamage; } }
    public int MagicDamage { set { magicDamage = value; } get { return magicDamage; } }
    public float AwardMultiplier { set { awardMultiplier = value; } get { return awardMultiplier; } }
    public float AttackingDamageMultiplier { get { return attackingDamageMultiplier; } set { attackingDamageMultiplier = value; } }
    public TypeOfAmmo Type { get { return type; } }
    public Enemy Target { get { return target; } set { target = value; } }
    public AudioSource Pew { get { return pew; } }
    public int ID { get; set; }
}
