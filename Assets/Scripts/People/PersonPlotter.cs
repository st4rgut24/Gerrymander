using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public enum Party
{
	None,
	Democrat,
	Republican
}

public class PersonPlotter : Singleton<PersonPlotter>
{
	[SerializeField]
	private int sickCount;

	[SerializeField]
	private Sprite RepubSprite;

    [SerializeField]
    private Sprite DemoSprite;

    [SerializeField]
	private GameObject PersonPrefabInst;

	[SerializeField]
	private Transform PersonParentTransform;

	public List<GameObject> PersonsList;

	private float buffer = 150; // pixels

    public Vector2 PersonSize;

    //Dictionary<Party, Color> PartyColors = new Dictionary<Party, Color>()
    //{
    //	{ Party.Democrat, Color.blue },
    //	{ Party.Republican, Color.red }
    //};

    Dictionary<Party, Sprite> PartyColors;

    private void OnEnable()
    {
        //Controller.DragEvent += OnDrag;
    }

    private void Awake()
    {
        PartyColors = new Dictionary<Party, Sprite>()
    {
        { Party.Democrat, DemoSprite },
        { Party.Republican, RepubSprite }
    };
    }

    public void OnDrag(Vector3 dragStart, Vector3 dragEnd)
	{
		Vector2 dragStartWorld = Camera.main.ScreenToWorldPoint(dragStart);
        Vector2 dragEndWorld = Camera.main.ScreenToWorldPoint(dragEnd);

        List<GameObject> Swimmers = GetSplashRadiusPeople(Consts.CurrentRadius, dragStartWorld, false);

        Vector3 dir = (dragEndWorld - dragStartWorld).normalized;

		Swimmers.ForEach((Swimmer) =>
		{
			PersonPrefab swimmer = Swimmer.GetComponent<PersonPrefab>();
			Vector2 finalSwimPos = swimmer.transform.position + dir * Consts.CurrentDist;

			swimmer.SetTargetPosition(finalSwimPos);
		});
	}


	public bool IsPeopleInBounds(Bounds bounds)
	{
		for (int i = 0; i < PersonsList.Count; i++)
		{
			if (bounds.Contains(PersonsList[i].transform.position))
			{
				return true;
			}
		}
		return false;
	}

    // Use this for initialization
    void Start()
	{
        PersonsList = new List<GameObject>();

        Vector2 bufferVector = Vector2.one * buffer;
		Vector2 minWorldPoint = Camera.main.ScreenToWorldPoint(Vector2.zero + bufferVector);
		Vector2 maxWorldPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height) - bufferVector);

        PersonSize = PersonPrefabInst.transform.localScale;

        InitPopulation(minWorldPoint, maxWorldPoint);
	}

	/// <summary>
	/// Criteria for getting sick:
	/// Persons must be in splash radius AND an adjacent CONNECTED room
	/// </summary>
	/// <param name="radius"></param>
	/// <param name="center"></param>
	/// <returns></returns>
	public List<GameObject> GetSplashRadiusPeople(float radius, Vector3 center, bool IncludeAdjRoom)
	{
		RoomPrefab room = Map.Instance.FindRoomWorldCoords(center);

		List<GameObject> ProximalPersons = new List<GameObject>();

		PersonsList.ForEach((Person) =>
		{

			if (Vector2.Distance(Person.transform.position, center) < radius)
			{
				PersonPrefab otherPerson = Person.GetComponent<PersonPrefab>();
				RoomPrefab otherRoom = Map.Instance.FindRoomWorldCoords(otherPerson.transform.position);

				if (room == otherRoom)
					ProximalPersons.Add(Person);
				else if (IncludeAdjRoom && room.IsAdjacentConnectedRoom(otherRoom))
                    ProximalPersons.Add(Person);
            }
        });

		return ProximalPersons;
	}

	void InitPopulation(Vector2 minWorldPoint, Vector2 maxWorldPoint)
	{
		List<Party> PartyList = GameManager.Instance.PartyList;

        for (int i=0;i< PartyList.Count;i++)
		{
			Party voterParty = PartyList[i];

            float randWorldX = Random.Range(minWorldPoint.x, maxWorldPoint.x);
			float randWorldY = Random.Range(minWorldPoint.y, maxWorldPoint.y);

			GameObject PersonGo = Instantiate(PersonPrefabInst, PersonParentTransform);
			PersonPrefab person = PersonGo.GetComponent<PersonPrefab>();

			PersonGo.transform.position = new Vector2(randWorldX, randWorldY);
			person.SetPartyAffiliation(voterParty, PartyColors[voterParty]);

			PersonsList.Add(PersonGo);
		}
	}

	public Party GetAffiliation(List<PersonPrefab> persons)
	{
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
			return Party.Democrat;
		else if (repCount > demCount)
			return Party.Republican;
		else
			return Party.None;
    }

    private void OnDisable()
    {
        //Controller.DragEvent -= OnDrag;
    }
}

