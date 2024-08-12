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

	public Move[] AIMoves = new Move[] { Move.Divide, Move.Join, Move.Fill };

	Party affiliation;
    Party enemyAffiliation;

    public Agent(Difficulty difficulty, Party party, Party enemyParty)
	{
		this.affiliation = party;
		Debug.Log("Ai affiliation is " + affiliation);
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

	public void MakeRandomMove()
	{
		int randMove = UnityEngine.Random.Range(0, AIMoves.Length);
		Move move = AIMoves[randMove];

		if (move == Move.Divide)
			DivideRoom();
		else if (move == Move.Join)
			JoinRoom();
		else if (move == Move.Fill)
			Debug.LogError("Fill is not implemented yet");
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

    public void JoinRoom()
	{
		Debug.Log("AI decides to join rooms");
		List<JoinRewardInfo> fillRewardInfos = new List<JoinRewardInfo>();

        List<(RoomPrefab room1, RoomPrefab room2)> roomPairs = GenerateUniqueCompletedRoomPairs(Map.Instance.Rooms);

		roomPairs.ForEach((roomPair) =>
		{
			// determine value of filling each room pair and prioritize it
			int fillReward = CalculateFilledRoomReward(roomPair.room1, roomPair.room2);

			fillRewardInfos.Add(new JoinRewardInfo(roomPair.room1, roomPair.room2, fillReward));
        });

        List<JoinRewardInfo> SortedRewardInfos = fillRewardInfos.OrderBy(obj => obj.reward).ToList();
        int rewardIdx = (int)(SortedRewardInfos.Count * probChooseBestMove);

        RoomPrefab ChosenFillRoom1 = SortedRewardInfos[rewardIdx].fillRoom1;
		RoomPrefab ChosenFillRoom2 = SortedRewardInfos[rewardIdx].fillRoom2;

        Debug.Log("AI Best rooms to fill out of " + SortedRewardInfos.Count + " has a reward of " + SortedRewardInfos[rewardIdx].reward);
        Map.Instance.JoinRoom(ChosenFillRoom1, ChosenFillRoom2);
    }

    public void DivideRoom()
	{
		Debug.Log("AI decides to divide rooms");
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
		int rewardIdx = (int)(SortedRewardInfos.Count * probChooseBestMove);

		RoomPrefab ChosenDivideRoom = SortedRewardInfos[rewardIdx].room;

		Debug.Log("AI Best room to divide out of " + SortedRewardInfos.Count + " has a reward of " + SortedRewardInfos[rewardIdx].reward);
		Map.Instance.DivideRoom(ChosenDivideRoom);
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

	private int CalculateFilledRoomReward(RoomPrefab room1, RoomPrefab room2)
	{
		Party partyRoom1 = room1.GetParty();
		Party partyRoom2 = room2.GetParty();
		int beginScore = GetPartyScore(partyRoom1) + GetPartyScore(partyRoom2);

		List<PersonPrefab>persons = room1.GetPersonsInBounds();
		persons.AddRange(room2.GetPersonsInBounds());
		Debug.Log("AI number of persons in bounds " + persons.Count);
		Party combinedPartyAffiliation = Party.None;

		int demCount = 0;
		int repCount = 0;

		persons.ForEach((p) =>
		{
			if (p.party == Party.Republican)
				repCount++;
			if (p.party == Party.Democrat)
				demCount++;
		});

		if (demCount > repCount)
			combinedPartyAffiliation = Party.Democrat;
		else if (repCount > demCount)
			combinedPartyAffiliation = Party.Republican;

		int endScore = GetPartyScore(combinedPartyAffiliation);
		int rewardDiff = endScore - beginScore;
		Debug.Log("ai join reward diff is endscore " + endScore + " minus beginscore " + beginScore + " affiliation of joined room is " + combinedPartyAffiliation);

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

