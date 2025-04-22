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
    private Dictionary<string, SoundLibraryModel> _sounds = new();
    private Dictionary<string, AsyncOperationHandle<AudioClip>> _handles = new(); // Thêm dòng này
    public void Init()
    {
    }

    public async UniTask<SoundLibraryModel> GetSound(string path)
    {
        if (_sounds.TryGetValue(path, out var sound))
        {
            return sound;
        }

        var handle = Addressables.LoadAssetAsync<AudioClip>("Audios/" + path);
        var audioClip = await handle.ToUniTask();

        var newSound = new SoundLibraryModel
        {
            Type = (SoundType)Enum.Parse(typeof(SoundType), audioClip.name.Split('_')[0]),
            AudioClip = audioClip
        };

        _sounds[path] = newSound;
        _handles[path] = handle; // Lưu handle để sau này release

        return newSound;
    }
    public void ReleaseAll()
    {
        foreach (var handle in _handles.Values)
        {
            Addressables.Release(handle);
        }

        _handles.Clear();
        _sounds.Clear();
    }
}

public enum SoundType
{
    BGM,
    ENV,
    FX
}

[Serializable]
public class SoundLibraryModel
{
    public SoundType Type;
    public AudioClip AudioClip; // Lưu AudioClip sau khi tải từ Addressable
}