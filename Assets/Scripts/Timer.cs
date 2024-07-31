using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : Singleton<Timer>
{
    [SerializeField]
    private TextMeshProUGUI timerText;

    public const float SecToMove = 10;

    private bool SuspendTimerFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        timerText.text = FormatTime(SecToMove);
    }

    private string FormatTime(float seconds)
    {
        int roundedSec = (int)seconds;
        if (roundedSec < 10)
        {
            return "00:0" + roundedSec.ToString();
        }
        else {
            return "00:" + roundedSec.ToString();
        }
    }

    public void SuspendTimer()
    {
        SuspendTimerFlag = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public IEnumerator StartTimer()
    {
        SuspendTimerFlag = false;
        
        float secLeft = SecToMove;

        while (secLeft > 0 && !SuspendTimerFlag)
        {
            timerText.text = FormatTime(secLeft);
            yield return new WaitForSeconds(1f);

            secLeft -= 1f; // maybe no
        }

        if (!SuspendTimerFlag) // timer has not been suspended, so trigger an action
            GameManager.Instance.FinishTurn();
    }
}
