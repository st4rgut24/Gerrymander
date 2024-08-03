using UnityEngine;
using System.Collections;
using System;
using static TutorialManager;

public class TutorialController : Singleton<TutorialController>
{
    public int slideIdx;
    bool PauseTouchEvent = false;
    bool PauseDragEvent = true;

    public static Action<Vector3, Vector3> DragEvent;
    public static Action<Vector3> TouchEvent;
    public static Action<Vector3> TouchPressEvent;

    float dragThreshold = 15f;

    Vector3 touchStart;
    float touchTime;

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Input.mousePosition;
            touchTime = Time.time;
        }
        if (Input.GetMouseButtonUp(0))
        {
            SendTouchEvent(Input.mousePosition);
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            int touches = Input.touchCount;
            Touch touch = Input.GetTouch(touches - 1); // Consider the last touch to support multitasking (eg hitting while walking)

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStart = touch.position;
                    break;

                case TouchPhase.Moved:
                    break;

                case TouchPhase.Ended:
                    // Use melee weapon if the touches were made within permissable area
                    SendTouchEvent(touch.position);
                    break;
            }
        }
#endif
    }

    public void SetupSlideEnvironment()
    {
        switch (slideIdx)
        {
            case (int)Slide.MonitorDaysLeft:
                PauseTouchEvent = true;
                break;
            case (int)Slide.DivideToScore:
                PauseTouchEvent = false;
                break;
            case (int)Slide.ColorMeaning:
                PauseTouchEvent = true;
                break;
            case (int)Slide.KeepDividing:
                PauseTouchEvent = false;
                break;
            case (int)Slide.JoinRoom:
                PauseDragEvent = false;
                PauseTouchEvent = true;
                break;
            case (int)Slide.RebuildRoom:
                PauseDragEvent = true;
                PauseTouchEvent = false;
                break;
            case (int)Slide.EndGameCondition:
                PauseTouchEvent = true;
                break;
        }
    }

    public void SetSlideIdx(int slideIdx)
    {
        this.slideIdx = slideIdx;
        SetupSlideEnvironment();
    }

    private void SendTouchEvent(Vector3 touchPos)
    {
        //if (PauseTouchEvent)
        //{
        //    return;
        //}
        if (IsSingleTouch(touchPos))
        {
            if (!PauseTouchEvent)
                TouchEvent?.Invoke(touchPos);
        }
        else if (!PauseDragEvent)
        {
            DragEvent?.Invoke(touchStart, touchPos);
        }
    }

    private bool IsSingleTouch(Vector3 endTouchPos)
    {
        return Vector3.Distance(touchStart, endTouchPos) < dragThreshold;
    }
}

