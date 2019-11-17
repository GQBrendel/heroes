using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopUp : MonoBehaviour
{
    [SerializeField, Range(0,5)] private float _dissapearTimer;

    [SerializeField] private Color _criticalColor;


    public float DISAPEAR_TIMER_MAX = .2f;
    public float _disapearSpeed = .2f;

    private Color _normalColor;
    private Color _textColor;
    private TextMeshPro _textMesh;

    private Vector3 moveVector;

    public static DamagePopUp Create(GameObject damagePopUpPrefab, Vector3 position, int damageAmount, bool isCritical)
    {
        DamagePopUp damagePopUp = Instantiate(damagePopUpPrefab, position, Quaternion.identity).GetComponent<DamagePopUp>();
        damagePopUp.Setup(damageAmount, isCritical);

        return damagePopUp;
    }

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshPro>();
        _textColor = _textMesh.color;
        _normalColor = _textColor;

        moveVector = new Vector3(.7f, 1.4f) * .6f;
    }

    public void Setup(int damageAmount, bool isCritical)
    {
        _textMesh.SetText(damageAmount.ToString());

        if (isCritical)
        {
            _textMesh.fontSize = 2.5f;
            _textColor = _criticalColor;
        }
        else
        {
            _textMesh.fontSize = 2f;
            _textColor = _normalColor;
        }
        _dissapearTimer = DISAPEAR_TIMER_MAX;
        _textMesh.color = _textColor;
    }

    private void Update()
    {
        Vector3 v = Camera.main.transform.position - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt(Camera.main.transform.position - v);
        transform.Rotate(0, 180, 0);

        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 1f * Time.deltaTime;


        _dissapearTimer -= Time.deltaTime;

        if(_dissapearTimer > DISAPEAR_TIMER_MAX * 0.5f)
        {
            float increaseScaleAmount = 1f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        }
        else
        {
            float decreaseSceleAmount = 0.4f;
            transform.localScale -= Vector3.one * decreaseSceleAmount * Time.deltaTime;
        }

        if (_dissapearTimer < 0)
        {
            _textColor.a -= _disapearSpeed * Time.deltaTime;
            _textMesh.color = _textColor;
            if(_textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
