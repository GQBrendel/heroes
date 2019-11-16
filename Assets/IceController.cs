using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioSource _explosionSound;

    public void BreakTheIce()
    {
        _animator.SetTrigger("Explode");
        _explosionSound.Play(); 
        StartCoroutine(WaitAndDestroy());
    }

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, -0.5f, transform.position.z);
        transform.rotation = Quaternion.identity;
        transform.localScale = transform.localScale * 0.4f;
    }

    private IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
