using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Dialogue : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject nextIcon;

    [Header("Dialogue")]
    [SerializeField] private string[] names;

    [TextArea(2, 5)]
    [SerializeField] private string[] dialogues;

    [Header("Typing")]
    [SerializeField] private float typingSpeed = 0.03f;

    [Header("Dialogue Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip dialogueSound;

    [Header("Typing Sound")]
    [SerializeField] private AudioSource typingAudioSource;
    [SerializeField] private AudioClip typingSound;

    [Header("Next Scene")]
    [SerializeField] private string nextSceneName = "Tavern";

    private int currentIndex = 0;

    private bool isTyping = false;
    private bool dialogueStarted = false;

    private Coroutine typingCoroutine;

    private void Start()
    {
        if (dialogueBox != null)
            dialogueBox.SetActive(false);

        if (nextIcon != null)
            nextIcon.SetActive(false);

        StopTypingSound();
    }

    private void Update()
    {
        if (!dialogueStarted)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                CompleteCurrentDialogue();
            }
            else
            {
                GoToNextDialogue();
            }
        }
    }

    public void StartDialogue()
    {
        if (dialogues == null || dialogues.Length == 0)
        {
            Debug.LogWarning("Dialogue: ไม่มี Dialogue");
            return;
        }

        currentIndex = 0;
        dialogueStarted = true;

        if (dialogueBox != null)
            dialogueBox.SetActive(true);

        ShowDialogue();
    }

    private void ShowDialogue()
    {
        if (currentIndex >= dialogues.Length)
        {
            LoadNextScene();
            return;
        }

        if (nameText != null)
        {
            if (names != null && currentIndex < names.Length)
            {
                nameText.text = names[currentIndex];
            }
            else
            {
                nameText.text = "";
            }
        }

        if (audioSource != null && dialogueSound != null)
        {
            audioSource.PlayOneShot(dialogueSound);
        }

        if (nextIcon != null)
            nextIcon.SetActive(false);

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        StopTypingSound();

        typingCoroutine = StartCoroutine(
            TypeDialogue(dialogues[currentIndex])
        );
    }

    private IEnumerator TypeDialogue(string message)
    {
        isTyping = true;

        dialogueText.text = "";

        if (typingAudioSource != null && typingSound != null)
        {
            typingAudioSource.clip = typingSound;
            typingAudioSource.loop = true;
            typingAudioSource.Play();
        }

        foreach (char letter in message)
        {
            dialogueText.text += letter;

            yield return new WaitForSeconds(typingSpeed);
        }

        StopTypingSound();

        isTyping = false;

        if (nextIcon != null)
            nextIcon.SetActive(true);
    }

    private void CompleteCurrentDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        StopTypingSound();

        dialogueText.text = dialogues[currentIndex];

        isTyping = false;

        if (nextIcon != null)
            nextIcon.SetActive(true);
    }

    private void StopTypingSound()
    {
        if (typingAudioSource != null)
        {
            typingAudioSource.Stop();
        }
    }

    private void GoToNextDialogue()
    {
        currentIndex++;

        if (currentIndex >= dialogues.Length)
        {
            LoadNextScene();
            return;
        }

        ShowDialogue();
    }

    private void LoadNextScene()
    {
        StopTypingSound();

        dialogueStarted = false;
        isTyping = false;

        if (dialogueBox != null)
            dialogueBox.SetActive(false);

        if (string.IsNullOrEmpty(nextSceneName))
        {
            return;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}