﻿using UnityEngine;

namespace ProceduralDungeon
{
    public class Road
    {
        public Vector3 Start { get { return m_start; } }
        public Vector3 End { get { return m_end; } }
        public bool IsVertical { get { return m_isVertical; } }
        public float MinBorder { get { return m_minBorder; } }
        public float MaxBorder { get { return m_maxBorder; } }

        private Vector3 m_start;
        private Vector3 m_end;
        private bool m_isVertical;
        private float m_minBorder;
        private float m_maxBorder;

        public Road(Vector3 start, Vector3 end)
        {
            m_start = start;
            m_end = end;

            m_isVertical = m_start.x == m_end.x;
            if (m_isVertical)
            {
                m_minBorder = m_start.z;
                m_maxBorder = m_end.z;
            }
            else
            {
                m_minBorder = m_start.x;
                m_maxBorder = m_end.x;
            }

            if (m_maxBorder < m_minBorder)
            {
                float tempValue = m_minBorder;
                m_minBorder = m_maxBorder;
                m_maxBorder = tempValue;
            }
        }
    }
}