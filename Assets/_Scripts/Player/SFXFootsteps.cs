using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class SFXFootsteps : MonoBehaviour
{
    [SerializeField] private List<AudioClip> _footstepPool;
    private AudioSource _source;

    private void Awake()
    {
        _source = this.GetComponent<AudioSource>();
    }

    private AudioClip GetRandomClip()
    {
        if (_footstepPool.Count == 0)
        {
            throw new IndexOutOfRangeException("Empty sound list, smartypants");
        }
        
        int random = Random.Range(0, _footstepPool.Count);

        return _footstepPool[random];
    }

    public void PlayRandomStepSound()
    {
        _source.clip = GetRandomClip();
        _source.Play();
    }
}
