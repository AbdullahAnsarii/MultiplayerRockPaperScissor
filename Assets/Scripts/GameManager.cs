using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public event Action<Selections> PlayerSelected;
    public event Action StartGame;
    public event Action GameWon;

    [SerializeField]
    private List<Button> SelectingButtons;
    [SerializeField]
    private GameObject EndGamePanel;
    [SerializeField]
    private GameObject DrawTextUI;
    [SerializeField]
    private GameObject WinTextUI;
    [SerializeField]
    private GameObject LoseTextUI;
    [SerializeField]
    private GameObject ProfilePanel;
    [SerializeField]
    private GameObject PlayerProfile;
    [SerializeField]
    private GameObject EnemyProfile;
    [SerializeField]
    private GameObject RetryButton;
    [SerializeField]
    private List<Text> ProfilePanelTexts;


    private Selections? PlayerSelection;
    public Selections? EnemySelection;

    private Gamemodes gameMode;

    private Enemy Enemy;

    private User EnemyInfo;

    // Start is called before the first frame update
    void Start()
    {
        Enemy = FindObjectOfType<Enemy>();
        Enemy.EnemySelected += EndGame;

        User myUser = JSONManager.DeserealizeData<User>(Path.Combine(Application.persistentDataPath, "UserInfo.json"));

        PlayerProfile.GetComponentInChildren<Text>().text = myUser.Name;

        PlayerProfile.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/zodiac/" + myUser.Star);

        gameMode = (Gamemodes)PlayerPrefs.GetInt("Multi");

        if (gameMode == Gamemodes.Multi)
        {
            MultiManager.Inctance.EnemySelected += Multimanager_EnemySelected;
            MultiManager.Inctance.EnemyInfoReceived += Multimanager_EnemyInfoReceived;
        }
    }

    private void Multimanager_EnemyInfoReceived(User obj)
    {
        EnemyInfo = obj;

        ProfilePanelTexts[0].text = EnemyInfo.Name;
        ProfilePanelTexts[1].text = EnemyInfo.Age.ToString();
        ProfilePanelTexts[2].text = EnemyInfo.Gender;
        ProfilePanelTexts[3].text = EnemyInfo.Star;

        Sprite enemyStar = Resources.Load<Sprite>("Sprites/zodiac/" + EnemyInfo.Star);

        ProfilePanel.GetComponentInChildren<Image>().sprite = enemyStar;
        EnemyProfile.GetComponent<Image>().sprite = enemyStar;
        EnemyProfile.GetComponentInChildren<Text>().text = EnemyInfo.Name;
    }

    private void Multimanager_EnemySelected(Selections obj)
    {
        EnemySelection = obj;

        if (PlayerSelection != null)
        {
            StartGame?.Invoke();
        }
    }

    public void Select(int selection)
    {
        PlayerSelection = (Selections)selection;

        EnableDisableButtons(false);
        //Notify All listeners
        PlayerSelected?.Invoke((Selections)PlayerSelection);

        if (gameMode == Gamemodes.Single)
        {
            StartGame?.Invoke();
        }

        if (EnemySelection != null)
        {
            StartGame?.Invoke();
        }
    }

    public void EnemyProfileClick()
    {
        ProfilePanel.SetActive(true);
    }

    public void EnemyProfileCLose()
    {
        ProfilePanel.SetActive(false);
    }

    public void MainMenuClick()
    {
        SceneManager.LoadScene(0);
    }

    public void RetryButtonClick()
    {
        SceneManager.LoadScene(1);
    }

    private void EndGame(Selections selection)
    {
        StartCoroutine(EndGame((int)selection));
    }

    private IEnumerator EndGame(int enemySelection)
    {
        Selections selection = (Selections)enemySelection;
        bool gameWon = false;

        yield return new WaitForSeconds(2);

        if (PlayerSelection == selection)
        {
            DrawTextUI.SetActive(true);
            WinTextUI.SetActive(false);
            LoseTextUI.SetActive(false);
        }
        else if (PlayerSelection == Selections.Rock && selection == Selections.Scissors)
        {
            gameWon = true;

            DrawTextUI.SetActive(false);
            WinTextUI.SetActive(true);
            LoseTextUI.SetActive(false);
        }
        else if (PlayerSelection == Selections.Paper && selection == Selections.Rock)
        {
            gameWon = true;

            DrawTextUI.SetActive(false);
            WinTextUI.SetActive(true);
            LoseTextUI.SetActive(false);
        }
        else if (PlayerSelection == Selections.Scissors && selection == Selections.Paper)
        {
            gameWon = true;

            DrawTextUI.SetActive(false);
            WinTextUI.SetActive(true);
            LoseTextUI.SetActive(false);
        }
        else if (selection == Selections.Rock && PlayerSelection == Selections.Scissors)
        {
            DrawTextUI.SetActive(false);
            WinTextUI.SetActive(false);
            LoseTextUI.SetActive(true);
        }
        else if (selection == Selections.Paper && PlayerSelection == Selections.Rock)
        {
            DrawTextUI.SetActive(false);
            WinTextUI.SetActive(false);
            LoseTextUI.SetActive(true);
        }
        else if (selection == Selections.Scissors && PlayerSelection == Selections.Paper)
        {
            DrawTextUI.SetActive(false);
            WinTextUI.SetActive(false);
            LoseTextUI.SetActive(true);
        }

        if (gameWon && gameMode == Gamemodes.Multi)
        {
            GameWon?.Invoke();
        }


        if (gameMode == Gamemodes.Multi)
        {
            RetryButton.SetActive(false);
        }
        else
        {
            RetryButton.SetActive(true);
        }

        if (DrawTextUI.activeInHierarchy)
        {
            EndGamePanel.SetActive(false);
            EnableDisableButtons(true);

            if(gameMode == Gamemodes.Single)
            {
                SceneManager.LoadScene(1);
            }
        }
        else
        {
            EndGamePanel.SetActive(true);
        }

    }

    private void EnableDisableButtons(bool enable)
    {
        foreach (Button button in SelectingButtons)
        {
            button.interactable = enable;
        }
    }
}
