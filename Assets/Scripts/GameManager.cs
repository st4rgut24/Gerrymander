using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    Score score;
    Days days;

    public int DaysTilElection = 20;
    public float DemocratPct = .5f;
    public float RepublicanPct;
    public int population = 50;

    public List<Party> PartyList = new List<Party>();

    public int democraticDistricts = 0;
    public int republicanDistricts = 0;

    private void OnEnable()
    {

    }

    private void Awake()
    {
        RepublicanPct = 1 - DemocratPct;
        InitVoterComposition();
    }

    // Use this for initialization
    void Start()
	{
        score = GameObject.Find(Consts.ScoreGo).GetComponent<Score>();
        days = GameObject.Find(Consts.DaysGo).GetComponent<Days>();
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
}

