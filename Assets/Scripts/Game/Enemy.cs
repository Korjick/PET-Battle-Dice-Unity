using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int hp, rewardAmount;
    [SerializeField] [Range(0, float.MaxValue)] float speedMultiplier;
    [SerializeField] private float speedFPS;
    [SerializeField] GameObject text;

    private Transform self;
    private float navigationTime;
    private Text hpLabel;
    private int pointToGo, id;
    private bool isDestroyed;

    public int ID { set { id = value; } }
    public int HP { get { return hp; } }
    public float SpeedMultiplier { get { return speedMultiplier; } set { speedMultiplier = value; } }

    public void Awake()
    {
        pointToGo = 0;
        navigationTime = 0;

        self = GetComponent<Transform>();
        hpLabel = transform.GetChild(0).GetComponent<Text>();
        hpLabel.text = hp.ToString();
        isDestroyed = false;
    }

    public void Start()
    {
        GameManager.instance.RegisterEnemy(this);
    }

    public void Update()
    {
        if (GameManager.instance.WayPoints != null && HP > 0)
        {
            navigationTime += Time.deltaTime * speedMultiplier;
            if (navigationTime > speedFPS)
            {
                if (pointToGo < GameManager.instance.WayPoints.Length) self.position = Vector3.MoveTowards(self.position, GameManager.instance.WayPoints[pointToGo].transform.position, navigationTime);
                else
                {
                    self.position = Vector3.MoveTowards(self.position, GameManager.instance.Exit.transform.position, navigationTime);
                }

                navigationTime = 0;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "WayPoint")
        {
            pointToGo++;
        }

        if (collision.tag == "Finish")
        {
            RDG.Vibration.Vibrate(100);
            GameManager.instance.TotalEscaped += 1;
            GameManager.instance.SetCurrentGameState();
            GameManager.instance.CheckCurrentGameState();
            GameUIManager.instance.RemoveHealth();
            GameManager.instance.UnRegisterEnemy(this);
        }

        if(collision.tag == "Ammo")
        {
            Ammo curAmmo = collision.GetComponent<Ammo>();
            if (curAmmo.Target == this)
            {
                GetComponent<AudioSource>().clip = curAmmo.Pew.clip;
                GetComponent<AudioSource>().Play();

                SelfHit(Mathf.RoundToInt(curAmmo.AttackingDamage * curAmmo.AttackingDamageMultiplier), curAmmo.AwardMultiplier);
                if(this != null)
                {
                    SelfHit(Mathf.RoundToInt(curAmmo.AttackingDamage * curAmmo.AttackingDamageMultiplier * 
                        (PlayerPrefs.GetInt("CriticalDamage", 100) * 1f / 100)), curAmmo.AwardMultiplier);

                    GameObject CD = Instantiate(text, transform);
                    CD.GetComponent<Canvas>().overrideSorting = true;
                    CD.GetComponent<Canvas>().sortingOrder = 5;
                    CD.GetComponent<Text>().color = Color.red;
                    CD.GetComponent<RectTransform>().anchoredPosition = Vector2.right * Random.Range(-40, 41);
                    CD.GetComponent<Text>().text = Mathf.RoundToInt(curAmmo.AttackingDamage * curAmmo.AttackingDamageMultiplier *
                        (PlayerPrefs.GetInt("CriticalDamage", 100) * 1f / 100)).ToString();
                    StartCoroutine(DestroyDmg(CD));
                }
                if (this != null) UltimateManager.instance.Ultimate(curAmmo, this);
                Destroy(curAmmo.gameObject);
            }
        }
    }

    public void SelfHit(int damage, float awardMultiplier)
    {
        if (this != null)
        {
            StartCoroutine(ReColor());

            GameObject curDmg = Instantiate(text, transform);
            curDmg.GetComponent<Canvas>().overrideSorting = true;
            curDmg.GetComponent<Canvas>().sortingOrder = 5;
            curDmg.GetComponent<RectTransform>().anchoredPosition = Vector2.right * Random.Range(-40, 41);
            curDmg.GetComponent<Text>().text = Mathf.RoundToInt(damage).ToString();
            StartCoroutine(DestroyDmg(curDmg));

            hp -= damage;
            hpLabel.text = hp.ToString();

            if (hp <= 0 && !isDestroyed)
            {
                isDestroyed = true;
                GetComponent<Collider2D>().enabled = false;
                GameManager.instance.TotalKilled += 1;
                GameManager.instance.AddMoney(Mathf.RoundToInt(rewardAmount * awardMultiplier));
                GameManager.instance.SetCurrentGameState();
                GameManager.instance.CheckCurrentGameState();
                GameManager.instance.UnRegisterEnemy(this);
            }
        }
    }

    private IEnumerator ReColor()
    {
        GetComponent<Image>().color = new Vector4(1, 0, 0, 1);
        yield return new WaitForSeconds(.1f);
        GetComponent<Image>().color = new Vector4(1, 1, 1, 1);
    }

    private IEnumerator DestroyDmg(GameObject curDmg)
    {
        yield return new WaitForSeconds(.3f);
        Destroy(curDmg);
    }
}
