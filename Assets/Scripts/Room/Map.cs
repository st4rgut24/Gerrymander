using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UIElements;

/// <summary>
/// The map defining the layout of the spaces
/// if you want to define a type of space (eg room),
/// you must reference the cells in the map
/// </summary>
public class Map : Singleton<Map>
{
    [SerializeField]
    private GameObject DistrictGoPrefab;

    [SerializeField]
    private Transform RoomDaddy;

    [SerializeField]
    private Transform DistrictDaddy;

    [SerializeField]
    private Transform EdgeDaddy;

    [SerializeField]
    private GameObject RoomGo;

    [SerializeField]
	private int horCells = 100; // cell count horizontally

    public int DistrictId = 0;

    List<RoomPrefab> roomsToRecalculate; // room that we need to determine political boundaries for
    GameObject districtToDelete;

    public List<RoomPrefab> Rooms;

    Dictionary<int, District> DistrictMap = new Dictionary<int, District>();

    Dictionary<Party, Color> PartyColors = new Dictionary<Party, Color>()
    {
        { Party.None, Color.grey },
        { Party.Democrat, new Color(0, 151f / 255f, 255f / 255f) },
        { Party.Republican, new Color(255f / 255f, 87f / 255f, 60f / 255f) }
    };

    Grid grid;

    bool IsFirstRoomCreated = false;

	int vertCells; // cell count vertically

    int roomId = 0;

    float cellLength; // cell length in pixels (cell height == cell width)

    public static Action<RoomPrefab> RemoveRoomEvent;

    private void OnEnable()
    {
        Controller.TouchEvent += OnSingleTouch;
        Controller.DragEvent += OnDrag;

        TutorialController.TouchEvent += OnSingleTouch;
        TutorialController.DragEvent += OnDrag;

        LineAnimator.PartyLineDrawn += RecalculatePartyLines;
    }

    private void Awake()
    {
        roomsToRecalculate = new List<RoomPrefab>();

        grid = new Grid();
        Rooms = new List<RoomPrefab>();
    }

    // Use this for initialization
    void Start()
	{
        Vector2 bottomLeft = Vector2.zero;
        Vector2 topRight = new Vector2(Screen.width, Screen.height);

		vertCells = (int)(horCells / Camera.main.aspect);
        cellLength = (float)Screen.width / horCells;

        CreateMap();

        RoomPrefab room = CreateRoom(Vector2Int.zero, new Vector2Int(horCells, vertCells), true);
        District district = AddDistrict(Party.None);

        ColorDistrictByParty(Party.None, new List<RoomPrefab>() { room }, district.gameObject);

        room.SetDistrict(district);
    }

    District AddDistrict(Party party)
    {
        DistrictId++;

        GameObject DistrictGo = Instantiate(DistrictGoPrefab, DistrictDaddy);
        District partyDistrict = DistrictGo.GetComponent<District>();
        partyDistrict.Init(DistrictId, party);

        GameManager.Instance.AddPartyVoter(party);

        return partyDistrict;
    }

    void GetAllConnectedRooms(RoomPrefab room, HashSet<RoomPrefab> SeenRooms)
    {
        SeenRooms.Add(room);

        room.AdjacentConnectedRooms.ForEach((adjRoom) =>
        {
            if (!SeenRooms.Contains(adjRoom))
            {
                GetAllConnectedRooms(adjRoom, SeenRooms);
            }
        });
    }

    private void ColorDistrictByParty(Party party, List<RoomPrefab> connectedRooms, GameObject DistrictGo)
    {
        connectedRooms.ForEach((area) =>
        {
            area.SetPartyBox(PartyColors[party], DistrictGo);
        });
    }

    /// <summary>
    /// calcualte what party affiliation a district that a room belongs to has
    /// </summary>
    public Party GetPartyFromRoom(RoomPrefab room)
    {
        HashSet<RoomPrefab> SeenRoomSet = new HashSet<RoomPrefab>();
        GetAllConnectedRooms(room, SeenRoomSet);

        List<RoomPrefab> connectedRoomList = new List<RoomPrefab>(SeenRoomSet);

        Party party = CheckDistrictParty(connectedRoomList);

        return party;
    }

    public void RecalculatePartyLines()
    {
        List<RoomPrefab> recalculatedRoomList = new List<RoomPrefab>();

        foreach (RoomPrefab room in roomsToRecalculate)
        {
            if (recalculatedRoomList.Contains(room))
            {
                // skip recalculating this room as it was a connected room to a previously recalculated room
                continue;
            }

            HashSet<RoomPrefab> SeenRoomSet = new HashSet<RoomPrefab>();
            GetAllConnectedRooms(room, SeenRoomSet);

            List<RoomPrefab> connectedRoomList = new List<RoomPrefab>(SeenRoomSet);

            Party party = CheckDistrictParty(connectedRoomList);

            connectedRoomList.ForEach((room) =>
            {
                if (room.district != null)
                {
                    GameObject.DestroyImmediate(room.district.gameObject);
                }
            });

            District district = AddDistrict(party);
            ColorDistrictByParty(party, connectedRoomList, district.gameObject);

            connectedRoomList.ForEach((room) => room.SetDistrict(district));

            recalculatedRoomList.AddRange(connectedRoomList);
            recalculatedRoomList.Add(room);
        };

        if (districtToDelete != null)
            GameObject.DestroyImmediate(districtToDelete);

        GameManager.Instance.UpdateScore();

        roomsToRecalculate.Clear();
        districtToDelete = null;

        StartCoroutine(GameManager.Instance.FinishTurn());
    }

    private Party CheckDistrictParty(List<RoomPrefab> areas)
    {
        List<GameObject> PersonsList = PersonPlotter.Instance.PersonsList;

        int repubCount = 0;
        int democratCount = 0;

        // replace this logic with comparing the types of voters within the list of rooms
        // and determining what the district should be 

        for (int x = 0; x < areas.Count; x++)
        {
            RoomPrefab room = areas[x];

            for (int i = 0; i < PersonsList.Count; i++)
            {
                GameObject PersonGo = PersonsList[i];

                // room's adjacent connected rooms and their adjacent connected rooms
                if (room.BoundsContains(PersonGo.transform.position))
                {
                    Party party = PersonGo.GetComponent<PersonPrefab>().party;

                    if (party == Party.Republican)
                        repubCount++;
                    else
                        democratCount++;
                }
            }
        };

        //Debug.Log("Republican count " + repubCount + " Democrat Count " + democratCount);

        if (repubCount != democratCount)
        {
            return repubCount > democratCount ? Party.Republican : Party.Democrat;
        }
        else
            return Party.None;
    }

    /// <summary>
    /// when a single click is detected either divide a room into two OR
    /// restore a room's missing walls
    /// </summary>
    private void OnSingleTouch(Vector3 touchPos)
    {
        // dont create new rooms/walls until line animator is finished animating
        if (LineAnimator.Instance.IsAnimating)
            return;

        RoomPrefab room = FindRoom(Input.mousePosition);

        if (room.IsRoomCompleted())
        {
            DivideRoom(room);
        }
        else
        {
            FillAndClearRoom(room);
        }
    }

    public void FillAndClearRoom(RoomPrefab room)
    {
        FillRoom(room);

        room.AdjacentConnectedRooms.ForEach((adjRoom) =>
        {
            adjRoom.RemoveConnectedRoom(room);
        });

        room.ClearConnectedRoom();
    }

    private bool IsDivisionValid(Box box)
    {
        Vector2 PersonSize = PersonPlotter.Instance.PersonSize;
        //Debug.Log("Person size " + PersonSize);
        //Debug.Log("box size " + box.bounds);
        return box.bounds.size.x / 2 > PersonSize.x || box.bounds.size.y / 2 > PersonSize.y;
    }

    private void OnDestroy()
    {
        EdgeMaker.ClearEdgeDicts();
    }

    private void OnDrag(Vector3 dragStart, Vector3 dragEnd)
    {
        RoomPrefab room1 = FindRoom(dragStart);
        RoomPrefab room2 = FindRoom(dragEnd);

        if (room1.gameObject == room2.gameObject)
        {
            return;
        }

        JoinRoom(room1, room2);
    }

    public RoomPrefab FindRoomWorldCoords(Vector3 worldCoords)
    {
        Vector3 screenCoords = Camera.main.WorldToScreenPoint(worldCoords);
        return Rooms.Find((room) => room.Contains(screenCoords) != null);
    }

    private RoomPrefab FindRoom(Vector3 coord)
    {
        return Rooms.Find((room) => room.Contains(coord) != null);
    }

    private RoomPrefab PretendCreateRoom(Vector2Int minCoord, Vector2Int maxCoord, bool ShowPerimeter)
    {
        List<Cell> cells = grid.GetCells(minCoord, maxCoord);
        Box box = new Box(cells, minCoord, maxCoord);

        GameObject RoomObject = Instantiate(RoomGo);

        RoomObject.transform.parent = RoomDaddy;

        roomId++;

        RoomPrefab room = RoomObject.GetComponent<RoomPrefab>();

        room.roomId = roomId;
        RoomObject.name = "Room " + room.roomId.ToString();

        room.AnnexSpace(box);

        return room;
    }

    public RoomPrefab CreateRoom(Vector2Int minCoord, Vector2Int maxCoord, bool ShowPerimeter)
    {
        List<Cell> cells = grid.GetCells(minCoord, maxCoord);
        Box box = new Box(cells, minCoord, maxCoord);

        GameObject RoomObject = Instantiate(RoomGo);

        RoomObject.transform.parent = RoomDaddy;

        roomId++;

        RoomPrefab room = RoomObject.GetComponent<RoomPrefab>();        

        room.roomId = roomId;
        RoomObject.name = "Room " + room.roomId.ToString();

        room.AnnexSpace(box);

        Rooms.Add(room);

        if (!IsFirstRoomCreated)
        {
            room.InitRoomWithoutAnimating();
            IsFirstRoomCreated = true;
        }
        else {
            room.CreatePerimeter(ShowPerimeter);
        }

        return room;
    }

    /// <summary>
    /// Join adjacent rooms by hiding the walls between them
    /// </summary>
    public void JoinRoom(RoomPrefab room1, RoomPrefab room2)
    {
        if (room1.AdjacentConnectedRooms.Contains(room2))
        {
            // exit if these rooms are already joined
            return;
        }

        List<Edge> roomBorders = room1.GetSharedEdges(room2);

        if (roomBorders.Count == 0) // not adjacent rooms
        {
            return;
        }

        roomBorders.ForEach((borderLine) =>
        {
            borderLine.ToggleLine(false);
        });

        room1.ReplaceColliders();
        room2.ReplaceColliders();

        room1.AddConnectedRoom(room2);
        room2.AddConnectedRoom(room1);

        roomsToRecalculate = new List<RoomPrefab>() { room1 };
        RecalculatePartyLines(); // dont need to wait for edge animation to complete
    }

    /// <summary>
    /// Completes a room that has a missing wall(s)
    /// </summary>
    private void FillRoom(RoomPrefab room)
    {
        room.CreatePerimeter(true);

        roomsToRecalculate = new List<RoomPrefab>() { room };

        roomsToRecalculate.AddRange(room.AdjacentRooms);
        //RecalculatePartyLines(room);

        room.RemoveAdjacentRooms();
    }

    /// <summary>
    /// creates new rooms on either side of a divider and
    /// removes the room that is divided
    /// </summary>
    /// <param name="plotCoord">coord where user wants to split into new rooms</param>
    public void DivideRoom(RoomPrefab room)
    {
        if (IsDivisionValid(room.box))
        {
            if (room.district != null)
                districtToDelete = room.district.gameObject;

            Box box = room.box;
            RemoveRoom(room);
            CreateDividedRooms(box, false);
        }
    }

    

    public List<RoomPrefab> PretendDivideRoom(RoomPrefab room)
    {
        Box box = room.box;

        CreateDividedRooms(box, true);

        return roomsToRecalculate;
    }

    private float GetAspect()
    {
        return (float) Screen.width / (float) Screen.height;
    }

    /// <summary>
    /// Should divide the room vertically or horizontally depending on orientation and previous cut
    /// </summary>
    /// <param name="roomAspectRatio">aspect ratio fo room being dividied</param>
    private void CreateDividedRooms(Box box, bool pretendDivide)
    {
        //Debug.Log("Aspect camera " + Camera.main.aspect);
        float aspect = GetAspect();
        float deltaAspectRatio = Mathf.Abs(aspect - box.getAspectRatio());

        bool landscape = aspect > 1;

        float dividedAspectRatio = landscape ? aspect / 2 : aspect * 2;
        float deltaDividedAspectRatio = Mathf.Abs(dividedAspectRatio - box.getAspectRatio());

        bool mismatch = deltaAspectRatio > deltaDividedAspectRatio;

        bool DivideHor = landscape ? mismatch : !mismatch;

        Vector2Int box1Min = box.min;
        Vector2Int box1Max = DivideHor ? new Vector2Int(box.max.x, box.getMidY()) : new Vector2Int(box.getMidX(), box.max.y);

        Vector2Int box2Min = DivideHor ? new Vector2Int(box1Min.x, box1Max.y) : new Vector2Int(box1Max.x, box1Min.y); 
        Vector2Int box2Max = box.max;

        RoomPrefab room2;
        RoomPrefab room1;
        // bounds will be created in room 1, bounds will be shared with room 2
        if (pretendDivide)
        {
            room2 = PretendCreateRoom(box2Min, box2Max, false);
            room1 = PretendCreateRoom(box1Min, box1Max, true);
        }
        else
        {
            room2 = CreateRoom(box2Min, box2Max, false);
            room1 = CreateRoom(box1Min, box1Max, true);
        }

        room1.AddAdjacentRoom(room2);
        room2.AddAdjacentRoom(room1);

        roomsToRecalculate = new List<RoomPrefab>() { room1, room2 };
    }

    private void RemoveRoom(RoomPrefab room)
    {
        District roomDistrict = room.district;

        Rooms.Remove(room);
        room.RemoveEdgeCollider();
        RemoveRoomEvent?.Invoke(room);

        GameObject.Destroy(room.gameObject);
    }

    private void CreateMap()
    {
        for (int c=0; c < horCells; c++)
        {
            for (int r=0; r < vertCells; r++)
            {
                Vector2Int pos = new Vector2Int(c, r);
                Cell cell = CreateCell(pos);

                grid.Add(cell);
            }
        }
    }

    private Cell CreateCell(Vector2Int pos)
    {
        int col = pos.x;
        int row = pos.y;

        Vector2 bl = new Vector2(col * cellLength, row * cellLength).RoundOff();
        Vector2 br = new Vector2(bl.x + cellLength, bl.y).RoundOff();
        Vector2 tl = new Vector2(bl.x, bl.y + cellLength).RoundOff();
        Vector2 tr = new Vector2(bl.x + cellLength, bl.y + cellLength).RoundOff();

        return new Cell(bl, br, tl, tr, row, col, EdgeDaddy);
    }

    private void OnDisable()
    {
        Controller.TouchEvent -= OnSingleTouch;
        Controller.DragEvent -= OnDrag;

        TutorialController.TouchEvent -= OnSingleTouch;
        TutorialController.DragEvent -= OnDrag;

        LineAnimator.PartyLineDrawn -= RecalculatePartyLines;

    }
}

