using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour {

    public GameObject audioListenerObject;
    
    //From resources folder//
    public string seAudioClipPath = "AudioClips/SE";
    public string bgmAudioClipPath = "AudioClips/BGM";
    public string se3dAudioClipPath = "AudioClips/SE3D";
    public string jingledAudioClipPath = "AudioClips/JINGLE";
    public int seMaxNum = 5;//SEの同時再生上限数//

    private static AudioSource jingleAudioSource = new AudioSource();
    private static List<AudioSource> seAudioSourceList = new List<AudioSource>();
    private static List<AudioSource> bgmAudioSourceList = new List<AudioSource>();

    public AudioMixer mixer;
    private static AudioMixer _mixer;
    public AudioMixerGroup mixerSE;
    public AudioMixerGroup mixerBGM;
    public AudioMixerGroup mixerJINGLE;
    public AudioMixerGroup mixerVOICE;

    //読み込むサウンドデータのリスト//
    private static List<AudioClip> seAudioClips = new List<AudioClip>();
    private static List<AudioClip> se3dAudioClips = new List<AudioClip>();
    private static List<AudioClip> bgmAudioClips = new List<AudioClip>();
    private static List<AudioClip> jingleAudioClips = new List<AudioClip>();

    //public static AudioSource currentBGMAudioSource;//再生中のAudioSourceの参照//
    //public static AudioSource fadeOutingBGMAudioSource;

    public bool isDebugMuteMode = false;

    public bool IsPaused {get; private set;}

    public bool enableDebugMonitor = false;
    private static bool _enableDebugMonitor;

    private static GameObject thisGameObject;
    private const float DEFAULT_FADETIME = 2f;
    private static Action jingleCallback;

    //public AnimationCurve bgmFadeIn = AnimationCurve.Linear(0, 0, 1, 1);

    private enum BGMState {Stop, Playing, Pausing, CrossFade, FadeOut,FadeIn }
    [SerializeField]
    private BGMState currentBGMState = BGMState.Stop;

    void OnValidate()
    {
        if (enableDebugMonitor)
        {
            _enableDebugMonitor = enableDebugMonitor;
        }


        //メインカメラからAudioListenerを剥がす//
        /*
        GameObject mainCamera = Camera.main.gameObject;
        mainCamera.GetComponent<AudioListener>().enabled = false;     //エディターモードではデストロイが呼べないため無効にするだけ//

        if (GameObject.Find("AudioListener") == false)
        {
            audioListenerObject = new GameObject("AudioListener");
            audioListenerObject.AddComponent<AudioListener>();
            DontDestroyOnLoad(audioListenerObject);
        }
        */
    }

    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (SoundManager)FindObjectOfType(typeof(SoundManager));

                if (instance == null)
                {
                    Debug.LogError(typeof(SoundManager) + "is nothing");
                }
            }

            return instance;
        }
    }

    void Awake()
    {
        thisGameObject = this.gameObject;
        DontDestroyOnLoad(thisGameObject);

        _mixer = mixer;

        //リソース読み込み（同期読み）//
        seAudioClips = LoadAudioClipsFromResourcesFolder(seAudioClipPath);
        bgmAudioClips = LoadAudioClipsFromResourcesFolder(bgmAudioClipPath);
        se3dAudioClips = LoadAudioClipsFromResourcesFolder(se3dAudioClipPath);
        jingleAudioClips = LoadAudioClipsFromResourcesFolder(jingledAudioClipPath);

        //オーディオソースの生成//
        bgmAudioSourceList = InstantiateAudioSourceList(2, true);
        seAudioSourceList = InstantiateAudioSourceList(seMaxNum, false);

        jingleAudioSource = this.gameObject.AddComponent<AudioSource>() as AudioSource;
        jingleAudioSource.loop = false;

        SetAudioSourceListToMixer(bgmAudioSourceList, mixerBGM);
        SetAudioSourceListToMixer(seAudioSourceList, mixerSE);
        jingleAudioSource.outputAudioMixerGroup = mixerJINGLE;
    }

    public  void SetAudioListenerFollower(Transform cameraTransform)
    {
        // attach audio listner to camera
        audioListenerObject.transform.parent = cameraTransform;

        // align audio listener to camera (do after parent)
        audioListenerObject.transform.localPosition = Vector3.zero;
        audioListenerObject.transform.localRotation = Quaternion.identity;

    }

    public void ClearAudioListenerPos()
    {
        audioListenerObject.transform.parent = thisGameObject.transform;

        audioListenerObject.transform.localPosition = Vector3.zero;
        audioListenerObject.transform.localRotation = Quaternion.identity;
    }

    /*
    public static void PlaySE(Constants.Const_SoundData soundName)
    {
        Constants.Const_SoundData.fieldList.
        PlaySE(clipName, _fixedSeVolume);
    }
     */

    public static void PlaySE(string clipName)
    {
        PlaySE(clipName, 1f);
    }

    public static void PlaySE(string clipName, float volume)
    {
        //空いているaudioSourceはあるか？//
        AudioSource _audioSource = GetStoppedAudioSoureFromList(seAudioSourceList);

        if (_audioSource == null) { Debug.LogWarning("No Audio Source"); return; }

        //clipNameのaudioClipはロードされているか？//
        _audioSource.clip = GetAudioClipFromLoadedList(clipName, seAudioClips);

        _audioSource.volume = volume;
        _audioSource.Play();

        if (_enableDebugMonitor) Debug.Log(clipName + "が再生されました。");
    }

    public void OnPause()
    {
        IsPaused = true;
        seAudioSourceList.ForEach(ase => ase.Pause());

        OnBGMPause();
    }

    public void OnBGMPause()
    {
        jingleAudioSource.Pause();
        bgmAudioSourceList.ForEach(source => source.Pause());

        
        //フェードアウトしてからポーズ//
    }

    void Update()
    {
        switch (currentBGMState)
        {
            case BGMState.Stop:
                break;
            case BGMState.Playing:
                break;
            case BGMState.CrossFade:
                break;
            case BGMState.FadeOut:
                break;
            case BGMState.FadeIn:

                break;
            default:
                break;
        }
    }

    public void OnResume()
    {
        IsPaused = false;
        seAudioSourceList.ForEach(ase => ase.UnPause());

        OnBGMResume();
    }

    public void OnBGMResume()
    {
        jingleAudioSource.UnPause();
        bgmAudioSourceList.ForEach(source => source.UnPause());
    }

    public static void PlayBGM(string clipName)
    {
        PlayBGMWithFade(clipName, 0.1f);
    }

    static void PlayBGMWithFade(string clipName, float fadeTime = DEFAULT_FADETIME)
    {
        if (fadeTime == 0f)
            fadeTime = DEFAULT_FADETIME;

        StopBGMWithFade(fadeTime);//currentを止め始める//

        AudioSource m_AudioSource = bgmAudioSourceList.First(source => !source.isPlaying);
        m_AudioSource.clip = GetAudioClipFromLoadedList(clipName, bgmAudioClips);
        m_AudioSource.Play();

    }

    public static void StopBGM()
    {
        StopBGMWithFade(0.1f);
    }

    public static void StopBGMWithFade(float fadeTime)
    {
        if (fadeTime == 0f)
            fadeTime = DEFAULT_FADETIME;

        foreach(AudioSource audioSource in bgmAudioSourceList)
        {
            if(audioSource.isPlaying)
            {
                audioSource.Stop();                
            }
        }
        
    }



    #region DebugMonitor

    void OnGUI()
    {
        if (!_enableDebugMonitor)
            return;

        GUI.Label(new Rect(0, 0, 600, 30), "Audio Monitor");

        for (int i = 0; i < bgmAudioSourceList.Count; i++)
        {
            if (bgmAudioSourceList[i] != null)
            {
                string setBGMName = bgmAudioSourceList[i].clip == null ? "" : bgmAudioSourceList[i].clip.name;
                GUI.Label(new Rect(0, 10*(i+1), 600, 30), "BGMSource "+i+" "+
                    setBGMName
                    + " :isPlaying "+
                    bgmAudioSourceList[i].isPlaying);
            }
        }

        if (jingleAudioSource.clip != null)
        {
            GUI.Label(new Rect(0, 30, 600, 30), "Jingle " + jingleAudioSource.clip.name + ":isPlaying " + jingleAudioSource.isPlaying);
        }
    }

    #endregion

    #region Util

    List<AudioSource> InstantiateAudioSourceList(int num, bool isLoop)
    {
        List<AudioSource> _audioSourceList = new List<AudioSource>();

        for (int i = 0; i < num; i++)
        {
            AudioSource _audioSource = this.gameObject.AddComponent<AudioSource>() as AudioSource;
            _audioSource.playOnAwake = false;
            _audioSource.loop = isLoop;
            _audioSourceList.Add(_audioSource);
        }

        return _audioSourceList;
    }

    static void SetAudioSourceListToMixer(List<AudioSource> audioSourceList, AudioMixerGroup mixerGroup)
    {
        //Debug.Log("bgm source num "+ audioSourceList.Count);
        audioSourceList.ForEach(ase => ase.outputAudioMixerGroup = mixerGroup);
    }

    static AudioSource GetStoppedAudioSoureFromList(List<AudioSource> audioSourceList)
    {
        AudioSource _audioSource = audioSourceList.FirstOrDefault(ase => ase.isPlaying == false);
        if (_audioSource == null)
        {
            if (_enableDebugMonitor)
                Debug.LogWarning("audioSourceSEの再生上限です。");
        }
        return _audioSource;
    }

    public static AudioClip GetSEAudioClipFromLoadedList(string clipName)
    {
        return GetAudioClipFromLoadedList(clipName, seAudioClips);
    }

    private static AudioClip GetAudioClipFromLoadedList(string clipName, List<AudioClip> clipList)
    {
        AudioClip _clip = clipList.Find(i => i.name == clipName);

        if (_clip == null)
        {
            if (_enableDebugMonitor)
                Debug.LogWarning("オーディオクリップ「" + clipName + "」は読み込まれていません。");
        }

        return _clip;
    }

    public static AudioClip GetAudioClipLoadedSe3d(string clipName)
    {
        return GetAudioClipFromLoadedList(clipName, se3dAudioClips);
    }


    private List<AudioClip> LoadAudioClipsFromResourcesFolder(string path)
    {
        return Resources.LoadAll(path, typeof(AudioClip)).Cast<AudioClip>().ToList();

    }

    //Resoucesフォルダ内のパスを列挙する方法が見つかったら使用する//
    IEnumerator LoadAsyncAudioClip()
    {
        ResourceRequest request = Resources.LoadAsync("AudioClips/SE/Footstep_L");
        yield return request;
        seAudioClips.Add(request.asset as AudioClip);
    }


    #endregion

    public static void StopAllSE()
    {
        seAudioSourceList.ForEach(ase => ase.Stop());
    }

    public static int AddIntVolumeToMaster(int amount)
    {
        int vol = GetIntVolumeFromMixerGroup("Master") + amount;
        vol = Mathf.Min(10,Mathf.Max(0,vol));

        SetIntVolumeToMixerGroup("Master", vol);
        return vol;
    }

    public static int IntVolumeOfMaster
    {
        get
        {
            return GetIntVolumeFromMixerGroup("Master");
        }
        set
        {
            SetIntVolumeToMixerGroup("Master",value);
        }
    }

    static void SetIntVolumeToMixerGroup(string mixerGroupName, int volume)
    {
        float fvolume = (float)volume * 0.1f;
        //Debug.Log("volume float" + fvolume);

        float dvolume =  Mathf.Lerp(-80, 0, fvolume);
        //Debug.Log("volume decivel" + dvolume);

        _mixer.SetFloat(mixerGroupName, dvolume);
    }

    static int GetIntVolumeFromMixerGroup(string mixerGroupName)
    {
        float volume;
        _mixer.GetFloat(mixerGroupName, out volume);
        //Debug.Log("volume decivel" + volume);
        volume =  Mathf.InverseLerp(-80, 0, volume) * 10f;
        //Debug.Log("volume float x10: " + volume);

        return (int)volume ;
    }


    public static void PlayJingle(string jingleName)
    {
        if (jingleAudioSource.isPlaying)//不要かも//
        {
            jingleAudioSource.Stop();
        }

        if (jingleAudioSource.volume == 0f)
        {
            jingleAudioSource.volume = 1f;
        }

        jingleAudioSource.clip = GetAudioClipFromLoadedList(jingleName,jingleAudioClips);
        jingleAudioSource.Play();
    }

    public static void StopJingleWithFade(float fadeTime)
    {
        if (fadeTime == 0f)
            fadeTime = DEFAULT_FADETIME;

        if (!jingleAudioSource.isPlaying)
            return;

        /*
        iTween.AudioTo(thisGameObject, iTween.Hash(
            "audiosource", jingleAudioSource,
            "time", 1f,//秒かけて//
            "volume", 0f,//ボリュームをゼロに//
            "oncomplete", "OnJingleFadeOutComplete",
            "oncompletetarget", thisGameObject
        ));
        */
    }

    public static void StopJingleWithFade(float fadeTime, Action callback)
    {
        jingleCallback = callback;
        StopJingleWithFade(fadeTime);
    }


    //staticにすると呼ばれない//
    void OnJingleFadeOutComplete()
    {   
        jingleAudioSource.Stop();

        if(jingleCallback != null)
        {
            jingleCallback();
            jingleCallback = null;
        }

    }
}