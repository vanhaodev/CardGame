using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using Globals;
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
    SoundLibrarySO _soundLibrarySO; //NO REF, JUST ADDRESSABLE
    private DynamicObjectPool<SoundPlayer> _pool;

    /// <summary>
    /// To find to stop a loop sound
    /// </summary>
    private Dictionary<byte /*uniqueKey*/, SoundPlayer> _playingLoopSounds = new Dictionary<byte, SoundPlayer>();

    //events
    private Dictionary<SoundType, float> _volumes;

    /// <summary>
    /// all spawned object
    /// </summary>
    private Dictionary<SoundType, List<SoundPlayer>> _spawnedSounds;

    public async UniTask Init()
    {
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

        if (_soundLibrarySO == null)
        {
            //Debug.LogError("_soundLibrarySO = await");
            // Sử dụng AddressableLoader để tải ScriptableObject
            _soundLibrarySO = await Global.Instance.Get<AddressableLoader>()
                .LoadAssetAsync<SoundLibrarySO>("SoundLibrarySO.asset");
            _soundLibrarySO.Init();
        }
        var save = new SaveManager();
        //set volume
        Debug.Log("Loading sound data");
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
        SoundPlayer player = _pool.Get();
        player.gameObject.SetActive(true);
        var lib = await _soundLibrarySO.GetSound(id);
        player.AudioSource.clip = lib.AudioClip;
        player.AudioSource.loop = false;
        player.Id = id;
        player.Type = lib.Type;
        SetVolume(player.AudioSource, lib.Type);
        _spawnedSounds[lib.Type].Add(player);
        player.AudioSource.Play();

        // Sau khi âm thanh kết thúc, trả lại vào pool
        StartCoroutine(WaitAndReturnToPool(player));
    }

    /// <summary>
    /// Single and loop
    /// </summary>
    /// <param name="id"></param>
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

        var lib = await _soundLibrarySO.GetSound(id);
        player.AudioSource.clip = lib.AudioClip;
        player.AudioSource.loop = true;
        player.Id = id;
        player.Type = lib.Type;
        SetVolume(player.AudioSource, lib.Type);
        _spawnedSounds[lib.Type].Add(player);
        player.AudioSource.Play();
    }

    public void StopSoundLoop(byte uniqueKey)
    {
        if (_playingLoopSounds.TryGetValue(uniqueKey, out var player))
        {
            player.AudioSource.Stop();
            _spawnedSounds[player.Type].Remove(player);
            _pool.Put(player);
        }
    }

    private IEnumerator WaitAndReturnToPool(SoundPlayer player)
    {
        // Đợi cho âm thanh chơi xong
        yield return new WaitForSeconds(player.AudioSource.clip.length);
        _spawnedSounds[player.Type].Remove(player);
        // Sau khi âm thanh kết thúc, trả lại pool
        _pool.Put(player);
    }
}