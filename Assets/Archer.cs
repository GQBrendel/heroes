using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : MonoBehaviour
{
    [SerializeField] private ParticleSystem _icyTrailParticles;
    [SerializeField] private GameObject _iceArrowPrefab;
    [SerializeField] private Transform _iceArrowOrigin;
    [SerializeField] private GameObject _effectOnCollision;

    private RFX4_PhysicsMotion _effect;

    private void Start()
    {
        _icyTrailParticles.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        { 
            CastIceArrow();
        }
    }

    public void CastIceArrow()
    {
        var effectObject = Instantiate(_iceArrowPrefab, _iceArrowOrigin.position, _iceArrowOrigin.rotation);
        _effect = effectObject.GetComponentInChildren<RFX4_PhysicsMotion>();
        _effect.OnEffectCollide += HandleEffectCollided;

    }
    private void HandleEffectCollided()
    {
        if(GetComponent<HeroController>().CurrentEnemy == null)
        {
            GetComponent<HeroController>().CurrentEnemy = FindObjectOfType<Enemy>();

        }
        Vector3 spawnPosition = GetComponent<HeroController>().CurrentEnemy.transform.position;
        var instance = Instantiate(_effectOnCollision, spawnPosition, new Quaternion()) as GameObject;
    }

    public void TurnOnParticles()
    {
        _icyTrailParticles.gameObject.SetActive(true);
    }
    public void TurnOffParticles()
    {
        _icyTrailParticles.gameObject.SetActive(false);
    }
}
