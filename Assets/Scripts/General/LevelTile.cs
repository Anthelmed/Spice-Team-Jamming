using System.Collections.Generic;
using _3C.Player;
using Units;
using UnityEngine;

public class LevelTile : MonoBehaviour
{
    
    LevelTilesManager worldTilesManager;
 
    public Transform teleportPoint;
    public bool hasPlayer;
    public bool tileActivated;
    WorldTileStatus status = WorldTileStatus.neutral;
    public Vector2Int gridLocation;
    public GameTile MapTile { get; set; }
    [SerializeField] RenderTexture groundFXPersistantRT;
    [SerializeField] GameObject RTCam;
    [SerializeField] GameObject environmentArt;
    [SerializeField] Transform healSpawnPoint;
    
    private const int UPDATE_RATE = 6;

    private int m_updateTurn;
    bool healSpawned;
    private void Awake()
    {
        Sleep();

        Vegetation.Clear();
        Pawns.Clear();
        Knights.Clear();
        Players.Clear();
    }
    private void Start()
    {
        if (LevelTilesManager.instance != null) worldTilesManager = LevelTilesManager.instance;
        m_updateTurn = Random.Range(0, UPDATE_RATE);
    }

    private void Update()
    {
        int fire = 0;
        int ice = 0;
        int total = 0;

        for (int i = 0; i < Vegetation.Count; ++i)
        {
            if (Vegetation[i].Team == Faction.Fire)
                ++fire;
            if (Vegetation[i].Team == Faction.Ice)
                ++ice;
            ++total;
        }

        var newStatus = WorldTileStatus.neutral;
        var threshold = total / 2;
        if (fire > threshold)
            newStatus = WorldTileStatus.burnt;
        else if (ice > threshold)
            newStatus = WorldTileStatus.frozen;
        else if (fire + ice > threshold)
            newStatus = WorldTileStatus.contested;

        if (newStatus != status)
        {
            status = newStatus;
            MapTile.SetTileState(status);
            if (newStatus == WorldTileStatus.neutral && !healSpawned && worldTilesManager.healPrefab != null && healSpawnPoint != null)
            {
                print("heal spawned");
                var heal = Instantiate(worldTilesManager.healPrefab, healSpawnPoint.position + (Vector3.left * 2), Quaternion.identity);
                heal.transform.SetParent(environmentArt.transform, true);
                healSpawned = true; // dont spawn more than one ever.
            }
              
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerStateHandler player))
            return;
     
      //  hasPlayer = true;
        tileActivated = true;
        worldTilesManager.UpdateActiveTiles(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out PlayerStateHandler player))
            return;

      //  hasPlayer = false;
    }

    internal void WakeUp()
    {
        if (!tileActivated)
        {
            tileActivated = true;
            environmentArt.SetActive(true);

            foreach (var unit in Vegetation) unit.ChangeVisibility(true);
            foreach (var unit in Pawns) unit.ChangeVisibility(true);
            foreach (var unit in Knights) unit.ChangeVisibility(true);
            foreach (var unit in Players) unit.ChangeVisibility(true);
        }
    }
    internal void Sleep()
    {
        tileActivated = false;
        environmentArt.SetActive(false);
        hasPlayer = false;
        //tell mobs to go home or whatever

        foreach (var unit in Vegetation) unit.ChangeVisibility(false);
        foreach (var unit in Pawns) unit.ChangeVisibility(false);
        foreach (var unit in Knights) unit.ChangeVisibility(false);
        foreach (var unit in Players) unit.ChangeVisibility(false);
    }

    internal void Init(MapTileData mapTileData, Vector2Int gridCoordinates)
    {
        status = mapTileData.tileStatus;
        gridLocation = gridCoordinates;
    }

    #region Jordi's stuff
    public List<Unit> Vegetation = new List<Unit>();
    public List<Unit> Pawns = new List<Unit>();
    public List<Unit> Knights = new List<Unit>();
    public List<Unit> Players = new List<Unit>();

    private static List<Unit> m_queryResultNoAlloc = new List<Unit>(20);

    private List<Unit> GetListForType(Unit.Type type)
    {
        switch (type)
        {
            case Unit.Type.Vegetation:
                return Vegetation;
            case Unit.Type.Pawn:
                return Pawns;
            case Unit.Type.Knight:
                return Knights;
            case Unit.Type.Player:
                return Players;
        }
        return null;
    }

    public void Register(Unit unit)
    {
        GetListForType(unit.UnitType).Add(unit);
    }

    public void Unregister(Unit unit)
    {
        GetListForType(unit.UnitType).Remove(unit);
    }

    public Unit FindClosestEnemyOfType(Vector3 position, float maxDistance, Unit.Type type, Faction myTeam, out float distance)
    {
        distance = maxDistance;
        Unit result = null;

        var list = GetListForType(type);
        float newDist;
        for (int i = 0; i < list.Count; ++i)
        {
            if (list[i].Team == myTeam) continue;
            newDist = (list[i].transform.position - position).magnitude;
            newDist -= list[i].Radius;
            if (newDist < distance)
            {
                distance = newDist;
                result = list[i];
            }
        }

        return result;
    }

    public List<Unit> QueryCircleEnemies(Vector3 center, float radius, Faction team, bool mergePrevious = false)
    {
        if (!mergePrevious)
            m_queryResultNoAlloc.Clear();

        QueryCircleEnemiesOfType(center, radius, Unit.Type.Vegetation, team, true);
        QueryCircleEnemiesOfType(center, radius, Unit.Type.Pawn, team, true);
        QueryCircleEnemiesOfType(center, radius, Unit.Type.Knight, team, true);
        QueryCircleEnemiesOfType(center, radius, Unit.Type.Player, team, true);

        return m_queryResultNoAlloc;
    }

    public List<Unit> QueryCircleEnemiesOfType(Vector3 center, float radius, Unit.Type type, Faction team, bool mergePrevious = false)
    {
        if (!mergePrevious)
            m_queryResultNoAlloc.Clear();

        var list = GetListForType(type);
        for (int i = 0; i < list.Count; ++i)
        {
            if (list[i].Team == team) continue;
            if (CircleCircleIntersect(center, radius, list[i].transform.position, list[i].Radius))
                m_queryResultNoAlloc.Add(list[i]);
        }
        return m_queryResultNoAlloc;
    }

    public List<Unit> QueryFanEnemies(Vector3 center, float radius, Vector3 dir, float angle, Faction team, bool mergePrevious = false)
    {
        if (!mergePrevious) m_queryResultNoAlloc.Clear();

        QueryFanEnemiesOfType(center, radius, dir, angle, Unit.Type.Vegetation, team, true);
        QueryFanEnemiesOfType(center, radius, dir, angle, Unit.Type.Pawn, team, true);
        QueryFanEnemiesOfType(center, radius, dir, angle, Unit.Type.Knight, team, true);
        QueryFanEnemiesOfType(center, radius, dir, angle, Unit.Type.Player, team, true);

        return m_queryResultNoAlloc;
    }

    public List<Unit> QueryFanEnemiesOfType(Vector3 center, float radius, Vector3 dir, float angle,
        Unit.Type type, Faction team, bool mergePrevious = false)
    {
        if (!mergePrevious)
            m_queryResultNoAlloc.Clear();

        var halfAngle = angle * 0.5f;
        dir.Normalize();

        var list = GetListForType(type);
        for (int i = 0; i < list.Count; ++i)
        {
            if (list[i].Team == team) continue;
            if (CircleFanIntersect(list[i].transform.position, list[i].Radius, center, radius, dir, halfAngle))
                m_queryResultNoAlloc.Add(list[i]);
        }
        return m_queryResultNoAlloc;
    }

    public List<Unit> QueryCircleAll(Vector3 center, float radius, bool mergePrevious = false)
    {
        if (!mergePrevious)
            m_queryResultNoAlloc.Clear();

        QueryCircleAllOfType(center, radius, Unit.Type.Vegetation, true);
        QueryCircleAllOfType(center, radius, Unit.Type.Pawn, true);
        QueryCircleAllOfType(center, radius, Unit.Type.Knight, true);
        QueryCircleAllOfType(center, radius, Unit.Type.Player, true);

        return m_queryResultNoAlloc;
    }

    public List<Unit> QueryCircleAllOfType(Vector3 center, float radius, Unit.Type type, bool mergePrevious = false)
    {
        if (!mergePrevious)
            m_queryResultNoAlloc.Clear();

        var list = GetListForType(type);
        for (int i = 0; i < list.Count; ++i)
        {
            if (CircleCircleIntersect(center, radius, list[i].transform.position, list[i].Radius))
                m_queryResultNoAlloc.Add(list[i]);
        }
        return m_queryResultNoAlloc;
    }

    private bool CircleCircleIntersect(Vector3 center1, float radius1, Vector3 center2, float radius2)
    {
        var maxDistSq = radius1 + radius2;
        maxDistSq *= maxDistSq;
        return ((center1 - center2).sqrMagnitude < maxDistSq);
    }

    private bool CircleFanIntersect(Vector3 center1, float radius1, Vector3 center2, float radius2, Vector3 dir2, float halfAngle)
    {
        // If a fan intersects, a circle will always intersect as well
        if (!CircleCircleIntersect(center1, radius1, center2, radius2))
            return false;

        if (Vector3.Angle(center1 - center2, dir2) < (halfAngle))
            return true;

        return false;
    }

    #endregion
}

public enum WorldTileStatus
{
    neutral,
    contested,
    burnt,
    frozen
}
