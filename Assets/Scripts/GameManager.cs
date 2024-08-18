using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DanielLochner.Assets.SimpleScrollSnap;

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}

public class GameManager : Singleton<GameManager>
{
    public GameObject CrownPrefab;

    public GameObject BlueLazerPrefab;
    public GameObject RedLazerPrefab;
    public GameObject MoustachePrefab;
    public GameObject GlassesPrefab;
    public GameObject BandanaPrefab;

    public GameObject BidenSwagPrefab;
    public GameObject BillClintonSwagPrefab;
    public GameObject BushSwagPrefab;
    public GameObject CarterSwagPrefab;
    public GameObject DoleSwagPrefab;
    public GameObject DukakiSwagPrefab;
    public GameObject FordSwagPrefab;
    public GameObject GoldwaterSwagPrefab;
    public GameObject GoreSwagPrefab;
    public GameObject HillarySwagPrefab;
    public GameObject HumphreySwagPrefab;
    public GameObject HWBushSwagPrefab;
    public GameObject JohnsonSwagPrefab;
    public GameObject KamalaSwagPrefab;
    public GameObject KennedySwagPrefab;
    public GameObject KerrySwagPrefab;
    public GameObject McCainSwagPrefab;
    public GameObject McGovernSwagPrefab;
    public GameObject MondaleSwagPrefab;
    public GameObject NixonSwagPrefab;
    public GameObject ObamaSwagPrefab;
    public GameObject ReaganSwagPrefab;
    public GameObject RomneySwagPrefab;
    public GameObject TrumpSwagPrefab;

    // TODO: Add swag prefabs for the rest of the candidates

    public PartyDetails demPartyDetails;
    public PartyDetails repPartyDetils;

    public GameObject demProfileGo;
    public GameObject repProfileGo;

    public bool PlayerTurn;
    public bool GameOver = false;

    FirebaseManager.User user;
    public FirebaseManager.User defaultUser;

    public Dictionary<Swag, GameObject> SwagPrefabDict;
    public Dictionary<int, ElectionDetails> ElectionMap;
    public Dictionary<string, GameObject> CandidateSwagPrefabDict;

    Score score;
    Days days;

    public int ElectionYear = 2024;
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

    public Color DemColor = new Color(0f / 255f, 151f / 255f, 255f / 255f);

    public Color RepColor = new Color(255f / 255f, 87f / 255f, 60f / 255f);

    public Color AvgColor;

    Agent agent;

    public bool[] agentActionsTest = new bool[] { true, true, true, false, false, false }; // true means divide, false means join
    int agentMoveIdx = 0;

    public int startElectionYear = 1960;
    public int endElectionYear = 2024;

    public static System.Action SpinEvent;

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

        defaultUser = new FirebaseManager.User();

        AvgColor = new Color((DemColor.r + RepColor.r) / 2, (DemColor.g + RepColor.g) / 2, (DemColor.b + RepColor.b) / 2);

        SwagPrefabDict = new Dictionary<Swag, GameObject>()
        {
            { Swag.BlueLazer, BlueLazerPrefab },
            { Swag.RedLazer, RedLazerPrefab },
            { Swag.Glasses, GlassesPrefab },
            { Swag.Moustache, MoustachePrefab },
            { Swag.Bandana, BandanaPrefab }
        };

        InitElectionMap();
        InitCandidateSwagPrefabDict();
    }

    public void InitCandidateSwagPrefabDict()
    {
        CandidateSwagPrefabDict = new Dictionary<string, GameObject>()
        {
            { "kamala", KamalaSwagPrefab },
            { "trump", TrumpSwagPrefab },
            { "biden", BidenSwagPrefab },
            { "clinton", HillarySwagPrefab },
            { "obama", ObamaSwagPrefab },
            { "romney", RomneySwagPrefab },
            { "mccain", McCainSwagPrefab },
            { "kerry", KerrySwagPrefab },
            { "bush", BushSwagPrefab },
            { "gore", GoreSwagPrefab },
            { "billclinton", BillClintonSwagPrefab },
            { "dole", DoleSwagPrefab },
            { "hwbush", HWBushSwagPrefab },
            { "dukakis", DukakiSwagPrefab },
            { "reagan", ReaganSwagPrefab },
            { "mondale", MondaleSwagPrefab },
            { "carter", CarterSwagPrefab },
            { "ford", FordSwagPrefab },
            { "nixon", NixonSwagPrefab },
            { "mcgovern", McGovernSwagPrefab },
            { "humphrey", HumphreySwagPrefab },
            { "johnson", JohnsonSwagPrefab },
            { "goldwater", GoldwaterSwagPrefab },
            { "kennedy", KennedySwagPrefab },

        };
    }

    public GameObject GetSwagPrefabFromElectionDetails(PartyDetails details)
    {
        return GameManager.Instance.CandidateSwagPrefabDict[details.picFile];
    }

    public void InitElectionMap()
    {
        ElectionMap = new Dictionary<int, ElectionDetails>()
        {
            {
                2024,
                new ElectionDetails(
                    "Trump challenges Biden amid rising political tensions after narrowly surviving an assasination attempt.",                    2024,
                    new PartyDetails("kamala", "Kamala Harris", Party.Democrat, new List<string>() {"Beat Trump", "Protect Democracy"}, .5f),
                    new PartyDetails("trump", "Donald Trump", Party.Republican, new List<string>() {"MAGA"}, .5f),
                    Party.None,
                    "The election results are still pending."
                )
            },
            {
                2020,
                new ElectionDetails(
                    "The pandemic and economic downturn dominate the election debate, with Trump under scrutiny and Biden offering a recovery plan.",                    2020,
                    new PartyDetails("biden", "Joe Biden", Party.Democrat, new List<string>() {"Beat Trump", "Protect Democracy"}, .5f),
                    new PartyDetails("trump", "Donald Trump", Party.Republican, new List<string>() {"MAGA"}, .5f),
                    Party.Democrat,
                            "Biden won with a 4.5% margin in the popular vote, driven by his pandemic response and recovery vision."
                    )
            },
            {
                2016,
                new ElectionDetails(
            "Trump taps into voter discontent with the establishment, while Clinton faces challenges over trust and corruption issues.",                    2016,
                    new PartyDetails("clinton", "Hillary Clinton", Party.Democrat, new List<string>() {"Clinton Foundation Experience", "Expand Healthcare"}, .48f),
                    new PartyDetails("trump", "Donald Trump", Party.Republican, new List<string>() {"Make America Great Again", "Immigration Reform"}, .46f),
                    Party.Republican,
        "Trump’s victory was driven by his change promises and appeal in battleground states, winning with a 2.1% popular vote margin."                )
            },
            {
                2012,
                new ElectionDetails(
            "Obama seeks re-election amid economic recovery and foreign policy challenges from the Arab Spring and Libyan conflict.",                    2012,
                    new PartyDetails("obama", "Barack Obama", Party.Democrat, new List<string>() {"Affordable Care Act", "Economic Recovery"}, .51f),
                    new PartyDetails("romney", "Mitt Romney", Party.Republican, new List<string>() {"Economic Reform", "Reduce Government Spending"}, .47f),
                    Party.Democrat,
                        "Obama secured a 3.9% margin in the popular vote, bolstered by economic recovery and foreign policy achievements.")
            },
            {
                2008,
                new ElectionDetails(
            "Obama’s campaign emphasizes change and hope amidst economic crisis and dissatisfaction with the Iraq War.",                    2008,
                    new PartyDetails("obama", "Barack Obama", Party.Democrat, new List<string>() {"Change", "End the Iraq War"}, .53f),
                    new PartyDetails("mccain", "John McCain", Party.Republican, new List<string>() {"Stay the Course in Iraq", "Tax Cuts"}, .46f),
                    Party.Democrat,
        "Obama won with a 7.3% margin in the popular vote, driven by his message of change and hope during a time of crisis."                )
            },
            {
                2004,
                new ElectionDetails(
            "The focus of the 2004 election is on the War on Terror and national security, with Bush defending his policies and Kerry criticizing them.",                    2004,
                    new PartyDetails("kerry", "John Kerry", Party.Democrat, new List<string>() {"End the Iraq War", "Healthcare Reform"}, .48f),
                    new PartyDetails("bush", "George W. Bush", Party.Republican, new List<string>() {"War on Terror", "Tax Cuts"}, .51f),
                    Party.Republican,
                    "George W. Bush's victory was secured by his strong stance on national security and the War on Terror, which resonated with many voters concerned about safety and stability. His campaign effectively used these issues to maintain a 2.5% margin in the popular vote and secure 286 electoral votes."
                )
            },
            {
                2000,
                new ElectionDetails(
            "The election is dominated by a close and controversial recount in Florida, raising questions about ballot design and voting procedures.",                    2000,
                    new PartyDetails("gore", "Al Gore", Party.Democrat, new List<string>() {"Environmental Protection", "Social Security Reform"}, .48f),
                    new PartyDetails("bush", "George W. Bush", Party.Republican, new List<string>() {"Tax Cuts", "Education Reform"}, .48f),
                    Party.Republican,
                "Bush won with a narrow 0.5% popular vote margin, driven by a contentious Florida recount and vote count scrutiny.")
            },
            {
                1996,
                new ElectionDetails(
            "Clinton's re-election campaign benefits from a strong economy, while Dole struggles to offer a compelling alternative.",                    1996,
                    new PartyDetails("billclinton", "Bill Clinton", Party.Democrat, new List<string>() {"Economic Prosperity", "Welfare Reform"}, .49f),
                    new PartyDetails("dole", "Bob Dole", Party.Republican, new List<string>() {"Tax Cuts", "Balanced Budget"}, .41f),
                    Party.Democrat,
        "Clinton’s re-election was marked by an 8.5% margin in the popular vote, driven by economic prosperity and job growth."                )
            },
            {
                1992,
                new ElectionDetails(
            "Clinton challenges Bush on economic issues and political direction, with Perot's third-party bid adding complexity.",                    1992,
                    new PartyDetails("billclinton", "Bill Clinton", Party.Democrat, new List<string>() {"Economic Growth", "Healthcare Reform"}, .43f),
                    new PartyDetails("hwbush", "George H. W. Bush", Party.Republican, new List<string>() {"Foreign Policy Experience", "Tax Increases"}, .37f),
                    Party.Democrat,
        "Clinton’s win, with a 5.6% margin in the popular vote, was fueled by economic dissatisfaction and a strong challenge from Perot."                )
            },
            {
                1988,
                new ElectionDetails(
            "Bush campaigns on Reagan's legacy and national security, contrasting with Dukakis’s domestic reform proposals.",                    1988,
                    new PartyDetails("dukakis", "Michael Dukakis", Party.Democrat, new List<string>() {"Healthcare Reform", "Education Investment"}, .46f),
                    new PartyDetails("hwbush", "George H. W. Bush", Party.Republican, new List<string>() {"Continue Reagan's Policies", "Strengthen Defense"}, .53f),
                    Party.Republican,
        "Bush secured a 7.7% margin in the popular vote, leveraging Reagan’s popularity and economic stability."                )
            },
            {
                1984,
                new ElectionDetails(
            "Reagan's campaign benefits from strong economic growth and foreign policy successes, contrasting with Mondale's calls for reform.",                    1984,
                    new PartyDetails("reagan", "Ronald Reagan", Party.Republican, new List<string>() {"Economic Growth", "Strong Defense"}, .59f),
                    new PartyDetails("mondale", "Walter Mondale", Party.Democrat, new List<string>() {"Social Security Reform", "Tax Increases"}, .41f),
                    Party.Republican,
        "Reagan’s landslide victory, with an 18.2% margin in the popular vote, was driven by economic prosperity and strong foreign policy."                )
            },
            {
                1980,
                new ElectionDetails(
            "Reagan's campaign focuses on economic discontent and the Iran hostage crisis, offering a new direction for the country.",                    1980,
                    new PartyDetails("reagan", "Ronald Reagan", Party.Republican, new List<string>() {"Economic Revival", "Strong Defense"}, .55f),
                    new PartyDetails("carter", "Jimmy Carter", Party.Democrat, new List<string>() {"Energy Policy", "Inflation Control"}, .45f),
                    Party.Republican,
        "Reagan won with a 10.8% margin in the popular vote, driven by his economic revival promises and strong national security stance."                )
            },
            {
                1976,
                new ElectionDetails(
            "Carter runs as an outsider amid dissatisfaction with the establishment and economic challenges.",                    1976,
                    new PartyDetails("carter", "Jimmy Carter", Party.Democrat, new List<string>() {"Restore Integrity", "Economic Reform"}, .51f),
                    new PartyDetails("ford", "Gerald Ford", Party.Republican, new List<string>() {"Continuation of Nixon Policies", "Economic Stability"}, .49f),
                    Party.Democrat,
        "Carter won with a 2.1% margin in the popular vote, appealing as a fresh alternative to Nixon’s administration."                )
            },
            {
                1972,
                new ElectionDetails(
            "Nixon leverages stability and foreign policy successes, while McGovern's campaign faces internal divisions and limited appeal.",                    1972,
                    new PartyDetails("nixon", "Richard Nixon", Party.Republican, new List<string>() {"Vietnam War Diplomacy", "Economic Prosperity"}, .62f),
                    new PartyDetails("mcgovern", "George McGovern", Party.Democrat, new List<string>() {"End the Vietnam War", "Social Reform"}, .38f),
                    Party.Republican,
        "Nixon’s landslide victory included a 23.0% margin in the popular vote, bolstered by his foreign policy successes and strong economic performance."                )
            },
            {
                1968,
                new ElectionDetails(
            "Nixon's campaign focuses on law and order and Vietnam policy, while Humphrey struggles with social unrest and a divided party.",                    1968,
                    new PartyDetails("nixon", "Richard Nixon", Party.Republican, new List<string>() {"Law and Order", "Vietnam War Policy"}, .5f),
                    new PartyDetails("humphrey", "Hubert Humphrey", Party.Democrat, new List<string>() {"Great Society Continuation", "Vietnam War End"}, .5f),
                    Party.Republican,
        "Nixon won with a 0.7% margin in the popular vote, driven by his strong stance on law and order and Vietnam policies."                )
            },
            {
                1964,
                new ElectionDetails(
            "Johnson highlights his Great Society successes and civil rights achievements, while Goldwater's conservative platform struggles.",                    1964,
                    new PartyDetails("johnson", "Lyndon B. Johnson", Party.Democrat, new List<string>() {"Great Society", "Civil Rights"}, .62f),
                    new PartyDetails("goldwater", "Barry Goldwater", Party.Republican, new List<string>() {"Conservative Principles", "Limited Government"}, .38f),
                    Party.Democrat,
        "Johnson won with a 22.6% margin in the popular vote, driven by his Great Society programs and civil rights achievements."                )
            },
            {
                1960,
                new ElectionDetails(
            "Kennedy emphasizes Cold War issues and domestic reform, while Nixon struggles with emerging campaign dynamics.",                    1960,
                    new PartyDetails("kennedy", "John F. Kennedy", Party.Democrat, new List<string>() {"New Frontier", "Civil Rights"}, .50f),
                    new PartyDetails("nixon", "Richard Nixon", Party.Republican, new List<string>() {"Experience in Government", "Foreign Policy"}, .50f),
                    Party.Democrat,
        "Kennedy won with a 0.2% margin in the popular vote, aided by effective use of televised debates and his New Frontier promise."                )
            }


        };
    }

    public int GetIndexFromElectionYear()
    {
        return (ElectionYear - startElectionYear) / 4;
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
        for (int i = startElectionYear; i <= endElectionYear; i += 4)
        {
            int electionYear = i;
            FirebaseManager.Election election;

            election = FindElection(elections, i) ?? new FirebaseManager.Election(i, 0, 0, (int)Swag.None, (int)Swag.None);

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
            demTopFill.GetComponent<Image>().color = DemColor;
            demBotFill.GetComponent<Image>().color = DemColor;
            //}

            Transform repTopFill = ElectionEntry.transform.Find("RepFillTop");
            Transform repBotFill = ElectionEntry.transform.Find("RepFillBottom");

            RectTransform repTopFillRect = repTopFill.GetComponent<RectTransform>();
            repTopFillRect.sizeDelta = new Vector2(repWidth, repTopFillRect.sizeDelta.y);

            RectTransform repBotFillRect = repBotFill.GetComponent<RectTransform>();
            repBotFillRect.sizeDelta = new Vector2(repWidth, repBotFillRect.sizeDelta.y);

            repTopFill.GetComponent<Image>().color = RepColor;
            repBotFill.GetComponent<Image>().color = RepColor;
            //}

            Transform DemProfile = ElectionEntry.transform.Find("DemProfile");
            Transform RepProfile = ElectionEntry.transform.Find("RepProfile");

            if (election.demVotes > election.repVotes)
                Instantiate(CrownPrefab, DemProfile);
            else if (election.repVotes > election.demVotes)
                Instantiate(CrownPrefab, RepProfile);

            //// TODO: REMOVE AFTER TESTS
            //election.demSwag = (int)Swag.Bandana;
            //election.repSwag = (int)Swag.Bandana;

            if (election.demSwag != (int) Swag.None)
            {
                GameObject DemSwagPrefab = CandidateSwagPrefabDict[GetCandidateKeyFromPartyYear(i, Party.Democrat)];
                ShowSwag(DemSwagPrefab, DemProfile, (Swag)election.demSwag);
            }

            if (election.repSwag != (int)Swag.None)
            {
                GameObject RepSwagPrefab = CandidateSwagPrefabDict[GetCandidateKeyFromPartyYear(i, Party.Republican)];
                ShowSwag(RepSwagPrefab, RepProfile, (Swag)election.repSwag);
            }
        }

        yield return null;
    }

    public void ShowSwag(GameObject prefabGo, Transform ProfileTransform, Swag swag)
    {
        GameObject SwagParent = Instantiate(prefabGo, ProfileTransform);

        Transform SwagTransform = null;

        if (swag == Swag.Bandana)
        {
            SwagTransform = SwagParent.transform.Find("BandanaContainer");
        }
        else if (swag == Swag.BlueLazer)
        {
            SwagTransform = SwagParent.transform.Find("BlueLazers");
        }
        else if (swag == Swag.Glasses)
        {
            SwagTransform = SwagParent.transform.Find("GlassesContainer");
        }
        else if (swag == Swag.Moustache)
        {
            SwagTransform = SwagParent.transform.Find("MoustacheContainer");
        }
        else if (swag == Swag.RedLazer)
        {
            SwagTransform = SwagParent.transform.Find("DeathLazerz");
        }

        foreach (Transform child in SwagParent.transform)
        {
            child.gameObject.SetActive(child == SwagTransform);
        }
    }
     
    public void QuitGame()
    {
        Application.Quit();
    }

    public string GetCandidateKeyFromPartyYear(int electionYear, Party party)
    {
        ElectionDetails details = ElectionMap[electionYear];

        return party == Party.Democrat ? details.GetDemDetails().picFile : details.GetRepubDetails().picFile;
    }

    public ElectionDetails GetDetailsFromPartyYear()
    {
        if (ElectionMap == null)
        {
            return null;
        }
        if (ElectionMap.ContainsKey(ElectionYear))
            return ElectionMap[ElectionYear];
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
        //// TODO: REMOVEA FTER TESTING
        //if (scene.name != Consts.ResultsScene)
        //    SceneManager.LoadScene(Consts.ResultsScene);

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
        {
            string[] candidateNames = partyDetails.candidate.Split(" ");
            nameText.text = candidateNames[candidateNames.Length - 1] + "'s\n Turn";
        }
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

    void OnDestroy()
    {
        // Unsubscribe from the scene loaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

