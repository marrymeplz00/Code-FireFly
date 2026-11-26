using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HouseDialogue : MonoBehaviour
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
            // ถ้าข้อความกำลังค่อยๆแสดง → คลิกเพื่อแสดงข้อความทั้งหมด
            if (isTyping)
            {
                CompleteCurrentDialogue();
            }
            // ถ้าข้อความแสดงเสร็จแล้ว → ไปข้อความถัดไป
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
            Debug.LogWarning("HouseDialogue: ไม่มี Dialogue");
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
        // ถ้า Dialogue หมดแล้ว
        if (currentIndex >= dialogues.Length)
        {
            LoadNextScene();
            return;
        }

        // ชื่อ
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


        // เสียงตอนเปลี่ยน Dialogue
        if (audioSource != null && dialogueSound != null)
        {
            audioSource.PlayOneShot(dialogueSound);
        }


        // ไอคอนอันต่อไป
        if (nextIcon != null)
            nextIcon.SetActive(false);


        // หยุดพิมพ์ข้อความ
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        StopTypingSound();


        // เริ่มพิมพ์ข้อความ       
        typingCoroutine = StartCoroutine(
            TypeDialogue(dialogues[currentIndex])
        );
    }


    private IEnumerator TypeDialogue(string message)
    {
        isTyping = true;

        dialogueText.text = "";


        // เริ่มเสียงพิมพ์
        if (typingAudioSource != null && typingSound != null)
        {
            typingAudioSource.clip = typingSound;
            typingAudioSource.loop = true;
            typingAudioSource.Play();
        }


        // พิมพ์ข้อความทีละตัว
        foreach (char letter in message)
        {
            dialogueText.text += letter;

            yield return new WaitForSeconds(typingSpeed);
        }


        // เมื่อพิมพ์เสร็จ
        StopTypingSound();

        isTyping = false;

        if (nextIcon != null)
            nextIcon.SetActive(true);
    }


    private void CompleteCurrentDialogue()
    {
        // หยุดการพิมพ์
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // หยุดเสียงพิมพ์
        StopTypingSound();

        // แสดงข้อความเต็มทันที
        dialogueText.text = dialogues[currentIndex];

        isTyping = false;

        // แสดงลูกศร
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

        // เมื่อ Dialogue หมดแล้ว
        if (currentIndex >= dialogues.Length)
        {
            LoadNextScene();
            return;
        }

        // ไป Dialogue ถัดไป
        ShowDialogue();
    }


    private void LoadNextScene()
    {
        StopTypingSound();

        dialogueStarted = false;
        isTyping = false;

        // ปิด Dialogue
        if (dialogueBox != null)
            dialogueBox.SetActive(false);


        // โหลด Scene ถัดไป
        if (string.IsNullOrEmpty(nextSceneName))
        {
            return;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}