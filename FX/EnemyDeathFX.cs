using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathFX : MonoBehaviour
{
    void Start()
    {
        Animator animator = GetComponent<Animator>();
        float time = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, time);
    }
}