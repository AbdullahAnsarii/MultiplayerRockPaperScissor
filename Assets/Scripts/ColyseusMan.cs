using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Colyseus;

class ColyseusMan
{
    public event Action<bool> PlayersConnected;
    public event Action<string> RoomError;
    public event Action<int> EnemySelected;
    public event Action<User> EnemyInfoReceived;

    private Client client;
    private Room<State> room;

    private int numPlayers = 0;
    private int playernumber = 1;

    public async void Matchmake(User user)
    {
        try
        {
            client = new Colyseus.Client("ws://10.57.30.20:2567");
            room = await client.JoinOrCreate<State>("game");
            await Task.Delay(1000);
            await room.Send(
                new
                {
                    userInfo =
                    new
                    {
                        user.DeviceID,
                        user.Gender,
                        user.Name,
                        user.Star,
                        user.Age
                    }
                });
            room.State.players.OnAdd += Players_OnAdd;
            room.State.players.OnChange += Players_OnChange;
            room.State.OnChange += State_OnChange;
            room.OnError += Room_OnError;

            numPlayers++;

            if (room.State.players.Count == 2)
            {
                playernumber = 2;
                USER value = ((PlayerSchema)(room.State.players.Items[0])).User;

                EnemyInfoReceived?.Invoke(new User()
                {
                    Age = (int)value.Age,
                    Name = value.Name,
                    Star = value.Star,
                    Gender = value.Gender
                });

                PlayersConnected?.Invoke(true);
            }
        }
        catch (Exception e)
        {
            PlayersConnected?.Invoke(false);
            Debug.Log("Error: " + e.Message);
        }
    }

    private void State_OnChange(List<Colyseus.Schema.DataChange> changes)
    {
        foreach (var change in changes)
        {
            Debug.LogWarning(change.Value);
            if (change.Field == "player1Choice")
            {
                if (playernumber == 2)
                {
                    EnemySelected?.Invoke(Convert.ToInt32(change.Value));
                }
            }

            if (change.Field == "player2Choice")
            {
                if (playernumber == 1)
                {
                    EnemySelected?.Invoke(Convert.ToInt32(change.Value));
                }
            }
        }
    }

    public async void UpdateChoice(Selections choice)
    {
        try
        {
            await room.Send(new { choice = (int)choice });
        }
        catch (Exception err)
        {
            Debug.LogError(err.Message);
        }
    }

    public async void NotifyServerWinner(string deviceID, string name)
    {
        try
        {
            await room.Send(new { winner = new { deviceID, name } });
        }
        catch (Exception err)
        {
            Debug.Log(err.Message);
        }
    }

    private void Players_OnChange(PlayerSchema value, string key)
    {
        Debug.Log("player state changed");
        if (value.SessionID != room.SessionId && value.Choice != -1)
        {
            Debug.Log("choice received of the enemy");
            EnemySelected?.Invoke((int)value.Choice);
        }

        if (value.SessionID != room.SessionId && value.User.Age != 0)
        {
            Debug.LogWarning(value.User.Name);

            EnemyInfoReceived?.Invoke(new User()
            {
                Age = (int)value.User.Age,
                Name = value.User.Name,
                Star = value.User.Star,
                Gender = value.User.Gender
            });
        }
    }

    private void Room_OnError(string message)
    {
        RoomError?.Invoke(message);
    }

    private void Players_OnAdd(PlayerSchema value, string key)
    {
        numPlayers++;

        Debug.Log("A player has joined");

        if (numPlayers == 2)
        {
            Debug.LogWarning("changing scenes");

            PlayersConnected?.Invoke(true);
        }
    }

    public async void Leave()
    {
        if (room != null)
        {
            await room.Leave();
        }
    }
}
