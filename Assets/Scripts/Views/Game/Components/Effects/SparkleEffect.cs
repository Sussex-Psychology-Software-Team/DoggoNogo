using System.Collections;
using UnityEngine;

public class SparkleEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem sparkleSystem;

    public void Play(float duration)
    {
        StartCoroutine(PlaySparklesRoutine(duration));
    }

    public void PlayIndefinitely()
    {
        sparkleSystem.Play();
    }

    public void Stop()
    {
        sparkleSystem.Stop();
    }

    private IEnumerator PlaySparklesRoutine(float duration)
    {
        sparkleSystem.Play();
        yield return new WaitForSeconds(duration);
        sparkleSystem.Stop();
    }
}