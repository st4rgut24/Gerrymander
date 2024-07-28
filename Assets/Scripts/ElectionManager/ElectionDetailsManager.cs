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

    Button playBtn;

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
        playBtn = GameObject.Find(Consts.DetailsPlayBtn).GetComponent<Button>();

        ElectionMap = new Dictionary<int, ElectionDetails>()
        {
            {
                2024,
                new ElectionDetails(
                    "Having survived an assasination attempt, former president Trump rematches Biden in a tense election.",
                    2024,
                    new PartyDetails("biden", "Joe Biden", Party.Democrat, new List<string>() {"Beat Trump", "Protect Democracy"}, .5f),
                    new PartyDetails("trump", "Donald Trump", Party.Republican, new List<string>() {"MAGA"}, .5f)
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
        GameManager.Instance.LoadGameScene(details.GetDemPartyPct(), PlayerParty);
    }

    void CreateFunnel()
    {
        Rect boxRect = RectTransformToScreenSpace(BoxOpening);
        Vector2 openingHorWallBegin = new Vector2(boxRect.x, boxRect.y + boxRect.height);
        Vector2 openingHorWallEnd = new Vector2(boxRect.x + boxRect.width, boxRect.y + boxRect.height);
        EdgeCollider2D openingEdgeCollider = Funnel.AddComponent<EdgeCollider2D>();
        openingEdgeCollider.points = new Vector2[] { openingHorWallBegin, openingHorWallEnd };

        // now create the edge collider sides (2 on each side)
        Rect DemRect = RectTransformToScreenSpace(DemVoteContainer);
        EdgeCollider2D DemWallCollider = CreateVertWallForVoteContainer(DemRect, 0);
        EdgeCollider2D slantDemCollider = Funnel.AddComponent<EdgeCollider2D>();
        slantDemCollider.points = new Vector2[] { DemWallCollider.points[1], openingEdgeCollider.points[0] };

        Rect RepRect = RectTransformToScreenSpace(RepVoteContainer);
        EdgeCollider2D RepWallCollider = CreateVertWallForVoteContainer(RepRect, RepRect.width);
        EdgeCollider2D slantRepCollider = Funnel.AddComponent<EdgeCollider2D>();
        slantRepCollider.points = new Vector2[] { RepWallCollider.points[1], openingEdgeCollider.points[1] };
    }

    public EdgeCollider2D CreateVertWallForVoteContainer(Rect rect, float xOffset)
    {
        Vector2 demVertWallBegin = new Vector2(rect.x + xOffset, rect.y + rect.size.y);
        Vector2 demVertWallEnd = new Vector2(rect.x + xOffset, rect.y);
        EdgeCollider2D wallCollider = Funnel.AddComponent<EdgeCollider2D>();
        wallCollider.points = new Vector2[] { demVertWallBegin, demVertWallEnd };

        return wallCollider;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("box opening triggered by " + collision.gameObject.name);
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
            playBtn.interactable = true;
        }
        else
        {
            PlayerParty = Party.None;
            playBtn.interactable = false;
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
            playBtn.interactable = true;
        }
        else
        {
            PlayerParty = Party.None;
            playBtn.interactable = false;
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

        string sloganText = "Slogans\n\n";

        slogans.ForEach((slogan) =>
        {
            sloganText = sloganText + "• " + slogan + "\n";
        });

        SloganText.text = sloganText;


        TextMeshProUGUI VoteText = PartyContainer.Find(Consts.VoteText).GetComponent<TextMeshProUGUI>();
        VoteText.text = partyDetails.popVotePct.ToString() + "% Popular Vote";

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
