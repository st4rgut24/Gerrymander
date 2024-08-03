using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;
using TMPro;

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

    int prevPlayerScore;
    int prevAgentScore;

    int playCount = 0;
    float keepDividingBkgndSec = 3; // num sec this background is up
    float keepDividingCounter = 0;

    const string InstructionGoName = "Instruction";

    Party playerParty;
    Party aiParty;

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
        OpponentTurn,
        KeepDividing,
        JoinRoom,        // DAY 3
        RebuildRoom,     // DAY 4
        EndGameCondition
    }

    [SerializeField]
    private GameObject ButtonGo;

    [SerializeField]
    private GameObject[] Slides;

    [SerializeField]
    private GameObject[] Cursors;

    [SerializeField]
    private GameObject FinishBtn;

    [SerializeField]
    private GameObject RestartBtn;

    private Button ExitBtn;

    Vector2 topHalfWorldCenter;
    Vector2 bottomHalfWorldCenter;

    RoomPrefab joinedRoom1;
    RoomPrefab joinedRoom2;

    Vector2 joinedRoomCenter;
    Vector2 joinedRoomBackgroundPos;

    Vector2 DynamicBackgroundPos;
    Vector2 DynamicCursorPos;

    GameObject ActiveSlide;
    int slideIdx = -1;

    public Vector2 cursorEndLoc;
    public Vector2 cursorStartLocation; // Assign the start position in the Inspector
    public float duration = .5f;   // Duration of the movement in seconds

    private float startTime;
    private bool movingToEnd = true; // Flag to track direction of movement

    RoomPrefab playerPartyFirstRoom;

    bool PartyLineDrawn = false;
    bool StartDrawPartyLine = false;

    int democratScoreTracker = 0;
    int republicanScoreTracker = 0;
    
    void setPrevTurnState()
    {
        PartyLineDrawn = false;

        democratScoreTracker = GameManager.Instance.democraticDistricts;
        republicanScoreTracker = GameManager.Instance.republicanDistricts;
    }

    public IEnumerator AdvanceSlide()
    {
        setPrevTurnState();

        slideIdx++;

        if (ActiveSlide != null)
        {
            ActiveSlide.SetActive(false);
        }

        TutorialController.Instance.SetSlideIdx(slideIdx);

        if (slideIdx != (int)Slide.OpponentTurn) // don't want to wait for line to finish drawing for this slide
        {
            while (StartDrawPartyLine && !PartyLineDrawn) // don't advance to next slide state until political layout is stabilized
                yield return null;
        }

        Timer.Instance.UnpauseTimer();
        SetupSlideEnvironment();

        ActiveSlide = Slides[slideIdx];
        ActiveSlide.SetActive(true);

        if (slideIdx == Slides.Length - 1)
        {
            FinishBtn.SetActive(true);
            RestartBtn.SetActive(true);
        }
    }

    public void SetCursorPositionFromRoomWithVirus()
    {
        RoomPrefabWithVirus = RoomWithPlayerParty(virus.gameObject);
        SetCursorPositionFromRoom(RoomPrefabWithVirus);
    }

    public Vector2 SetCursorPositionFromRoom(RoomPrefab room)
    {
        DynamicCursorPos = room.GetCenter();
        return DynamicCursorPos;
    }

    public void SetBackgroundPositionFromVirusPos()
    {
        Vector2 VirusPos = Vector2.zero;

        DynamicBackgroundPos = VirusPos;
    }

    public void SetCursorUIPosition()
    {
        if (Cursors[slideIdx] != null)
            SetUIPosition(Cursors[slideIdx], DynamicCursorPos);
    }

    public void SetLerpedCursorUIPosition(Vector2 pos)
    {
        if (Cursors[slideIdx] != null)
            SetUIPosition(Cursors[slideIdx], pos);
    }

    public void SetBackgroundUIPosition()
    {
        GameObject SlideGo = Slides[slideIdx];
        GameObject BackgroundGo = Helpers.GetChildByName(SlideGo, InstructionGoName);
        SetUIPosition(BackgroundGo, DynamicBackgroundPos);
    }

    public void SetUIPosition(GameObject UI, Vector2 worldPos)
    {
        Vector2 viewportPoint = Camera.main.WorldToViewportPoint(worldPos);  //convert game object position to VievportPoint

        RectTransform rectTransform = UI.GetComponent<RectTransform>();
        // set MIN and MAX Anchor values(positions) to the same position (ViewportPoint)

        //Debug.Log("set viewport point " + viewportPoint);

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

    public RoomPrefab RoomWithPlayerParty(GameObject virusGo)
    {
        return Map.Instance.FindRoomWorldCoords(virusGo.transform.position);
    }

    public void SetBackgroundPositionFromRoom(RoomPrefab room)
    {
        DynamicBackgroundPos = room.GetCenter();
    }

    public void SetBackgroundPosition(Vector2 worldPos)
    {
        DynamicBackgroundPos = worldPos;
    }

    public string GetEvaluationSubText(int delta)
    {
        if (delta < 0)
        {
            return "lost a vote";
        }
        else if (delta == 0)
        {
            return "votes remain unchanged";
        }
        else
        {
            return "gained a vote";
        }
    }

    public string GetEvaluationText(Party party)
    {
        int deltaDem = GameManager.Instance.democraticDistricts - democratScoreTracker;
        int deltaRep = GameManager.Instance.republicanDistricts - republicanScoreTracker;

        if (party == Party.Democrat)
        {
            return "The Democrats " + GetEvaluationSubText(deltaDem) + "and the Republicans " + GetEvaluationSubText(deltaRep); 
        }
        else
        {
            return "The Republicans " + GetEvaluationSubText(deltaRep) + "and the Democrats " + GetEvaluationSubText(deltaDem);
        }
    }



    private int getScore(Party party)
    {
        if (party == Party.Republican)
            return GameManager.Instance.republicanDistricts;
        else
        {
            return GameManager.Instance.democraticDistricts;
        }
    }

    // set the text based on the evaluation of the score
    public IEnumerator EvaluateTurn(string prefix, bool isPlayersTurn)
    {
        AdvancingSlide = true; // prevents evalute turn from getting called many times
        TextMeshProUGUI slideText = ActiveSlide.GetComponentInChildren<TextMeshProUGUI>();

        while (!PartyLineDrawn) // wait for the divider to be drawn
        {
            yield return null;
        }

        yield return new WaitForSeconds(1);

        slideText.text = prefix + GetEvaluationText(isPlayersTurn ? playerParty : aiParty);

        StartCoroutine(DelayNextSlide(3));
    }

    public IEnumerator DelayNextSlide(int seconds)
    {
        AdvancingSlide = true;
        yield return new WaitForSeconds(seconds);
        StartCoroutine(AdvanceSlide());
        AdvancingSlide = false;
    }

    public RoomPrefab GetPlayerPartyRoom(List<RoomPrefab> rooms)
    {
        foreach (RoomPrefab room in rooms)
        {
            if (room.GetParty() == playerParty)
            {
                return room;
            }
        };

        return null;
    }

    public RoomPrefab GetAdjacentDistrictByParty(RoomPrefab room, Party party)
    {
        RoomPrefab defaultRoom = room.AdjacentRooms[0];

        foreach (RoomPrefab adjRoom in room.AdjacentRooms)
        {
            if (adjRoom.GetParty() == party)
            {
                defaultRoom = adjRoom;
                break;
            }
        }

        Debug.Log("Couldnt find adjacent district with party " + party);
        return defaultRoom;
    }

    public RoomPrefab GetOpponentDistrict()
    {
        foreach (RoomPrefab district in Map.Instance.Rooms)
        {
            if (district.GetParty() == aiParty)
            {
                return district;
            }
        }
        return null;
    }

    public bool IsOpponentDistrictExist()
    {
        foreach (RoomPrefab district in Map.Instance.Rooms)
        {
            if (district.GetParty() == aiParty)
            {
                return true;
            }
        }
        return false;
    }

    public void SetupSlideEnvironment()
    {
        DynamicCursorPos = Vector2.zero;
        cursorEndLoc = Vector2.zero;

        switch (slideIdx)
        {
            case (int)Slide.MonitorDaysLeft:
                Timer.Instance.PauseTimer();
                break;
            case (int)Slide.MonitorTimer:
                Timer.Instance.UnpauseTimer();
                break;
            case (int)Slide.DivideToScore:
                break;
            case (int)Slide.ColorMeaning:
                topHalfWorldCenter = Map.Instance.Rooms[0].GetCenter();
                bottomHalfWorldCenter = Map.Instance.Rooms[1].GetCenter();

                //roomWithFewerVotes = GetRoomWithFewerVotes(Map.Instance.Rooms[0], Map.Instance.Rooms[1]);
                //RoomPrefab roomWithMoreVotes = Map.Instance.Rooms[0] == roomWithFewerVotes ? Map.Instance.Rooms[1] : Map.Instance.Rooms[0];
                playerPartyFirstRoom = GetPlayerPartyRoom(Map.Instance.Rooms);
                SetBackgroundPositionFromRoom(playerPartyFirstRoom);
                SetBackgroundUIPosition();

                //SetCursorPositionFromRoom(roomWithMoreVotes);
                //SetCursorUIPosition();
                break;
            case (int)Slide.OpponentTurn:
                // evaluate the opponent's move
                bool IsBottomCenterFarther = Vector2.Distance(topHalfWorldCenter, playerPartyFirstRoom.GetCenter()) < Vector2.Distance(bottomHalfWorldCenter, playerPartyFirstRoom.GetCenter());
                Vector2 fartherPos = IsBottomCenterFarther ? bottomHalfWorldCenter : topHalfWorldCenter;

                SetBackgroundPosition(fartherPos);
                SetBackgroundUIPosition();
                break;
            case (int)Slide.KeepDividing:
                break;
            case (int)Slide.JoinRoom:
                Debug.Log("Join room");
                RoomPrefab toDistrict;
                RoomPrefab OppDistrict = GetOpponentDistrict();

                if (OppDistrict != null)
                {
                    toDistrict = OppDistrict;
                }
                else
                {
                    toDistrict = Map.Instance.Rooms[0]; // get any room
                }

                RoomPrefab fromDistrict = GetAdjacentDistrictByParty(toDistrict, playerParty);

                joinedRoom1 = toDistrict;
                joinedRoom2 = fromDistrict;

                // set to cursor
                // set from cursor
                SetCursorPositionFromRoom(fromDistrict);
                cursorEndLoc = toDistrict.GetCenter();
                Debug.Log("To district " + toDistrict.GetCenter());
                Debug.Log("From district " + fromDistrict.GetCenter());

                joinedRoomCenter = cursorEndLoc;
                bool isFartherCenter = Vector2.Distance(topHalfWorldCenter, joinedRoomCenter) < Vector2.Distance(bottomHalfWorldCenter, joinedRoomCenter);
                joinedRoomBackgroundPos = Vector2.Distance(topHalfWorldCenter, joinedRoomCenter) < Vector2.Distance(bottomHalfWorldCenter, joinedRoomCenter) ?
                    bottomHalfWorldCenter :
                    topHalfWorldCenter;


                DynamicBackgroundPos = joinedRoomBackgroundPos;
                SetBackgroundUIPosition();
                break;
            case (int)Slide.RebuildRoom:
                DynamicCursorPos = joinedRoomCenter;
                SetBackgroundPosition(joinedRoomBackgroundPos);

                SetBackgroundUIPosition();
                SetCursorUIPosition();
                break;
            case (int)Slide.EndGameCondition:
                resetCursorPos();
                DynamicBackgroundPos = topHalfWorldCenter;
                SetBackgroundUIPosition();
                return; // dont set cursor locations again
        }

        if (DynamicCursorPos.Equals(Vector2.zero) && Cursors[slideIdx] != null)
        {
            DynamicCursorPos = Camera.main.ScreenToWorldPoint(Cursors[slideIdx].GetComponent<RectTransform>().position);
        }
        //else
        //{
        cursorStartLocation = DynamicCursorPos;
        //}

        if (cursorEndLoc.Equals(Vector2.zero))
        {
            cursorEndLoc = cursorStartLocation + Vector2.up * .1f;
        }
    }

    void resetCursorPos()
    {
        DynamicCursorPos = Vector2.zero;
        cursorEndLoc = Vector2.zero;
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
        LineAnimator.PartyLineDrawn += OnPartyLineDrawn;
        LineAnimator.DrawPartyLine += OnDrawingPartyLine;
    }

    void OnDrawingPartyLine()
    {
        StartDrawPartyLine = true;
    }

    void OnPartyLineDrawn()
    {
        playCount++;
        PartyLineDrawn = true;
        StartDrawPartyLine = false;
    }

    private void OnDisable()
    {
        LineAnimator.PartyLineDrawn -= OnPartyLineDrawn;
    }

    private void Awake()
    {
        // tODO CHANGE THis to be configurable via the tutorial menu
        playerParty = Consts.PlayerTutorialParty;
        aiParty = Consts.AiTutorialParty;
    }


    // Start is called before the first frame update
    void Start()
    {
        Timer.Instance.SecToMove = Consts.TutorialSecToMove;
        startTime = Time.time;
        StartCoroutine(AdvanceSlide());
    }

    void MoveCursor()
    {
        // Calculate the fraction of time elapsed since movement started
        float elapsedTime = (Time.time - startTime) / duration;

        if (movingToEnd)
        {
            //Debug.Log("cursor start location " + cursorStartLocation + " cursor end location " + cursorEndLoc);
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

    public bool JoinedRoomExists()
    {
        foreach (RoomPrefab room in Map.Instance.Rooms)
        {
            if (room.AdjacentConnectedRooms.Count > 0)
            {
                return true;
            }
        }
        return false;
    }

    // Advance slide conditions here, check every frame
    void Update()
    {
        if (Timer.Instance.secLeft < 1)
        {
            Timer.Instance.PauseTimer();
        }

        MoveCursor();

        if (AdvancingSlide)
        {
            return;
        }

        if (slideIdx == (int)Slide.MonitorDaysLeft)
        {
            StartCoroutine(DelayNextSlide(3));
            //StartCoroutine(DelayNextSlide(5));
        }
        if (slideIdx == (int)Slide.MonitorTimer)
        {

            StartCoroutine(DelayNextSlide(3));
            //StartCoroutine(DelayNextSlide(5));
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
            if (!GameManager.Instance.PlayerTurn && StartDrawPartyLine) // opponent starts drawing party line
                StartCoroutine(AdvanceSlide());
        }
        if (slideIdx == (int)Slide.OpponentTurn)
        {
            // set background position in the room that the ai split

            StartCoroutine(EvaluateTurn("", false));
        }
        if (slideIdx == (int)Slide.KeepDividing)
        {
            keepDividingCounter += Time.deltaTime;
            if (keepDividingCounter > keepDividingBkgndSec)
            {
                Slides[slideIdx].SetActive(false);
            }
            if (playCount > 6 || (IsOpponentDistrictExist() && playCount >= 4))
            {
                if (playCount > 6)
                {
                    Debug.Log("advancing slide because more than 6 plays ahve happend");
                }
                else
                {
                    Debug.Log("advancing slide because oooionent party exists");
                }
                StartCoroutine(DelayNextSlide(2)); // time for ai to move
            }
        }
        if (slideIdx == (int)Slide.JoinRoom && JoinedRoomExists())
        {
            StartCoroutine(AdvanceSlide());
        }
        if (slideIdx == (int)Slide.RebuildRoom)
        {
            if (joinedRoom1.IsRoomCompleted() || joinedRoom2.IsRoomCompleted())
            {
                StartCoroutine(AdvanceSlide());
            }
        }
    }
}
