using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class ReviveAndDownController : NetworkBehaviour
{
    private PlayerState _playerState;
    private bool _onPlayer;
    private GameObject _playerStoodOn;
    private float _timer;
    private float _startTime;
    private float _holdTime;
    private bool _action;
    private IndicatorTrigger _downIndicatorTrigger;
    void Start()
    {
        _downIndicatorTrigger = GetComponentInChildren<IndicatorTrigger>();
        _onPlayer = false;
        _playerStoodOn = null;
        _playerState = GetComponent<PlayerState>();
        _startTime = 0f;
        _timer = 0f;
        _holdTime = 2f;
    }

    void Update()
    {
        if (_onPlayer && _playerStoodOn.GetComponent<PlayerState>().IsDown)
        {
            if (Input.GetButtonDown("Pickup"))
            {
                _action = false;
                _timer = Time.time;
                _startTime = _timer;
            }
            if(Input.GetButton("Pickup") && !_action)
            {
                _timer += Time.deltaTime;
                if (_timer > _startTime + _holdTime)
                {
                    _action = true;
                    ReviveOtherRPC(_playerStoodOn);
                }
            }

        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            _onPlayer = true;
            _playerStoodOn = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            _onPlayer = false;
            _playerStoodOn = null;
        }
    }

    [ServerRpc (RequireOwnership = false)]
    private void ReviveOtherRPC(GameObject Player)
    {
        NetworkObject playerNObject = Player.GetComponent<NetworkObject>();
        if (playerNObject == null || playerNObject.Owner == null) return;
        else ReviveTargetRPC(playerNObject.Owner);
    }

    [TargetRpc]
    private void ReviveTargetRPC(NetworkConnection con)
    {
        GameManager.Instance.LocalPlayer.GetComponent<ReviveAndDownController>().ResPlayerServer();
    }

    [ServerRpc (RequireOwnership = true)]
    public void DownServer()
    {
        DownObservers();
    }
    
    [ObserversRpc]
    private void DownObservers()
    {
        _playerState.Health = 0;
        _playerState.IsDown = true;
        _downIndicatorTrigger.SpawnIndicatorWithParent(_playerState.gameObject);
    }

    [ServerRpc (RequireOwnership = true)]
    public void ResPlayerServer()
    {
        ResPlayerObservers();
    }

    [ObserversRpc]
    private void ResPlayerObservers()
    {
        _playerState.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        _playerState.Health = _playerState.MaxHealth/2;
        _playerState.IsDown = false;
        _playerState.CrawlTimer = 0;
        PromptManager.Instance.CloseCurrentIndicator();
    }
}