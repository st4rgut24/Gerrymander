using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ElectionDetailsManager : Singleton<ElectionDetailsManager>
{
    [SerializeField]
    private Toggle DemocratCheckbox;

    [SerializeField]
    private Toggle RepublicanCheckbox;

    [SerializeField]
    private Transform DemocratContainer;

    [SerializeField]
    private Transform RepublicanContainer;

    [SerializeField]
    private GameObject DemocraticChip;

    [SerializeField]
    private GameObject RepublicanChip;

    [SerializeField]
    private RectTransform DemVoteContainer;

    [SerializeField]
    private RectTransform RepVoteContainer;

    [SerializeField]
    private RectTransform BoxOpening;

    [SerializeField]
    private GameObject Funnel;

    bool IsFunneling = false;

    public ElectionDetails details;
    public const int populationSize = 50;
    public float DemPartyPct;

    Party PlayerParty = Party.None;

    public static Dictionary<int, ElectionDetails> ElectionMap;

    public List<GameObject> PartyObjects;

    const float boxOpeningWidth = 300;
    const float boxOpeningHeight = 381;

    private void Awake()
    {
        PartyObjects = new List<GameObject>();

        ElectionMap = new Dictionary<int, ElectionDetails>()
        {
            {
                2024,
                new ElectionDetails(
                    "Having survived an assassination attempt, former president Trump rematches Biden in a tense election.",
                    2024,
                    new PartyDetails("kamala", "Kamala Harris", Party.Democrat, new List<string>() {"Beat Trump", "Protect Democracy"}, .5f),
                    new PartyDetails("trump", "Donald Trump", Party.Republican, new List<string>() {"MAGA"}, .5f),
                    Party.None,
                    "TBD"
                )
            },
            {
                2020,
                new ElectionDetails(
                    "President Trump plays Biden in a tense election.",
                    2020,
                    new PartyDetails("biden", "Joe Biden", Party.Democrat, new List<string>() {"Beat Trump", "Protect Democracy"}, .5f),
                    new PartyDetails("trump", "Donald Trump", Party.Republican, new List<string>() {"MAGA"}, .5f),
                    Party.Democrat,
                    "Joe Biden won the presidency with a margin of 4.5% in the popular vote and secured 306 electoral votes, compared to Donald Trump's 232 electoral votes."
                )
            },
            {
                2016,
                new ElectionDetails(
                    "In a shocking outcome, Donald Trump wins against Hillary Clinton, marking a dramatic shift in American politics.",
                    2016,
                    new PartyDetails("clinton", "Hillary Clinton", Party.Democrat, new List<string>() {"Clinton Foundation Experience", "Expand Healthcare"}, .48f),
                    new PartyDetails("trump", "Donald Trump", Party.Republican, new List<string>() {"Make America Great Again", "Immigration Reform"}, .46f),
                    Party.Republican,
                    "Donald Trump won the presidency by securing 306 electoral votes to Hillary Clinton's 227, despite losing the popular vote by 2.1%."
                )
            },
            {
                2012,
                new ElectionDetails(
                    "Barack Obama secures a second term against Mitt Romney amid a struggling economy and ongoing foreign conflicts.",
                    2012,
                    new PartyDetails("obama", "Barack Obama", Party.Democrat, new List<string>() {"Affordable Care Act", "Economic Recovery"}, .51f),
                    new PartyDetails("romney", "Mitt Romney", Party.Republican, new List<string>() {"Economic Reform", "Reduce Government Spending"}, .47f),
                    Party.Democrat,
                    "Barack Obama won re-election with a 3.9% margin in the popular vote and captured 332 electoral votes, defeating Mitt Romney, who received 206 electoral votes."
                )
            },
            {
                2008,
                new ElectionDetails(
                    "Barack Obama makes history as the first African American president, defeating John McCain in a historic election.",
                    2008,
                    new PartyDetails("obama", "Barack Obama", Party.Democrat, new List<string>() {"Change", "End the Iraq War"}, .53f),
                    new PartyDetails("mccain", "John McCain", Party.Republican, new List<string>() {"Stay the Course in Iraq", "Tax Cuts"}, .46f),
                    Party.Democrat,
                    "Barack Obama won the presidency with a substantial 7.3% margin in the popular vote and received 365 electoral votes, compared to John McCain's 173 electoral votes."
                )
            },
            {
                2004,
                new ElectionDetails(
                    "Incumbent President George W. Bush defeats John Kerry in a close race dominated by foreign policy and national security issues.",
                    2004,
                    new PartyDetails("kerry", "John Kerry", Party.Democrat, new List<string>() {"End the Iraq War", "Healthcare Reform"}, .48f),
                    new PartyDetails("bush", "George W. Bush", Party.Republican, new List<string>() {"War on Terror", "Tax Cuts"}, .51f),
                    Party.Republican,
                    "George W. Bush secured his re-election with a 2.5% margin in the popular vote and 286 electoral votes, defeating John Kerry, who received 251 electoral votes."
                )
            },
            {
                2000,
                new ElectionDetails(
                    "George W. Bush wins a contentious election against Al Gore, which is decided by a narrow margin in Florida.",
                    2000,
                    new PartyDetails("gore", "Al Gore", Party.Democrat, new List<string>() {"Environmental Protection", "Social Security Reform"}, .48f),
                    new PartyDetails("bush", "George W. Bush", Party.Republican, new List<string>() {"Tax Cuts", "Education Reform"}, .48f),
                    Party.Republican,
                    "George W. Bush won the presidency by narrowly securing 271 electoral votes to Al Gore's 266, despite losing the popular vote by 0.5%."
                )
            },
            {
                1996,
                new ElectionDetails(
                    "Bill Clinton wins re-election against Bob Dole, as the economy continues to improve and the political landscape shifts.",
                    1996,
                    new PartyDetails("billclinton", "Bill Clinton", Party.Democrat, new List<string>() {"Economic Prosperity", "Welfare Reform"}, .49f),
                    new PartyDetails("dole", "Bob Dole", Party.Republican, new List<string>() {"Tax Cuts", "Balanced Budget"}, .41f),
                    Party.Democrat,
                    "Bill Clinton won re-election with an 8.5% margin in the popular vote and a commanding 379 electoral votes, compared to Bob Dole's 159 electoral votes."
                )
            },
            {
                1992,
                new ElectionDetails(
                    "Bill Clinton defeats incumbent George H. W. Bush, capitalizing on economic dissatisfaction and his appeal as a 'new Democrat'.",
                    1992,
                    new PartyDetails("billclinton", "Bill Clinton", Party.Democrat, new List<string>() {"Economic Growth", "Healthcare Reform"}, .43f),
                    new PartyDetails("hwbush", "George H. W. Bush", Party.Republican, new List<string>() {"Foreign Policy Experience", "Tax Increases"}, .37f),
                    Party.Democrat,
                    "Bill Clinton won the presidency with a 5.6% margin in the popular vote and 370 electoral votes, defeating incumbent George H.W. Bush, who received 168 electoral votes."
                )
            },
            {
                1988,
                new ElectionDetails(
                    "George H. W. Bush wins against Michael Dukakis, emphasizing his experience and promising to continue Reagan's policies.",
                    1988,
                    new PartyDetails("dukakis", "Michael Dukakis", Party.Democrat, new List<string>() {"Healthcare Reform", "Education Investment"}, .46f),
                    new PartyDetails("hwbush", "George H. W. Bush", Party.Republican, new List<string>() {"Continue Reagan's Policies", "Strengthen Defense"}, .53f),
                    Party.Republican,
                    "George H.W. Bush won the presidency with a 7.7% margin in the popular vote and a decisive 426 electoral votes, compared to Michael Dukakis's 111 electoral votes."
                )
            },
            {
                1984,
                new ElectionDetails(
                    "Ronald Reagan achieves a landslide victory over Walter Mondale, with the country enjoying economic prosperity and a strong foreign policy stance.",
                    1984,
                    new PartyDetails("reagan", "Ronald Reagan", Party.Republican, new List<string>() {"Economic Growth", "Strong Defense"}, .59f),
                    new PartyDetails("mondale", "Walter Mondale", Party.Democrat, new List<string>() {"Social Security Reform", "Tax Increases"}, .41f),
                    Party.Republican,
                    "Ronald Reagan won re-election with an 18.2% margin in the popular vote and a commanding 525 electoral votes, compared to Walter Mondale's 13 electoral votes."
                )
            },
            {
                1980,
                new ElectionDetails(
                    "Ronald Reagan defeats incumbent Jimmy Carter amid economic troubles and the Iran hostage crisis, promising a new direction for the country.",
                    1980,
                    new PartyDetails("reagan", "Ronald Reagan", Party.Republican, new List<string>() {"Economic Revival", "Strong Defense"}, .55f),
                    new PartyDetails("carter", "Jimmy Carter", Party.Democrat, new List<string>() {"Energy Policy", "Inflation Control"}, .45f),
                    Party.Republican,
                    "Ronald Reagan won the presidency with a 10.8% margin in the popular vote and 489 electoral votes, defeating Jimmy Carter, who received 49 electoral votes."
                )
            },
            {
                1976,
                new ElectionDetails(
                    "Jimmy Carter wins a narrow victory over Gerald Ford, focusing on his outsider status and a promise to restore honesty to the White House.",
                    1976,
                    new PartyDetails("carter", "Jimmy Carter", Party.Democrat, new List<string>() {"Honesty in Government", "Energy Policy"}, .51f),
                    new PartyDetails("ford", "Gerald Ford", Party.Republican, new List<string>() {"Economic Stability", "Vietnam War Experience"}, .49f),
                    Party.Democrat,
                    "Jimmy Carter won the presidency with a 2.1% margin in the popular vote and 297 electoral votes, compared to Gerald Ford's 240 electoral votes."
                )
            },
            {
                1972,
                new ElectionDetails(
                    "Richard Nixon secures a landslide victory over George McGovern, with the campaign heavily influenced by the ongoing Vietnam War and Nixon's strong foreign policy stance.",
                    1972,
                    new PartyDetails("nixon", "Richard Nixon", Party.Republican, new List<string>() {"Vietnam War", "Detente with China"}, .62f),
                    new PartyDetails("mcgovern", "George McGovern", Party.Democrat, new List<string>() {"End the Vietnam War", "Social Reforms"}, .38f),
                    Party.Republican,
                    "Richard Nixon won re-election with a 23.2% margin in the popular vote and 520 electoral votes, compared to George McGovern's 17 electoral votes."
                )
            },
            {
                1968,
                new ElectionDetails(
                    "Richard Nixon wins the presidency in a close race against Hubert Humphrey, capitalizing on a strong anti-establishment sentiment and concerns over the Vietnam War.",
                    1968,
                    new PartyDetails("nixon", "Richard Nixon", Party.Republican, new List<string>() {"Law and Order", "Vietnam War"}, .5f),
                    new PartyDetails("humphrey", "Hubert Humphrey", Party.Democrat, new List<string>() {"Great Society Programs", "End of Vietnam War"}, .5f),
                    Party.Republican,
                    "Richard Nixon won the presidency with a 0.7% margin in the popular vote and 301 electoral votes, compared to Hubert Humphrey's 191 electoral votes."
                )
            },
            {
                1964,
                new ElectionDetails(
                    "Lyndon B. Johnson wins a decisive victory over Barry Goldwater, riding a wave of support for his Great Society programs and civil rights legislation.",
                    1964,
                    new PartyDetails("johnson", "Lyndon B. Johnson", Party.Democrat, new List<string>() {"Great Society", "Civil Rights"}, .62f),
                    new PartyDetails("goldwater", "Barry Goldwater", Party.Republican, new List<string>() {"Conservative Principles", "Limited Government"}, .38f),
                    Party.Democrat,
                    "Lyndon B. Johnson won re-election with a 22.6% margin in the popular vote and 486 electoral votes, compared to Barry Goldwater's 52 electoral votes."
                )
            },
            {
                1960,
                new ElectionDetails(
                    "John F. Kennedy narrowly defeats Richard Nixon, with the election focusing on Cold War issues, domestic reforms, and the televised debates.",
                    1960,
                    new PartyDetails("kennedy", "John F. Kennedy", Party.Democrat, new List<string>() {"New Frontier", "Civil Rights"}, .5f),
                    new PartyDetails("nixon", "Richard Nixon", Party.Republican, new List<string>() {"Experience in Government", "Foreign Policy"}, .5f),
                    Party.Democrat,
                    "John F. Kennedy won the presidency with a narrow 0.2% margin in the popular vote and 303 electoral votes, compared to Richard Nixon's 219 electoral votes."
                )
            }

        };  

    }

    // Start is called before the first frame update
    void Start()
    {
        DemocratCheckbox.isOn = false;
        RepublicanCheckbox.isOn = false;

        DemocratCheckbox.onValueChanged.AddListener(delegate
        {
            ToggleDemocratValueChanged(DemocratCheckbox);
        });

        RepublicanCheckbox.onValueChanged.AddListener(delegate
        {
            ToggleRepublicanValueChanged(RepublicanCheckbox);
        });

        details = ElectionMap[GameManager.Instance.ElectionYear];

        InitOverviewUI(details.year, details.summary);

        InitPartyUI(details, DemocratContainer, DemocraticChip, details.GetDemDetails(), details.GetDemSlogan());
        InitPartyUI(details, RepublicanContainer, RepublicanChip, details.GetRepubDetails(), details.GetRepSlogan());

        GameManager.Instance.demPartyDetails = details.GetDemDetails();
        GameManager.Instance.repPartyDetils = details.GetRepubDetails();
    }

    public void InitGame()
    {
        GameManager.Instance.LoadGameScene(details.GetDemPartyPct(), details.GetRepPartyPct(), PlayerParty);
    }

    void CreateFunnel()
    {
        Rect boxRect = RectTransformToScreenSpace(BoxOpening);
        Vector2 openingHorWallBegin = new Vector2(boxRect.x, boxRect.y + boxRect.height);
        Vector2 openingHorWallEnd = new Vector2(boxRect.x + boxRect.width, boxRect.y + boxRect.height);
        //EdgeCollider2D openingEdgeCollider = Funnel.AddComponent<EdgeCollider2D>();
        //openingEdgeCollider.points = new Vector2[] { openingHorWallBegin, openingHorWallEnd };
        //openingEdgeCollider.isTrigger = true;

        // now create the edge collider sides (2 on each side)
        Rect DemRect = RectTransformToScreenSpace(DemVoteContainer);
        EdgeCollider2D DemWallCollider = CreateVertWallForVoteContainer(DemRect, 0);
        EdgeCollider2D slantDemCollider = Funnel.AddComponent<EdgeCollider2D>();
        slantDemCollider.points = new Vector2[] { DemWallCollider.points[1], openingHorWallBegin };

        Rect RepRect = RectTransformToScreenSpace(RepVoteContainer);
        EdgeCollider2D RepWallCollider = CreateVertWallForVoteContainer(RepRect, RepRect.width);
        EdgeCollider2D slantRepCollider = Funnel.AddComponent<EdgeCollider2D>();
        slantRepCollider.points = new Vector2[] { RepWallCollider.points[1], openingHorWallEnd };
    }

    public EdgeCollider2D CreateVertWallForVoteContainer(Rect rect, float xOffset)
    {
        Vector2 demVertWallBegin = new Vector2(rect.x + xOffset, rect.y + rect.size.y);
        Vector2 demVertWallEnd = new Vector2(rect.x + xOffset, rect.y);
        EdgeCollider2D wallCollider = Funnel.AddComponent<EdgeCollider2D>();
        wallCollider.points = new Vector2[] { demVertWallBegin, demVertWallEnd };

        return wallCollider;
    }

    //private IEnumerator StartGame()
    //{
    //    float start = Time.time;
    //    float maxDuration = 8;

    //    while (Time.time - start < maxDuration)
    //    {
    //        yield return null;
    //    }

    //    InitGame();
    //}

    public static Rect RectTransformToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        return new Rect((Vector2)transform.position - (size * transform.pivot), size);
    }

    void FunnelPartyTokens()
    {
        IsFunneling = true;
        CreateFunnel();
        PartyObjects.ForEach((partyGo) =>
        {
            partyGo.GetComponent<Rigidbody2D>().isKinematic = false;
        });
        //StartCoroutine(StartGame());
    }

    void ToggleRepublicanValueChanged(Toggle change)
    {
        if (IsFunneling)
        {
            return;
        }

        if (change.isOn && DemocratCheckbox.isOn)
            DemocratCheckbox.isOn = false;

        if (change.isOn)
        {
            PlayerParty = Party.Republican;
            GameManager.Instance.PlayerParty = PlayerParty;
        }
        else
        {
            PlayerParty = Party.None;
        }

        FunnelPartyTokens();
    }

    void ToggleDemocratValueChanged(Toggle change)
    {
        if (IsFunneling)
        {
            return;
        }

        if (change.isOn && RepublicanCheckbox.isOn)
            RepublicanCheckbox.isOn = false;

        if (change.isOn)
        {
            PlayerParty = Party.Democrat;
            GameManager.Instance.PlayerParty = PlayerParty;
        }
        else
        {
            PlayerParty = Party.None;
        }

        FunnelPartyTokens();
    }

    public void InitOverviewUI(int electionYear, string summary)
    {
        TextMeshProUGUI ElectionYearText = GameObject.Find(Consts.ElectionHeader).GetComponent<TextMeshProUGUI>();
        ElectionYearText.text = electionYear.ToString() + " Election";

        //TextMeshProUGUI ElectionSummaryText = GameObject.Find(Consts.Summary).GetComponent<TextMeshProUGUI>();
        //ElectionSummaryText.text = summary;
    }

    public void InitPartyUI(ElectionDetails details, Transform PartyContainer, GameObject partyPrefab, PartyDetails partyDetails, List<string> slogans)
    {
        Transform VoteContainer = PartyContainer.Find(Consts.VoteContainer);
        InitVoteContainer(VoteContainer, partyDetails.popVoteCount, partyPrefab);

        TextMeshProUGUI SloganText = PartyContainer.Find(Consts.Slogans).GetComponent<TextMeshProUGUI>();

        string sloganText = "";

        slogans.ForEach((slogan) =>
        {
            sloganText = sloganText + "• " + slogan + "\n";
        });

        SloganText.text = sloganText;


        TextMeshProUGUI VoteText = PartyContainer.Find(Consts.VoteText).GetComponent<TextMeshProUGUI>();
        VoteText.text = partyDetails.popVoteCount.ToString() + " Voters";

        Image partyPic = PartyContainer.Find(Consts.Picture).GetComponent<Image>();
        Sprite sprite = Resources.Load<Sprite>(partyDetails.picFile);

        partyDetails.partySprite = sprite;

        partyPic.sprite = sprite;
    }

    public void InitVoteContainer(Transform VoteTransform, int popVoteCount, GameObject partyPrefab)
    {
        for (int i = 0; i < popVoteCount; i++)
        {
            GameObject partyGo = Instantiate(partyPrefab, VoteTransform);
            PartyObjects.Add(partyGo);
        }
    }

    bool IsFunnelOver()
    {
        foreach (GameObject PartyChip in PartyObjects)
        {
            RectTransform rect = PartyChip.GetComponent<RectTransform>();
            if (rect.anchoredPosition.y >= -Screen.height)
            {
                return false;
            }

        }
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsFunnelOver())
            InitGame();
    }
}
