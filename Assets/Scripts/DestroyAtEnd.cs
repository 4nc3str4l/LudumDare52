using Assets.Scripts;
using UnityEngine;

public class DestroyAtEnd : MonoBehaviour
{

    public int LevelBorn;

    private void OnEnable()
    {
        LevelBorn = GameController.Instance.Level;
        GarbageCollector.Instance.RegisterToDestroy(this);

    }

    private void OnDisable()
    {
        GarbageCollector.Instance.UnRegisterToDestroy(this);
    }
}
