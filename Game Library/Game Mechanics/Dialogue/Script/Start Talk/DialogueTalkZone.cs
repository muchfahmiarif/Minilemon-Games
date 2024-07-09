using MalbersAnimations.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace KasperDev.Dialogue.Example.Ex01
{
    public class DialogueTalkZone : MonoBehaviour
    {
        [SerializeField] private GameObject speechBubble;
        [SerializeField] private KeyCode talkKey = KeyCode.E;
        [SerializeField] private Button talkButton;
        //[SerializeField] private Text keyInputText;

        private DialogueTalk dialogueTalk;

        private void Awake()
        {
            speechBubble.SetActive(false);
            //keyInputText.text = talkKey.ToString();
            dialogueTalk = GetComponent<DialogueTalk>();

            // Menggunakan UnityAction untuk listener pada Button
            talkButton.onClick.AddListener(StartDialogue);

            talkButton.gameObject.SetActive(false);
        }

        void Update()
        {
            if (Input.GetKeyDown(talkKey) && speechBubble.activeSelf)
            {
                dialogueTalk.StartDialogue();
            }
        }

        // Perubahan di sini: Mengubah menjadi method public
        public void StartDialogue()
        {
            if (speechBubble.activeSelf)
            {
                dialogueTalk.StartDialogue();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) // Menggunakan CompareTag lebih optimal untuk tag
            {
                speechBubble.SetActive(true);
                talkButton.gameObject.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player")) // Menggunakan CompareTag lebih optimal untuk tag
            {
                speechBubble.SetActive(false);
                talkButton.gameObject.SetActive(false);
            }
        }
    }
}
