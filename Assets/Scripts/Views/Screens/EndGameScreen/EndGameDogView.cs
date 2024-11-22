using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndGameDogView : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private RectTransform dogImage;
    [SerializeField] private AudioSource dogSound;

    [Header("Animation Settings")]
    [SerializeField] private float jumpHeight = 50f;
    [SerializeField] private float jumpDuration = 0.5f;
    [SerializeField] private float jumpDelay = 0.5f;
    [SerializeField] private float jumpSpeed = 1f;
    [SerializeField] private AnimationCurve jumpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 _originalPosition;
    private bool _isJumping = true;
    private Coroutine _currentJumpRoutine;

    private void Awake()
    {
        _originalPosition = dogImage.localPosition;
    }

    private void Start()
    {
        StartJumping();
    }

    public void PlaySound() => dogSound.Play();

    public void StartJumping()
    {
        _isJumping = true;
        if (_currentJumpRoutine == null)
        {
            _currentJumpRoutine = StartCoroutine(JumpLoop());
        }
    }

    public void StopJumping()
    {
        _isJumping = false;
        if (_currentJumpRoutine != null)
        {
            StopCoroutine(_currentJumpRoutine);
            _currentJumpRoutine = null;
        }
        // Return to original position
        dogImage.localPosition = _originalPosition;
    }

    private IEnumerator JumpLoop()
    {
        while (_isJumping)
        {
            yield return StartCoroutine(JumpUp());
            yield return StartCoroutine(JumpDown());
            yield return new WaitForSeconds(jumpDelay);
        }
    }

    private IEnumerator JumpUp()
    {
        float elapsedTime = 0f;
        Vector3 startPosition = dogImage.localPosition;
        Vector3 targetPosition = startPosition + Vector3.up * jumpHeight;
        float adjustedDuration = jumpDuration / jumpSpeed;

        while (elapsedTime < adjustedDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / adjustedDuration;
            float curveValue = jumpCurve.Evaluate(normalizedTime);
            
            dogImage.localPosition = Vector3.Lerp(startPosition, targetPosition, curveValue);
            yield return null;
        }

        dogImage.localPosition = targetPosition;
    }

    private IEnumerator JumpDown()
    {
        float elapsedTime = 0f;
        Vector3 startPosition = dogImage.localPosition;
        float adjustedDuration = jumpDuration / jumpSpeed;

        while (elapsedTime < adjustedDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / adjustedDuration;
            float curveValue = jumpCurve.Evaluate(normalizedTime);
            
            dogImage.localPosition = Vector3.Lerp(startPosition, _originalPosition, curveValue);
            yield return null;
        }

        dogImage.localPosition = _originalPosition;
    }

    private void OnDisable()
    {
        StopJumping();
    }
}