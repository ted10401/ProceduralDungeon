﻿using UnityEngine;

namespace Dungeon
{
    public class RoomBasedProceduralDungeon : BaseProceduralDungeon
    {
        [Header("Dungeon Settings")]
        [SerializeField] private int m_totalRoomCount;
        [SerializeField] private int m_selectRoomCount;
        [SerializeField] private IntVector2 m_minRoomSize;
        [SerializeField] private IntVector2 m_maxRoomSize;

        [Header("Gizmos")]
        [SerializeField] private bool m_drawGrid;
        [SerializeField] private bool m_drawRoom;
        [SerializeField] private bool m_drawSelectRoom;
        [SerializeField] private bool m_drawSelectRoomCenterBias;
        [SerializeField] private bool m_drawDelaunayTriangulation;
        [SerializeField] private bool m_drawSpanningTree;
        [SerializeField] private bool m_drawRoads;

        private RoomGenerator m_roomGenerator;
        private CorridorGenerator m_roadGenerator;

        protected override void StartGenerate()
        {
            m_roomGenerator = new RoomGenerator(m_mapSize, m_totalRoomCount, m_selectRoomCount, m_minRoomSize, m_maxRoomSize);
            m_roadGenerator = new CorridorGenerator(m_mapSize, m_roomGenerator.SelectRooms);
            GenerateObjects(m_roomGenerator.SelectRooms, m_roadGenerator.Corridors);
        }

        public override Vector3 GetRandomPosition()
        {
            return GetRandomRoom().Center * m_gridSize;
        }

        private Room GetRandomRoom()
        {
            return m_roomGenerator.SelectRooms[Random.Range(0, m_roomGenerator.SelectRooms.Length)];
        }

        private void OnDrawGizmos()
        {
            DrawGrid();
            DrawRoom();
            DrawSelectRoom();
            DrawDelaunayTriangulation();
            DrawSpanningTree();
            DrawRoads();
        }

        private void DrawGrid()
        {
            if (!m_drawGrid)
            {
                return;
            }

            Vector3 left;
            Vector3 right;
            for (int z = 0; z <= m_mapSize.z; z++)
            {
                left = new Vector3(0, 0, z);
                right = new Vector3(m_mapSize.x, 0, z);

                Gizmos.DrawLine(left, right);
            }

            Vector3 bottom;
            Vector3 up;
            for (int x = 0; x <= m_mapSize.x; x++)
            {
                bottom = new Vector3(x, 0, 0);
                up = new Vector3(x, 0, m_mapSize.z);

                Gizmos.DrawLine(bottom, up);
            }
        }

        private void DrawRoom()
        {
            if (!m_drawRoom)
            {
                return;
            }

            if (m_roomGenerator == null || m_roomGenerator.Rooms == null || m_roomGenerator.Rooms.Length == 0)
            {
                return;
            }

            Gizmos.color = Color.red;

            Room cacheRoom;
            Vector3 leftUp;
            Vector3 rightUp;
            Vector3 leftBottom;
            Vector3 rightBottom;

            for (int i = 0; i < m_roomGenerator.Rooms.Length; i++)
            {
                cacheRoom = m_roomGenerator.Rooms[i];
                if (cacheRoom == null)
                {
                    continue;
                }

                leftBottom = new Vector3(cacheRoom.MinBorder.x, 0, cacheRoom.MinBorder.z);
                rightBottom = new Vector3(cacheRoom.MaxBorder.x, 0, cacheRoom.MinBorder.z);
                leftUp = new Vector3(cacheRoom.MinBorder.x, 0, cacheRoom.MaxBorder.z);
                rightUp = new Vector3(cacheRoom.MaxBorder.x, 0, cacheRoom.MaxBorder.z);

                Gizmos.DrawSphere(cacheRoom.Center, 0.1f);
                Gizmos.DrawLine(leftBottom, rightBottom);
                Gizmos.DrawLine(leftUp, rightUp);
                Gizmos.DrawLine(leftBottom, leftUp);
                Gizmos.DrawLine(rightBottom, rightUp);
            }
        }

        private void DrawSelectRoom()
        {
            if (!m_drawSelectRoom)
            {
                return;
            }

            if (m_roomGenerator == null || m_roomGenerator.SelectRooms == null || m_roomGenerator.SelectRooms.Length == 0)
            {
                return;
            }

            Gizmos.color = Color.green;

            Room cacheRoom;
            Vector3 leftUp;
            Vector3 rightUp;
            Vector3 leftBottom;
            Vector3 rightBottom;

            for (int i = 0; i < m_roomGenerator.SelectRooms.Length; i++)
            {
                cacheRoom = m_roomGenerator.SelectRooms[i];

                leftBottom = new Vector3(cacheRoom.MinBorder.x, 0, cacheRoom.MinBorder.z);
                rightBottom = new Vector3(cacheRoom.MaxBorder.x, 0, cacheRoom.MinBorder.z);
                leftUp = new Vector3(cacheRoom.MinBorder.x, 0, cacheRoom.MaxBorder.z);
                rightUp = new Vector3(cacheRoom.MaxBorder.x, 0, cacheRoom.MaxBorder.z);

                Gizmos.DrawSphere(cacheRoom.Center, 0.1f);
                Gizmos.DrawLine(leftBottom, rightBottom);
                Gizmos.DrawLine(leftUp, rightUp);
                Gizmos.DrawLine(leftBottom, leftUp);
                Gizmos.DrawLine(rightBottom, rightUp);
            }

            if (m_drawSelectRoomCenterBias)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < m_roomGenerator.SelectRooms.Length; i++)
                {
                    cacheRoom = m_roomGenerator.SelectRooms[i];
                    Gizmos.DrawSphere(cacheRoom.Center + cacheRoom.CenterBias, 0.1f);
                }
            }
        }

        private void DrawDelaunayTriangulation()
        {
            if (!m_drawDelaunayTriangulation)
            {
                return;
            }

            if (m_roadGenerator == null ||
                m_roadGenerator.VoronoiDiagram == null ||
                m_roadGenerator.VoronoiDiagram.Edges == null ||
                m_roadGenerator.VoronoiDiagram.Edges.Count == 0)
            {
                return;
            }

            Gizmos.color = Color.red;
            for (int i = 0; i < m_roadGenerator.VoronoiDiagram.Edges.Count; i++)
            {
                Gizmos.DrawLine(m_roadGenerator.VoronoiDiagram.Edges[i].LeftSite, m_roadGenerator.VoronoiDiagram.Edges[i].RightSite);
            }
        }

        private void DrawSpanningTree()
        {
            if (!m_drawSpanningTree)
            {
                return;
            }

            if (m_roadGenerator == null ||
                m_roadGenerator.SpanningTree == null ||
                m_roadGenerator.SpanningTree.Segments == null ||
                m_roadGenerator.SpanningTree.Segments.Count == 0)
            {
                return;
            }

            Gizmos.color = Color.black;
            for (int i = 0; i < m_roadGenerator.SpanningTree.Segments.Count; i++)
            {
                Gizmos.DrawLine(m_roadGenerator.SpanningTree.Segments[i].PointA, m_roadGenerator.SpanningTree.Segments[i].PointB);
            }
        }

        private void DrawRoads()
        {
            if (!m_drawRoads)
            {
                return;
            }

            if (m_roadGenerator == null ||
                m_roadGenerator.Corridors == null ||
                m_roadGenerator.Corridors.Length == 0)
            {
                return;
            }

            Gizmos.color = Color.blue;
            for (int i = 0; i < m_roadGenerator.Corridors.Length; i++)
            {
                Gizmos.DrawLine(m_roadGenerator.Corridors[i].Start, m_roadGenerator.Corridors[i].End);
            }
        }
    }
}