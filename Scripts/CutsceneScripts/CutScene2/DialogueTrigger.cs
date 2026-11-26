using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Cutscene")]
    [SerializeField] private CutsceneController2 cutsceneController;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered)
            return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            // หยุดการเล่น Player
            PlayerControl player = other.GetComponent<PlayerControl>();

            if (player != null)
            {
                // หยุดเสียงทั้งหมดของ Player ก่อนเข้าคัตซีน
                AudioSource[] audioSources = player.GetComponentsInChildren<AudioSource>();

                foreach (AudioSource audioSource in audioSources)
                {
                    audioSource.Stop();
                }

                // ปิดการควบคุม Player
                player.enabled = false;
            }

            // เริ่ม Cutscene
            if (cutsceneController != null)
            {
                cutsceneController.StartCutscene();
            }
            else
            {
                Debug.LogError("DialogueTrigger: ยังไม่ได้ใส่ CutsceneController2!");
            }

            // ปิด Trigger ที่ใช้เริ่ม Cutscene
            gameObject.SetActive(false);
        }
    }
}