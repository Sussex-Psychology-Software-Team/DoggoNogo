using System.Collections;
using UnityEngine;

public class EndDog : MonoBehaviour
{
    public RectTransform uiImage; // The RectTransform of the UI image
    public float jumpHeight = 50f; // Height of the jump
    public float jumpDuration = 0.5f; // Duration of one jump cycle
    public float jumpDelay = 0.5f; // Delay between jumps
    public float jumpSpeed = 1f; // Speed multiplier for the jump

    Vector3 originalPosition;
    bool isJumping = true; // Controls whether the object is jumping

    public AudioSource endDogNoise;

    void Start()
    {
        // Store the initial local position (relative to parent)
        originalPosition = uiImage.localPosition;

        // Start the jump coroutine
        StartCoroutine(JumpLoop());
    }

    public void makeNoise(){
        endDogNoise.Play();
    }
    public void ToggleJump(bool enable)
    {
        isJumping = enable;
        if (isJumping)
        {
            StartCoroutine(JumpLoop()); // Restart jump if turned on
        }
    }

    IEnumerator JumpLoop()
    {
        while (isJumping) // Loop continues as long as jumping is enabled
        {
            // Move up
            yield return StartCoroutine(JumpUp());

            // Move down
            yield return StartCoroutine(JumpDown());

            // Wait for the next jump
            yield return new WaitForSeconds(jumpDelay);
        }
    }

    IEnumerator JumpUp()
    {
        float timeElapsed = 0f;
        Vector3 startPosition = uiImage.localPosition;
        Vector3 targetPosition = new Vector3(startPosition.x, startPosition.y + jumpHeight, startPosition.z);

        while (timeElapsed < jumpDuration / jumpSpeed) // Adjusting the duration by jumpSpeed
        {
            timeElapsed += Time.deltaTime;

            // Lerp the position upwards (Y-axis only)
            uiImage.localPosition = new Vector3(startPosition.x, Mathf.Lerp(startPosition.y, targetPosition.y, timeElapsed / (jumpDuration / jumpSpeed)), startPosition.z);

            yield return null; // Wait for the next frame
        }
    }

    IEnumerator JumpDown()
    {
        float timeElapsed = 0f;
        Vector3 startPosition = uiImage.localPosition;
        Vector3 targetPosition = originalPosition;

        while (timeElapsed < jumpDuration / jumpSpeed) // Adjusting the duration by jumpSpeed
        {
            timeElapsed += Time.deltaTime;

            // Lerp the position downwards (Y-axis only)
            uiImage.localPosition = new Vector3(startPosition.x, Mathf.Lerp(startPosition.y, targetPosition.y, timeElapsed / (jumpDuration / jumpSpeed)), startPosition.z);

            yield return null; // Wait for the next frame
        }
    }
}
