using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyDetails
{
    Sprite pic;
    string candidate;
    Party party;
    public List<string> slogans;
    public int popVoteCount;
    public int popVotePct;

    public PartyDetails(Sprite pic, string candidate, Party party, List<string> slogans, float pctPopVote)
	{
        this.pic = pic;
        this.slogans = slogans;

        this.popVotePct = (int) (pctPopVote * 100);

        popVoteCount = (int)(ElectionDetailsManager.populationSize * pctPopVote);

        // grid initialize
    }
}

