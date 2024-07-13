using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Agent
{
	public Difficulty diff;
	public float probChooseBestMove;

	Party affiliation;
    Party enemyAffiliation;

    public Agent(Difficulty difficulty, Party party, Party enemyParty)
	{
		this.affiliation = party;
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

    public void DivideRoom()
	{
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

		Debug.Log("Best room to divide out of " + SortedRewardInfos.Count + " has a reward of " + SortedRewardInfos[rewardIdx].reward);
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

