using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Agent;

public class Agent
{
	public enum Move
	{
		Divide,
		Join,
		Fill
	}

	public Difficulty diff;
	public float probChooseBestMove;

	public Move[] AIMoves = new Move[] { Move.Divide, Move.Fill };

	Party affiliation;
    Party enemyAffiliation;

    public Agent(Difficulty difficulty, Party party, Party enemyParty)
	{
		this.affiliation = party;
		// Debug.LogLog("Ai affiliation is " + affiliation);
		this.enemyAffiliation = enemyParty;
		this.diff = difficulty;

		if (this.diff == Difficulty.Easy)
		{
			probChooseBestMove = .3f;
		}
		else if (this.diff == Difficulty.Medium)
		{
			probChooseBestMove = .5f;
		}
		else
		{
			probChooseBestMove = .999f;
		}
	}

    public struct DivideRewardInfo
    {
        public RoomPrefab room;  // Assuming RoomPrefab is some existing class or struct
        public int reward;

        // Constructor to initialize the properties
        public DivideRewardInfo(RoomPrefab room, int reward)
        {
            this.room = room;
            this.reward = reward;
        }
    }

	public struct JoinRewardInfo
	{
		public RoomPrefab fillRoom1;
		public RoomPrefab fillRoom2;

		public int reward;

		public JoinRewardInfo(RoomPrefab fillRoom1, RoomPrefab fillRoom2, int reward)
		{
			this.fillRoom1 = fillRoom1;
			this.fillRoom2 = fillRoom2;
			this.reward = reward;
		}
	}

	public void MakeMove(Move move)
	{
        if (move == Move.Fill)
        {
            bool FillIsPossible = FillRoom();

            if (!FillIsPossible)
            {
                move = Move.Divide;
                // Debug.LogLogWarning("Fill is NOT possible");
            }
            else
            {
                // Debug.LogLog("Fill is possible");
            }
        }
        if (move == Move.Divide)
        {
            bool canDivide = DivideRoom();

            if (!canDivide)
            {
                move = Move.Join;
                // Debug.LogLogWarning("Divide is not possible");
            }
            else
            {
                // Debug.LogLog("Join is possible");
            }
        }
        if (move == Move.Join)
        {
            JoinRoom();
        }
    }

	public void MakeRandomMove()
	{
		int randMove = UnityEngine.Random.Range(0, AIMoves.Length);
		Move move = AIMoves[randMove];

		MakeMove(move);
	}

	private List<(RoomPrefab room1, RoomPrefab room2)> GenerateUniqueCompletedRoomPairs(List<RoomPrefab> items)
    {
        var pairs = new List<(RoomPrefab, RoomPrefab)>();

        for (int i = 0; i < items.Count; i++)
        {
            for (int j = i + 1; j < items.Count; j++)
            {
				if (items[i].IsRoomCompleted() && items[j].IsRoomCompleted() && items[i].IsAdjacentTo(items[j]))
					pairs.Add((items[i], items[j]));
            }
        }

        return pairs;
    }

	public bool FillRoom()
	{
		// Debug.LogLog("AI decides to fill room");
		List<RoomPrefab> fillableRooms = GetUncompletedRooms();

		for (int i=0;i<fillableRooms.Count;i++)
		{
			RoomPrefab FillableRoom = fillableRooms[i];

			List<PersonPrefab> persons = FillableRoom.GetPersonsInBounds();

			//List<RoomPrefafb> adjRooms = Room.AdjacentConnectedRooms; // only connected rooms will be affected by filling a room

            Party party = FillableRoom.district.party;
            int beforeScore = GetPartyScore(party);

            //int afterScore = 0;
            Party filledRoomPartyAffiliation = PersonPlotter.Instance.GetAffiliation(persons);

			List<PersonPrefab> JoinedPersons = new List<PersonPrefab>(); // persons in the joined room(s)

			FillableRoom.AdjacentConnectedRooms.ForEach((ConRoom) =>
			{
				JoinedPersons.AddRange(ConRoom.GetPersonsInBounds());
			});
            Party joinedRoomsPartyAffiliation = PersonPlotter.Instance.GetAffiliation(JoinedPersons);


			int filledRoomScore = GetPartyScore(filledRoomPartyAffiliation);
			int joinedRoomsScore = GetPartyScore(joinedRoomsPartyAffiliation);

            int afterScore = filledRoomScore + joinedRoomsScore; // a bit simplified because doesnt take into account changes in affiliation of the other connected rooms
			// Debug.LogLog("After score " + afterScore + " before score " + beforeScore);
			if (afterScore > beforeScore)
			{
				Map.Instance.FillAndClearRoom(FillableRoom);
				return true;
			}
			else
			{
				Debug.Log("Fill is NOT Possible because not to our advantage");
			}
        };
		if (fillableRooms.Count == 0)
			Debug.Log("Fill is NOT Possible because fillable rooms dont exist");
        return false;
	}

	public List<RoomPrefab> GetUncompletedRooms()
	{
		List<RoomPrefab> unfinishedRooms = new List<RoomPrefab>();

		Map.Instance.Rooms.ForEach((Room) =>
		{
			if (!Room.IsRoomCompleted())
				unfinishedRooms.Add(Room);
		});

		return unfinishedRooms;
    }

    public void JoinRoom()
	{
		// Debug.LogLog("AI decides to join rooms");
		List<JoinRewardInfo> fillRewardInfos = new List<JoinRewardInfo>();

        List<(RoomPrefab room1, RoomPrefab room2)> roomPairs = GenerateUniqueCompletedRoomPairs(Map.Instance.Rooms);

		roomPairs.ForEach((roomPair) =>
		{
			// determine value of filling each room pair and prioritize it
			int fillReward = CalculateJoinedRoomReward(roomPair.room1, roomPair.room2);

			fillRewardInfos.Add(new JoinRewardInfo(roomPair.room1, roomPair.room2, fillReward));
        });

        List<JoinRewardInfo> SortedRewardInfos = fillRewardInfos.OrderBy(obj => obj.reward).ToList();
        int rewardIdx = (int)(SortedRewardInfos.Count * probChooseBestMove);

        RoomPrefab ChosenFillRoom1 = SortedRewardInfos[rewardIdx].fillRoom1;
		RoomPrefab ChosenFillRoom2 = SortedRewardInfos[rewardIdx].fillRoom2;

        //// Debug.LogLog("AI Best rooms to fill out of " + SortedRewardInfos.Count + " has a reward of " + SortedRewardInfos[rewardIdx].reward);
        Map.Instance.JoinRoom(ChosenFillRoom1, ChosenFillRoom2);
    }

    public bool DivideRoom()
	{
		// Debug.LogLog("AI decides to divide rooms");
        List<DivideRewardInfo> divideRewardInfos = new List<DivideRewardInfo>();

        Map.Instance.Rooms.ForEach((room) =>
		{
			if (room.IsRoomCompleted()) // only completed rooms can be divided
			{
				int divisionReward = CalculateDividedRoomReward(room);
				divideRewardInfos.Add(new DivideRewardInfo(room, divisionReward));
            }
        });

		// lowest to highest
        List<DivideRewardInfo> SortedRewardInfos = divideRewardInfos.OrderBy(obj => obj.reward).ToList();
		if (SortedRewardInfos.Count == 0)
			return false;

        int rewardIdx = (int)(SortedRewardInfos.Count * probChooseBestMove);

		RoomPrefab ChosenDivideRoom = SortedRewardInfos[rewardIdx].room;

		//// Debug.LogLog("AI Best room to divide out of " + SortedRewardInfos.Count + " has a reward of " + SortedRewardInfos[rewardIdx].reward);
		Map.Instance.DivideRoom(ChosenDivideRoom);

		return true;
    }




	private int GetPartyScore(Party party)
	{
		if (party == affiliation)
			return 1;
		else if (party == enemyAffiliation)
			return -1;
		else
			return 0;
	}

	private int CalculateJoinedRoomReward(RoomPrefab room1, RoomPrefab room2)
	{
		Party partyRoom1 = room1.GetParty();
		Party partyRoom2 = room2.GetParty();
		int beginScore = GetPartyScore(partyRoom1) + GetPartyScore(partyRoom2);

		List<PersonPrefab>persons = room1.GetPersonsInBounds();
		persons.AddRange(room2.GetPersonsInBounds());
		//// Debug.LogLog("AI number of persons in bounds " + persons.Count);
		Party combinedPartyAffiliation = PersonPlotter.Instance.GetAffiliation(persons);

		int endScore = GetPartyScore(combinedPartyAffiliation);
		int rewardDiff = endScore - beginScore;
		//// Debug.LogLog("ai join reward diff is endscore " + endScore + " minus beginscore " + beginScore + " affiliation of joined room is " + combinedPartyAffiliation);

		return rewardDiff;
	}

	private int CalculateDividedRoomReward(RoomPrefab room)
	{
        Party party = room.GetParty();
		int beginScore = GetPartyScore(party);

		int endScore = 0;
        List<RoomPrefab> dividedRooms = Map.Instance.PretendDivideRoom(room);

        dividedRooms.ForEach((dividedRoom) =>
        {
            Party party = Map.Instance.GetPartyFromRoom(dividedRoom);
			endScore += GetPartyScore(party);
        });

		dividedRooms.ForEach((room) => GameObject.Destroy(room.gameObject)); // clean up the rooms afer doing the calculation
		dividedRooms.Clear();
		int rewardDiff = endScore - beginScore;

		return rewardDiff;
    }
}

