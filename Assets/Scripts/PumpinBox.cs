using Assets.Scripts;
using Assets.Scripts.MonsterAI;
using System;
using UnityEngine;

public class PumpinBox : MonoBehaviour
{
    public GameObject[] PumpkinInBoxVisuals;

    public BoxCollider CoverCollider;

    public int MaxPumkingsInside = 2;
    public int NumPumpkingsInside = 0;

    public static event Action<Pumpkin> OnPumkinCollected;

    public bool IsFull { get { return NumPumpkingsInside >= MaxPumkingsInside; } }

    public Arrow MyArrow;

    private void Awake()
    {
        CoverCollider.enabled = false;
        MyArrow.SetVisible(false);
    }

    private void OnEnable()
    {
        GameController.OnStatusChanged += GameController_OnStatusChanged;
    }

    private void OnDisable()
    {
        GameController.OnStatusChanged -= GameController_OnStatusChanged;
    }

    private void GameController_OnStatusChanged(GameStatus obj)
    {
        if(obj != GameStatus.IN_GAME)
        {
            MyArrow.SetVisible(false);
        }
    }

    private void Update()
    {
        if(GameController.Instance.State != GameStatus.IN_GAME) { return; }
        MyArrow.SetVisible(Player.Instance.CurrentStatus == PlayerStatus.CarryingPumpkin && !IsFull);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (IsFull) { return; }
        if(other.tag != "Pumpkin") { return; }
        var p = other.GetComponent<Pumpkin>();
        if(p.CurrentState != PumpkinState.OnTheGround) { return; }
        p.OnCollected(); 
        NumPumpkingsInside++;
        for(int i = 0; i < NumPumpkingsInside; ++i)
        {
            PumpkinInBoxVisuals[i].SetActive(true);
        }
        OnPumkinCollected?.Invoke(p);
        CoverCollider.enabled = IsFull;
    }

    public void Reset()
    {
        NumPumpkingsInside = 0;
        for (int i = 0; i < PumpkinInBoxVisuals.Length; ++i)
        {
            PumpkinInBoxVisuals[i].SetActive(false);
        }
        CoverCollider.enabled = IsFull;
    }
}
