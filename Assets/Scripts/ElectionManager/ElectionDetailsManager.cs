using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ElectionDetailsManager : Singleton<ElectionDetailsManager>
{
    [SerializeField]
    private TextMeshProUGUI SummaryText;

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
                    "Former President Donald Trump, having survived an assassination attempt, challenges President Joe Biden in an election characterized by heightened political tensions and concerns over security.",
                    2024,
                    new PartyDetails("kamala", "Kamala Harris", Party.Democrat, new List<string>() {"Beat Trump", "Protect Democracy"}, .5f),
                    new PartyDetails("trump", "Donald Trump", Party.Republican, new List<string>() {"MAGA"}, .5f),
                    Party.None,
                    "The election results are still pending."
                )
            },
            {
                2020,
                new ElectionDetails(
                    "The election is shaped by the global pandemic, economic downturn, and debates over handling COVID-19, with President Trump facing strong scrutiny over his response and Joe Biden presenting an alternative vision for recovery.",
                    2020,
                    new PartyDetails("biden", "Joe Biden", Party.Democrat, new List<string>() {"Beat Trump", "Protect Democracy"}, .5f),
                    new PartyDetails("trump", "Donald Trump", Party.Republican, new List<string>() {"MAGA"}, .5f),
                    Party.Democrat,
                    "Joe Biden won the presidency largely due to his strong response to the COVID-19 pandemic and a compelling vision for economic recovery. Biden's ability to connect with voters' frustrations over the handling of the crisis and his promise of a more unified approach helped him secure a decisive victory with a 4.5% margin in the popular vote and 306 electoral votes."
                )
            },
            {
                2016,
                new ElectionDetails(
                    "Donald Trump capitalizes on widespread discontent with the political establishment and promises significant change, while Hillary Clinton's campaign struggles to address concerns over trust and corruption.",
                    2016,
                    new PartyDetails("clinton", "Hillary Clinton", Party.Democrat, new List<string>() {"Clinton Foundation Experience", "Expand Healthcare"}, .48f),
                    new PartyDetails("trump", "Donald Trump", Party.Republican, new List<string>() {"Make America Great Again", "Immigration Reform"}, .46f),
                    Party.Republican,
                    "Donald Trump's victory is attributed to his ability to tap into voter dissatisfaction with the political elite and his compelling promises for change. His strong appeal in key battleground states and effective use of populist rhetoric helped him secure 306 electoral votes despite losing the popular vote by 2.1%."
                )
            },
            {
                2012,
                new ElectionDetails(
                    "Barack Obama's re-election bid is influenced by a recovering economy and ongoing foreign policy challenges, including the aftermath of the Arab Spring and the conflict in Libya.",
                    2012,
                    new PartyDetails("obama", "Barack Obama", Party.Democrat, new List<string>() {"Affordable Care Act", "Economic Recovery"}, .51f),
                    new PartyDetails("romney", "Mitt Romney", Party.Republican, new List<string>() {"Economic Reform", "Reduce Government Spending"}, .47f),
                    Party.Democrat,
                    "Barack Obama’s re-election was bolstered by the recovering economy and successful implementation of the Affordable Care Act. His campaign effectively highlighted the progress made under his presidency and his foreign policy achievements, allowing him to secure 332 electoral votes and a 3.9% margin in the popular vote."
                )
            },
            {
                2008,
                new ElectionDetails(
                    "Barack Obama runs on a platform of change and hope during a period of economic crisis and dissatisfaction with the Iraq War, while John McCain's campaign struggles to gain traction against these prevailing issues.",
                    2008,
                    new PartyDetails("obama", "Barack Obama", Party.Democrat, new List<string>() {"Change", "End the Iraq War"}, .53f),
                    new PartyDetails("mccain", "John McCain", Party.Republican, new List<string>() {"Stay the Course in Iraq", "Tax Cuts"}, .46f),
                    Party.Democrat,
                    "Barack Obama's message of change and hope resonated strongly with voters facing economic uncertainty and dissatisfaction with the Iraq War. His strong campaign and effective use of grassroots support led him to a substantial 7.3% margin in the popular vote and 365 electoral votes."
                )
            },
            {
                2004,
                new ElectionDetails(
                    "The 2004 election focuses on the ongoing War on Terror and national security, with President George W. Bush's handling of the Iraq War being a central issue against John Kerry's criticism of the administration's policies.",
                    2004,
                    new PartyDetails("kerry", "John Kerry", Party.Democrat, new List<string>() {"End the Iraq War", "Healthcare Reform"}, .48f),
                    new PartyDetails("bush", "George W. Bush", Party.Republican, new List<string>() {"War on Terror", "Tax Cuts"}, .51f),
                    Party.Republican,
                    "George W. Bush's victory was secured by his strong stance on national security and the War on Terror, which resonated with many voters concerned about safety and stability. His campaign effectively used these issues to maintain a 2.5% margin in the popular vote and secure 286 electoral votes."
                )
            },
            {
                2000,
                new ElectionDetails(
                    "The election is marked by controversy and a narrow focus on Florida's vote count, with issues of ballot design and voting procedures coming under intense scrutiny amidst a closely contested race.",
                    2000,
                    new PartyDetails("gore", "Al Gore", Party.Democrat, new List<string>() {"Environmental Protection", "Social Security Reform"}, .48f),
                    new PartyDetails("bush", "George W. Bush", Party.Republican, new List<string>() {"Tax Cuts", "Education Reform"}, .48f),
                    Party.Republican,
                    "George W. Bush won the presidency through a contentious and highly scrutinized recount in Florida, ultimately securing 271 electoral votes. Despite losing the popular vote by 0.5%, his narrow victory in Florida and effective campaign strategy proved decisive."
                )
            },
            {
                1996,
                new ElectionDetails(
                    "Bill Clinton's re-election campaign benefits from a growing economy and strong job numbers, while Bob Dole struggles to present a compelling alternative vision for the country.",
                    1996,
                    new PartyDetails("billclinton", "Bill Clinton", Party.Democrat, new List<string>() {"Economic Prosperity", "Welfare Reform"}, .49f),
                    new PartyDetails("dole", "Bob Dole", Party.Republican, new List<string>() {"Tax Cuts", "Balanced Budget"}, .41f),
                    Party.Democrat,
                    "Bill Clinton's success was fueled by the strong economic performance and job growth during his first term. His campaign effectively leveraged these economic gains, while Bob Dole's campaign failed to counter the incumbent's positive economic message, resulting in a commanding 8.5% margin in the popular vote and 379 electoral votes."
                )
            },
            {
                1992,
                new ElectionDetails(
                    "Bill Clinton seizes on economic dissatisfaction and the perception of a new political direction, effectively challenging incumbent George H. W. Bush and Ross Perot's third-party bid.",
                    1992,
                    new PartyDetails("billclinton", "Bill Clinton", Party.Democrat, new List<string>() {"Economic Growth", "Healthcare Reform"}, .43f),
                    new PartyDetails("hwbush", "George H. W. Bush", Party.Republican, new List<string>() {"Foreign Policy Experience", "Tax Increases"}, .37f),
                    Party.Democrat,
                    "Bill Clinton's victory was driven by widespread frustration with the economic recession and his promise of a new direction. His ability to connect with voters on economic issues, combined with Ross Perot's third-party challenge, led to a decisive 5.6% margin in the popular vote and 370 electoral votes."
                )
            },
            {
                1988,
                new ElectionDetails(
                    "George H. W. Bush's campaign focuses on his experience and the continuation of Ronald Reagan's policies, contrasting sharply with Michael Dukakis's proposals for domestic reforms.",
                    1988,
                    new PartyDetails("dukakis", "Michael Dukakis", Party.Democrat, new List<string>() {"Healthcare Reform", "Education Investment"}, .46f),
                    new PartyDetails("hwbush", "George H. W. Bush", Party.Republican, new List<string>() {"Continue Reagan's Policies", "Strengthen Defense"}, .53f),
                    Party.Republican,
                    "George H. W. Bush won by capitalizing on the positive economic climate and the popularity of Ronald Reagan's policies. His focus on national security and economic stability contrasted effectively with Michael Dukakis's proposals, securing him a 7.7% margin in the popular vote and 426 electoral votes."
                )
            },
            {
                1984,
                new ElectionDetails(
                    "Ronald Reagan's campaign is bolstered by strong economic growth and a robust foreign policy, including a successful stance against the Soviet Union, contrasting with Walter Mondale's calls for change.",
                    1984,
                    new PartyDetails("reagan", "Ronald Reagan", Party.Republican, new List<string>() {"Economic Growth", "Strong Defense"}, .59f),
                    new PartyDetails("mondale", "Walter Mondale", Party.Democrat, new List<string>() {"Social Security Reform", "Tax Increases"}, .41f),
                    Party.Republican,
                    "Ronald Reagan achieved a landslide victory thanks to his effective economic policies and strong foreign policy stance. His campaign successfully leveraged the country's prosperity and his leadership, leading to an 18.2% margin in the popular vote and 525 electoral votes."
                )
            },
            {
                1980,
                new ElectionDetails(
                    "Ronald Reagan's campaign capitalizes on widespread discontent with the economic situation and the Iran hostage crisis, promising a fresh start and a new direction for the country.",
                    1980,
                    new PartyDetails("reagan", "Ronald Reagan", Party.Republican, new List<string>() {"Economic Revival", "Strong Defense"}, .55f),
                    new PartyDetails("carter", "Jimmy Carter", Party.Democrat, new List<string>() {"Energy Policy", "Inflation Control"}, .45f),
                    Party.Republican,
                    "Ronald Reagan's victory was fueled by his promise of economic revival and a strong stance on national security, which resonated with voters frustrated by economic difficulties and the Iran hostage crisis. His campaign's focus on these issues helped him secure a 10.8% margin in the popular vote and 489 electoral votes."
                )
            },
            {
                1976,
                new ElectionDetails(
                    "Jimmy Carter capitalizes on widespread dissatisfaction with the political establishment and economic issues, positioning himself as an outsider who can restore integrity to the presidency.",
                    1976,
                    new PartyDetails("carter", "Jimmy Carter", Party.Democrat, new List<string>() {"Restore Integrity", "Economic Reform"}, .50f),
                    new PartyDetails("ford", "Gerald Ford", Party.Republican, new List<string>() {"Continuation of Nixon Policies", "Economic Stability"}, .48f),
                    Party.Democrat,
                    "Jimmy Carter's win was largely due to his appeal as a fresh alternative to the Nixon administration and his promises to restore honesty and address economic issues. His outsider status and focus on reform enabled him to secure a narrow 2.1% margin in the popular vote and 297 electoral votes."
                )
            },
            {
                1972,
                new ElectionDetails(
                    "Richard Nixon leverages the nation's relative stability and his successes in foreign policy to appeal to voters, while George McGovern's campaign struggles with internal party divisions and a lack of broad appeal.",
                    1972,
                    new PartyDetails("nixon", "Richard Nixon", Party.Republican, new List<string>() {"Vietnam War Diplomacy", "Economic Prosperity"}, .60f),
                    new PartyDetails("mcgovern", "George McGovern", Party.Democrat, new List<string>() {"End the Vietnam War", "Social Reform"}, .35f),
                    Party.Republican,
                    "Richard Nixon's overwhelming victory was driven by his successful foreign policy and the perception of stability during his presidency. McGovern's campaign failed to gain traction due to internal divisions and limited appeal, leading to Nixon's massive 23.2% margin in the popular vote and 520 electoral votes."
                )
            },
            {
                1968,
                new ElectionDetails(
                    "Richard Nixon capitalizes on the public's desire for law and order and a strong stance on the Vietnam War, while Hubert Humphrey's campaign struggles to address deepening social unrest and a divided Democratic Party.",
                    1968,
                    new PartyDetails("nixon", "Richard Nixon", Party.Republican, new List<string>() {"Law and Order", "Vietnam War Policy"}, .43f),
                    new PartyDetails("humphrey", "Hubert Humphrey", Party.Democrat, new List<string>() {"Great Society Continuation", "Vietnam War End"}, .42f),
                    Party.Republican,
                    "Richard Nixon's victory was fueled by his strong positioning on law and order and Vietnam War policies, which resonated with voters amidst widespread social unrest. Humphrey's campaign struggled to address these concerns effectively, resulting in Nixon's narrow 0.7% margin in the popular vote and 301 electoral votes."
                )
            },
            {
                1964,
                new ElectionDetails(
                    "Lyndon B. Johnson runs on the success of his Great Society programs and the civil rights achievements, while Barry Goldwater's more conservative platform struggles to gain widespread support.",
                    1964,
                    new PartyDetails("johnson", "Lyndon B. Johnson", Party.Democrat, new List<string>() {"Great Society", "Civil Rights"}, .62f),
                    new PartyDetails("goldwater", "Barry Goldwater", Party.Republican, new List<string>() {"Conservative Principles", "Limited Government"}, .38f),
                    Party.Democrat,
                    "Lyndon B. Johnson's landslide victory was driven by his successful Great Society initiatives and strong support for civil rights. Goldwater's conservative platform did not resonate as widely with voters, resulting in Johnson's substantial 22.6% margin in the popular vote and 486 electoral votes."
                )
            },
            {
                1960,
                new ElectionDetails(
                    "John F. Kennedy's campaign focuses on Cold War issues and domestic reform, capitalizing on the new era of televised debates, while Richard Nixon's campaign struggles to address these emerging concerns effectively.",
                    1960,
                    new PartyDetails("kennedy", "John F. Kennedy", Party.Democrat, new List<string>() {"New Frontier", "Civil Rights"}, .50f),
                    new PartyDetails("nixon", "Richard Nixon", Party.Republican, new List<string>() {"Experience in Government", "Foreign Policy"}, .50f),
                    Party.Democrat,
                    "John F. Kennedy's narrow victory was significantly influenced by his effective use of televised debates and his promise of a 'New Frontier' of progress. Despite Nixon's strong campaign, Kennedy's ability to address Cold War and domestic issues resonated more with voters, leading to a 0.2% margin in the popular vote and 303 electoral votes."
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
        SummaryText.text = summary;
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
