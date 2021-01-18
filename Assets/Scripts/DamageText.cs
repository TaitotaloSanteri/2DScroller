using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public TextMeshPro text;
    private float xSpeed, ySpeed, fadeSpeed;

    private void Awake()
    {
        xSpeed = Random.Range(-1.5f, 1.5f);
        ySpeed = Random.Range(2f, 3.5f);
        fadeSpeed = Random.Range(0.3f, 0.6f);
        Destroy(gameObject, 3f);
    }

    private void Update()
    {
        transform.position += (Vector3.right * xSpeed + Vector3.up * ySpeed) * Time.deltaTime;
        text.color = new Color(text.color.r, text.color.g, 
                               text.color.b, text.color.a - fadeSpeed * Time.deltaTime);
    }
}
