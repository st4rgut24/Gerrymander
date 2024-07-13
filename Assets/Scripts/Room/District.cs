using System;
using UnityEngine;

public class District : MonoBehaviour
{
	public int id;
	public Party party;

	public void Init(int id, Party party)
    {
        this.id = id;
        this.party = party;
    }

    private void OnDestroy()
    {
        GameManager.Instance.RemovePartyVoter(party);
    }
}

