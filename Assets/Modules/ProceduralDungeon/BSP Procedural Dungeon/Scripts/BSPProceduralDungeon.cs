﻿using UnityEngine;
using BinarySpacePartitioning;
using System.Collections.Generic;

namespace Dungeon
{
    public class BSPProceduralDungeon : BaseProceduralDungeon
    {
        [Header("Dungeon Settings")]
        [SerializeField] private int m_splitIteration = 1;
        [SerializeField] private IntVector2 m_minBSPSize = new IntVector2(3, 3);
        [SerializeField] private Vector2 m_minRoomSizeRatio = new Vector2(0.45f, 0.45f);

        [Header("Gizmos")]
        [SerializeField] private bool m_drawGrid;
        [SerializeField] private bool m_drawBSP;
        [SerializeField] private bool m_drawRoom;
        [SerializeField] private bool m_drawCorridor;

        private BSPTree m_tree;
        private List<BSPNode> m_leafNodes;
        private List<BSPNode> m_parentNodes;

        protected override void StartGenerate()
        {
            m_tree = new BSPTree(m_mapSize, m_minBSPSize, m_minRoomSizeRatio);

            for (int i = 0; i < m_splitIteration; i++)
            {
                m_tree.Split();
            }

            m_leafNodes = m_tree.GetAllLeafNodes();
            for (int i = 0; i < m_leafNodes.Count; i++)
            {
                m_leafNodes[i].GenerateRoomRect();
            }

            List<BSPNode> levelNodes = null;
            for (int i = m_splitIteration; i >= 0; i--)
            {
                levelNodes = m_tree.GetNodesByLevel(i);
                for (int j = 0; j < levelNodes.Count; j++)
                {
                    levelNodes[j].GenerateCorridorRect();
                }
            }

            m_parentNodes = m_tree.GetAllParentNodes();

            if(m_generateObjects)
            {
                List<Room> rooms = new List<Room>();
                for (int i = 0; i < m_leafNodes.Count; i++)
                {
                    if (m_leafNodes[i].Room == null)
                    {
                        continue;
                    }

                    rooms.Add(m_leafNodes[i].Room);
                }

                List<Corridor> corridors = new List<Corridor>();
                for (int i = 0; i < m_parentNodes.Count; i++)
                {
                    if (m_parentNodes[i].Corridor == null)
                    {
                        continue;
                    }

                    corridors.Add(m_parentNodes[i].Corridor);
                }

                GenerateObjects(rooms.ToArray(), corridors.ToArray());
            }
        }

        public override Vector3 GetRandomPosition()
        {
            IntRect roomRect = GetRandomNode().Room.Rect;
            return new Vector3(roomRect.Center.x, 0, roomRect.Center.y) * m_gridSize;
        }

        private BSPNode GetRandomNode()
        {
            return m_leafNodes[Random.Range(0, m_leafNodes.Count)];
        }

        private void OnDrawGizmos()
        {
            DrawGrid();
            DrawBSP();
            DrawRoom();
            DrawCorridor();
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

        private void DrawBSP()
        {
            if (!m_drawBSP)
            {
                return;
            }

            if(m_leafNodes == null || m_leafNodes.Count == 0)
            {
                return;
            }

            Gizmos.color = Color.green;
            for(int i = 0; i < m_leafNodes.Count; i++)
            {
                DrawRect(m_leafNodes[i].Rect);
            }
        }

        private void DrawRoom()
        {
            if (!m_drawRoom)
            {
                return;
            }

            if (m_leafNodes == null || m_leafNodes.Count == 0)
            {
                return;
            }

            Gizmos.color = Color.blue;
            for (int i = 0; i < m_leafNodes.Count; i++)
            {
                DrawRect(m_leafNodes[i].Room.Rect);
            }
        }

        private void DrawCorridor()
        {
            if (!m_drawCorridor)
            {
                return;
            }

            if (m_parentNodes == null || m_parentNodes.Count == 0)
            {
                return;
            }

            Gizmos.color = Color.red;
            for (int i = 0; i < m_parentNodes.Count; i++)
            {
                if(m_parentNodes[i].Corridor == null)
                {
                    continue;
                }

                DrawRect(m_parentNodes[i].Corridor.Rect);
            }
        }

        private void DrawRect(IntRect rect)
        {
            Gizmos.DrawLine(new Vector3(rect.X, 0, rect.Y), new Vector3(rect.X + rect.Width, 0, rect.Y));
            Gizmos.DrawLine(new Vector3(rect.X, 0, rect.Y + rect.Height), new Vector3(rect.X + rect.Width, 0, rect.Y + rect.Height));
            Gizmos.DrawLine(new Vector3(rect.X, 0, rect.Y), new Vector3(rect.X, 0, rect.Y + rect.Height));
            Gizmos.DrawLine(new Vector3(rect.X + rect.Width, 0, rect.Y), new Vector3(rect.X + rect.Width, 0, rect.Y + rect.Height));
        }
    }
}