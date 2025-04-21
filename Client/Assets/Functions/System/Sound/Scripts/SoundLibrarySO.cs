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

    public void Init()
    {
    }

    public async UniTask<SoundLibraryModel> GetSound(string path)
    {
        // Kiểm tra xem âm thanh đã được tải và lưu trong cache chưa
        if (_sounds.TryGetValue(path, out var sound))
        {
            return sound; // Nếu có sẵn, trả về âm thanh từ cache
        }

        // Nếu âm thanh chưa có trong cache, tải từ Addressable
        var handle = Addressables.LoadAssetAsync<AudioClip>("Audios/" + path);
        var audioClip = await handle.ToUniTask();

        // Lưu âm thanh đã tải vào cache
        var newSound = new SoundLibraryModel
        {
            Type = (SoundType)Enum.Parse(typeof(SoundType), audioClip.name.Split('_')[0]),
            AudioClip = audioClip
        };
        _sounds[path] = newSound;

        return newSound;
    }

    public void Clear()
    {
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