using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

namespace KasperDev.Dialogue.Example.Ex01
{
    public class DialogueUIController : MonoBehaviour
    {
        private DialogueController dialogueControllers;

        [Header("Panel")]
        [SerializeField] private GameObject dialogueUI;

        [Header("Text")]
        [SerializeField] private TMP_Text textName;
        [SerializeField] private TMP_Text textBox;

        [Header("Image")]
        [SerializeField] private Image leftImage;
        [SerializeField] private Image rightImage;
        [SerializeField] private Sprite blankImage; // Tambahkan SerializeField untuk blank image

        [Header("Buttons")]
        [SerializeField] private Button talkButton;

        [Space]
        [SerializeField] private Button button01;
        [SerializeField] private TMP_Text buttonText01;

        [Space]
        [SerializeField] private Button button02;
        [SerializeField] private TMP_Text buttonText02;

        [Space]
        [SerializeField] private Button button03;
        [SerializeField] private TMP_Text buttonText03;

        [Header("Continue")]
        [SerializeField] private Button buttonContinue;

        [Header("disable interactable")]
        [SerializeField] private Color textDisableColor;
        [SerializeField] private Color buttonDisableColor;

        [Header("interactable")]
        [SerializeField] private Color textInteractableColor;

        [Header("Typing")]
        [SerializeField] public float typingSpeed = 0.05f;
        [Space]
        public string currentDialogueText;
        [Space]
        private float textProgressThreshold = 0.45f; // Threshold untuk menampilkan tombol continue       
        public bool isHalfWayLine = false;
        public bool isFinished = false;
        private Coroutine displayLineCoroutine;

        [Header("Turn Off Game Object")]
        [SerializeField] private List<GameObject> gameObjectsToTurnOff; // Tambahkan array GameObject

        private List<Button> buttons = new List<Button>();
        private List<TMP_Text> buttonsTexts = new List<TMP_Text>();
        private List<DialogueData_BaseContainer> baseContainers;

        private void Awake()
        {
            ShowDialogueUI(false);

            buttons.Add(button01);
            buttons.Add(button02);
            buttons.Add(button03);

            buttonsTexts.Add(buttonText01);
            buttonsTexts.Add(buttonText02);
            buttonsTexts.Add(buttonText03);

            dialogueControllers = FindObjectOfType<DialogueController>(); // Inisialisasi dialogueControllers
            talkButton.gameObject.SetActive(false);
        }

        public void ShowDialogueUI(bool show)
        {
            dialogueUI.SetActive(show);
            //talkButton.gameObject.SetActive(!show);
            TurnOffGameObject(!show); // Panggil TurnOffGameObject dengan kebalikan dari nilai show
        }

        // Tambahkan metode TurnOffGameObject
        private void TurnOffGameObject(bool show)
        {
            foreach (var obj in gameObjectsToTurnOff)
            {
                if (obj != null)
                {
                    obj.SetActive(show);
                }
            }
        }

        // Dalam DialogueUIController
        public void SetText(string text, UnityAction onTextComplete)
        {
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }
            displayLineCoroutine = StartCoroutine(TypeTextCoroutine(text, onTextComplete));
        }

        private IEnumerator TypeTextCoroutine(string text, UnityAction onTextComplete)
        {
            // Set the text to the full line, but set the visible characters to 0
            currentDialogueText = text; // Simpan teks dialog saat ini
            textBox.text = text;
            textBox.maxVisibleCharacters = 0;

            // Hide continue button while text is typing
            buttonContinue.gameObject.SetActive(false);
            isHalfWayLine = false;
            isFinished = false;

            bool isAddingRichTextTag = false;
            int totalCharacters = text.Length;

            // Display each letter one at a time
            for (int i = 0; i < text.Length; i++)
            {
                char letter = text[i];

                // Check for rich text tag, if found, add it without waiting
                if (letter == '<' || isAddingRichTextTag)
                {
                    isAddingRichTextTag = true;
                    if (letter == '>')
                    {
                        isAddingRichTextTag = false;
                    }
                }
                // If not rich text, add the next letter and wait a small time
                else
                {
                    textBox.maxVisibleCharacters++;
                    yield return new WaitForSeconds(typingSpeed);

                    // Calculate progress and show continue button if threshold reached
                    float progress = (float)textBox.maxVisibleCharacters / totalCharacters;
                    if (progress >= textProgressThreshold && !isHalfWayLine)
                    {
                        isHalfWayLine = true;
                        ShowContinueButton();
                    }
                }
            }

            // Actions to take after the entire line has finished displaying
            isHalfWayLine = false;
            isFinished = true;
            onTextComplete?.Invoke();
        }

        public void SetName(string text)
        {
            textName.text = text;
        }

        public void SetImage(Sprite leftImage, Sprite rightImage)
        {
            this.leftImage.sprite = leftImage != null ? leftImage : blankImage; // Mengatur blank image jika leftImage kosong
            this.rightImage.sprite = rightImage != null ? rightImage : blankImage; // Mengatur blank image jika rightImage kosong
        }

        public void HideButtons()
        {
            buttons.ForEach(button => button.gameObject.SetActive(false));
            buttonContinue.gameObject.SetActive(false);
        }

        public void SetButtons(List<DialogueButtonContainer> dialogueButtonContainers)
        {
            HideButtons();

            for (int i = 0; i < dialogueButtonContainers.Count; i++)
            {
                buttons[i].onClick = new Button.ButtonClickedEvent();
                buttons[i].interactable = true;
                buttonsTexts[i].color = textInteractableColor;

                if (dialogueButtonContainers[i].ConditionCheck || dialogueButtonContainers[i].ChoiceState == ChoiceStateType.GrayOut)
                {
                    buttonsTexts[i].text = $"{i + 1}: " + dialogueButtonContainers[i].Text;
                    buttons[i].gameObject.SetActive(true);

                    if (!dialogueButtonContainers[i].ConditionCheck)
                    {
                        buttons[i].interactable = false;
                        buttonsTexts[i].color = textDisableColor;
                        var colors = buttons[i].colors;
                        colors.disabledColor = buttonDisableColor;
                        buttons[i].colors = colors;
                    }
                    else
                    {
                        buttons[i].onClick.AddListener(dialogueButtonContainers[i].UnityAction);
                    }
                }
            }
        }

        public void SetContinue(UnityAction unityAction)
        {
            buttonContinue.onClick = new Button.ButtonClickedEvent();
            buttonContinue.onClick.AddListener(() =>
            {
                // Memanggil metode untuk menangani tombol continue
                if (isHalfWayLine)
                {
                    HandleContinueButton();
                }
                else if (!isHalfWayLine)
                {
                    isHalfWayLine = false;
                    unityAction?.Invoke();
                }

            });
            buttonContinue.gameObject.SetActive(true); // Mulai dengan tombol continue tidak aktif
        }

        private void ShowContinueButton()
        {
            buttonContinue.gameObject.SetActive(true);
        }

        private void HandleContinueButton()
        {
            // Hentikan penulisan teks dan tampilkan seketika jika belum selesai
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }
            textBox.maxVisibleCharacters = currentDialogueText.Length; // Menampilkan semua teks seketika
            isHalfWayLine = false;
            isFinished = true;

            // Cek apakah dialogueControllers tidak null
            if (dialogueControllers != null)
            {
                // Cek apakah ada pilihan, jika ada maka tampilkan tombol choice
                dialogueControllers.Buttons();
            }
        }
    }
}
