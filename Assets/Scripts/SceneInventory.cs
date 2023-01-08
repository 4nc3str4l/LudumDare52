using Assets.Scripts;
using UnityEngine;

public class SceneInventory : MonoBehaviour
{

    public static SceneInventory Instance;

    public Fence[] AllFences;
    public Pumpkin[] AllPumpkins;
    public Portal[] AllPortals;
    public PumpinBox[] AllBoxes;


    private void Awake()
    {
        Instance = this;    
    }


    private void OnEnable()
    {
        GameController.OnStatusChanged += GameController_OnStatusChanged;
    }

    private void OnDisable()
    {
        GameController.OnStatusChanged -= GameController_OnStatusChanged;
    }

    public Fence GetClosestFence(Vector3 _position)
    {
        float minDist = Mathf.Infinity;
        Fence result = null;
        foreach (var f in AllFences)
        {
            float dist = Vector3.Distance(_position, f.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                result = f;
            }
        }
        return result;
    }

    public Portal GetPortal(Vector3 _position)
    {
        return AllPortals[Random.Range(0, AllPortals.Length)];
    }

    public float MinDistanceToPortals(Vector3 _position)
    {
        float minDist = Mathf.Infinity;
        foreach (Portal p in AllPortals)
        {
            minDist = Mathf.Min(Vector3.Distance(p.transform.position, _position), minDist);
        }
        return minDist;
    }


    public Pumpkin GetClosestFreePumpkin(Vector3 _position)
    {
        float minDist = Mathf.Infinity;
        Pumpkin result = null;
        foreach (var pumpkin in AllPumpkins)
        {
            // Skip harvested pumpkins and the ones that are being carried
            if(pumpkin.CurrentState == PumpkinState.Harvested ||
                pumpkin.CurrentState == PumpkinState.OnMonster ||
                pumpkin.CurrentState == PumpkinState.Stolen || 
                pumpkin.CurrentState == PumpkinState.OnPlayer)
            {
                continue;
            }
            float dist = Vector3.Distance(_position, pumpkin.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                result = pumpkin;
            }
        }
        return result;
    }



    private void GameController_OnStatusChanged(GameStatus obj)
    {
        if(obj == GameStatus.NOT_STARTED)
        {
            foreach (Pumpkin p in AllPumpkins)
            {
                p.Reset();
            }

            foreach (PumpinBox pb in AllBoxes)
            {
                pb.Reset();
            }

        }
        else if(obj == GameStatus.SLEEPING)
        {
            MonsterSpawner.Instance.RemoveMonsters();

        }
        else if(obj == GameStatus.STARTING_GAME)
        {
            foreach (Fence f in AllFences)
            {
                f.Repair();
            }
        }
    }
}
