using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using System;
using System.IO;

public class MultiManager : MonoBehaviour
{
    public static MultiManager Inctance { get; set; }
    private ColyseusMan colyseus;
    private User user;

    public event Action<Selections> EnemySelected;
    public event Action<User> EnemyInfoReceived;

    // Start is called before the first frame update
    void Awake()
    {
        if (Inctance == null)
        {
            Inctance = this;
            DontDestroyOnLoad(this);
        }
        else if (Inctance == this)
        {
            Destroy(gameObject);
        }

        if (string.IsNullOrEmpty(PlayerPrefs.GetString("name"))) return;

        SetUserVariable();

        FindObjectOfType<MainMenu>().UserInfoCollected += MultiManager_UserInfoCollected;
    }

    private void MultiManager_UserInfoCollected(User obj)
    {
        SetUserVariable();
    }

    public void Connect()
    {
        colyseus = new ColyseusMan();
        colyseus.PlayersConnected += StartGame;
        colyseus.Matchmake(user);
    }

    private void StartGame(bool status)
    {
        if (status)
        {
            colyseus.RoomError += ErrorThrown;
            colyseus.EnemySelected += Colyseus_EnemySelected;
            colyseus.EnemyInfoReceived += Colyseus_EnemyInfoReceived;
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
            SceneManager.LoadScene(1);
        }
    }

    private void Colyseus_EnemyInfoReceived(User obj)
    {
        EnemyInfoReceived?.Invoke(obj);
    }

    private void NotifyWinner()
    {
        colyseus.NotifyServerWinner(user.DeviceID, user.Name);
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.PlayerSelected += GameManager_PlayerSelected;
        gameManager.GameWon += NotifyWinner;

        SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }

    private void Colyseus_EnemySelected(int obj)
    {
        Debug.Log("broadcasting choice of the enemy!");
        EnemySelected?.Invoke((Selections)obj);
    }

    private void GameManager_PlayerSelected(Selections obj)
    {
        colyseus.UpdateChoice(obj);
    }

    private void ErrorThrown(string error)
    {
        Debug.LogError(error);
        SceneManager.LoadScene(0);
    }

    public void LeaveGame()
    {
        colyseus.Leave();
    }

    private void SetUserVariable()
    {
        user = JSONManager.DeserealizeData<User>(Path.Combine(Application.persistentDataPath, "Userinfo.json"));
    }

    private void OnApplicationQuit()
    {
        LeaveGame();
    }
}
