using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
[CreateAssetMenu(fileName = "SoundLibrarySO", menuName = "SoundLibrarySO")]
public class SoundLibrarySO : ScriptableObject
{
    public List<SoundLibrary> Sounds = new List<SoundLibrary>();
    private Dictionary<ushort, SoundLibrary> _sounds = new Dictionary<ushort, SoundLibrary>();
    public void Init()
    {
        foreach (var sound in Sounds)
        {
            if (!_sounds.ContainsKey(sound.Id))
            {
                _sounds.Add(sound.Id, sound);
            }
        }
    }
    public SoundLibrary GetSound(ushort type)
    {
        if (_sounds.TryGetValue(type, out var sound))
        {
            return sound;
        }
        return default;
    }
    public void Clear() 
    { 
        _sounds.Clear(); 
    }
}
public enum SoundType
{
    BackgroundMusic,
    Enviroment,
    Effect
}
[Serializable]
public struct SoundLibrary
{
    public SoundType Type;
    public ushort Id;
    public AudioClip AudioClip;
}