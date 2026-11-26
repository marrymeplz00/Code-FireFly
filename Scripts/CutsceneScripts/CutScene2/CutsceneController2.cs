using UnityEngine;

public class CutsceneController2 : MonoBehaviour
{
    [Header("Cutscene Image")]
    [SerializeField] private GameObject homeImage;

    [Header("Dialogue")]
    [SerializeField] private Dialogue dialogue;

    private bool cutsceneStarted = false;

    private void Start()
    {
        // ปิดภาพบ้านก่อนเริ่มเกม
        if (homeImage != null)
            homeImage.SetActive(false);
    }

    public void StartCutscene()
    {
        if (cutsceneStarted)
            return;

        cutsceneStarted = true;

        // เปิดภาพบ้าน
        if (homeImage != null)
            homeImage.SetActive(true);

        // เริ่ม Dialogue
        if (dialogue != null)
        {
            dialogue.StartDialogue();
        }
        else
        {
            Debug.LogError("CutsceneController2: ยังไม่ได้ใส่ Dialogue!");
        }
    }
}