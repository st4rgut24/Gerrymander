using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectionDetails
{
    public string summary;
    public int year;

    PartyDetails demDetails;
    PartyDetails repDetails;


    public ElectionDetails(string summary, int year, PartyDetails demDetails, PartyDetails repDetails)
    {
        this.summary = summary;
        this.year = year;

        this.demDetails = demDetails;
        this.repDetails = repDetails;
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
