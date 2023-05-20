using EnhancedDodoServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace GamePad
{
    /// <summary>
    /// Describes possible button layout options
    /// </summary>
    public enum ButtonsLayoutOptions
    {
        XboxPad,
        BellyRumble_FloatingJoystick, BellyRumble_DynamicJoystick,
        BellyRumble_ButtonsOnly,
        FieldsOfGlory_FloatingJoystick,
        PotionMaster_FloatingJoystick,
        CityUnderAttack_FloatingJoystick,
        Empatki,
        Submarine_FloatingJoystick,
        Empty,
        Gamepad
    }
    /// <summary>
    /// All the keys to be handled    
    /// </summary>
    public enum ActionButton { L_STICK, R_STICK, D_PAD, START, BACK, GUIDE, TURBO, L_TRIGGER, R_TRIGGER, L_BUMPER, R_BUMPER, A, B, X, Y, LEFT, RIGHT, DOWN, UP }
    /// <summary>
    /// Holds references to buttons for one layout option
    /// </summary>
    public class ButtonsLayout : MonoBehaviour
    {
        public ButtonsLayoutOptions option;

        public VariableJoystick leftStick;
        public string leftStickAction;
        public VariableJoystick rightStick;
        public string rightStickAction;
        public VariableJoystick directionalPad;
        public string directionalPadAction;
        public InputButton start;
        public string startAction;
        public InputButton back;
        public string backAction;
        public InputButton guide;
        public string guideAction;
        public InputButton turbo;//nie ma tego na XboxPadzie, ale jest na naszych
        public string turboAction;
        public InputButton leftTrigger;
        public string leftTriggerAction;
        public InputButton rightTrigger;
        public string rightTriggerAction;
        public InputButton leftBumper;
        public string leftBumperAction;
        public InputButton rightBumper;
        public string rightBumperAction;
        public InputButton AButton;
        public string AButtonAction;
        public InputButton BButton;
        public string BButtonAction;
        public InputButton XButton;
        public string XButtonAction;
        public InputButton YButton;
        public string YButtonAction;
        public InputButton leftButton;
        public string leftButtonAction;
        public InputButton downButton;
        public string downButtonAction;
        public InputButton upButton;
        public string upButtonAction;
        public InputButton rightButton;
        public string rightButtonAction;

        public GameObject floatingTextPrefab;
        private Dictionary<Transform, string> button_action_Dictionary = new Dictionary<Transform, string>();
        //private List<Image> floatingTexts = new List<Image>();
        public List<CanvasRenderer> floatingTexts = new List<CanvasRenderer>();
        Coroutine currentCoroutine;
        private void Awake()
        {
            button_action_Dictionary.Add(leftStick.transform, leftStickAction);
            button_action_Dictionary.Add(rightStick.transform, rightStickAction);
            button_action_Dictionary.Add(directionalPad.transform, directionalPadAction);
            button_action_Dictionary.Add(start.transform, startAction);
            button_action_Dictionary.Add(back.transform, backAction);
            button_action_Dictionary.Add(guide.transform, guideAction);
            button_action_Dictionary.Add(turbo.transform, turboAction);
            button_action_Dictionary.Add(leftTrigger.transform, leftTriggerAction);
            button_action_Dictionary.Add(rightTrigger.transform, rightTriggerAction);
            button_action_Dictionary.Add(leftBumper.transform, leftBumperAction);
            button_action_Dictionary.Add(rightBumper.transform, rightBumperAction);
            button_action_Dictionary.Add(AButton.transform, AButtonAction);
            button_action_Dictionary.Add(BButton.transform, BButtonAction);
            button_action_Dictionary.Add(XButton.transform, XButtonAction);
            button_action_Dictionary.Add(YButton.transform, YButtonAction);
            button_action_Dictionary.Add(leftButton.transform, leftButtonAction);
            button_action_Dictionary.Add(downButton.transform, downButtonAction);
            button_action_Dictionary.Add(upButton.transform, upButtonAction);
            button_action_Dictionary.Add(rightButton.transform, rightButtonAction);

            CreateFloatingTexts();
        }

        private void OnEnable()
        {
            if (option != ButtonsLayoutOptions.Empty)
            {
                Debug.Log("Connect");
                if (Client.instance.ServerIP != null)
                {
                    UDPProtocolClient.instance.Connect(Client.instance.ServerIP);
                }
                else
                {
                    UDPProtocolClient.instance.Connect();
                }
                Client.instance.udpInitialized = true;
            }
        }

        /// <summary>
        /// Shows the info about each button's action
        /// </summary>
        /// <param name="fadeInBefore">Should fade in to appear</param>
        /// <param name="fadeOutAfter">Should fade out to disappear</param>
        /// <param name="fadeInTime">The time of fade</param>
        /// <param name="lifeTime">The time of life</param>
        /// <param name="fadeOutTime">The time of fade out</param>
        public void ShowInfo(bool fadeInBefore, bool fadeOutAfter, float fadeInTime, float lifeTime, float fadeOutTime)
        {
            //if (currentCoroutine != null) StopAllCoroutines();
            if (currentCoroutine == null)
                currentCoroutine = StartCoroutine(FloatingTextsCoroutine(fadeInBefore, fadeOutAfter, fadeInTime, lifeTime, fadeOutTime));
        }
        IEnumerator FloatingTextsCoroutine(bool fadeInBefore, bool fadeOutAfter, float fadeInTime, float lifeTime, float fadeOutTime)
        {
            EnableFloatingTexts();

            if (fadeInBefore)
            {
                foreach (CanvasRenderer floatingText in floatingTexts)
                {
                    StartCoroutine(FloatingTextFade(floatingText, fadeInTime, true));
                }
                yield return new WaitForSeconds(fadeInTime);
            }

            yield return new WaitForSeconds(lifeTime);

            if (fadeOutAfter)
            {
                foreach (CanvasRenderer floatingText in floatingTexts)
                {
                    StartCoroutine(FloatingTextFade(floatingText, fadeOutTime, false));
                }
                yield return new WaitForSeconds(fadeOutTime);
            }

            DisableFloatingTexts();
        }
        /// <summary>
        /// Shows floating texts with infos about buttons
        /// </summary>
        public void EnableFloatingTexts()
        {
            foreach (CanvasRenderer floatingText in floatingTexts)
            {
                floatingText.gameObject.SetActive(true);
            }
        }
        /// <summary>
        /// Hides floating texts
        /// </summary>
        public void DisableFloatingTexts()
        {
            foreach (CanvasRenderer floatingText in floatingTexts)
            {
                floatingText.gameObject.SetActive(false);
            }
            currentCoroutine = null;
        }
        IEnumerator FloatingTextFade(CanvasRenderer image, float time, bool fadeIn)
        {
            //Color startColor, endColor;
            //if (fadeIn)
            //{
            //    startColor = image.color;
            //    startColor.a = 0f;
            //    endColor = image.color;
            //    endColor.a = 1f;
            //}
            //else
            //{
            //    startColor = image.color;
            //    startColor.a = 1f;
            //    endColor = image.color;
            //    endColor.a = 0f;
            //}
            float startAlpha, endAlpha;
            if (fadeIn)
            {
                startAlpha = 0f;
                endAlpha = 1f;
            }
            else
            {
                startAlpha = 1f;
                endAlpha = 0f;
            }
            float timer = 0f;

            //image.color = startColor;
            image.SetAlpha(startAlpha);
            while (timer < time)
            {
                timer += Time.deltaTime;
                //image.color = Color.Lerp(startColor, endColor, timer / time);
                image.SetAlpha(Mathf.Lerp(startAlpha, endAlpha, timer / time));
                yield return null;
            }
            //image.color = endColor;
            image.SetAlpha(endAlpha);
        }
        void CreateFloatingTexts()
        {
            foreach (KeyValuePair<Transform, string> pair in button_action_Dictionary)
            {
                if (pair.Value != "")
                {
                    GameObject floatingTextGO = CreateFloatingText(pair.Key, pair.Value);
                    //floatingTexts.Add(floatingTextGO.GetComponent<CanvasRenderer>());
                    floatingTexts.AddRange(floatingTextGO.GetComponentsInChildren<CanvasRenderer>());
                    floatingTextGO.SetActive(false);
                }
            }
        }
        GameObject CreateFloatingText(Transform transform, string action)
        {
            // Przycisk jest parentem - jeśli nic nie nachodzi
            GameObject floatingTextGO = Instantiate(floatingTextPrefab, transform);
            // Bez parenta - przyciski nachodziły na floatingtext
            //GameObject floatingTextGO = Instantiate(floatingTextPrefab, transform.parent);
            //floatingTextGO.GetComponent<RectTransform>().localPosition = transform.GetComponent<RectTransform>().anchoredPosition + transform.GetComponent<RectTransform>().rect.center;            
            //
            UnityEngine.UI.Text text = floatingTextGO.GetComponentInChildren<UnityEngine.UI.Text>();
            text.text = action;
            return floatingTextGO;
        }
        //public ActionButton left, right, up, down, movement, jump, shoot;
    }
}