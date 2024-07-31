using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class TutorialManager : Singleton<TutorialManager>
{
    [SerializeField]
    private RectTransform scoreRect;

    PersonPrefab virus;
    PersonPrefab healthy;
    RoomPrefab RoomPrefabWithVirus;
    RoomPrefab RoomWithHealthyCell;
    RoomPrefab DestroyedRoom;

    float DelaySickSec = 3;
    bool AdvancingSlide = false;
    bool VirusIsQuarantined = false;

    bool IsTopRoomFlag;

    //1. Number of days left til the election
    //2. Timer icon
    //3. How to score by dividing rooms
    //4. Meaning of the different colors
    //5. Joining Rooms
    //6. Filling Rooms
    //7. End game condition

    public enum Slide
    {
        MonitorDaysLeft,
        MonitorTimer,
        DivideToScore,   // DAY 1
        ColorMeaning,
        JoinRoom,        // DAY 2
        RebuildRoom,     // DAY 3
        EndGameCondition
    }

    [SerializeField]
    private GameObject[] Slides;

    [SerializeField]
    private GameObject[] Cursors;

    [SerializeField]
    private GameObject FinishBtn;

    [SerializeField]
    private GameObject RestartBtn;

    private Button ExitBtn;

    Vector2 DynamicBackgroundPos;
    Vector2 DynamicCursorPos;

    GameObject ActiveSlide;
    int slideIdx = -1;

    public Vector2 cursorEndLoc;
    public Vector2 cursorStartLocation; // Assign the start position in the Inspector
    public float duration = .5f;   // Duration of the movement in seconds

    private float startTime;
    private bool movingToEnd = true; // Flag to track direction of movement

    public void AdvanceSlide()
    {
        slideIdx++;

        if (ActiveSlide != null)
        {
            ActiveSlide.SetActive(false);
        }

        TutorialController.Instance.SetSlideIdx(slideIdx);
        SetupSlideEnvironment();

        ActiveSlide = Slides[slideIdx];
        ActiveSlide.SetActive(true);

        if (slideIdx == Slides.Length - 1)
        {
            FinishBtn.SetActive(true);
            RestartBtn.SetActive(true);
        }
    }

    public void SetIsTopRoom()
    {
        RoomPrefabWithVirus = RoomWithVirus(virus.gameObject);
        IsTopRoomFlag = IsTopRoom(RoomPrefabWithVirus);
    }

    public void SetCursorPositionFromRoomWithVirus()
    {
        RoomPrefabWithVirus = RoomWithVirus(virus.gameObject);
        SetCursorPositionFromRoom(RoomPrefabWithVirus);
    }

    public Vector2 SetCursorPositionFromRoom(RoomPrefab room)
    {
        //DynamicCursorPos = room.GetCenter();
        //return DynamicCursorPos;

        return Vector2.zero;
    }

    public void SetBackgroundPositionFromVirusPos()
    {
        Vector2 VirusPos = Vector2.zero;

        DynamicBackgroundPos = VirusPos;
    }

    public void SetCursorUIPosition()
    {
        SetUIPosition(Cursors[slideIdx], DynamicCursorPos);
    }

    public void SetLerpedCursorUIPosition(Vector2 pos)
    {
        SetUIPosition(Cursors[slideIdx], pos);
    }

    public void SetBackgroundUIPosition()
    {
        SetUIPosition(Slides[slideIdx], DynamicBackgroundPos);
    }

    public void SetUIPosition(GameObject UI, Vector2 worldPos)
    {
        Vector2 viewportPoint = Camera.main.WorldToViewportPoint(worldPos);  //convert game object position to VievportPoint

        RectTransform rectTransform = UI.GetComponent<RectTransform>();
        // set MIN and MAX Anchor values(positions) to the same position (ViewportPoint)

        rectTransform.anchorMin = viewportPoint;
        rectTransform.anchorMax = viewportPoint;

        rectTransform.anchoredPosition = Vector2.zero;
    }

    public void FillRoom(RoomPrefab filledRoom)
    {

    }

    public RoomPrefab RoomWithoutVirus(GameObject virusGo)
    {
        return Map.Instance.Rooms.Find((room) => room.Contains(Camera.main.WorldToScreenPoint(virusGo.transform.position)) == null);
    }

    public bool IsTopRoom(RoomPrefab room)
    {
        //float roomCenterY = room.GetCenter().y;
        //List<RoomPrefab> rooms = Map.Instance.Rooms;

        //return (roomCenterY >= rooms[0].GetCenter().y && roomCenterY >= rooms[1].GetCenter().y);

        return false;
    }

    public RoomPrefab RoomWithVirus(GameObject virusGo)
    {
        return Map.Instance.FindRoomWorldCoords(virusGo.transform.position);
        //return Map.Instance.Rooms.Find((room) => room.Contains(virusGo.transform.position) != null);
    }

    public RoomPrefab RoomWithHCell(GameObject healthy)
    {
        return Map.Instance.FindRoomWorldCoords(healthy.transform.position);
    }

    public void SetBackgroundPositionForRoomWithoutVirus()
    {
    }

    public void SetBackgroundPositionFromRoom(RoomPrefab room)
    {
        //DynamicBackgroundPos = room.GetCenter();
    }

    public IEnumerator DelayNextSlide(int seconds)
    {
        AdvancingSlide = true;
        yield return new WaitForSeconds(seconds);
        AdvanceSlide();
        AdvancingSlide = false;
    }

    public void SetupSlideEnvironment()
    {
        cursorEndLoc = Vector2.zero;

        switch (slideIdx)
        {
            case (int)Slide.MonitorDaysLeft:
                break;
            case (int)Slide.MonitorTimer:
                break;
            case (int)Slide.DivideToScore:

                break;
            case (int)Slide.ColorMeaning:
                SetBackgroundPositionForRoomWithoutVirus();

                SetIsTopRoom();
                SetCursorPositionFromRoomWithVirus();
                SetCursorUIPosition();
                break;
            case (int)Slide.JoinRoom:

                DestroyedRoom = RoomWithVirus(virus.gameObject);
                SetCursorPositionFromRoomWithVirus();

                SetBackgroundUIPosition();
                SetCursorUIPosition();
                break;
            case (int)Slide.RebuildRoom:
                SetBackgroundUIPosition();
                SetCursorUIPosition();
                break;
            case (int)Slide.EndGameCondition:
                DynamicCursorPos = Vector2.zero;

                if (IsTopRoomFlag)
                    DynamicBackgroundPos += Vector2.up * .7f;

                SetBackgroundUIPosition();
                return; // dont set cursor locations again
        }

        if (DynamicCursorPos.Equals(Vector2.zero))
        {
            DynamicCursorPos = Camera.main.ScreenToWorldPoint(Cursors[slideIdx].GetComponent<RectTransform>().position);
        }
        else
        {
            cursorStartLocation = DynamicCursorPos;
        }

        if (cursorEndLoc.Equals(Vector2.zero))
        {
            cursorEndLoc = cursorStartLocation + Vector2.up * .1f;
        }
    }

    public void SetCustomStartEndCursorLocation(float distance, Vector2 startPos)
    {
        float actualDist = Vector2.Distance(cursorEndLoc, startPos);


        if (actualDist > distance)
        {
            Vector2 direction = (cursorEndLoc - startPos).normalized;
            float moveDistance = (actualDist - distance) / 2f; // Move half of the excess distance

            cursorStartLocation = startPos + direction * moveDistance;
            cursorEndLoc -= direction * moveDistance;
        }
    }

    public void OnRestart()
    {
        SceneManager.LoadScene(Consts.TutorialScene);
    }

    public void OnFinish()
    {
        SceneManager.LoadScene(Consts.LandingScene);
    }

    private void OnEnable()
    {
        //ExitBtn = GameObject.Find(Consts.ExitBtn).GetComponent<Button>();
        //ExitBtn.onClick.AddListener(GameManager.Instance.OnExit);
    }

    private void OnDisable()
    {
        //ExitBtn.onClick.RemoveListener(GameManager.Instance.OnExit);
    }

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        AdvanceSlide();
    }

    void MoveCursor()
    {
        // Calculate the fraction of time elapsed since movement started
        float elapsedTime = (Time.time - startTime) / duration;

        if (movingToEnd)
        {
            // Move towards the endLocation
            Vector2 lerpedPos = Vector3.Lerp(cursorStartLocation, cursorEndLoc, elapsedTime);

            SetLerpedCursorUIPosition(lerpedPos);

            // If reached the end, switch direction and reset start time
            if (elapsedTime >= 1.0f)
            {
                movingToEnd = false;
                startTime = Time.time;
            }
        }
        else
        {
            // Move towards the startLocation (reverse direction)
            Vector2 lerpedPos = Vector3.Lerp(cursorEndLoc, cursorStartLocation, elapsedTime);

            SetLerpedCursorUIPosition(lerpedPos);

            // If reached the start, switch direction and reset start time
            if (elapsedTime >= 1.0f)
            {
                movingToEnd = true;
                startTime = Time.time;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //MoveCursor();

        if (AdvancingSlide)
        {
            return;
        }

        if (slideIdx == (int)Slide.MonitorDaysLeft)
        {
            StartCoroutine(DelayNextSlide(3));
        }
        if (slideIdx == (int)Slide.MonitorTimer)
        {
            StartCoroutine(DelayNextSlide(3));
        }
        if (slideIdx == (int)Slide.DivideToScore)
        {
            if (Map.Instance.Rooms.Count > 1)
            {
                StartCoroutine(DelayNextSlide(2));
            }
        }
        if (slideIdx == (int)Slide.ColorMeaning)
        {
            AdvanceSlide();
        }
        if (slideIdx == (int)Slide.JoinRoom)
        {

        }
        if (slideIdx == (int)Slide.RebuildRoom)
        {
            if (DestroyedRoom.IsRoomCompleted())
            {
                AdvanceSlide();
            }
        }
    }
}
