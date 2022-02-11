using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField, Range(0.1f, 10.0f)] private float lifetime = 5f;
    private float timer;
    void Start()
    {
        timer = lifetime;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            Destroy(this.gameObject);
        }
    }
}
