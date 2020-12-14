using Lean.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    public static MenuUIManager instance;

    [SerializeField] private int countOfPreset;
    [SerializeField] private GameObject levels, presetSlot, improveMenu, openedScroll, availableScroll;
    [SerializeField] private GameObject improve;
    [SerializeField] private GameObject startGame, startGamePreset;
    [SerializeField] private Text starCountLabel, moneyCountLabel, startGameLabel, criticalDamage, criticalDamageImproveMenu;
    [SerializeField] private Sprite levelGrey, levelNoStar, levelOneStar, levelTwoStar, levelThreeStar;

    [Header("Settings")]
    [SerializeField] private GameObject settings;
    [SerializeField] private AudioSource music;
    [SerializeField] private LeanToggle musicTog, soundTog, animTog;

    [Header("Store")]
    [SerializeField] private GameObject store;
    [SerializeField] private Text starCount;
    [SerializeField] private Text moneyCount;

    private int dice1, dice2, dice3, dice4, dice5;
    private bool animate;

    public GameObject PresetSlot { get { return presetSlot; } }
    public GameObject OpenedScroll { get { return openedScroll; } }

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
        }
        else
        {
            animTog.On = false;
            animate = false;
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

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        if (instance != null) Destroy(gameObject);
        instance = this;

        starCountLabel.text = PlayerPrefs.GetInt("StarCount", 0).ToString();
        moneyCountLabel.text = PlayerPrefs.GetInt("MoneyCount", 0).ToString();

        if (!PlayerPrefs.HasKey("Game0StarCount"))
        {
            PlayerPrefs.SetInt("Game0StarCount", 0);
            PlayerPrefs.Save();
        }
        if (!PlayerPrefs.HasKey("Dice0"))
        {
            PlayerPrefs.SetInt("Dice0", 0);
            PlayerPrefs.SetInt("Dice1", 1);
            PlayerPrefs.SetInt("Dice2", 2);
            PlayerPrefs.SetInt("Dice3", 3);
            PlayerPrefs.SetInt("Dice4", 4);

            PlayerPrefs.SetInt("Dice0Enable", 1);
            PlayerPrefs.SetInt("Dice1Enable", 1);
            PlayerPrefs.SetInt("Dice2Enable", 1);
            PlayerPrefs.SetInt("Dice3Enable", 1);
            PlayerPrefs.SetInt("Dice4Enable", 1);

            PlayerPrefs.SetInt("Dice5Enable", 0);
            PlayerPrefs.SetInt("Dice6Enable", 0);
            PlayerPrefs.SetInt("Dice7Enable", 1);
            PlayerPrefs.Save();
        }

        if (!PlayerPrefs.HasKey("CriticalDamage"))
        {
            PlayerPrefs.SetInt("CriticalDamage", 100);
            PlayerPrefs.SetInt("CriticalCount", 0);
            PlayerPrefs.SetInt("CriticalMaxCount", 1);
            PlayerPrefs.Save();
        }

        dice1 = PlayerPrefs.GetInt("Dice0");
        dice2 = PlayerPrefs.GetInt("Dice1");
        dice3 = PlayerPrefs.GetInt("Dice2");
        dice4 = PlayerPrefs.GetInt("Dice3");
        dice5 = PlayerPrefs.GetInt("Dice4");

        for (int i = 1; i < levels.transform.childCount; i++)
        {
            if (PlayerPrefs.GetInt("Game" + (i - 1) + "StarCount", -1) > 0)
            {
                levels.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -10, 0);
                levels.transform.GetChild(i).GetChild(0).GetComponent<Text>().color = new Vector4(0.8509805f, 0.3568628f, 0, 1);
                levels.transform.GetChild(i).GetChild(0).GetComponent<Outline>().effectColor = new Vector4(1, 1, 1, 1);
                levels.transform.GetChild(i).GetComponent<Button>().interactable = true;

                if (PlayerPrefs.GetInt("Game" + i + "StarCount", 0) == 0) levels.transform.GetChild(i).GetComponent<Image>().sprite = levelNoStar;
                if (PlayerPrefs.GetInt("Game" + i + "StarCount", 0) == 1) levels.transform.GetChild(i).GetComponent<Image>().sprite = levelOneStar;
                if (PlayerPrefs.GetInt("Game" + i + "StarCount", 0) == 2) levels.transform.GetChild(i).GetComponent<Image>().sprite = levelTwoStar;
                if (PlayerPrefs.GetInt("Game" + i + "StarCount", 0) == 3) levels.transform.GetChild(i).GetComponent<Image>().sprite = levelThreeStar;

                if (PlayerPrefs.GetInt("Game" + (i - 1) + "StarCount", 0) == 0) levels.transform.GetChild(i - 1).GetComponent<Image>().sprite = levelNoStar;
                if (PlayerPrefs.GetInt("Game" + (i - 1) + "StarCount", 0) == 1) levels.transform.GetChild(i - 1).GetComponent<Image>().sprite = levelOneStar;
                if (PlayerPrefs.GetInt("Game" + (i - 1) + "StarCount", 0) == 2) levels.transform.GetChild(i - 1).GetComponent<Image>().sprite = levelTwoStar;
                if (PlayerPrefs.GetInt("Game" + (i - 1) + "StarCount", 0) == 3) levels.transform.GetChild(i - 1).GetComponent<Image>().sprite = levelThreeStar;
            }
            else
            {
                levels.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 10, 0);
                levels.transform.GetChild(i).GetChild(0).GetComponent<Text>().color = new Vector4(0.7294118f, 0.7294118f, 0.7294118f, 1);
                levels.transform.GetChild(i).GetChild(0).GetComponent<Outline>().effectColor = new Vector4(0, 0, 0, 1);
                levels.transform.GetChild(i).GetComponent<Image>().sprite = levelGrey;
                levels.transform.GetChild(i).GetComponent<Button>().interactable = false;
            }
        }

        if (PlayerPrefs.GetInt("LoadedFromDefeat", -1) > -1)
        {
            ActivateStartGameButton(PlayerPrefs.GetInt("LoadedFromDefeat", -1));
            ActivateImproveMenu();
        }
    }

    public void ActivateStartGameButton(int idx)
    {
        startGame.SetActive(!startGame.activeSelf);

        if (startGame.activeSelf)
        {
            criticalDamage.text = PlayerPrefs.GetInt("CriticalDamage", 100) + "%";
            startGameLabel.text = (idx + 1).ToString();
            for (int i = 0; i < countOfPreset; i++)
            {
                startGamePreset.transform.GetChild(0).GetChild(i).GetComponent<Image>().sprite =
                    DiceCollector.instance.Dices[PlayerPrefs.GetInt("Dice" + i)].GetComponent<Image>().sprite;

                startGamePreset.transform.GetChild(0).GetChild(i).GetComponent<Animator>().runtimeAnimatorController =
                    DiceCollector.instance.Dices[PlayerPrefs.GetInt("Dice" + i)].GetComponent<Animator>().runtimeAnimatorController;

                startGamePreset.transform.GetChild(1).GetChild(i).GetComponent<Text>().text =
                    DiceCollector.instance.Dices[PlayerPrefs.GetInt("Dice" + i)].name.Split('(')[0];
            }
            startGame.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => StartGame(idx));
            startGame.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() =>
            {
                ActivateImproveMenu();
                startGame.SetActive(false);
            });
        }
    }

    private void StartGame(int gameID)
    {
        PlayerPrefs.SetInt("LoadedFromDefeat", -1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Game" + gameID);
    }

    public void ActivateImproveMenu()
    {
        try
        {
            for (int i = 0; i < presetSlot.transform.childCount; i++)
                for (int j = 0; j < presetSlot.transform.GetChild(i).childCount; j++)
                    Destroy(presetSlot.transform.GetChild(i).GetChild(j).gameObject);

            for (int i = 0; i < openedScroll.transform.childCount; i++) Destroy(openedScroll.transform.GetChild(i).gameObject);
            for (int i = 0; i < availableScroll.transform.childCount; i++) Destroy(availableScroll.transform.GetChild(i).gameObject);


            criticalDamageImproveMenu.text = PlayerPrefs.GetInt("CriticalDamage", 100).ToString() + "%";
            improveMenu.SetActive(true);

            improveMenu.transform.GetChild(7).GetComponent<Text>().text = PlayerPrefs.GetInt("StarCount", 0).ToString();
            improveMenu.transform.GetChild(8).GetComponent<Text>().text = PlayerPrefs.GetInt("MoneyCount", 0).ToString();

            {
                GameObject curDice = Instantiate(DiceCollector.instance.Dices[dice1], presetSlot.transform.GetChild(0));
                curDice.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                curDice.GetComponent<Dice>().LvlLabel.transform.parent.gameObject.SetActive(false);

                curDice.GetComponent<Dice>().InMenu = true;
                curDice.GetComponent<Dice>().SubMenu = true;
                curDice.tag = "Menu";
            }
            {
                GameObject curDice = Instantiate(DiceCollector.instance.Dices[dice2], presetSlot.transform.GetChild(1));
                curDice.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                curDice.GetComponent<Dice>().LvlLabel.transform.parent.gameObject.SetActive(false);

                curDice.GetComponent<Dice>().InMenu = true;
                curDice.GetComponent<Dice>().SubMenu = true;
                curDice.tag = "Menu";
            }
            {
                GameObject curDice = Instantiate(DiceCollector.instance.Dices[dice3], presetSlot.transform.GetChild(2));
                curDice.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                curDice.GetComponent<Dice>().LvlLabel.transform.parent.gameObject.SetActive(false);

                curDice.GetComponent<Dice>().InMenu = true;
                curDice.GetComponent<Dice>().SubMenu = true;
                curDice.tag = "Menu";
            }
            {
                GameObject curDice = Instantiate(DiceCollector.instance.Dices[dice4], presetSlot.transform.GetChild(3));
                curDice.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                curDice.GetComponent<Dice>().LvlLabel.transform.parent.gameObject.SetActive(false);

                curDice.GetComponent<Dice>().InMenu = true;
                curDice.GetComponent<Dice>().SubMenu = true;
                curDice.tag = "Menu";
            }
            {
                GameObject curDice = Instantiate(DiceCollector.instance.Dices[dice5], presetSlot.transform.GetChild(4));
                curDice.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                curDice.GetComponent<Dice>().LvlLabel.transform.parent.gameObject.SetActive(false);

                curDice.GetComponent<Dice>().InMenu = true;
                curDice.GetComponent<Dice>().SubMenu = true;
                curDice.tag = "Menu";
            }

            for (int i = 0; i < DiceCollector.instance.Dices.Length; i++)
            {
                if (PlayerPrefs.GetInt("Dice" + i + "Enable") != 0)
                {
                    GameObject curDice = Instantiate(DiceCollector.instance.Dices[i], openedScroll.transform);
                    curDice.GetComponent<Dice>().LvlLabel.transform.parent.gameObject.SetActive(false);
                    curDice.GetComponent<Button>().interactable = true;
                    curDice.GetComponent<Image>().color = Color.white;
                    curDice.GetComponent<Dice>().enabled = true;
                    curDice.GetComponent<Button>().onClick.AddListener(() => ActivateImprove(curDice));
                    curDice.tag = "Untagged";

                    curDice.GetComponent<Dice>().InMenu = true;
                    curDice.GetComponent<Dice>().SubMenu = false;

                    curDice.GetComponent<Animator>().cullingMode = AnimatorCullingMode.CullCompletely;
                }
                else
                {
                    GameObject curDice = Instantiate(DiceCollector.instance.Dices[i], availableScroll.transform);
                    curDice.GetComponent<Dice>().LvlLabel.transform.parent.gameObject.SetActive(false);
                    curDice.GetComponent<Button>().interactable = false;
                    curDice.GetComponent<Image>().color = new Vector4(1, 1, 1, 0.5f);

                    curDice.GetComponent<Dice>().InMenu = true;
                    curDice.GetComponent<Dice>().SubMenu = false;
                    curDice.GetComponent<Dice>().Start();

                    curDice.GetComponent<Dice>().enabled = false;
                    curDice.tag = "Untagged";

                    curDice.GetComponent<Animator>().runtimeAnimatorController = null;
                }
            }

            StartCoroutine(ScrollResize());
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private IEnumerator ScrollResize()
    {
        openedScroll.GetComponent<ContentSizeFitter>().enabled = true;
        yield return new WaitForSeconds(.1f);
        openedScroll.GetComponent<ContentSizeFitter>().enabled = false;

        yield return new WaitForSeconds(.1f);

        availableScroll.GetComponent<ContentSizeFitter>().enabled = true;
        yield return new WaitForSeconds(.1f);
        availableScroll.GetComponent<ContentSizeFitter>().enabled = false;
    }

    public void DeactivateImproveMenu()
    {
        for (int i = 0; i < countOfPreset; i++) PlayerPrefs.SetInt("Dice" + i, presetSlot.transform.GetChild(i).GetChild(0).GetComponent<Dice>().ID);
        dice1 = PlayerPrefs.GetInt("Dice0");
        dice2 = PlayerPrefs.GetInt("Dice1");
        dice3 = PlayerPrefs.GetInt("Dice2");
        dice4 = PlayerPrefs.GetInt("Dice3");
        dice5 = PlayerPrefs.GetInt("Dice4");

        improveMenu.SetActive(false);
    }

    public void ActivateImprove(GameObject curDice)
    {
        Dice dice = curDice.GetComponent<Dice>();
        improve.SetActive(true);
        improve.GetComponent<Improve>().ID = dice.ID;
        improve.GetComponent<Improve>().AttackInc = dice.AttackInc;
        improve.GetComponent<Improve>().SpeedInc = dice.SpeedInc;
        improve.GetComponent<Improve>().RichmentInc = dice.RichmentInc;
        improve.GetComponent<Improve>().MagicInc = dice.MagicInc;
        improve.GetComponent<Improve>().Init();
    }

    public void DeactiveImprove()
    {
        improve.SetActive(false);
    }

    public void ActivateStore()
    {
        store.SetActive(!store.activeSelf);
        if (store.activeSelf)
        {
            starCount.text = PlayerPrefs.GetInt("StarCount", 0).ToString();
            moneyCount.text = PlayerPrefs.GetInt("MoneyCount", 0).ToString();
        }
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

        Cloud[] clouds = FindObjectsOfType<Cloud>();

        foreach (Cloud cloud in clouds)
        {
            cloud.enabled = !cloud.enabled;
        }

        animate = !animate;
        if (animate) PlayerPrefs.SetInt("Animate", 1);
        else PlayerPrefs.SetInt("Animate", 0);
        PlayerPrefs.Save();
    }

    public void ActivateSettings()
    {
        settings.SetActive(!settings.activeSelf);
    }

    private void Update()
    {
        if (!animate)
        {
            Animator[] animatorsInTheScene = FindObjectsOfType<Animator>();
            foreach (Animator animatorItem in animatorsInTheScene)
            {
                animatorItem.enabled = false;
            }
        }
        else
        {
            Animator[] animatorsInTheScene = FindObjectsOfType<Animator>();
            foreach (Animator animatorItem in animatorsInTheScene)
            {
                animatorItem.enabled = true;
            }
        }

    }
}
