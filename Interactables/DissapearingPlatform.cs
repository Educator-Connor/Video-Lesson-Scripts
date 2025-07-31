using UnityEngine;
using System.Collections;

public class DisappearingPlatform : MonoBehaviour
{
    public GameObject platformVisualAndCollider;
    public float waitBeforeDisappear = 1f;
    public float disappearDuration = 3f;

    private bool isPlayerOnPlatform = false;
    private Coroutine disappearRoutine;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Collision with: {collision.collider.name}, tag: {collision.collider.tag}");
        if (collision.collider.CompareTag("Player") && !isPlayerOnPlatform)
        {
            Debug.Log("Player landed on platform - starting disappear cycle.");
            isPlayerOnPlatform = true;
            disappearRoutine = StartCoroutine(DisappearCycle());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Player left platform - stopping disappear cycle.");
            isPlayerOnPlatform = false;
            if (disappearRoutine != null)
                StopCoroutine(disappearRoutine);

            SetPlatformActive(true);
        }
    }

    private IEnumerator DisappearCycle()
    {
        yield return new WaitForSeconds(waitBeforeDisappear);

        SetPlatformActive(false);

        yield return new WaitForSeconds(disappearDuration);

        SetPlatformActive(true);

        if (isPlayerOnPlatform)
        {
            disappearRoutine = StartCoroutine(DisappearCycle());
        }
    }

    private void SetPlatformActive(bool active)
    {
        platformVisualAndCollider.SetActive(active);
        Debug.Log($"Platform set active: {active}");
    }
}
