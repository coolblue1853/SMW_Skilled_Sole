using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sounds
{
    public string name;
    public List<AudioClip> clips; 
}
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("Clips")]
    [SerializeField] private List<Sounds> _bgmEntries;
    [SerializeField] private List<Sounds> _sfxEntries;

    private Dictionary<string, AudioClip> _bgmDict = new();
    private Dictionary<string, List<AudioClip>> _sfxDict = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitDict();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        PlayBGM("Bgm");
    }

    private void InitDict()
    {
        foreach (var entry in _bgmEntries)
        {
            if (entry.clips.Count > 0 && !_bgmDict.ContainsKey(entry.name))
            {
                _bgmDict.Add(entry.name, entry.clips[0]); 
            }
        }
        foreach (var entry in _sfxEntries)
        {
            if (!_sfxDict.ContainsKey(entry.name))
            {
                _sfxDict.Add(entry.name, entry.clips);
            }
        }
    }

    // 배경음악 재생
    public void PlayBGM(string name)
    {
        if (_bgmDict.TryGetValue(name, out var clip))
        {
            if (_bgmSource.clip != clip)
            {
                _bgmSource.clip = clip;
                _bgmSource.loop = true;
                _bgmSource.Play();
            }
        }
    }

    // 효과음 재생
    public void PlaySFX(string name, Transform pos)
    {
        if (_sfxDict.TryGetValue(name, out var clips) && clips.Count > 0)
        {
            var randomClip = clips[Random.Range(0, clips.Count)];
            var sfx = Instantiate(_sfxSource, pos);
            sfx.clip = randomClip;
            sfx.Play();
            Destroy(sfx.gameObject, randomClip.length);
        }
    }


}
