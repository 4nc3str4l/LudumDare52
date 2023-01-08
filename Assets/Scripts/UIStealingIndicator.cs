using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStealingIndicator : MonoBehaviour
{
    public Image MovingEnemyPrefab;
    public float MaxDistance;
    public float MaxUiDistance = 418;

    private Dictionary<Pumpkin, Image> m_RegisteredPumkins = new Dictionary<Pumpkin, Image>();

    public static UIStealingIndicator Instance;

    private Vector3 m_InitialScale;

    private void Awake()
    {
        m_InitialScale = transform.localScale;
        Instance = this;
    }

    private void Start()
    {
        float maxDistance = 0;
        foreach(Portal p in SceneInventory.Instance.AllPortals)
        {
            maxDistance = Mathf.Max(Vector3.Distance(p.transform.position, Cabin.Instance.transform.position), maxDistance);
        }
        MaxDistance = maxDistance * 1.1f;
    }

    public void Register(Pumpkin p)
    {
        if (m_RegisteredPumkins.ContainsKey(p))
        {
            return;   
        }
        m_RegisteredPumkins.Add(p, GameObject.Instantiate(MovingEnemyPrefab, transform));
        transform.DOShakeScale(0.3f).OnComplete(() =>
        {
            transform.localScale = m_InitialScale;
        });
    }


    public void Unregister(Pumpkin p)
    {
        if (!m_RegisteredPumkins.ContainsKey(p))
        {
            return;
        }
        Destroy(m_RegisteredPumkins[p].gameObject);
        m_RegisteredPumkins.Remove(p);
    }


    private void Update()
    {
        foreach(var kv in m_RegisteredPumkins)
        {
            var distance = SceneInventory.Instance.MinDistanceToPortals(kv.Key.transform.position);
            float ratio = 1 - (distance / MaxDistance);
            if (ratio < 0)
            {
                ratio = 0;
            }

            if (ratio > 1) 
            { 
                ratio = 1;
            }

            var rt = kv.Value.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(0, - (MaxUiDistance / 2) + ratio * MaxUiDistance, 0);
        }
    }

}
