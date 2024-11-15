using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// Handles bone visuals and animations
public class BoneView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image boneImage;
    [SerializeField] private Image dogImage;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject scoreContainer;
    [SerializeField] private AudioSource boneThrowSound;

    public void Hide()
    {
        boneImage.enabled = false;
    }

    public bool Hidden()
    {
        return !boneImage.enabled;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public Dictionary<string, float> Show()
    {
        RandomTransform();
        Dictionary<string, float> stimSpec = GetTransform();
        boneImage.enabled = true;
        return stimSpec;
    }

    private void RandomTransform()
    {
        RectTransform boneRectTransform = boneImage.rectTransform;
        
        // Random scale
        float randomScale = Random.Range(0.3f, 0.7f);
        boneRectTransform.localScale = new Vector3(randomScale, randomScale, 0f);
        
        // Random rotation
        boneRectTransform.Rotate(new Vector3(0, 0, Random.Range(0, 360)));
        
        // Random position
        boneRectTransform.localPosition = RandomPosition();
    }

    private Dictionary<string, float> GetTransform()
    {
        RectTransform boneRectTransform = boneImage.rectTransform;
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();

        return new Dictionary<string, float>
        {
            // Canvas
            {"canvasWidth", canvasRectTransform.rect.width},
            {"canvasHeight", canvasRectTransform.rect.height},
            {"canvasScale", canvasRectTransform.localScale.x},
            // Bone
            {"x", boneRectTransform.localPosition.x},
            {"y", boneRectTransform.localPosition.y},
            {"scale", boneRectTransform.localScale.x},
            {"rotation", boneRectTransform.rotation.z}
        };
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private Vector2 RandomPosition()
    {
        // Get component references and calculate offsets
        RectTransform boneRectTransform = boneImage.rectTransform;
        RectTransform dogRectTransform = dogImage.rectTransform;
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        RectTransform scoringRectTransform = scoreContainer.GetComponent<RectTransform>();

        // Calculate bounds and offsets
        float boneOffset = (boneRectTransform.rect.width * boneRectTransform.localScale.x) / 2f;
        float dogOffset = (dogRectTransform.rect.width * dogRectTransform.localScale.x) / 2f;
        float dogLocationX = dogRectTransform.localPosition.x;
        
        // Canvas bounds
        float xBound = canvasRectTransform.rect.width / 2f;
        float yBound = canvasRectTransform.rect.height / 2f;

        // Calculate X position (avoiding dog)
        float randomX;
        if (Random.value < 0.5f)
        {
            // Left side
            float leftStart = -xBound + boneOffset;
            float leftBound = dogLocationX - dogOffset - boneOffset;
            randomX = Random.Range(leftStart, leftBound);
        }
        else
        {
            // Right side
            float rightStart = dogLocationX + dogOffset + boneOffset;
            float rightBound = xBound - boneOffset;
            randomX = Random.Range(rightStart, rightBound);
        }

        // Calculate Y position (below score card)
        float scoringOffset = (scoringRectTransform.rect.height * scoringRectTransform.localScale.y) / 2f;
        float scoringBottom = scoringRectTransform.localPosition.y - scoringOffset;
        float randomY = Random.Range(-yBound + boneOffset, scoringBottom - (boneOffset * 2f));

        return new Vector2(randomX, randomY);
    }

    public void Throw()
    {
        boneThrowSound.Play();
        StartCoroutine(ThrowAnimation());
    }

    public void Eat()
    {
        StartCoroutine(EatAnimation());
    }

    private IEnumerator ThrowAnimation()
    {
        Vector3 initialScale = transform.localScale;
        float duration = 1f;
        float spinSpeed = 360f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, timer / duration);
            transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);
            yield return null;
        }

        transform.localScale = Vector3.zero;
    }

    private IEnumerator EatAnimation()
    {
        Vector3 targetScale = new Vector3(0.1f, 0.1f, 0.1f);
        
        // Calculate mouth position relative to dog
        RectTransform dogRectTransform = dogImage.rectTransform;
        float mouthXFromCentre = ((dogRectTransform.rect.width/2) - 121f) * dogRectTransform.localScale.x;
        float mouthYFromCentre = ((dogRectTransform.rect.height/2) - 219f) * dogRectTransform.localScale.y;
        Vector3 targetPosition = new Vector3(
            dogRectTransform.localPosition.x - mouthXFromCentre,
            dogRectTransform.localPosition.y + mouthYFromCentre,
            0
        );

        float duration = 0.3f;
        float elapsed = 0f;
        Vector3 startPosition = boneImage.rectTransform.localPosition;
        Vector3 startScale = boneImage.rectTransform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            boneImage.rectTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            boneImage.rectTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
            
            yield return null;
        }

        Hide();
    }
}