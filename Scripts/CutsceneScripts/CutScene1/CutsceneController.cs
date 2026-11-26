using System.Collections;
using UnityEngine;
using TMPro;

public class CutsceneController : MonoBehaviour
{
    [Header("Scene Images")]
    [SerializeField] private GameObject cityView;
    [SerializeField] private GameObject officeView;
    [SerializeField] private GameObject houseView;

    [Header("Chat")]
    [SerializeField] private GameObject chatPanel;

    [SerializeField] private GameObject[] messages;

    [SerializeField] private TMP_Text[] messageTexts;

    [SerializeField] private string[] messageContents;

    [Header("House Dialogue")]
    [SerializeField] private HouseDialogue houseDialogue;

    [Header("Notification")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip notificationSound;

    [Header("Timing")]
    [SerializeField] private float cityDuration = 3f;

    [SerializeField] private float messageDelay = 2f;

    [SerializeField] private float typingSpeed = 0.03f;

    [SerializeField] private float chatDuration = 15f;

    private void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        
        // ภาพเมือง
        cityView.SetActive(true);
        officeView.SetActive(false);
        houseView.SetActive(false);
        chatPanel.SetActive(false);

        // รอภาพเมือง 3 วินาที
        yield return new WaitForSeconds(cityDuration);


        // ออฟฟิต
        cityView.SetActive(false);
        officeView.SetActive(true);

        // เปิด Chat
        chatPanel.SetActive(true);

        // ซ่อนข้อความทั้งหมดก่อน
        for (int i = 0; i < messages.Length; i++)
        {
            messages[i].SetActive(false);
        }

        // แชทขึ้นบบหน้าจอ
        float chatTimer = 0f;

        for (int i = 0; i < messages.Length; i++)
        {

            // เสียงแจ้งเตือน
            if (audioSource != null && notificationSound != null)
            {
                audioSource.PlayOneShot(notificationSound);
            }

            // เปิดข้อความ
            messages[i].SetActive(true);

            // แสดงข้อความทีละอัน
            yield return StartCoroutine(
                TypeMessage(messageTexts[i], messageContents[i])
            );

            // รอข้อความถัดไป
            float timer = 0f;

            while (timer < messageDelay)
            {
                timer += Time.deltaTime;
                chatTimer += Time.deltaTime;

                yield return null;
            }

            // เช็กเวลารวมของ Chat
            if (chatTimer >= chatDuration)
            {
                break;
            }
        }

        // บ้าน(โรงเตี๊ยม)
        chatPanel.SetActive(false);

        officeView.SetActive(false);
        houseView.SetActive(true);

        // เริ่ม Dialogue ที่บ้าน
        if (houseDialogue != null)
        {
            houseDialogue.StartDialogue();
        }
    }


    private IEnumerator TypeMessage(TMP_Text text, string message)
    {
        text.text = "";

        foreach (char letter in message)
        {
            text.text += letter;

            yield return new WaitForSeconds(typingSpeed);
        }
    }
}