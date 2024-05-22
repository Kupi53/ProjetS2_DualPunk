using System.Collections;
using System.Collections.Generic;
using FishNet.Component.Animating;
using FishNet.Object;
using UnityEngine;

public class EnemyAnimator : NetworkBehaviour
{
    private NetworkAnimator _animator;
    private NPCState _npcState;
    [SerializeField] private AnimationClip _runAnimation;
    [SerializeField] private AnimationClip _walkAnimation;
    [SerializeField] private AnimationClip _idleAnimation;


    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<NetworkAnimator>();
        _npcState = GetComponent<NPCState>();
        _animator.Play(_walkAnimation.name);
    }

    // Update is called once per frame
    void Update()
    {
        /*if (_npcState.Run) {
            _animator.Play(_runAnimation.name);
        }
        else if (_npcState.Stop) {
            _animator.Play(_idleAnimation.name);
        }
        else {
            _animator.Play(_walkAnimation.name);
        }*/
    }
}
