using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Globals;
using Newtonsoft.Json;
using Save;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.U2D.IK;
using Utils;

public class SoundManager : MonoBehaviour, IGlobal
{
    //spawner
    [SerializeField] private SoundPlayer soundPlayerPrefab;
    SoundLoader _soundLoader; //NO REF, JUST ADDRESSABLE
    private DynamicObjectPool<SoundPlayer> _pool;

    /// <summary>
    /// To find to stop a loop sound
    /// </summary>
    [SerializeReference]
    private Dictionary<byte /*uniqueKey*/, SoundPlayer> _playingLoopSounds = new Dictionary<byte, SoundPlayer>();

    //events
    private Dictionary<SoundType, float> _volumes;

    /// <summary>
    /// all spawned object
    /// </summary>
    [SerializeReference] private Dictionary<SoundType, List<SoundPlayer>> _spawnedSounds;
    // private readonly Semaphore _concurrent = new Semaphore(1,1); // max 4 sound fx chạy đồng thời

    public async UniTask Init()
    {
        _soundLoader = new SoundLoader();
        if (_volumes == null)
        {
            //Debug.LogError(" _volumes = new");
            _volumes = new Dictionary<SoundType, float>()
            {
                { SoundType.BGM, 1 },
                { SoundType.ENV, 1 },
                { SoundType.FX, 1 },
            };
        }

        if (_spawnedSounds == null)
        {
            //Debug.LogError("_spawnedSounds = new");
            _spawnedSounds = new Dictionary<SoundType, List<SoundPlayer>>()
            {
                { SoundType.BGM, new List<SoundPlayer>() },
                { SoundType.ENV, new List<SoundPlayer>() },
                { SoundType.FX, new List<SoundPlayer>() }
            };
        }

        if (_pool == null)
        {
            //Debug.LogError("_pool = new");
            _pool = new DynamicObjectPool<SoundPlayer>(
                createFunc: CreateSoundPlayer,
                resetAction: ResetSoundPlayer
            );
        }

        var save = new SaveManager();
        //set volume
        // Debug.Log("Loading sound data");
        var sound = await save.Load<SaveSettingSoundModel>();

        SetVolumeAll(SoundType.BGM, sound.MusicVolume);
        SetVolumeAll(SoundType.ENV, sound.EnviromentVolume);
        SetVolumeAll(SoundType.FX, sound.EffectVolume);
    }

    [Button]
    public void SetVolumeAll(SoundType type, float volume)
    {
        _volumes[type] = volume;
        _spawnedSounds[type].FindAll(i => i.gameObject.activeSelf).ForEach(i => i.AudioSource.volume = volume);
    }

    [Button]
    public void SetVolume(AudioSource sound, SoundType type)
    {
        sound.volume = _volumes[type];
    }

    private SoundPlayer CreateSoundPlayer()
    {
        // Instantiate trực tiếp prefab SoundPlayer mà không cần GetComponent
        SoundPlayer soundPlayer = Instantiate(soundPlayerPrefab, transform);
        return soundPlayer;
    }

    private void ResetSoundPlayer(SoundPlayer player)
    {
        player.Reset(); // Reset lại trạng thái của SoundPlayer
    }

    /// <summary>
    /// Multiple and oneshot
    /// </summary>
    /// <param name="id"></param>
    public async void PlaySoundOneShot(string id)
    {
        //Số lượng âm thanh phát cùng lúc không vượt quá 18 (2 là bgm và env), để tránh quá tải và phasing
        if (_spawnedSounds[SoundType.FX]?.Count > 16)
        {
            Debug.LogWarning("Sound player count is over 18, remove first");
            var firstFx = _spawnedSounds[SoundType.FX][0];
            _spawnedSounds[SoundType.FX].Remove(firstFx);
            _pool.Put(firstFx);
        }

        SoundPlayer player = _pool.Get();
        player.gameObject.SetActive(true);
        var lib = await _soundLoader.GetSound(id);
        player.AudioSource.clip = lib.AudioClip;
        player.AudioSource.loop = false;
        player.Id = id;
        player.Type = lib.Type;
        SetVolume(player.AudioSource, lib.Type);
        if (!_spawnedSounds[lib.Type].Contains(player))
        {
            _spawnedSounds[lib.Type].Add(player);
        }

        player.AudioSource.Play();
        Debug.LogError($"Play one shot of type {lib.Type}: {id}");
        // Sau khi âm thanh kết thúc, trả lại vào pool
        StartCoroutine(WaitAndReturnToPool(player));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="uniqueKey">Key là layer, có thể phát cùng lúc nhiều Bgm bằng key khác nhau</param>
    /// <param name="isRestartIfPlaying"></param>
    public async void PlaySoundLoop(string id, byte uniqueKey, bool isRestartIfPlaying = false)
    {
        SoundPlayer player = null;
        if (_playingLoopSounds.TryGetValue(uniqueKey, out var sound))
        {
            if (!isRestartIfPlaying && sound.Id == id) return;
            player = sound;
        }
        else
        {
            player = _pool.Get();
            _playingLoopSounds.Add(uniqueKey, player);
        }

        var lib = await _soundLoader.GetSound(id);
        player.AudioSource.clip = lib.AudioClip;
        player.AudioSource.loop = true;
        player.Id = id;
        player.Type = lib.Type;
        SetVolume(player.AudioSource, lib.Type);
        if (!_spawnedSounds[lib.Type].Contains(player))
        {
            _spawnedSounds[lib.Type].Add(player);
        }

        player.gameObject.SetActive(true);
        player.AudioSource.Play();
    }

    public void StopSoundLoop(byte uniqueKey)
    {
        if (_playingLoopSounds.TryGetValue(uniqueKey, out var player))
        {
            player.AudioSource.Stop();
            _spawnedSounds[player.Type].Remove(player);
            _playingLoopSounds.Remove(uniqueKey);
            _pool.Put(player);
        }
    }

    private IEnumerator WaitAndReturnToPool(SoundPlayer player)
    {
        string originalId = player.Id;
        yield return new WaitForSeconds(player.AudioSource.clip.length);

        _spawnedSounds[player.Type].Remove(player);
        if (player.Id != originalId)
        {
            //Debug.LogError("player đã bị tái sử dụng cho âm thanh khác: " + JsonConvert.SerializeObject(player));
            yield break; // player đã bị tái sử dụng cho âm thanh khác
        }

        _pool.Put(player);
    }
}