using System.Collections;
using UnityEngine;
using TMPro;

public class ComicCutscene : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject nextIcon;

    [Header("Dialogue Data")]
    [SerializeField] private string[] names;

    [TextArea(2, 5)]
    [SerializeField] private string[] dialogues;

    [Header("Typing")]
    [SerializeField] private float typingSpeed = 0.03f;

    private int currentLine = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private void Start()
    {
        if (nextIcon != null)
            nextIcon.SetActive(false);

        ShowLine();
    }

    public void OnClick()
    {
        // ถ้าข้อความกำลังค่อยๆแสดง → คลิกเพื่อแสดงข้อความทั้งหมด
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);

            dialogueText.text = dialogues[currentLine];

            isTyping = false;

            if (nextIcon != null)
                nextIcon.SetActive(true);

            return;
        }

        // ไปข้อความถัดไป
        currentLine++;

        if (currentLine >= dialogues.Length)
        {
            EndCutscene();
            return;
        }

        ShowLine();
    }

    private void ShowLine()
    {
        if (currentLine < names.Length)
            nameText.text = names[currentLine];

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        isTyping = true;

        dialogueText.text = "";

        if (nextIcon != null)
            nextIcon.SetActive(false);

        string text = dialogues[currentLine];

        foreach (char letter in text)
        {
            dialogueText.text += letter;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        if (nextIcon != null)
            nextIcon.SetActive(true);
    }

    private void EndCutscene()
    {
        Debug.Log("Cutscene Finished");

        gameObject.SetActive(false);
    }
}