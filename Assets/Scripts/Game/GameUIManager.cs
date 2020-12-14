using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance;

    [SerializeField] private int maxSlots, slotCost, countOfPresets;
    [SerializeField] private GameObject slot, slotsPlace, buttonPlace, sellPanel;
    [SerializeField] private Button yesSelling;
    [SerializeField] private LeanButton addSlot;
    [SerializeField] private Text heartLabel, gameOverScoreLabel, awardScoreLabel;
    [SerializeField] private GameObject gameOverPanel, awardPanel;

    [Header("Settings")]
    [SerializeField] private GameObject settings;
    [SerializeField] private AudioSource music;
    [SerializeField] private LeanToggle musicTog, soundTog, animTog;

    private List<GameObject> slots;
    private GameObject freeSlot;
    private int heart;
    private bool animate;

    public GameObject GameOverPanel { get { return gameOverPanel; } }
    public GameObject AwardPanel { get { return awardPanel; } }
    public Text GameOverScoreLabel { get { return gameOverScoreLabel; } }
    public Text AwardScoreLabel { get { return awardScoreLabel; } }

    public void Awake()
    {
        if (instance != null) Destroy(gameObject);
        instance = this;

        slots = new List<GameObject>();
        heart = 3;
    }

    public void Start()
    {
        music.ignoreListenerVolume = true;

        if (!PlayerPrefs.HasKey("Animate"))
        {
            PlayerPrefs.SetInt("Animate", 1);
            PlayerPrefs.SetInt("Sound", 1);
            PlayerPrefs.SetInt("Music", 1);
            PlayerPrefs.Save();
        }

        if (PlayerPrefs.GetInt("Animate") == 1)
        {
            animTog.On = true;
            animate = true;

            Animator[] animatorsInTheScene = FindObjectsOfType<Animator>();
            foreach (Animator animatorItem in animatorsInTheScene)
            {
                animatorItem.enabled = animate;
            }
        }
        else
        {
            animTog.On = false;
            animate = false;

            Animator[] animatorsInTheScene = FindObjectsOfType<Animator>();
            foreach (Animator animatorItem in animatorsInTheScene)
            {
                animatorItem.enabled = animate;
            }
        }

        if (PlayerPrefs.GetInt("Sound") == 1)
        {
            soundTog.On = true;
            AudioListener.volume = 1;
        }
        else
        {
            soundTog.On = false;
            AudioListener.volume = 0;
        }

        if (PlayerPrefs.GetInt("Music") == 1)
        {
            musicTog.On = true;
            music.enabled = true;
        }
        else
        {
            musicTog.On = false;
            music.enabled = false;
        }

        for (int i = 0; i < slotsPlace.transform.childCount; i++) slots.Add(slotsPlace.transform.GetChild(i).gameObject);

        for (int i = 0; i < countOfPresets; i++)
        {
            buttonPlace.transform.GetChild(i).GetComponent<UpdateTowerButton>().Image.gameObject.GetComponent<Animator>().runtimeAnimatorController =
                DiceCollector.instance.Dices[PlayerPrefs.GetInt("Dice" + i, 0)].GetComponent<Animator>().runtimeAnimatorController;
            buttonPlace.transform.GetChild(i).GetComponent<UpdateTowerButton>().Image.gameObject.GetComponent<Image>().sprite =
                DiceCollector.instance.Dices[PlayerPrefs.GetInt("Dice" + i, 0)].GetComponent<Image>().sprite;
            buttonPlace.transform.GetChild(i).GetComponent<UpdateTowerButton>().NameLabel.text =
                 DiceCollector.instance.Dices[PlayerPrefs.GetInt("Dice" + i, 0)].name.Trim('(');
        }
    }

    public void Update()
    {
        for (int i = 0; i < countOfPresets; i++)
        {
            UpdateTowerButton updateTowerButton = buttonPlace.transform.GetChild(i).GetComponent<UpdateTowerButton>();

            if (updateTowerButton.LvlCost > GameManager.instance.TotalMoney)
            {
                updateTowerButton.transform.GetChild(0).GetComponent<Image>().color = Color.HSVToRGB(0, 0, .3f);

                updateTowerButton.transform.GetChild(4).GetComponent<Image>().color = Color.HSVToRGB(0, 1, .3f);
                updateTowerButton.transform.GetChild(4).GetChild(0).GetComponent<Text>().color = Color.HSVToRGB(0, 0, .3f);

                updateTowerButton.transform.GetChild(1).GetComponent<Text>().color = Color.HSVToRGB(0, 0, .3f);
                updateTowerButton.transform.GetChild(0).GetComponent<Animator>().cullingMode = AnimatorCullingMode.CullCompletely;
                updateTowerButton.Upgrade.GetComponent<LeanButton>().interactable = false;

                updateTowerButton.Upgrade.transform.GetChild(0).GetComponent<Text>().color = Color.HSVToRGB(0, 0, .51f);
                updateTowerButton.Upgrade.transform.GetChild(1).GetComponent<Text>().color = Color.HSVToRGB(0, 0, .51f);
                updateTowerButton.Upgrade.transform.GetChild(2).GetComponent<Image>().color = new Vector4(1, 1, 1, .5f);
            }
            else
            {
                updateTowerButton.transform.GetChild(0).GetComponent<Image>().color = Vector4.one;

                updateTowerButton.transform.GetChild(4).GetComponent<Image>().color = Color.red;
                updateTowerButton.transform.GetChild(4).GetChild(0).GetComponent<Text>().color = Vector4.one;

                updateTowerButton.transform.GetChild(1).GetComponent<Text>().color = Vector4.one;
                updateTowerButton.transform.GetChild(0).GetComponent<Animator>().cullingMode = AnimatorCullingMode.AlwaysAnimate;
                updateTowerButton.Upgrade.GetComponent<LeanButton>().interactable = true;

                updateTowerButton.Upgrade.transform.GetChild(0).GetComponent<Text>().color = Vector4.one;
                updateTowerButton.Upgrade.transform.GetChild(1).GetComponent<Text>().color = Vector4.one;
                updateTowerButton.Upgrade.transform.GetChild(2).GetComponent<Image>().color = Vector4.one;
            }
        }

        AddTowerButton addTowerButton = buttonPlace.transform.GetChild(countOfPresets).GetComponent<AddTowerButton>();
        if (addTowerButton.Cost > GameManager.instance.TotalMoney || !IsFreeSlot())
        {
            addTowerButton.GetComponent<Image>().color = Color.HSVToRGB(0, 0, .25f);
            addTowerButton.transform.GetChild(0).GetComponent<Text>().color = Color.HSVToRGB(0, 0, .25f);
            addTowerButton.transform.GetChild(1).GetComponent<Text>().color = Color.HSVToRGB(0, 0, .51f);
            addTowerButton.transform.GetChild(2).GetComponent<Image>().color = new Vector4(1,1,1,.5f);
            addTowerButton.GetComponent<LeanButton>().interactable = false;
        }
        else
        {
            addTowerButton.GetComponent<Image>().color = Vector4.one;
            addTowerButton.transform.GetChild(0).GetComponent<Text>().color = Vector4.one;
            addTowerButton.transform.GetChild(1).GetComponent<Text>().color = Vector4.one;
            addTowerButton.transform.GetChild(2).GetComponent<Image>().color = Vector4.one;
            addTowerButton.GetComponent<LeanButton>().interactable = true;
        }

        /*
        if (slotCost > GameManager.instance.TotalMoney)
        {
            addSlot.gameObject.SetActive(false);
        }
        else
        {
            addSlot.gameObject.SetActive(true);
        }
        */
    }

    public void AddSlot()
    {
        if (slotsPlace.transform.childCount < maxSlots && GameManager.instance.TotalMoney >= slotCost)
        {
            GameObject curSlot = Instantiate(slot, slotsPlace.transform);
            slots.Add(curSlot);
            GameManager.instance.SubtractMoney(slotCost);
            slotCost += 100;
            addSlot.transform.GetChild(0).GetComponent<Text>().text = slotCost.ToString();
        }

        if (slotsPlace.transform.childCount >= maxSlots) addSlot.gameObject.SetActive(false);
    }

    public void AddTower(GameObject button)
    {
        AddTowerButton addTowerButton = button.GetComponent<AddTowerButton>();

        if (GameManager.instance.TotalMoney >= addTowerButton.Cost)
        {
            if ((freeSlot = IsFreeSlot()) != null)
            {
                int idx = Random.Range(0, countOfPresets);
                GameObject curTower = Instantiate(DiceCollector.instance.Dices[PlayerPrefs.GetInt("Dice" + idx, 0)],
                    freeSlot.transform);
                curTower.transform.localPosition = Vector3.zero;
                curTower.GetComponent<Button>().onClick.AddListener(() => SellDice(curTower, addTowerButton));
                curTower.GetComponent<Dice>().Lvl = buttonPlace.transform.GetChild(idx).GetComponent<UpdateTowerButton>().Lvl;
                addTowerButton.TmpCost = addTowerButton.Cost;
                GameManager.instance.SubtractMoney(addTowerButton.Cost);
                addTowerButton.Cost += addTowerButton.DiceCostIncrease;
                addTowerButton.CostLabel.text = addTowerButton.Cost.ToString();
                return;
            }
        }
    }

    public void UpgradeTower(GameObject button)
    {
        UpdateTowerButton upgradeTowerButton = button.GetComponent<UpdateTowerButton>();

        if (GameManager.instance.TotalMoney >= upgradeTowerButton.LvlCost)
        {
            upgradeTowerButton.Lvl++;
            upgradeTowerButton.LvlLabel.text = "Lvl." + (upgradeTowerButton.Lvl + 1);
            upgradeTowerButton.LvlCircleLabel.text = upgradeTowerButton.Lvl.ToString();

            GameManager.instance.SubtractMoney(upgradeTowerButton.LvlCost);
            upgradeTowerButton.LvlCost += upgradeTowerButton.LvlCostIncrease;
            upgradeTowerButton.LvlCostLabel.text = upgradeTowerButton.LvlCost.ToString();

            for(int i = 0; i < slotsPlace.transform.childCount; i++)
            {
                if (slotsPlace.transform.GetChild(i).childCount > 0 
                    && slotsPlace.transform.GetChild(i).GetChild(0).GetComponent<Dice>().ID == PlayerPrefs.GetInt("Dice" + upgradeTowerButton.transform.GetSiblingIndex()))
                {
                    Dice curDice = slotsPlace.transform.GetChild(i).GetChild(0).GetComponent<Dice>();
                    curDice.Lvl = upgradeTowerButton.Lvl;
                    curDice.LvlLabel.text = upgradeTowerButton.Lvl.ToString();
                }
            }
        }
    }

    public void RemoveHealth()
    {
        heart--;
        heartLabel.text = heart.ToString();
    }

    public void LoadMenu()
    {
        PlayerPrefs.SetInt("LoadedFromDefeat", -1);
        PlayerPrefs.Save();
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadMenuFromDefeat()
    {
        PlayerPrefs.SetInt("LoadedFromDefeat", SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.Save();
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadCurrentGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SellDice(GameObject curTower, AddTowerButton addTower)
    {
        sellPanel.SetActive(true);
        sellPanel.transform.GetChild(1).GetComponent<Text>().text = (int)(addTower.TmpCost * 0.6f) + " coins";
        yesSelling.onClick.AddListener(() => YesSelling(curTower, addTower));
    }

    private void YesSelling(GameObject curTower, AddTowerButton addTower)
    {
        sellPanel.SetActive(false);
        Destroy(curTower);
        GameManager.instance.AddMoney((int)(addTower.Cost * 0.6f));
    }

    public void NoSelling()
    {
        sellPanel.SetActive(false);
    }

    private GameObject IsFreeSlot()
    {
        foreach (GameObject slot in slots)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return null;
    }

    public void OnOffMusic()
    {
        music.enabled = !music.enabled;
        if (music.enabled) PlayerPrefs.SetInt("Music", 1);
        else PlayerPrefs.SetInt("Music", 0);
        PlayerPrefs.Save();
    }

    public void OnOffSound()
    {
        AudioListener.volume = Mathf.Abs(AudioListener.volume - 1);
        PlayerPrefs.SetInt("Sound", (int)AudioListener.volume);
        PlayerPrefs.Save();
    }

    public void OnOffAnimation()
    {
        animate = !animate;

        Animator[] animatorsInTheScene = FindObjectsOfType<Animator>();
        foreach (Animator animatorItem in animatorsInTheScene)
        {
            animatorItem.enabled = animate;
        }

        if (animate) PlayerPrefs.SetInt("Animate", 1);
        else PlayerPrefs.SetInt("Animate", 0);
        PlayerPrefs.Save();
    }

    public void ActivateSettings()
    {
        GameManager.instance.Paused = !GameManager.instance.Paused;
        Time.timeScale = Mathf.Abs(Time.timeScale - 1);
        settings.SetActive(!settings.activeSelf);
    }

    /*
    private bool IsEmptySlot()
    {
        for (int i = 0; i < slotsPlace.transform.childCount; i++)
        {
            if (slotsPlace.transform.GetChild(i).childCount == 0)
            {
                return true;
            }
        }
        return false;
    }
    */
}
