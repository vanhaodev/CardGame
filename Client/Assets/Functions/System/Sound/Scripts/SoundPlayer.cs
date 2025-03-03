using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] AudioSource _audioSource;
    public int Id;
    public SoundType Type;
    public AudioSource AudioSource { get => _audioSource; set => _audioSource = value; }

    public void Reset()
    {
        AudioSource.Stop();
        AudioSource.clip = null;
        gameObject.SetActive(false);
    }
}
