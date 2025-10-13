using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public class PersistentMusic : MonoBehaviour
{
    [System.Serializable]
    public class ScenePlaylist
    {
        [Tooltip("Nome exato da cena (SceneManager). Ex.: MainMenu, CharacterSelect, Fight")]
        public string sceneName;

        [Tooltip("Faixas disponíveis para essa cena")]
        public List<AudioClip> clips = new();

        [Header("Comportamento")]
        public bool randomOrder = false;     // Fight: true
        public bool loopPlaylist = true;     // se false, toca só uma e para

        [Header("Volume/Fade")]
        [Range(0f, 1f)] public float volume = 1f;
        public float fadeIn = 0.6f;
        public float fadeOut = 0.4f;
    }

    [Header("Mixer (opcional)")]
    public AudioMixerGroup musicGroup;       // arraste seu grupo "Music" (se usar Mixer + Snapshots)

    [Header("Playlists por cena")]
    public List<ScenePlaylist> playlists = new();

    AudioSource _src;
    ScenePlaylist _currentList;
    Coroutine _runner;
    int _lastIndex = -1;

    static PersistentMusic _instance;

    void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
    _instance = this;

    _src = GetComponent<AudioSource>();
    _src.playOnAwake = false;
    _src.loop = false;
    if (musicGroup) _src.outputAudioMixerGroup = musicGroup;

    DontDestroyOnLoad(gameObject);
    SceneManager.activeSceneChanged += OnSceneChanged;

    SelectAndPlayForScene(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    void OnSceneChanged(Scene prev, Scene next)
    {
        SelectAndPlayForScene(next.name);
    }

    // ===== API =====

    public void SelectAndPlayForScene(string sceneName)
    {
        var list = playlists.Find(p => p.sceneName == sceneName);
        _currentList = list;

        if (_runner != null) StopCoroutine(_runner);
        _runner = StartCoroutine(RunMusic());
    }

    /// <summary>Adiciona faixas à playlist da Fight em runtime (opcional).</summary>
    public void AddFightTracks(params AudioClip[] extra)
    {
        var list = playlists.Find(p => p.sceneName == "Fight");
        if (list == null) return;
        foreach (var c in extra) if (c && !list.clips.Contains(c)) list.clips.Add(c);
        // se já estamos na Fight e nada tocando, retoma
        if (_currentList == list && !_src.isPlaying && _runner == null)
            _runner = StartCoroutine(RunMusic());
    }

    // ===== Loop principal =====

    IEnumerator RunMusic()
    {
        // fade out da faixa anterior (se trocou de cena no meio)
        yield return StartCoroutine(FadeOutThenStop());

        if (_currentList == null || _currentList.clips == null || _currentList.clips.Count == 0)
            yield break;

        // configura volume alvo da playlist
        float targetVol = Mathf.Clamp01(_currentList.volume);

        do
        {
            // escolhe a próxima faixa
            int next = 0;
            if (_currentList.randomOrder)
            {
                // evita repetir a mesma imediatamente
                if (_currentList.clips.Count > 1)
                {
                    do { next = Random.Range(0, _currentList.clips.Count); }
                    while (next == _lastIndex);
                }
                else next = 0;
            }
            else
            {
                next = (_lastIndex + 1 + _currentList.clips.Count) % _currentList.clips.Count;
            }
            _lastIndex = next;

            var clip = _currentList.clips[next];
            if (!clip) { yield return null; continue; }

            _src.clip = clip;
            _src.volume = 0f;
            _src.Play();

            // fade in
            yield return StartCoroutine(FadeTo(targetVol, _currentList.fadeIn));

            // espera terminar
            while (_src.isPlaying) yield return null;

            // laço continua se loopPlaylist = true
        } while (_currentList != null && _currentList.loopPlaylist);
        
        // fim: fade out por garantia
        yield return StartCoroutine(FadeOutThenStop());
        _runner = null;
    }

    // ===== util de fade (um único AudioSource) =====

    IEnumerator FadeOutThenStop()
    {
        if (_src.isPlaying)
            yield return StartCoroutine(FadeTo(0f, _currentList != null ? _currentList.fadeOut : 0.3f));
        _src.Stop();
        _src.clip = null;
    }

    IEnumerator FadeTo(float target, float seconds)
    {
        float t = 0f;
        float start = _src.volume;
        if (seconds <= 0f) { _src.volume = target; yield break; }

        while (t < seconds)
        {
            t += Time.unscaledDeltaTime; // respeita pausas do jogo
            _src.volume = Mathf.Lerp(start, target, t / seconds);
            yield return null;
        }
        _src.volume = target;
    }
}
