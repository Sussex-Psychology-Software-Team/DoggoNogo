using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DogView : MonoBehaviour
{
    [Header("Visual Components")]
    [SerializeField] private Image dogImage;
    [SerializeField] private SparkleEffect sparkleEffect;
    [SerializeField] private Sprite[] evolutionSprites;
    
    [Header("Animation Settings")]
    [SerializeField] private int maxJumpHeight = 40;
    [SerializeField] private float jumpDuration = 0.3f;
    [SerializeField] private float damageShakeAmount = 3.0f;
    [SerializeField] private float damageFlickerDuration = 1.0f;
    
    [Header("Audio")]
    [SerializeField] private AudioSource barkSound;
    [SerializeField] private AudioSource chewSound;
    [SerializeField] private AudioSource whineSound1;
    [SerializeField] private AudioSource whineSound2;
    [SerializeField] private AudioSource surprisedSound;
    [SerializeField] private AudioSource twinkleSound;
    [SerializeField] private AudioSource levelUpSound;
    [SerializeField] private AudioSource pantSound;

    private Vector3 _startPosition;
    private bool _isJumping;
    private bool _isDamaged;
    private float _currentYPosition;

    private void Awake()
    {
        _startPosition = transform.localPosition;
        _currentYPosition = _startPosition.y;
    }

    public IEnumerator Evolve(int level)
    {
        sparkleEffect.Play(4.0f);
        twinkleSound.Play();
        levelUpSound.Play();
        
        yield return StartCoroutine(ShakeAndColorEffect(Color.grey, 4.0f, 3.0f, 0.2f));
        UpdateSprite(level);
        pantSound.Play();
    }

    public void TakeDamage()
    {
        if (!_isDamaged)
        {
            StartCoroutine(ShakeAndColorEffect(Color.red, damageShakeAmount, damageFlickerDuration, 0.2f));
        }
    }

    public void StartJump(int height)
    {
        if (!_isJumping)
        {
            maxJumpHeight = height;
            StartCoroutine(JumpAnimation());
        }
    }

    // Audio methods
    public void Whine() => PlayRandomWhine();
    public void Chew() => chewSound.Play();
    public void Bark() => barkSound.Play();
    public void Surprised() => surprisedSound.Play();

    private void PlayRandomWhine()
    {
        (Random.value > 0.5f ? whineSound1 : whineSound2).Play();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void UpdateSprite(int level)
    {
        int spriteIndex = level - 1;
        if (spriteIndex < evolutionSprites.Length)
        {
            dogImage.sprite = evolutionSprites[spriteIndex];
            dogImage.rectTransform.sizeDelta = new Vector2(
                dogImage.sprite.texture.width, 
                dogImage.sprite.texture.height
            );
        }
        else
        {
            Debug.LogError($"Evolution sprite index out of range: {spriteIndex}");
        }
    }

    private IEnumerator ShakeAndColorEffect(Color color, float shakeAmount, float duration, float interval)
    {
        _isDamaged = true;
        float endTime = Time.time + duration;
        bool flickerState = false;
        Vector3 originalPosition = transform.localPosition;

        while (Time.time < endTime)
        {
            // Color flicker
            dogImage.color = flickerState ? color : Color.white;
            
            // Position shake
            Vector3 randomOffset = new Vector3(
                Random.Range(-shakeAmount, shakeAmount),
                Random.Range(-shakeAmount, shakeAmount),
                0
            );
            transform.localPosition = originalPosition + randomOffset;
            
            flickerState = !flickerState;
            yield return new WaitForSeconds(interval);
        }

        // Reset to original state
        dogImage.color = Color.white;
        transform.localPosition = originalPosition;
        _isDamaged = false;
    }

    private IEnumerator JumpAnimation()
    {
        _isJumping = true;
        float startY = _currentYPosition;
        float elapsedTime = 0f;
        float halfJumpTime = jumpDuration / 2f;

        // Ascend
        while (elapsedTime < halfJumpTime)
        {
            float t = elapsedTime / halfJumpTime;
            _currentYPosition = Mathf.Lerp(startY, startY + maxJumpHeight, t);
            UpdatePosition();
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Descend
        elapsedTime = 0f;
        while (elapsedTime < halfJumpTime)
        {
            float t = elapsedTime / halfJumpTime;
            _currentYPosition = Mathf.Lerp(startY + maxJumpHeight, startY, t);
            UpdatePosition();
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final position is exact
        _currentYPosition = startY;
        UpdatePosition();
        _isJumping = false;
    }

    private void UpdatePosition()
    {
        transform.localPosition = new Vector3(
            transform.localPosition.x, 
            _currentYPosition, 
            transform.localPosition.z
        );
    }
}