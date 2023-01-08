using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public Transform[] SpawnPoints;

    public GameObject[] Monsters;

    private List<GameObject> m_InstantiatedMonsters = new List<GameObject>();

    public static MonsterSpawner Instance;


    private void Awake()
    {
        Instance = this;
    }

    public void SpawnMonsters(int number)
    {
        StartCoroutine(SpawnMonstersRoutine(number));
    }

    IEnumerator SpawnMonstersRoutine(int number)
    {
        for (int i = 0; i < number; ++i)
        {
            SpawnMonster(10);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void SpawnMonster(int tier)
    {
        Transform choosenPoint = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
        GameObject monsterPrefab = Monsters[Random.Range(0, Mathf.Min(tier, Monsters.Length))];

        var g = GameObject.Instantiate(monsterPrefab, choosenPoint.transform.position, choosenPoint.transform.rotation);
        m_InstantiatedMonsters.Add(g);
    }

    public void RemoveMonsters()
    {
        foreach(GameObject m in m_InstantiatedMonsters)
        {
            Destroy(m);
        }
        m_InstantiatedMonsters.Clear();
    }
}
