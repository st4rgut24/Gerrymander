using System;

public class Consts
{
    public const float BurstRadius = 1f;
    public const float BurstWallRadius = 5f;
    public const float CurrentRadius = .5f;
    public const float CurrentDist = 2f;
    public const float ImmunizeRadius = .75f;

    public const Party TutorialParty = Party.Democrat;

    //scenes
    public const string LandingScene = "LandingPage";
    public const string PlayMenu = "PlayMenu";
    public const string ElectionDetails = "ElectionDetails";
    public const string Game = "Game";
    public const string TutorialScene = "Tutorial";

    // names
    public const string PeopleGo = "People";
    public const string EdgesGo = "Edges";
    public const string ScoreGo = "Score";
    public const string DaysGo = "Days";
    public const string ContentGo = "Content";

    public const string DetailsPlayBtn = "PlayBtn";
    public const string ScrollRect = "ScrollRect";
    public const string YearBanner = "YearBanner";
    public const string Year = "Year";
    public const string VoteContainer = "VoteContainer";
    public const string VoteCheckbox = "VoteCheckbox";
    public const string VoteText = "VoteText";
    public const string Slogans = "Slogans";
    public const string Picture = "Picture";

    public const string ElectionHeader = "ElectionHeader";
    public const string Summary = "Summary";

    // time
    public const int AgentActiondelay = 3;
    public const int timeToDrawLongestDivider = 5;
    public const float waitTimeForRoomAction = 1;

    public const float expandDuration = .03f;

    public const float sicknessRecoverySec = 1;
    //public const float immunityCooldownSec = 3;
    //public const float minPressDuration = 2;

    // probabilities
    public const float contagionProb = .07f;
    public const float immunityProb = .25f;
    public const float fatalityProb = .7f;
    public const float spreadImmunityProb = .2f;

    // tags
    public const string PersonTag = "Person";
    public const string RoomTag = "Room";
    public const string chipUI = "chip";
}

