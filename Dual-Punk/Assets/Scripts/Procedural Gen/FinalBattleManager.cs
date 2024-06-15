using FishNet.Connection;
using FishNet.Object;
using Unity.Mathematics;
using UnityEngine;

public class FinalBattleManager : NetworkBehaviour
{
    private GameObject _winner;
    private GameObject _endRoom;
    private GameObject _boss;
    private PlayerState _player1State;
    private PlayerState _player2State;
    private bool _fightStarted;
    [SerializeField] private GameObject _room;
    [SerializeField] private GameObject _bossPrefab;
    private PromptTrigger _promptTrigger;

    void Start()
    {
        _promptTrigger = _room.GetComponentInChildren<PromptTrigger>();
        SetupGameState();
        SetupRoomPlayers();
    }

    void Update()
    {
        if (!_fightStarted || !IsServer) return;
        if (GameManager.Instance.Solo)
        {
            if (_boss == null)
            {
                Win(_player1State.gameObject);
            }
        }
        else
        {
            if (_player1State.IsDown)
            {
                Win(_player2State.gameObject);
                Lose(_player1State.gameObject);
            }
            else if (_player2State.IsDown)
            {
                Win(_player1State.gameObject);
                Lose(_player2State.gameObject);
            }
        }
    }


    void SetupGameState()
    {
        _fightStarted = false;
        _winner = null;
        GameManager.Instance.InFinalFight = true;
    }

    void SetupRoomPlayers()
    {
        _endRoom = Instantiate(_room);
        if (!IsServer) return;
        Vector3 player1Spawn = _endRoom.transform.GetChild(0).transform.position;
        GameManager.Instance.Player1.transform.position = player1Spawn;
        _player1State = GameManager.Instance.Player1State;
        if (GameManager.Instance.Solo)
        {
            Vector3 bossSpawn = _endRoom.transform.GetChild(2).transform.position;
            GameObject boss = Instantiate(_bossPrefab, bossSpawn, quaternion.identity);
            Spawn(boss);
            _boss = boss;
            _fightStarted = true;
        }
        else
        {
            Vector3 player2Spawn = _endRoom.transform.GetChild(1).transform.position;
            _player2State = GameManager.Instance.Player2State;
            SetupPlayer(_player2State.gameObject.GetComponent<NetworkObject>().Owner, player2Spawn);
            GameManager.Instance.Player1State.EnablePvp = true;
            GameManager.Instance.Player2State.EnablePvp = true;
            _fightStarted = true;
        }
    }

    void Win(GameObject winningPlayer)
    {
        WinningScreen(winningPlayer.GetComponent<NetworkObject>().Owner);
    }

    [TargetRpc]
    void WinningScreen(NetworkConnection con)
    {
        GameManager.Instance.Win();
    }

    void Lose(GameObject losingPlayer)
    {
        LosingScreen(losingPlayer.GetComponent<NetworkObject>().Owner);
    }

    [TargetRpc]
    void LosingScreen(NetworkConnection con)
    {
        GameManager.Instance.Lose();
    }

    [TargetRpc]
    void SetupPlayer(NetworkConnection con, Vector3 pos)
    {
        _promptTrigger.Spawn();
        GameManager.Instance.LocalPlayer.transform.position = pos;
        GameManager.Instance.LocalPlayerState.EnablePvp = true;
    }
}