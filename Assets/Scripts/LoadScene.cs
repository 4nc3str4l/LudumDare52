using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public string Target;

    public void LoadTarget()
    {
        SceneManager.LoadScene(Target);
    }
    
}
