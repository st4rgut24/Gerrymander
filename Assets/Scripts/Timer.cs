using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : Singleton<Timer>
{
    [SerializeField]
    private TextMeshProUGUI timerText;

    public float SecToMove = 10;

    private bool PauseTimerFlag = false;

    public float secLeft;

    // Start is called before the first frame update
    void Start()
    {
        timerText.text = FormatTime(SecToMove);
    }

    private void OnEnable()
    {
        LineAnimator.PartyLineDrawn += OnPartyLineDrawn;
        LineAnimator.DrawPartyLine += OnDrawingPartyLine;
    }


    private void OnDisable()
    {
        LineAnimator.PartyLineDrawn -= OnPartyLineDrawn;
        LineAnimator.DrawPartyLine -= OnDrawingPartyLine;

    }

    private void OnDrawingPartyLine()
    {
        // TODO: pause the timer until the line is drawn
        PauseTimerFlag = true;
    }

    private void OnPartyLineDrawn()
    {
        // reset the timer
        ResetTimer();
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

    public void ResetTimer()
    {
        PauseTimerFlag = false;
        secLeft = SecToMove;
        timerText.text = FormatTime(secLeft);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PauseTimer()
    {
        PauseTimerFlag = true;
    }

    public void UnpauseTimer()
    {
        PauseTimerFlag = false;
    }

    private void FixedUpdate()
    {
        if (!PauseTimerFlag)
        {
            secLeft -= Time.deltaTime;
            timerText.text = FormatTime(secLeft);

            if (secLeft <= 0) // time's up!
            {
                StartCoroutine(GameManager.Instance.FinishTurn());
                ResetTimer();
            }
        }
    }

    //public void StartTimer()
    //{

    //    while (secLeft > 0)
    //    {
    //        timerText.text = FormatTime(secLeft);
    //        yield return new WaitForSeconds(1f);

    //        if (!PauseTimerFlag)
    //            secLeft -= 1f; // maybe no

    //        //if (ResetTimerFlag)
    //        //{
    //        //    break;
    //        //}
    //    }

    //    ResetTimer();
    //    StartCoroutine(GameManager.Instance.FinishTurn());
    //}

    //public IEnumerator StartTimer()
    //{        
    //    secLeft = SecToMove;

    //    while (secLeft > 0)
    //    {
    //        timerText.text = FormatTime(secLeft);
    //        yield return new WaitForSeconds(1f);

    //        if (!PauseTimerFlag)
    //            secLeft -= 1f; // maybe no

    //        if (ResetTimerFlag)
    //        {
    //            break;
    //        }
    //    }

    //    ResetTimer();
    //    StartCoroutine(GameManager.Instance.FinishTurn());
    //}
}
