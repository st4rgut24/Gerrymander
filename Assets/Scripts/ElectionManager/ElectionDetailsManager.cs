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

    public Dictionary<int, ElectionDetails> ElectionMap;

    public List<GameObject> PartyObjects;

    const float boxOpeningWidth = 300;
    const float boxOpeningHeight = 381;

    private void Awake()
    {
        PartyObjects = new List<GameObject>();

        ElectionMap = new Dictionary<int, ElectionDetails>()
        {
            {
                2020,
                new ElectionDetails(
                    "Having survived an assassination attempt, former president Trump rematches Biden in a tense election.",
                    2020,
                    new PartyDetails("biden", "Joe Biden", Party.Democrat, new List<string>() {"Beat Trump", "Protect Democracy"}, .5f),
                    new PartyDetails("trump", "Donald Trump", Party.Republican, new List<string>() {"MAGA"}, .5f)
                )
            },
            {
                2016,
                new ElectionDetails(
                    "In a shocking outcome, Donald Trump wins against Hillary Clinton, marking a dramatic shift in American politics.",
                    2016,
                    new PartyDetails("clinton", "Hillary Clinton", Party.Democrat, new List<string>() {"Clinton Foundation Experience", "Expand Healthcare"}, .48f),
                    new PartyDetails("trump", "Donald Trump", Party.Republican, new List<string>() {"Make America Great Again", "Immigration Reform"}, .46f)
                )
            },
            {
                2012,
                new ElectionDetails(
                    "Barack Obama secures a second term against Mitt Romney amid a struggling economy and ongoing foreign conflicts.",
                    2012,
                    new PartyDetails("obama", "Barack Obama", Party.Democrat, new List<string>() {"Affordable Care Act", "Economic Recovery"}, .51f),
                    new PartyDetails("romney", "Mitt Romney", Party.Republican, new List<string>() {"Economic Reform", "Reduce Government Spending"}, .47f)
                )
            },
            {
                2008,
                new ElectionDetails(
                    "Barack Obama makes history as the first African American president, defeating John McCain in a historic election.",
                    2008,
                    new PartyDetails("obama", "Barack Obama", Party.Democrat, new List<string>() {"Change", "End the Iraq War"}, .53f),
                    new PartyDetails("mccain", "John McCain", Party.Republican, new List<string>() {"Stay the Course in Iraq", "Tax Cuts"}, .46f)
                )
            },
            {
                2004,
                new ElectionDetails(
                    "Incumbent President George W. Bush defeats John Kerry in a close race dominated by foreign policy and national security issues.",
                    2004,
                    new PartyDetails("kerry", "John Kerry", Party.Democrat, new List<string>() {"End the Iraq War", "Healthcare Reform"}, .48f),
                    new PartyDetails("bush", "George W. Bush", Party.Republican, new List<string>() {"War on Terror", "Tax Cuts"}, .51f)
                )
            },
            {
                2000,
                new ElectionDetails(
                    "George W. Bush wins a contentious election against Al Gore, which is decided by a narrow margin in Florida.",
                    2000,
                    new PartyDetails("gore", "Al Gore", Party.Democrat, new List<string>() {"Environmental Protection", "Social Security Reform"}, .48f),
                    new PartyDetails("bush", "George W. Bush", Party.Republican, new List<string>() {"Tax Cuts", "Education Reform"}, .48f)
                )
            },
            {
                1996,
                new ElectionDetails(
                    "Bill Clinton wins re-election against Bob Dole, as the economy continues to improve and the political landscape shifts.",
                    1996,
                    new PartyDetails("clinton", "Bill Clinton", Party.Democrat, new List<string>() {"Economic Prosperity", "Welfare Reform"}, .49f),
                    new PartyDetails("dole", "Bob Dole", Party.Republican, new List<string>() {"Tax Cuts", "Balanced Budget"}, .41f)
                )
            },
            {
                1992,
                new ElectionDetails(
                    "Bill Clinton defeats incumbent George H. W. Bush, capitalizing on economic dissatisfaction and his appeal as a 'new Democrat'.",
                    1992,
                    new PartyDetails("clinton", "Bill Clinton", Party.Democrat, new List<string>() {"Economic Growth", "Healthcare Reform"}, .43f),
                    new PartyDetails("bush", "George H. W. Bush", Party.Republican, new List<string>() {"Foreign Policy Experience", "Tax Increases"}, .37f)
                )
            },
            {
                1988,
                new ElectionDetails(
                    "George H. W. Bush wins against Michael Dukakis, emphasizing his experience and promising to continue Reagan's policies.",
                    1988,
                    new PartyDetails("dukakis", "Michael Dukakis", Party.Democrat, new List<string>() {"Healthcare Reform", "Education Investment"}, .46f),
                    new PartyDetails("bush", "George H. W. Bush", Party.Republican, new List<string>() {"Continue Reagan's Policies", "Strengthen Defense"}, .53f)
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

    private IEnumerator StartGame()
    {
        float start = Time.time;
        float maxDuration = 3;

        while (Time.time - start < maxDuration)
        {
            yield return null;
        }

        InitGame();
    }

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
        StartCoroutine(StartGame());
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
