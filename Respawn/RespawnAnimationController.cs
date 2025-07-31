using UnityEngine;
using System.Collections;

public class RespawnAnimationController : MonoBehaviour
{
    public float respawnAnimationDuration = 1f;

    private Animator anim;

    void Awake()
    {
        // Works even if Animator is on a child
        anim = GetComponentInChildren<Animator>();
        if (anim == null)
            Debug.LogWarning("RespawnAnimationController: Animator component missing.");
    }

    public IEnumerator PlayRespawnStart()
    {
        Debug.Log("RespawnAnimationController: PlayRespawnStart");
        if (anim != null)
        {
            anim.ResetTrigger("RespawnEnd");
            anim.SetTrigger("RespawnStart");
            yield return new WaitForSeconds(respawnAnimationDuration);
        }
        Destroy(gameObject);
    }

    public IEnumerator PlayRespawnEnd()
    {
        Debug.Log("RespawnAnimationController: PlayRespawnEnd");
        if (anim != null)
        {
            anim.ResetTrigger("RespawnStart");
            anim.SetTrigger("RespawnEnd");
            yield return new WaitForSeconds(respawnAnimationDuration);
        }
        Destroy(gameObject);
    }
}
