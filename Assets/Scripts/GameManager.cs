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

    public int democrats = 0;
    public int republicans = 0;

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

    public void InitVoterComposition()
    {
        democrats = (int) (population* DemocratPct);
        republicans = (int)(population * RepublicanPct);

        for (int i=0; i< democrats; i++)
        {
            PartyList.Add(Party.Democrat);
        }

        for (int i = 0; i < republicans; i++)
        {
            PartyList.Add(Party.Republican);
        }
    }

    public void CalculateScore ()
    {

    }

    // Update is called once per frame
    void Update()
	{
			
	}

    private void OnDisable()
    {
    }
}

