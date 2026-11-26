using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer.loopPointReached += OnCutsceneFinished;
    }

    void OnCutsceneFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene("Tavern");
    }
}