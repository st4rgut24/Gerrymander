using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum Matches
{
    TrumpBiden
}

public class ElectionDetailsManager : Singleton<ElectionDetailsManager>
{
    [SerializeField]
    private Transform DemocratContainer;

    [SerializeField]
    private Transform RepublicanContainer;

    [SerializeField]
    private Sprite TrumpPic;

    [SerializeField]
    private Sprite BidenPic;

    [SerializeField]
    private GameObject DemocraticChip;

    [SerializeField]
    private GameObject RepublicanChip;

    public const int populationSize = 50;

    public Dictionary<Matches, ElectionDetails> ElectionMap;

    private void Awake()
    {
        ElectionMap = new Dictionary<Matches, ElectionDetails>()
        {
            {
                Matches.TrumpBiden,
                new ElectionDetails(
                    "Having survived an assasination attempt, former president Trump rematches Biden in a tense election.",
                    2024,
                    new PartyDetails(BidenPic, "Joe Biden", Party.Democrat, new List<string>() {"Beat Trump", "Protect Democracy"}, .5f),
                    new PartyDetails(TrumpPic, "Donald Trump", Party.Republican, new List<string>() {"MAGA"}, .5f)
                    )
            }
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        ElectionDetails details = ElectionMap[Matches.TrumpBiden];

        InitOverviewUI(details.year, details.summary);

        InitPartyUI(details, DemocratContainer, DemocraticChip, details.GetDemDetails(), details.GetDemSlogan());
        InitPartyUI(details, RepublicanContainer, RepublicanChip, details.GetRepubDetails(), details.GetRepSlogan());
    }

    public void InitOverviewUI(int electionYear, string summary)
    {
        TextMeshProUGUI ElectionYearText = GameObject.Find(Consts.ElectionHeader).GetComponent<TextMeshProUGUI>();
        ElectionYearText.text = electionYear.ToString() + " Election";

        TextMeshProUGUI ElectionSummaryText = GameObject.Find(Consts.Summary).GetComponent<TextMeshProUGUI>();
        ElectionSummaryText.text = summary;
    }

    public void InitPartyUI(ElectionDetails details, Transform PartyContainer, GameObject partyPrefab, PartyDetails partyDetails, List<string> slogans)
    {
        Transform VoteContainer = PartyContainer.Find(Consts.VoteContainer);
        InitVoteContainer(VoteContainer, partyDetails.popVoteCount, partyPrefab);

        TextMeshProUGUI SloganText = PartyContainer.Find(Consts.Slogans).GetComponent<TextMeshProUGUI>();

        string sloganText = "Slogans\n";

        slogans.ForEach((slogan) =>
        {
            sloganText = sloganText + "• " + slogan + "\n";
        });

        SloganText.text = sloganText;


        TextMeshProUGUI VoteText = PartyContainer.Find(Consts.VoteText).GetComponent<TextMeshProUGUI>();
        VoteText.text = partyDetails.popVotePct.ToString() + "% Popular Vote";
    }

    public void InitVoteContainer(Transform VoteTransform, int popVoteCount, GameObject partyPrefab)
    {
        for (int i = 0; i < popVoteCount; i++)
        {
            Instantiate(partyPrefab, VoteTransform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
