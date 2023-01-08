using DG.Tweening;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    public static FXManager Instance;

    public RaycastLine LinePrefab;
    public GameObject Explosion;

    public GameObject[] PunkinDebris;
    public GameObject[] GeneralDebris;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnLine(Color c, Vector3 origin, Vector3 end, float duration, float speed)
    {
        var line = Instantiate(LinePrefab);
        line.Init(c, origin, end, duration, speed);
    }

    public void SpawnExplosion(Material m, float dim, Vector3 position, float duration)
    {
        var explosion = GameObject.Instantiate(Explosion, position, Quaternion.identity);
        if(m != null)
        {
            explosion.GetComponent<Renderer>().material = m;
        }
        explosion.transform.DOScale(transform.localScale * Random.Range(0, dim), duration).OnComplete(() =>
        {
            Destroy(explosion, 0.01f);
        });
    }


    public void SpawnDebris(Vector3 position, float duration, float scale, int num)
    {
        for(int i = 0; i < num; ++i)
        {
            var debris = GameObject.Instantiate(GeneralDebris[Random.Range(0, GeneralDebris.Length)], position, Quaternion.identity);
            debris.transform.localScale = Vector3.one * scale;
            debris.transform.localRotation = Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));
            debris.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-200, 200), Random.Range(-200, 200), Random.Range(-200, 200)));
            GameObject.Destroy(debris, duration);
        }
    }

    public void SpawnPunkingDebris(Vector3 position, float duration, float scale, int num)
    {
        for (int i = 0; i < num; ++i)
        {
            var debris = GameObject.Instantiate(PunkinDebris[Random.Range(0, PunkinDebris.Length)], position, Quaternion.identity);
            debris.transform.localScale = Vector3.one * scale;
            debris.transform.localRotation = Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));
            debris.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-200, 200), Random.Range(-200, 200), Random.Range(-200, 200)));
            GameObject.Destroy(debris, duration);
        }
    }


}
