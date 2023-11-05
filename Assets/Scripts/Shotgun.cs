using DG.Tweening;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Shotgun : Weapon
{
    private Animator m_Animator;

    private MeshRenderer[] m_Renderers;

    public int NumProjectiles = 5;
    public float Dispersion = 0.1f;

    private Vector3 m_LocalPosition;
    private Vector3 m_LocalScale;

    public ShotgunShell LeftShotGunShell;
    public ShotgunShell RightShotgunShell;

    public Material DebrisMaterial;


    private void Awake()
    {
        m_LocalPosition = transform.localPosition;
        m_LocalScale = transform.localScale;
        m_Animator = GetComponent<Animator>();
        m_Renderers = GetComponentsInChildren<MeshRenderer>();
    }

    public override void OnShow()
    {
        foreach (MeshRenderer r in m_Renderers)
        {
            r.enabled = true;
        }
    }

    public override void OnHide()
    {
        foreach(MeshRenderer r in m_Renderers)
        {
            r.enabled = false;
        }
    }

    public override void OnReload()
    {
        m_Animator.SetTrigger("Reload");
        StartCoroutine(ReloadEffect());
    }

    private IEnumerator ReloadEffect()
    {
        JukeBox.Instance.PlaySound(JukeBox.Instance.OpenShotgun, 0.5f);
        var leftClone = Instantiate(LeftShotGunShell, LeftShotGunShell.transform.parent);
        var rightClone = Instantiate(RightShotgunShell, RightShotgunShell.transform.parent);
        LeftShotGunShell.Hide();
        RightShotgunShell.Hide();
        yield return new WaitForSeconds(0.3f);
        leftClone.Eject();
        yield return new WaitForSeconds(0.1f);
        rightClone.Eject();
        yield return new WaitForSeconds(0.2f);
        LeftShotGunShell.Show();
        JukeBox.Instance.PlaySound(JukeBox.Instance.ShellEntering, 0.4f);
        yield return new WaitForSeconds(0.1f);
        RightShotgunShell.Show();
        JukeBox.Instance.PlaySound(JukeBox.Instance.ShellEntering, 0.5f);
        yield return new WaitForSeconds(0.1f);
        JukeBox.Instance.PlaySound(JukeBox.Instance.ShotClose, 0.5f);
    }

    public override void OnShot()
    {
        JukeBox.Instance.PlaySound(JukeBox.Instance.ShotgunShot, 0.05f);
        int layerMask = 1 << LayerMask.NameToLayer("Monster");
        FXManager.Instance.SpawnExplosion(null, 1f, Barrel.transform.position, 0.1f);

        for (int i = 0; i < NumProjectiles; ++i)
        {
            // Generate a random vector within a unit sphere.
            Vector3 randomVector = Random.insideUnitSphere * Dispersion;

            // Add the random vector to the base direction.
            Vector3 finalDirection = CameraController.Instance.transform.forward + randomVector;

            // Create a ray from the transform's position in the final direction.
            Ray ray = new Ray(CameraController.Instance.transform.position, finalDirection);


            Vector3 hitPoint = ray.origin + ray.direction * 100f;
            RaycastHit hit;
            // Perform the raycast.
            if (Physics.Raycast(ray, out hit, 100f))
            {
                hitPoint = hit.point;

                Debug.Log(hit.collider.name);
                var monster = hit.collider.GetComponentInParent<Monster>();
                if(monster != null)
                {
                    monster.HealthStatus.DealDmg(ProjectileDmg / NumProjectiles);
                    FXManager.Instance.SpawnExplosion(DebrisMaterial, 0.1f * hit.distance, hitPoint, Random.Range(0.15f, 0.35f));
                    FXManager.Instance.SpawnPunkingDebris(hitPoint, 5, Random.Range(0.01f, 0.05f), 3);
                }
                else
                {
                    var pumpkin = hit.collider.GetComponent<Pumpkin>();
                    if(pumpkin != null)
                    {
                        pumpkin.OnShot(ray, hit);
                    }
                }
                

                FXManager.Instance.SpawnExplosion(null, 0.1f * hit.distance, hitPoint, Random.Range(0.15f, 0.25f));
                FXManager.Instance.SpawnDebris(hitPoint, 5, Random.Range(0.01f, 0.05f), 3);

            }

            FXManager.Instance.SpawnLine(Color.white, Barrel.transform.position, hitPoint, 0.1f, 300f);

        }

        transform.DOShakeScale(0.1f, 0.2f).OnComplete(() =>
        {
            transform.localScale = m_LocalScale;
        });

    }

    public override void OnShootEmpty()
    {
        JukeBox.Instance.PlaySound(JukeBox.Instance.ShotgunEmpty, 0.7f);
    }
}
