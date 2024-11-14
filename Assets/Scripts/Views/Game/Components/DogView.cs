using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DogView : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Image dogImage;
    
    [Header("Audio")]
    [SerializeField] private AudioSource barkSound;
    [SerializeField] private AudioSource whineSound;
    [SerializeField] private AudioSource chewSound;
    
    [Header("Dog Sprites")]
    [SerializeField] private Sprite[] dogEvolutions;
    [SerializeField] private Sprite damagedSprite;
    [SerializeField] private Sprite surprisedSprite;
    [SerializeField] private Sprite chewingSprite;
    
    private Sprite normalSprite;
    private Vector3 startPosition;

    private void Awake()
    {
        normalSprite = dogImage.sprite;
        startPosition = transform.position;
    }

    public void Bark()
    {
        barkSound.Play();
        animator.SetTrigger("Bark");
    }

    public void Whine()
    {
        whineSound.Play();
        animator.SetTrigger("Whine");
    }

    public void Chew()
    {
        chewSound.Play();
        StartCoroutine(ChewAnimation());
    }

    public void TakeDamage()
    {
        StartCoroutine(DamageAnimation());
    }

    public void Surprised()
    {
        StartCoroutine(SurprisedAnimation());
    }

    public void StartJump(int height)
    {
        StartCoroutine(JumpAnimation(height));
    }

    private IEnumerator ChewAnimation()
    {
        dogImage.sprite = chewingSprite;
        yield return new WaitForSeconds(0.5f);
        dogImage.sprite = normalSprite;
    }

    private IEnumerator DamageAnimation()
    {
        dogImage.sprite = damagedSprite;
        yield return new WaitForSeconds(0.3f);
        dogImage.sprite = normalSprite;
    }

    private IEnumerator SurprisedAnimation()
    {
        dogImage.sprite = surprisedSprite;
        yield return new WaitForSeconds(0.3f);
        dogImage.sprite = normalSprite;
    }

    public IEnumerator Evolve(int level)
    {
        if (level <= 1 || level > dogEvolutions.Length + 1)
            yield break;

        // Fade out
        float alpha = 1;
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * 2;
            dogImage.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        // Change sprite
        dogImage.sprite = dogEvolutions[level - 2];
        normalSprite = dogImage.sprite;

        // Fade in
        alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime * 2;
            dogImage.color = new Color(1, 1, 1, alpha);
            yield return null;
        }
    }

    private IEnumerator JumpAnimation(int height)
    {
        float jumpDuration = 0.5f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        
        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / jumpDuration;
            
            // Parabolic jump
            float yOffset = height * Mathf.Sin(normalizedTime * Mathf.PI);
            transform.position = startPos + new Vector3(0, yOffset, 0);
            
            yield return null;
        }
        
        transform.position = startPos;
    }
}