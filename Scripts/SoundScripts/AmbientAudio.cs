using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AmbientAudio : MonoBehaviour
{
    [Header("Ambient Sound")]
    [SerializeField] private AudioClip ambientClip;

    [Header("Audio Settings")]
    [SerializeField, Range(0f, 1f)]
    private float ambientVolume = 0.2f;

    private AudioSource ambientAudioSource;

    private void Awake()
    {
        ambientAudioSource = GetComponent<AudioSource>();

        // ตั้งค่าเสียงพื้นหลังแยกจากเสียงเดินและเสียงโจมตี
        ambientAudioSource.playOnAwake = false;
        ambientAudioSource.loop = true;
        ambientAudioSource.spatialBlend = 0f;
        ambientAudioSource.volume = ambientVolume;
        ambientAudioSource.mute = false;
        ambientAudioSource.Stop();
        
        ambientAudioSource.clip = ambientClip;
    }

    private void Start()
    {
        if (ambientAudioSource != null &&
            ambientAudioSource.clip != null)
        {
            ambientAudioSource.Play();
        }
    }

    private void OnDestroy()
    {
        if (ambientAudioSource != null)
        {
            ambientAudioSource.Stop();
        }
    }
}