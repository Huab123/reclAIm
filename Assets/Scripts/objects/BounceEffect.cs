using UnityEngine;
using System.Collections;

public class BounceEffect : MonoBehaviour {
    public float bounceHeight = 0.3f; // Height of the bounce
    public float bounceCount = 3; // Number of bounces
    public float bounceDuration = 0.4f; // Duration of each bounce 

    public void StartBounce() {
        StartCoroutine(BounceHandler());
    }

    private IEnumerator BounceHandler() {
        Vector3 startPosition = transform.position;
        float localHeight = bounceHeight;
        float localDuration = bounceDuration;
        
        for (int i = 0; i < bounceCount; i++) {
            yield return StartCoroutine(Bounce(startPosition, localHeight, localDuration / 2));
            localHeight *= 0.5f; // Reduce height for each bounce
            localDuration *= 0.8f; // Reduce duration for each bounce
        }

        transform.position = startPosition; // Ensure it starts at the original position
    }


    private IEnumerator Bounce(Vector3 start, float height, float duration) {
        Vector3 peak = start + Vector3.up * height;
        float elapsedTime = 0f;

        // Move up to the peak
        while (elapsedTime < duration) {
            transform.position = Vector3.Lerp(start, peak, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        // Move down back to the start 
        while (elapsedTime < duration) {
            transform.position = Vector3.Lerp(peak, start, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
