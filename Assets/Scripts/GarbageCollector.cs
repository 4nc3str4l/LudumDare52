using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GarbageCollector : MonoBehaviour
{
    public static GarbageCollector Instance;

    private List<DestroyAtEnd> m_DestroyAtEndEntities = new List<DestroyAtEnd>();

    public const float DestroyInterval = 10;
    public const float DestroyRate = 100;

    private float m_NextCheck = 0;

    public void Awake()
    {
        Instance = this;
    }

    public void RegisterToDestroy(DestroyAtEnd entity)
    {
        m_DestroyAtEndEntities.Add(entity);
    }


    public void UnRegisterToDestroy(DestroyAtEnd entity)
    {
        m_DestroyAtEndEntities.Remove(entity);
    }

    private void Update()
    {
        if(m_NextCheck >= Time.realtimeSinceStartup)
        {
            return;
        }

        for(int i = m_DestroyAtEndEntities.Count -1; i >= 0; --i)
        {
            bool removed = false;
            try
            {
                // If the entity was already destroyed just finish
                if (m_DestroyAtEndEntities[i] == null || m_DestroyAtEndEntities[i].gameObject == null)
                {
                    m_DestroyAtEndEntities.RemoveAt(i);
                    removed = true;
                    continue;
                }

                if (m_DestroyAtEndEntities[i].LevelBorn != GameController.Instance.Level)
                {
                    Destroy(m_DestroyAtEndEntities[i].gameObject, 0.1f * i);
                    m_DestroyAtEndEntities.RemoveAt(i);
                    removed = true;
                }
            }
            catch(Exception ex)
            {
                Debug.Log(ex);
                if (!removed && m_DestroyAtEndEntities.Count > i)
                {
                    m_DestroyAtEndEntities.RemoveAt(i);
                }
            }
        }

        m_NextCheck = Time.realtimeSinceStartup + DestroyInterval;
    }
}
