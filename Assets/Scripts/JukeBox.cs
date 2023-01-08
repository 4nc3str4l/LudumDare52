using System.Collections;
using UnityEngine;

public class JukeBox : MonoBehaviour
{

    public static JukeBox Instance;
    public AudioSource TargetSource;
    public AudioClip EquipShotgunSound;
    public AudioClip OpenDoor;

    public AudioClip OpenShotgun;
    public AudioClip ShellEntering;
    public AudioClip ShotClose;
    public AudioClip ShotgunShot;
    public AudioClip ShotgunEmpty;


    public AudioClip PumpkinGather;

    public AudioClip PumkinRelease;
    public AudioClip PumkinCollected;

    public AudioClip MonsterWondering;
    public AudioClip MonsterHappy;

    public AudioClip MonsterDie;
    public AudioClip MonsterRessurrect;
    public AudioClip PumkinStolen;
    public AudioClip MonsterAttack;
    public AudioClip ShotgunMonsterHit;

    public AudioClip FenceHit;
    public AudioClip FenceDestroyed;

    public AudioClip PlayerHurt;


    public AudioClip PlayerDeath;
    public AudioClip GameOver;

    private void Awake()
    {
        Instance = this;
    }

    public void PlaySound(AudioClip _clip, float _volumne)
    {
        TargetSource.pitch = Random.Range(0.90f, 1.1f);
        TargetSource.PlayOneShot(_clip, _volumne * SoundManager.Instance.Voulume);
    }

    public void PlaySoundDelayed(AudioClip _clip, float _volumne, float _delay)
    {
        StartCoroutine(WaitAndPlay(_clip, _volumne, _delay));
    }

    IEnumerator WaitAndPlay(AudioClip _clip, float _volumne, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        PlaySound(_clip, _volumne);
    }

    public void PlaySoundAtSource(AudioSource s, AudioClip _clip, float _volumne)
    {
        s.pitch = Random.Range(0.90f, 1.1f);
        s.PlayOneShot(_clip, _volumne * SoundManager.Instance.Voulume);
    }

}