using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CoolDownController : MonoBehaviour
{
    public int CoolDownTimer;
    private int CurrentTimer;
    public Text CountDown;

    private IEnumerator TriggerCountDown() {
        
        while(CurrentTimer > 0) {
            CountDown.text = CurrentTimer.ToString();
            yield return new WaitForSeconds(1f);
            CurrentTimer--;
        }

    }

    public void StartCD() {
        CurrentTimer = CoolDownTimer;
        TriggerCountDown();
    }


}
