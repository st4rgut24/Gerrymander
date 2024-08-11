using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyDetails
{
    public string candidate;
    Party party;
    public List<string> slogans;
    public int popVoteCount;
    public int popVotePct;
    public string picFile;
    public float PartyPct;

    public Sprite partySprite;

    public PartyDetails(string picFile, string candidate, Party party, List<string> slogans, float pctPopVote)
	{
        this.picFile = picFile;

        this.slogans = slogans;

        this.popVotePct = (int) (pctPopVote * 100);

        this.candidate = candidate;

        popVoteCount = (int)(ElectionDetailsManager.populationSize * pctPopVote);
        PartyPct = ((float)popVotePct / 100f);
        
        // grid initialize
    }
}

