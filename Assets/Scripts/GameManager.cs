using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}

public class GameManager : Singleton<GameManager>
{
    public bool PlayerTurn;
    public bool GameOver = false;

    Score score;
    Days days;

    public int ElectionYear;
    public Party PlayerParty;
    public float DemocratPct = .5f;

    public int DaysTilElection = 20;
    public float RepublicanPct;
    public int population = 50;

    public List<Party> PartyList = new List<Party>();

    public int democraticDistricts = 0;
    public int republicanDistricts = 0;

    public bool IsTutorial = false;
    public const int TutorialTimeAgentTakesToMove = 3;

    Agent agent;

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
    }

    public void InitPlayerParty(float DemPartyPct, Party PlayerParty)
    {
        DemocratPct = DemPartyPct;
        RepublicanPct = 1 - DemocratPct;

        InitPartyAffiliations(PlayerParty, Difficulty.Hard);
        InitVoterComposition();
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
        }
    }

    public void AddPartyVoter(Party party)
    {
        Debug.Log("Add party voter for " + party);
        if (party == Party.Republican)
            republicanDistricts++;
        if (party == Party.Democrat)
            democraticDistricts++;
    }

    public void RemovePartyVoter(Party party)
    {
        Debug.Log("Remove party voter for " + party);
        if (party == Party.Republican)
            republicanDistricts--;
        if (party == Party.Democrat)
            democraticDistricts--;
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

    // player has finished their turn
    public IEnumerator FinishTurn()
    {
        PlayerTurn = !PlayerTurn;

        //StartCoroutine(Timer.Instance.StartTimer());
        //Timer.Instance.StartTimer();

        if (!PlayerTurn)
        {
            yield return new WaitForSeconds(Consts.AgentActiondelay);

            agent.DivideRoom();
            NextDay();
        }
    }

    public void NextDay()
    {
        DaysTilElection--;
        days.SetDays(GameManager.Instance.DaysTilElection);

        if (DaysTilElection == 0)
        {
            EndGame();
        }
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

