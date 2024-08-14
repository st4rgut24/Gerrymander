﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}

public class GameManager : Singleton<GameManager>
{
    public GameObject CrownPrefab;

    public PartyDetails demPartyDetails;
    public PartyDetails repPartyDetils;

    public GameObject demProfileGo;
    public GameObject repProfileGo;

    public bool PlayerTurn;
    public bool GameOver = false;

    FirebaseManager.User user;
    public FirebaseManager.User defaultUser;

    Score score;
    Days days;

    public int ElectionYear;
    public Party PlayerParty;
    public float DemocratPct = .5f;

    public int DaysTilElection = 1;
    public float RepublicanPct;
    public int population = 50;

    public List<Party> PartyList = new List<Party>();

    public int democraticDistricts = 0;
    public int republicanDistricts = 0;

    public bool IsTutorial = false;
    public const int TutorialTimeAgentTakesToMove = 3;

    public Color DemColor = new Color(22 / 255f, 109 / 255f, 243 / 255f);

    public Color RepColor = new Color(255 / 255f, 40 / 255f, 50 / 255f);

    public Color AvgColor;

    Agent agent;

    public bool[] agentActionsTest = new bool[] { true, true, true, false, false, false }; // true means divide, false means join
    int agentMoveIdx = 0;

    public int startElectionYear = 1960;
    public int endElectionYear = 2024;

    private void OnEnable()
    {

    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(this.gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        PlayerTurn = true;

        defaultUser = new FirebaseManager.User("", false);

        AvgColor = new Color((DemColor.r + RepColor.r) / 2, (DemColor.g + RepColor.g) / 2, (DemColor.b + RepColor.b) / 2);
    }

    public IEnumerator SetUser(FirebaseManager.User user)
    {
        yield return null;
        this.user = user;
    }

    public void InitPlayerParty(float DemPartyPct, Party PlayerParty)
    {
        DemocratPct = DemPartyPct;
        RepublicanPct = 1 - DemocratPct;

        InitPartyAffiliations(PlayerParty, Difficulty.Hard);
        InitVoterComposition();
    }

    FirebaseManager.Election FindElection(List<FirebaseManager.Election> elections, int year)
    {
        return elections.Find((e) => e.electionYear == year);
    }

    public IEnumerator PopulatePlayMenu(List<FirebaseManager.Election> elections)
    {
        //for (int i = startElectionYear; i <= endElectionYear; i += 4)
        for (int i = endElectionYear; i <= endElectionYear; i += 4) // TODO: REMOVE AFTER TESTING
        {
            int electionYear = i;
            FirebaseManager.Election election;

            election = FindElection(elections, i) ?? new FirebaseManager.Election(i, 0, 0);

            GameObject ElectionEntry = GameObject.Find(election.electionYear.ToString());

            Transform demTopFill = ElectionEntry.transform.Find("DemFillTop");
            Transform demBotFill = ElectionEntry.transform.Find("DemFillBottom");

            float widthVal = ElectionEntry.transform.GetComponent<RectTransform>().sizeDelta.x;

            float shareDemVotes;

            if (election.demVotes + election.repVotes == 0)
            {
                shareDemVotes = .5f; // no votes yet, so election is tied (no outcome)
            }
            else
            {
                shareDemVotes = (float)election.demVotes / (float)(election.demVotes + election.repVotes);
            }

            float demWidth = widthVal * shareDemVotes;
            float repWidth = widthVal - demWidth;

            RectTransform demTopFillRect = demTopFill.GetComponent<RectTransform>();
            demTopFillRect.sizeDelta = new Vector2(demWidth, demTopFillRect.sizeDelta.y);

            RectTransform demBotFillRect = demBotFill.GetComponent<RectTransform>();
            demBotFillRect.sizeDelta = new Vector2(demWidth, demBotFillRect.sizeDelta.y);

            // don't apply the gradient unless the opposing rectangle color is going to be visible
            if (repWidth > 0)
            {
                Material demMat = new Material(Shader.Find("Unlit/Texture"));
                demMat.mainTexture = GradientTextureGenerator.GenerateGradientTexture(demTopFillRect.sizeDelta, DemColor, AvgColor, .7f, false);

                demTopFill.GetComponent<Image>().material = demMat;
                demBotFill.GetComponent<Image>().material = demMat;
            }
            else {
                demTopFill.GetComponent<Image>().color = DemColor;
                demBotFill.GetComponent<Image>().color = DemColor;
            }

            Transform repTopFill = ElectionEntry.transform.Find("RepFillTop");
            Transform repBotFill = ElectionEntry.transform.Find("RepFillBottom");

            RectTransform repTopFillRect = repTopFill.GetComponent<RectTransform>();
            repTopFillRect.sizeDelta = new Vector2(repWidth, repTopFillRect.sizeDelta.y);

            RectTransform repBotFillRect = repBotFill.GetComponent<RectTransform>();
            repBotFillRect.sizeDelta = new Vector2(repWidth, repBotFillRect.sizeDelta.y);

            if (demWidth > 0)
            {
                Material repMat = new Material(Shader.Find("Unlit/Texture"));
                repMat.mainTexture = GradientTextureGenerator.GenerateGradientTexture(repTopFillRect.sizeDelta, RepColor, AvgColor, .7f, true);
                repTopFill.GetComponent<Image>().material = repMat;
                repBotFill.GetComponent<Image>().material = repMat;
            }
            else
            {
                repTopFill.GetComponent<Image>().color = RepColor;
                repBotFill.GetComponent<Image>().color = RepColor;
            }

            Transform DemProfile = ElectionEntry.transform.Find("DemProfile");
            Transform RepProfile = ElectionEntry.transform.Find("RepProfile");

            if (election.demVotes > election.repVotes)
                Instantiate(CrownPrefab, DemProfile);
            else if (election.repVotes > election.demVotes)
                Instantiate(CrownPrefab, RepProfile);

        }

        yield return null;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public ElectionDetails GetDetailsFromPartyYear()
    {
        if (ElectionDetailsManager.ElectionMap == null)
        {
            return null;
        }
        if (ElectionDetailsManager.ElectionMap.ContainsKey(ElectionYear))
            return ElectionDetailsManager.ElectionMap[ElectionYear];
        else
            return null;
    }

    public void LoadPlayMenu()
    {
        SceneManager.LoadScene(Consts.PlayMenu);
    }

    public void LoadTutorial()
    {
        IsTutorial = true;
        InitPlayerParty(.55f, Consts.PlayerTutorialParty);
        SceneManager.LoadScene(Consts.TutorialScene);
    }

    public void LoadElectionDetailsScene()
    {
        VerticalScrollSelector scrollSelector = GameObject.Find(Consts.ScrollRect).GetComponent<VerticalScrollSelector>();
        Transform Year = scrollSelector.transform.Find(Consts.ContentGo).GetChild(scrollSelector.selectedIdx).Find(Consts.YearBanner).Find(Consts.Year);
        string yearText = Year.GetComponent<TextMeshProUGUI>().text;

        ElectionYear = int.Parse(yearText);
        Debug.Log("Election Year is " + ElectionYear);

        SceneManager.LoadScene(Consts.ElectionDetails);
    }

    //public void InitElectionDetailsScene()
    //{
    //    VerticalScrollSelector scrollSelector = GameObject.Find(Consts.ScrollRect).GetComponent<VerticalScrollSelector>();
    //    Transform Year = scrollSelector.transform.Find(Consts.ContentGo).GetChild(scrollSelector.selectedIdx).Find(Consts.YearBanner).Find(Consts.Year);
    //    string yearText = Year.GetComponent<TextMeshProUGUI>().text;

    //    ElectionYear = int.Parse(yearText);
    //    Debug.Log("Election Year is " + ElectionYear);
    //}

    public void LoadGameScene(float DemPartyPct, float RepPartyPct, Party PlayerParty)
    {
        float partyPctShare = DemPartyPct / (DemPartyPct + RepPartyPct);

        InitPlayerParty(partyPctShare, PlayerParty);
        SceneManager.LoadScene(Consts.Game);
    }

    public void InitPartyAffiliations(Party PlayerParty, Difficulty difficulty)
    {
        Party AgentParty = PlayerParty == Party.Republican ? Party.Democrat : Party.Republican;

        agent = new Agent(difficulty, AgentParty, PlayerParty);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Equals(Consts.Game) || scene.name.Equals(Consts.TutorialScene))
        {
            score = GameObject.Find(Consts.ScoreGo).GetComponent<Score>();
            days = GameObject.Find(Consts.DaysGo).GetComponent<Days>();

            //StartCoroutine(Timer.Instance.StartTimer()); // player goes first start the timer when the scene loads
            Timer.Instance.ResetTimer();

            if (scene.name.Equals(Consts.Game))
            {
                demProfileGo = GameObject.Find("DemProfile");
                repProfileGo = GameObject.Find("RepProfile");

                demProfileGo.SetActive(false);
                repProfileGo.SetActive(false);
            }

            StartCoroutine(PauseForProfile());
        }
        if (scene.name.Equals(Consts.PlayMenu))
        {
            FirebaseManager.Instance.UpdatePlayMenu();
        }
    }

    public void AddPartyVoter(Party party)
    {
        if (party == Party.Republican)
            republicanDistricts++;
        if (party == Party.Democrat)
            democraticDistricts++;
    }

    public void RemovePartyVoter(Party party)
    {
        if (!GameOver)
        {
            if (party == Party.Republican)
                republicanDistricts--;
            if (party == Party.Democrat)
                democraticDistricts--;
        }
    }

    public void InitVoterComposition()
    {
        int democrats = (int) (population* DemocratPct);
        int republicans = (int)(population * RepublicanPct);

        for (int i=0; i< democrats; i++)
        {
            PartyList.Add(Party.Democrat);
        }

        for (int i = 0; i < republicans; i++)
        {
            PartyList.Add(Party.Republican);
        }
    }

    private IEnumerator PauseForProfile()
    {
        Timer.Instance.PauseTimer();
        if (SceneManager.GetActiveScene().name == Consts.Game)
        {
            yield return StartCoroutine(ShowProfile(PlayerTurn));
        }
        Timer.Instance.UnpauseTimer();
    }

    // player has finished their turn
    public IEnumerator FinishTurn()
    {
        if (DaysTilElection == 0)
        {
            EndGame();
            yield break;
        }

        PlayerTurn = !PlayerTurn;
        Timer.Instance.ResetTimer();

        yield return StartCoroutine(PauseForProfile());

        //Timer.Instance.PauseTimer();
        //yield return StartCoroutine(ShowProfile(PlayerTurn));
        //Timer.Instance.UnpauseTimer();

        if (!PlayerTurn)
        {
            Controller.PauseTouch = true;

            yield return new WaitForSeconds(Consts.AgentActiondelay);

            if (IsTutorial)
                agent.MakeMove(Agent.Move.Divide);
            else
                agent.MakeRandomMove();

            NextDay();
            agentMoveIdx++;
        }
        else
        {
            Controller.PauseTouch = false;
        }
    }

    private IEnumerator ShowProfile(bool playerTurn)
    {
        PartyDetails partyDetails;
        GameObject profileGo;

        if (playerTurn)
        {
            partyDetails = PlayerParty == Party.Republican ? repPartyDetils : demPartyDetails;
            profileGo = PlayerParty == Party.Republican ? repProfileGo : demProfileGo;
        }
        else
        {
            partyDetails = PlayerParty == Party.Republican ? demPartyDetails : repPartyDetils;
            profileGo = PlayerParty == Party.Republican ? demProfileGo : repProfileGo;
        }

        profileGo.SetActive(true);

        Image picGo = profileGo.transform.Find("PlayerPic").GetComponent<Image>();
        TextMeshProUGUI nameText = profileGo.transform.Find("NameBanner").Find("Name").GetComponent<TextMeshProUGUI>();

        picGo.sprite = partyDetails.partySprite;
        if (!playerTurn)
            nameText.text = partyDetails.candidate.Split(" ")[0] + "'s\n Turn";
        else
            nameText.text = "Your\n Turn";
        yield return new WaitForSeconds(1); // time to display the next turn ui

        if (profileGo != null)
            profileGo.SetActive(false);
    }

    public void NextDay()
    {
        DaysTilElection--;
        days.SetDays(GameManager.Instance.DaysTilElection);
    }

    public void EndGame()
    {
        GameOver = true;
        SceneManager.LoadScene(Consts.ResultsScene);
    }

    public void UpdateScore ()
    {
        score.SetScore(democraticDistricts, republicanDistricts);
    }

    // Update is called once per frame
    void Update()
	{
			
	}

    private void OnDisable()
    {
    }

    void OnDestroy()
    {
        // Unsubscribe from the scene loaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

