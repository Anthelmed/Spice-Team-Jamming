//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using static Targetable;
//using static UnityEditor.PlayerSettings;

//namespace Units
//{
//    public sealed class DummyWorld : MonoBehaviour
//    {
//        private static DummyWorld m_instance;
//        public static DummyWorld Instance 
//        {
//            get
//            {
//                if (m_instance)
//                    return m_instance;

//                m_instance = FindObjectOfType<DummyWorld>();
//                return m_instance;
//            }
//            private set
//            {
//                m_instance = value;
//            }
//        }

//        public bool visible = true;

//        private List<Unit> m_vegetation = new List<Unit>();
//        private List<Unit> m_pawns = new List<Unit>();
//        private List<Unit> m_knights = new List<Unit>();
//        private List<Unit> m_players = new List<Unit>();

//        private bool m_wasVisible;

//        private List<Unit> m_queryResultNoAlloc = new List<Unit>(20);

//        private List<Unit> GetListForType(Unit.Type type)
//        {
//            switch (type)
//            {
//                case Unit.Type.Vegetation:
//                    return m_vegetation;
//                case Unit.Type.Pawn:
//                    return m_pawns;
//                case Unit.Type.Knight:
//                    return m_knights;
//                case Unit.Type.Player:
//                    return m_players;
//            }
//            return null;
//        }

//        public void Register(Unit unit)
//        {
//            GetListForType(unit.UnitType).Add(unit);
//        }

//        public void Unregister(Unit unit)
//        {
//            GetListForType(unit.UnitType).Remove(unit);
//        }

//        public Unit FindClosestEnemyOfType(Vector3 position, float maxDistance, Unit.Type type, Faction myTeam, out float distance)
//        {
//            distance = maxDistance;
//            Unit result = null;

//            var list = GetListForType(type);
//            float newDist;
//            for (int i = 0; i < list.Count; ++i)
//            {
//                if (list[i].Team == myTeam) continue;
//                newDist = (list[i].transform.position - position).magnitude;
//                newDist -= list[i].Radius;
//                if (newDist < distance)
//                {
//                    distance = newDist;
//                    result = list[i];
//                }
//            }

//            return result;
//        }

//        public List<Unit> QueryCircleEnemies(Vector3 center, float radius, Faction team)
//        {
//            m_queryResultNoAlloc.Clear();

//            QueryCircleEnemiesOfType(center, radius, Unit.Type.Vegetation, team, true);
//            QueryCircleEnemiesOfType(center, radius, Unit.Type.Pawn, team, true);
//            QueryCircleEnemiesOfType(center, radius, Unit.Type.Knight, team, true);
//            QueryCircleEnemiesOfType(center, radius, Unit.Type.Player, team, true);

//            return m_queryResultNoAlloc;
//        }

//        public List<Unit> QueryCircleEnemiesOfType(Vector3 center, float radius, Unit.Type type, Faction team, bool mergePrevious = false)
//        {
//            if (!mergePrevious)
//                m_queryResultNoAlloc.Clear();

//            var list = GetListForType(type);
//            for (int i = 0; i < list.Count; ++i)
//            {
//                if (list[i].Team == team) continue;
//                if (CircleCircleIntersect(center, radius, list[i].transform.position, list[i].Radius))
//                    m_queryResultNoAlloc.Add(list[i]);
//            }
//            return m_queryResultNoAlloc;
//        }

//        public List<Unit> QueryFanEnemies(Vector3 center, float radius, Vector3 dir, float angle, Faction team)
//        {
//            m_queryResultNoAlloc.Clear();

//            QueryFanEnemiesOfType(center, radius, dir, angle, Unit.Type.Vegetation, team, true);
//            QueryFanEnemiesOfType(center, radius, dir, angle, Unit.Type.Pawn, team, true);
//            QueryFanEnemiesOfType(center, radius, dir, angle, Unit.Type.Knight, team, true);
//            QueryFanEnemiesOfType(center, radius, dir, angle, Unit.Type.Player, team, true);

//            return m_queryResultNoAlloc;
//        }

//        public List<Unit> QueryFanEnemiesOfType(Vector3 center, float radius, Vector3 dir, float angle, 
//            Unit.Type type, Faction team, bool mergePrevious = false)
//        {
//            if (!mergePrevious)
//                m_queryResultNoAlloc.Clear();

//            var cos = Mathf.Cos(angle);
//            dir.Normalize();

//            var list = GetListForType(type);
//            for (int i = 0; i < list.Count; ++i)
//            {
//                if (list[i].Team == team) continue;
//                if (CircleFanIntersect(list[i].transform.position, list[i].Radius, center, radius, dir, cos))
//                    m_queryResultNoAlloc.Add(list[i]);
//            }
//            return m_queryResultNoAlloc;
//        }

//        public List<Unit> QueryCircleAll(Vector3 center, float radius)
//        {
//            m_queryResultNoAlloc.Clear();

//            QueryCircleAllOfType(center, radius, Unit.Type.Vegetation, true);
//            QueryCircleAllOfType(center, radius, Unit.Type.Pawn, true);
//            QueryCircleAllOfType(center, radius, Unit.Type.Knight, true);
//            QueryCircleAllOfType(center, radius, Unit.Type.Player, true);

//            return m_queryResultNoAlloc;
//        }

//        public List<Unit> QueryCircleAllOfType(Vector3 center, float radius, Unit.Type type, bool mergePrevious = false)
//        {
//            if (!mergePrevious)
//                m_queryResultNoAlloc.Clear();

//            var list = GetListForType(type);
//            for (int i = 0; i < list.Count; ++i)
//            {
//                if (CircleCircleIntersect(center, radius, list[i].transform.position, list[i].Radius))
//                    m_queryResultNoAlloc.Add(list[i]);
//            }
//            return m_queryResultNoAlloc;
//        }

//        private bool CircleCircleIntersect(Vector3 center1, float radius1, Vector3 center2, float radius2)
//        {
//            var maxDistSq = radius1 + radius2;
//            maxDistSq *= maxDistSq;
//            return ((center1 - center2).sqrMagnitude < maxDistSq);
//        }

//        private bool CircleFanIntersect(Vector3 center1, float radius1, Vector3 center2, float radius2, Vector3 dir2, float cos2)
//        {
//            // If a fan intersects, a circle will always intersect as well
//            if (!CircleCircleIntersect(center1, radius1, center2, radius2))
//                return false;

//            var movedCenter = center2 - dir2 * radius1;
//            var toCenter = center1 - movedCenter;
//            toCenter.Normalize();
//            if (Vector3.Dot(toCenter, dir2) > cos2)
//                return true;

//            return false;
//        }

//        private void Awake()
//        {
//            Instance = this;
//            m_wasVisible = visible;
//        }

//        private void Update()
//        {
//            if (visible != m_wasVisible)
//            {
//                m_wasVisible = visible;
//                foreach (var unit in m_vegetation) unit.ChangeVisibility(visible);
//                foreach (var unit in m_pawns) unit.ChangeVisibility(visible);
//                foreach (var unit in m_knights) unit.ChangeVisibility(visible);
//                foreach (var unit in m_players) unit.ChangeVisibility(visible);
//            }
//        }

//#if UNITY_EDITOR
//        public List<Unit> Vegetation => m_vegetation;
//        public List<Unit> Pawns => m_pawns;
//        public List<Unit> Knights => m_knights;
//        public List<Unit> Players => m_players;
//#endif
//    }
//}