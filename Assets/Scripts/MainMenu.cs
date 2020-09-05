using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using System.Linq;

public class MainMenu : MonoBehaviour
{
    public event Action<User> UserInfoCollected;

    [SerializeField]
    private GameObject MainMenuPanel;
    [SerializeField]
    private GameObject LobbyPanel;
    [SerializeField]
    private GameObject FormPanel;
    [SerializeField]
    private List<Text> FormTexts;
    [SerializeField]
    private GameObject UserTablePrefab;
    [SerializeField]
    private GameObject ContentView;

    [SerializeField]
    private GameObject LeaderboardUI;

    // Start is called before the first frame update
    void Start()
    {
        //If user info is not already saved
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("name")))
        {
            //prompt the user to fill in the form
            FormPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Submit and save all the info for later use
    /// </summary>
    public void SubmitInfo()
    {
        User myUser = new User()
        {
            Name = FormTexts[0].text,
            Age = int.Parse(FormTexts[1].text),
            Gender = FormTexts[2].text,
            Star = FormTexts[3].text,
            DeviceID = SystemInfo.deviceUniqueIdentifier
        };

        print(myUser.DeviceID + myUser.Name);
        print(myUser.Name);
        print(myUser.Gender);

        string path = Path.Combine(Application.persistentDataPath, "UserInfo.json");

        JSONManager.SerializeData(myUser, path);

        FormPanel.SetActive(false);

        PlayerPrefs.SetString("name", myUser.Name);
        PlayerPrefs.Save();

        UserInfoCollected?.Invoke(myUser);
    }

    /// <summary>
    /// Starting Single Player Button
    /// </summary>
    public void SinglePlayer()
    {
        //Save gamemode for next Scene
        PlayerPrefs.SetInt("Multi", (int)Gamemodes.Single);
        PlayerPrefs.Save();

        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Back To Main Menu button
    /// </summary>
    public void BackButton()
    {
        //Cancel Search for Matchmaking
        MultiManager.Inctance.LeaveGame();

        //Update UI to show Main Menu
        LobbyPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void ShowLeaderboard()
    {
        LeaderboardUI.SetActive(true);
    }



    public void HideLeaderBoard()
    {
        LeaderboardUI.SetActive(false);
    }


    public void EditInfo()
    {
        FormPanel.SetActive(true);
    }

    /// <summary>
    /// Start Multiplayer Matchmaking Button
    /// </summary>
    public void MultiplayerButton()
    {
        //Save gamemode type
        PlayerPrefs.SetInt("Multi", (int)Gamemodes.Multi);
        PlayerPrefs.Save();

        LobbyPanel.SetActive(true);
        MainMenuPanel.SetActive(false);

        //Connect to server and start matchmaking
        MultiManager.Inctance.Connect();
    }

    public void GetLeaderBoard()
    {
        LeaderboardRequest();
    }

    private async void LeaderboardRequest()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get("https://multiplayer-rock-paper-s-5dbe4.firebaseio.com/Score.json");

        await webRequest.SendWebRequest();

        if (!webRequest.isNetworkError)
        {
            Debug.Log(webRequest.downloadHandler.text);

           // var objects = JsonUtility.FromJson<dynamic>(webRequest.downloadHandler.text);
            List<PlayerScores> leaderboard = new List<PlayerScores>();

            //foreach (var obj in objects)
            //{
            //    string nameOfPlayer = obj.name.ToString();  
            //    int score = obj.score;
            //
            //    leaderboard.Add(new PlayerScores(nameOfPlayer, score));
            //}

            leaderboard = leaderboard.OrderByDescending(x => x.score).ToList();

            foreach (PlayerScores scores in leaderboard)
            {
                GameObject gameObject = GameObject.Instantiate(UserTablePrefab, ContentView.transform);
                Text[] texts = gameObject.GetComponentsInChildren<Text>();

                texts[0].text = scores.name;
                texts[1].text = scores.score.ToString();
            }
        }
        else
        {
            //Internet problem
        }
    }
}

public class PlayerScores
{
    public string name;
    public int score;

    public PlayerScores(string _name, int _score)
    {
        name = _name;
        score = _score;
    }
    public void Exit()
    {
        Application.Quit();
    }
}