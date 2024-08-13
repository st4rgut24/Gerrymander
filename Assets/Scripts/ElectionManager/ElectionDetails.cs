using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectionDetails
{
    public string ActualResultsText;
    public Party ActualWinningParty;

    public string summary;
    public int year;

    PartyDetails demDetails;
    PartyDetails repDetails;


    public ElectionDetails(string summary, int year, PartyDetails demDetails, PartyDetails repDetails, Party ActualWinningParty, string ActualResultsText)
    {
        this.summary = summary;
        this.year = year;

        this.demDetails = demDetails;
        this.repDetails = repDetails;

        this.ActualResultsText = ActualResultsText;
        this.ActualWinningParty = ActualWinningParty;
    }

    public float GetRepPartyPct()
    {
        return repDetails.PartyPct;
    }

    public float GetDemPartyPct()
    {
        return demDetails.PartyPct;
    }

    public List<string> GetRepSlogan()
    {
        return repDetails.slogans;
    }

    public List<string> GetDemSlogan()
    {
        return demDetails.slogans;
    }

    public PartyDetails GetDemDetails() {
        return demDetails;
    }

    public PartyDetails GetRepubDetails()
    {
        return repDetails;
    }
}
