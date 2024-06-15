using FishNet.Object;
using Unity.Mathematics;
using UnityEngine;

public class FinalBattleManager : NetworkBehaviour
{
    private GameObject _winner;
    private GameObject _endRoom;
    private bool _fightStarted;
    [SerializeField] private GameObject _room;
    [SerializeField] private GameObject _boss;

    void Start()
    {
        SetupGameState();
        SetupRoomPlayers();
    }

    void Update()
    {
        if (!_fightStarted) return;
        if (GameManager.Instance.Solo)
        {

        }
        else
        {

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
        if (GameManager.Instance.Solo)
        {
            Vector3 bossSpawn = _endRoom.transform.GetChild(2).transform.position;
            GameObject boss = Instantiate(_boss, bossSpawn, quaternion.identity);
            Spawn(boss);
            _fightStarted = true;
        }
        else
        {
            Vector3 player2Spawn = _endRoom.transform.GetChild(1).transform.position;
            GameManager.Instance.Player2.transform.position = player2Spawn;
            GameManager.Instance.Player1State.EnablePvp = true;
            GameManager.Instance.Player2State.EnablePvp = true;
            _fightStarted = true;
        }

    }
}