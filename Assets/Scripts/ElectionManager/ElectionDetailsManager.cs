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
    private Image SummaryImage;

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

    public List<GameObject> PartyObjects;

    const float boxOpeningWidth = 300;
    const float boxOpeningHeight = 381;

    private void Awake()
    {
        PartyObjects = new List<GameObject>();  
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

        details = GameManager.Instance.GetDetailsFromPartyYear();

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
        SoundManager.Instance.PlaySoundEffect(Consts.FunnelChips);

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

        SummaryImage.color = GameManager.Instance.RepColor;

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

        SummaryImage.color = GameManager.Instance.DemColor;

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
