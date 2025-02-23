using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioScript : MonoBehaviour
{
    public static AudioScript instance;
    public Slider effectSlider;
    public AudioSource effectSource;
    public List<AudioClip> effectClips = new List<AudioClip>();
    public List<AudioClip> damageClips = new List<AudioClip>();
    public Slider backgroundSlider;
    public AudioSource backgroundSource;
    public List<AudioClip> backgroundClips = new List<AudioClip>();
    
    private AudioClip lastClip; // 이전에 재생한 곡(중복 방지)
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        effectSlider.value = PlayerPrefs.GetFloat("EffectVolume",0.5f);
        effectSource.volume = effectSlider.value;
        backgroundSlider.value = PlayerPrefs.GetFloat("BGMVolume",0.5f);
        backgroundSource.volume = backgroundSlider.value;
        if (backgroundClips.Count > 0)
        {
            StartCoroutine(PlayRandomMusic());
        }
    }

    IEnumerator PlayRandomMusic()
    {
        while (true)
        {
            AudioClip nextClip = GetRandomClip();
            yield return StartCoroutine(FadeOut()); // 페이드 아웃
            backgroundSource.clip = nextClip;
            backgroundSource.Play();
            yield return StartCoroutine(FadeIn()); // 페이드 인

            yield return new WaitForSeconds(nextClip.length - 1f);
        }
    }

    IEnumerator FadeOut(float duration = 1f)
    {
        float startVolume = backgroundSource.volume;
        while (backgroundSource.volume > 0)
        {
            backgroundSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }
        backgroundSource.Stop();
    }

    IEnumerator FadeIn(float duration = 1f)
    {
        float targetVolume = backgroundSlider.value;
        backgroundSource.volume = 0;
        while (backgroundSource.volume < targetVolume)
        {
            backgroundSource.volume += Time.deltaTime / duration;
            yield return null;
        }
    }

    private AudioClip GetRandomClip()
    {
        if (backgroundClips.Count == 1)
            return backgroundClips[0];

        AudioClip newClip;
        do
        {
            newClip = backgroundClips[Random.Range(0, backgroundClips.Count)];
        }
        while (newClip == lastClip); // 이전 곡과 같지 않도록 선택
        
        lastClip = newClip;
        return newClip;
    }

    public void PlayNextSong()
    {
        StopCoroutine(PlayRandomMusic());
        StartCoroutine(PlayRandomMusic());
    }
    public void ChangeBGMPitch(float pitch){
        backgroundSource.pitch = pitch;
    }
    public void PlayEffectSound(){
        effectSource.PlayOneShot(effectClips[Random.Range(0, effectClips.Count)]);
    }
    public void PlayDamageSound(){
        effectSource.PlayOneShot(damageClips[Random.Range(0, damageClips.Count)]);
    }
    public void ChangeEffectVolume(float volume){
        effectSource.volume = volume;
        PlayerPrefs.SetFloat("EffectVolume", volume);
        PlayerPrefs.Save();
    }
    public void ChangeBGMVolume(float volume){
        backgroundSource.volume = volume;
        PlayerPrefs.SetFloat("BGMVolume", volume);
        PlayerPrefs.Save();
    }
}
