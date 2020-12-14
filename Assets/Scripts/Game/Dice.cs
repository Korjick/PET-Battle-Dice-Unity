using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Dice : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private const float closestDistanceBetweenAmmoAndEnemy = 0.01f;

    private Enemy target;
    private float attackCounter;
    private bool isAttacking;
    private Transform parent;
    private int sibIDx, lvl, dotLvl;

    [SerializeField] private float timeBetweenAttack, attackingRadius;
    [SerializeField] private Ammo ammoTile;
    [SerializeField] private int id, attackingDamage, magicDamage;
    [SerializeField] [Range(1, int.MaxValue)] private float awardMultiplier;
    [SerializeField] private Text lvlLabel;

    [SerializeField] public float AttackInc;
    [SerializeField] public float SpeedInc;
    [SerializeField] public float RichmentInc;
    [SerializeField] public float MagicInc;

    [Header("Sub-Menu")]
    [SerializeField] private GameObject subMenu1, subMenu2;
    [SerializeField] public string Description, Rule;

    [Header("Points")]
    [SerializeField] private GameObject dot1, dot2, dot3, dot4, dot5, dot6;

    public int AttackingDamage { set { attackingDamage = value; } get { return attackingDamage; } }
    public int MagicDamage { set { magicDamage = value; } get { return magicDamage; } }
    public float AwardMultiplier { set { awardMultiplier = value; } get { return awardMultiplier; } }
    public float TimeBetweenAttack { set { timeBetweenAttack = value; } get { return timeBetweenAttack; } }

    public int ID { get { return id; } }
    public int Lvl { get { return lvl; } set { lvl = value; } }
    public Text LvlLabel { get { return lvlLabel; } set { lvlLabel = value; } }
    public bool InMenu { get; set; }
    public bool SubMenu { get; set; }

    public void Awake()
    {
        canvas = transform.root.GetComponent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        isAttacking = false;
        target = null;

        parent = transform.parent;
        sibIDx = transform.GetSiblingIndex();

        timeBetweenAttack = PlayerPrefs.GetFloat("Dice" + id + "AttackSpeed", timeBetweenAttack);
        attackingRadius = PlayerPrefs.GetFloat("Dice" + id + "AttackRadius", attackingRadius);
        attackingDamage = (int)PlayerPrefs.GetFloat("Dice" + id + "AttackDamage", attackingDamage);
        awardMultiplier = PlayerPrefs.GetFloat("Dice" + id + "AwardMultiplier", awardMultiplier);
        magicDamage = (int)PlayerPrefs.GetFloat("Dice" + id + "MagicDamage", magicDamage);
    }

    public void Start()
    {
        lvlLabel.text = lvl.ToString();
        dotLvl = 1;
        if(InMenu)
        {
            if (SubMenu) subMenu1.SetActive(true);
            else
            {
                subMenu1.SetActive(true);
                subMenu1.GetComponent<RectTransform>().anchoredPosition = Vector2.up * 90;
                subMenu2.SetActive(true);
            }

            subMenu1.transform.GetChild(0).GetComponent<Text>().text = gameObject.name.Split('(')[0].ToUpper();
            if (PlayerPrefs.GetInt("Dice" + id + "Enable", 0) != 0)
            {
                subMenu2.transform.GetChild(0).gameObject.SetActive(true);
                subMenu2.transform.GetChild(1).gameObject.SetActive(true);
                subMenu2.transform.GetChild(2).gameObject.SetActive(true);
                subMenu2.transform.GetChild(3).gameObject.SetActive(false);

                subMenu2.transform.GetChild(1).GetComponent<Text>().text = "UPGRADE";
                subMenu2.transform.GetChild(2).GetComponent<Text>().text = "x" + PlayerPrefs.GetInt("Dice" + id + "UpdateCost", 5);
                subMenu2.GetComponent<Button>().onClick.AddListener(() => MenuUIManager.instance.ActivateImprove(gameObject));
            }
            else
            {
                subMenu2.transform.GetChild(0).gameObject.SetActive(false);
                subMenu2.transform.GetChild(1).gameObject.SetActive(true);
                subMenu2.transform.GetChild(2).gameObject.SetActive(false);
                subMenu2.transform.GetChild(3).gameObject.SetActive(true);

                subMenu2.transform.GetChild(1).GetComponent<Text>().text = Rule.Split('@')[0];
                subMenu2.transform.GetChild(3).GetComponent<Text>().text = Rule.Split('@')[1];
            }
        }
    }

    public void Update()
    {
        if (GameManager.instance != null)
        {
            attackCounter -= Time.deltaTime;

            if (target == null)
            {
                target = GetNearestEnemy();
            }
            else
            {
                if (attackCounter <= 0)
                {
                    isAttacking = true;
                    attackCounter = timeBetweenAttack;
                }
                else isAttacking = false;

                if (Vector2.Distance(transform.position, target.transform.position) > attackingRadius) target = null;
            }
        }
    }

    public void FixedUpdate()
    {
        if (isAttacking) Attack();
    }

    private void Attack()
    {
        isAttacking = false;
        Ammo curAmmo = Instantiate(ammoTile, transform.parent.parent.parent);
        curAmmo.ID = id;
        curAmmo.transform.position = transform.position;
        curAmmo.AttackingDamageMultiplier = Mathf.Pow(1.1f, lvl - 1);
        curAmmo.AttackingDamage = attackingDamage;
        curAmmo.AwardMultiplier = awardMultiplier;
        curAmmo.MagicDamage = magicDamage;

        if (target == null) Destroy(curAmmo);
        else StartCoroutine(MoveAmmo(curAmmo));
    }

    private IEnumerator MoveAmmo(Ammo curAmmo)
    {
        Enemy curTarger = target;
        curAmmo.Target = curTarger;
        while (curTarger != null && GetTargetDistance(curTarger, curAmmo) > closestDistanceBetweenAmmoAndEnemy)
        {
            Vector2 dir = curTarger.transform.position - transform.position;
            float angleDirection = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

            curAmmo.transform.rotation = Quaternion.AngleAxis(angleDirection, Vector3.forward);
            curAmmo.transform.position = Vector2.MoveTowards(curAmmo.transform.position, curTarger.transform.position, 5f * Time.deltaTime);

            yield return null;
        }

        if (curTarger == null && curAmmo != null) Destroy(curAmmo.gameObject);
    }

    private float GetTargetDistance(Enemy thisEnemy, Ammo curAmmo)
    {
        if (thisEnemy == null || curAmmo == null) return 0;
        return Mathf.Abs(Vector2.Distance(curAmmo.transform.position, thisEnemy.transform.position));
    }

    private Enemy GetNearestEnemy()
    {
        /* Enemy nearestEnemy = null;
        float smallestDistance = float.PositiveInfinity;

        foreach (Enemy enemy in GetEnemiesInRange())
            if (Vector2.Distance(transform.position, enemy.transform.position) < smallestDistance)
            {
                smallestDistance = Vector2.Distance(transform.position, enemy.transform.position);
                nearestEnemy = enemy;
            }
        */
        try
        {
            Enemy first = null;
            float tmpDist = int.MaxValue;
            for (int i = 0; i < GameManager.instance.ListOfEnemies.Count; i++)
            {
                if (Vector2.Distance(transform.position, GameManager.instance.ListOfEnemies[i].transform.position) <= attackingRadius)
                {
                    if (Mathf.Abs(GameManager.instance.ListOfEnemies[i].transform.position.x - GameManager.instance.Exit.transform.position.x) < tmpDist)
                    {
                        first = GameManager.instance.ListOfEnemies[i];
                        tmpDist = Mathf.Abs(GameManager.instance.ListOfEnemies[i].transform.position.x - GameManager.instance.Exit.transform.position.x);
                    }
                }
            }
            return first;
        }
        catch
        {
            return null;
        }
    }

    /*
    private List<Enemy> GetEnemiesInRange()
    {
        List<Enemy> enemiesInRange = new List<Enemy>();

        foreach (Enemy enemy in GameManager.instance.ListOfEnemies)
            if (Vector2.Distance(transform.position, enemy.transform.position) <= attackingRadius) enemiesInRange.Add(enemy);

        return enemiesInRange;
    } */

    public void OnBeginDrag(PointerEventData eventData)
    {
        parent = transform.parent;
        sibIDx = transform.GetSiblingIndex();

        transform.SetParent(parent.parent.parent);
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.pointerEnter.tag == "Slot" && eventData.pointerEnter.transform.childCount == 0) parent = eventData.pointerEnter.transform;
        if (eventData.pointerEnter.tag == "Dice")
        {
            Dice child = eventData.pointerEnter.GetComponent<Dice>();

            if(child.id == id && child.dotLvl < 6)
            {
                switch (child.dotLvl)
                {
                    case 1:
                        child.dot1.SetActive(false);
                        child.dotLvl++;
                        child.dot2.SetActive(true);
                        break;
                    case 2:
                        child.dot2.SetActive(false);
                        child.dotLvl++;
                        child.dot3.SetActive(true);
                        break;
                    case 3:
                        child.dot3.SetActive(false);
                        child.dotLvl++;
                        child.dot4.SetActive(true);
                        break;
                    case 4:
                        child.dot4.SetActive(false);
                        child.dotLvl++;
                        child.dot5.SetActive(true);
                        break;
                    case 5:
                        child.dot5.SetActive(false);
                        child.dotLvl++;
                        child.dot6.SetActive(true);
                        break;
                }
                Destroy(gameObject);
                return;
            }

            transform.SetParent(child.transform.parent);
            transform.SetSiblingIndex(child.sibIDx);

            child.transform.SetParent(parent);
            child.transform.SetSiblingIndex(sibIDx);
            child.transform.localPosition = Vector3.zero;

            parent = transform.parent;
            child.sibIDx = child.transform.GetSiblingIndex();
        }
        if(eventData.pointerEnter.tag == "Menu")
        {
            for(int i = 0; i < MenuUIManager.instance.PresetSlot.transform.childCount; i++)
            {
                if(MenuUIManager.instance.PresetSlot.transform.GetChild(i).GetChild(0).GetComponent<Dice>().id == id)
                {
                    transform.SetParent(parent);
                    transform.SetSiblingIndex(sibIDx);
                    sibIDx = transform.GetSiblingIndex();

                    canvasGroup.alpha = 1f;
                    canvasGroup.blocksRaycasts = true;
                    transform.localPosition = Vector3.zero;
                    return;
                }
            }

            Dice child = eventData.pointerEnter.GetComponent<Dice>();
            Transform childParent = child.transform.parent;

            Destroy(child.gameObject);
            GameObject newDice = Instantiate(DiceCollector.instance.Dices[id], childParent);
            newDice.transform.localPosition = Vector3.zero;
            newDice.tag = "Menu";
            newDice.GetComponent<Dice>().InMenu = true;
            newDice.GetComponent<Dice>().SubMenu = true;
            newDice.GetComponent<Dice>().LvlLabel.transform.parent.gameObject.SetActive(false);
        }


        transform.SetParent(parent);
        transform.SetSiblingIndex(sibIDx);
        sibIDx = transform.GetSiblingIndex();

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        transform.localPosition = Vector3.zero;
    }

    public void OnDestroy()
    {
        for (int i = 0; i < canvas.transform.GetChild(0).childCount; i++)
        {
            GameObject curObj = canvas.transform.GetChild(0).GetChild(i).gameObject;
            if (curObj.tag == "Ammo") Destroy(curObj);
        }
    }
}
