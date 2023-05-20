using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnhancedDodoServer;
using System.Net;
using UnityEngine.EventSystems;
using System.Linq;
using System;

namespace GamePad
{

    /// <summary>
    /// Handles UI on the client
    /// </summary>
    public class UIControllerClient : Singleton<UIControllerClient>
    {
        //public Image logsImage;
        /// <summary>
        /// Input field for the client name
        /// </summary>
        public InputField nameInputField;
        Text nameInputFieldPlaceholder;
        string playerName;
        /// <summary>
        /// Dropdown containing names of the server
        /// </summary>
        public Dropdown serverNameDropdown;
        /// <summary>
        /// Log message
        /// </summary>
        string log;
        /// <summary>
        /// Panel holding all customizable settings
        /// </summary>
        [SerializeField] RectTransform settingsPanel;
        /// <summary>
        /// Holds all the buttons that let you choose button layout
        /// </summary>
        [SerializeField] RectTransform buttonsLayout_Content;
        /// <summary>
        /// Prefab of the button that lets you choose button layout
        /// </summary>
        [SerializeField] GameObject buttonsLayoutOptionPrefab;
        /// <summary>
        /// Buttons that open/close settings panel
        /// </summary>
        [SerializeField] Button settingsButtonInGame, settingsButtonInSettingsPanel, closeSettingsButton;
        /// <summary>
        /// Button showing information about all buttons' functions
        /// </summary>
        [SerializeField] Button infoButton;
        /// <summary>
        /// Should the XboxPadLayout be included in the build?
        /// </summary>
        [SerializeField] bool includeXboxPadLayout = true;
        /// <summary>
        /// Holds all the possible layouts
        /// </summary>        
        [SerializeField] ButtonsLayout[] layouts;
        /// <summary>
        /// Holds all possible skins
        /// </summary>
        [SerializeField] Skin[] skins;
        /// <summary>
        /// The current skin
        /// </summary>
        Skin currentSkin;
        /// <summary>
        /// Reference to Image component of the background
        /// </summary>
        [SerializeField] Image[] backgroundImage;
        /// <summary>
        /// Background sprites
        /// </summary>
        [SerializeField] Sprite[] backgrounds;
        private Dictionary<EnhancedDodoServer.Consts.Game, Sprite> game_sprite_Dictionary = new Dictionary<Consts.Game, Sprite>();
        List<Image> themeImages = new List<Image>();
        List<Text> texts = new List<Text>();
        /// <summary>
        /// Current layout option
        /// </summary>        
        [HideInInspector] public ButtonsLayout currentButtonsLayout;
        /// <summary>
        /// Dictionary binding a buttonLayoutOption with its button's Image
        /// </summary>
        Dictionary<ButtonsLayoutOptions, Image> option_buttonImage_Dictionary = new Dictionary<ButtonsLayoutOptions, Image>();
        /// <summary>
        /// Colors of the layoutOptions buttons
        /// </summary>
        public Color buttonPressedColor = Color.clear, buttonUnpressedColor = Color.clear, imageColor = Color.clear;
        /// <summary>
        /// Lifetime of the helping popup texts
        /// </summary>
        [SerializeField] float floatingTextFadeInTime, floatingTextLifeTime, floatingTextFadeOutTime;

        private static readonly string tagDontUseThemeColors = "DontUseThemeColors";

        private void OnEnable()
        {
            nameInputField.text = AdminPanel.GetPlayerNameWithID();
        }

        /// <summary>
        /// The buttonLayout the app starts with
        /// </summary>
        //public ButtonsLayoutOptions startingLayout;
        private void Update()
        {
            //nameInputFieldPlaceholder.text = playerName;
            //messageText.text = message;
        }

        public GameObject ShowButtonsLayoutsGameText;
        public GameObject HideButtonsLayoutsGameText;
        public GameObject ShowButtonsLayoutsGameLayoutText;
        public GameObject HideButtonsLayoutsGameLayoutText;
        public enum LOGTEXT { Empty, Show, Hide, ShowLayout, HideLayout}

        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="text">Log</param>
        public void Log(LOGTEXT type, string aditionalText = "")
        {
            //log = aditionalText;
            //logsText.text = text;
            switch (type)
            {
                case LOGTEXT.Empty:
                    ShowButtonsLayoutsGameText.SetActive(false);
                    HideButtonsLayoutsGameText.SetActive(false);
                    ShowButtonsLayoutsGameLayoutText.SetActive(false);
                    HideButtonsLayoutsGameLayoutText.SetActive(false);
                    break;
                case LOGTEXT.Show:
                    ShowButtonsLayoutsGameText.SetActive(true);
                    HideButtonsLayoutsGameText.SetActive(false);
                    ShowButtonsLayoutsGameLayoutText.SetActive(false);
                    HideButtonsLayoutsGameLayoutText.SetActive(false);
                    break;
                case LOGTEXT.Hide:
                    ShowButtonsLayoutsGameText.SetActive(false);
                    HideButtonsLayoutsGameText.SetActive(true);
                    ShowButtonsLayoutsGameLayoutText.SetActive(false);
                    HideButtonsLayoutsGameLayoutText.SetActive(false);
                    break;
                case LOGTEXT.ShowLayout:
                    ShowButtonsLayoutsGameText.SetActive(false);
                    HideButtonsLayoutsGameText.SetActive(false);
                    ShowButtonsLayoutsGameLayoutText.GetComponent<LocalizedText>()?.AddText(aditionalText);
                    ShowButtonsLayoutsGameLayoutText.SetActive(true);
                    HideButtonsLayoutsGameLayoutText.SetActive(false);
                    break;
                case LOGTEXT.HideLayout:
                    ShowButtonsLayoutsGameText.SetActive(false);
                    HideButtonsLayoutsGameText.SetActive(false);
                    ShowButtonsLayoutsGameLayoutText.SetActive(false);
                    HideButtonsLayoutsGameLayoutText.GetComponent<LocalizedText>()?.AddText(aditionalText);
                    HideButtonsLayoutsGameLayoutText.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        // uzywalem tego w przycisku Logs w gamepad.
        public void ResetMaxPing()
        {
            Client.instance.SetMaxPingToZero();
        }
        /// <summary>
        /// Sets player's name
        /// </summary>
        /// <param name="_playerName">Player's name</param>
        public void SetPlayerName(string _playerName)
        {
            playerName = _playerName;
        }
        /// <summary>
        /// Enables/Hides settings panel
        /// </summary>
        /// <param name="active">Enable or hide</param>
        public void SwitchSettingsPanel(bool active)
        {
            settingsPanel.gameObject.SetActive(active);
        }
        /// <summary>
        /// Changes current button layout
        /// </summary>
        /// <param name="option">Chosen button layout</param>
        public void SwitchButtonsLayout(ButtonsLayoutOptions option)
        {
            currentButtonsLayout?.DisableFloatingTexts();
            foreach (ButtonsLayout layout in layouts)
            {
                if (layout.option == option)
                {
                    layout.gameObject.SetActive(true);
                    JoystickInput.instance.SetButtonsLayout(layout);
                    currentButtonsLayout = layout;
                    Player.instance.ButtonsLayoutOption = option;
                }
                else
                    layout.gameObject.SetActive(false);

                Player.instance.buttonsEnabled = true;
            }
            foreach (Image buttonImage in option_buttonImage_Dictionary.Values)
            {
                buttonImage.color = imageColor * buttonUnpressedColor;
            }
            Image selectedButtonImage;
            if (option_buttonImage_Dictionary.TryGetValue(option, out selectedButtonImage))
                selectedButtonImage.color = imageColor * buttonPressedColor;
        }
        /// <summary>
        /// Changes a skin for the game's default one or a preferenced one if present
        /// </summary>
        /// <param name="game">Game name</param>
        public void SwitchSkin(string game)
        {
            Skin skin = new Skin();
            foreach (Skin s in skins)
            {
                if (s.skinData.game.ToString() == game)
                {
                    skin = new Skin(s);
                    break;
                }
            }
            Skin prefSkin = Player.instance.GetPreferencedSkin(game);
            if (prefSkin != null)
            {
                if (prefSkin.skinData.layoutOption == ButtonsLayoutOptions.XboxPad && !includeXboxPadLayout)
                {

                }
                else
                {
                    skin = new Skin(prefSkin);
                    skin.skinData.layoutOption = prefSkin.skinData.layoutOption;
                    skin.skinData.game = prefSkin.skinData.game;
                }
            }
            SwitchSkin(skin);
        }
        /// <summary>
        /// Changes skin to a certain one
        /// </summary>
        /// <param name="skin">Skin to change to</param>
        private void SwitchSkin(Skin skin)
        {
            currentSkin = skin;

            Sprite sprite;
            game_sprite_Dictionary.TryGetValue(skin.skinData.game, out sprite);
            //Sets background image
            foreach (Image img in backgroundImage)
                img.sprite = sprite;

            //Sets color theme
            //SetThemeAndTextColor(skin.skinData.themeColor, skin.skinData.textColor);

            if (currentButtonsLayout == null) SwitchButtonsLayout(skin.skinData.layoutOption);
            if (skin.skinData.applayThemeColorAndText)
            {
                SetThemeAndTextColor(skin.skinData.themeColor, skin.skinData.textColor);
            }
            SwitchButtonsLayout(skin.skinData.layoutOption);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="themeColor"></param>
        /// <param name="textColor"></param>
        private void SetThemeAndTextColor(Color themeColor, Color textColor)
        {
            List<Image> images = new List<Image>();
            List<Text> texts = new List<Text>();
            Color serverNameDropdownColor = serverNameDropdown.GetComponent<Image>().color;
            Color cmpColor = new Color(serverNameDropdownColor.r, serverNameDropdownColor.g, serverNameDropdownColor.b, serverNameDropdownColor.a);

            //images.Add(serverNameDropdown.GetComponent<Image>());
            //texts.Add(serverNameDropdown.GetComponentInChildren<Text>());
            //images.Add(currentButtonsLayout.start.GetComponent<Image>());
            //texts.Add(currentButtonsLayout.start.GetComponentInChildren<Text>());
            //images.Add(logsImage);
            //texts.Add(logsImage.GetComponentInChildren<Text>());
            //images.Add(infoButton.GetComponent<Image>());
            //images.Add(settingsButtonInGame.GetComponent<Image>());
            //images.Add(settingsButtonInSettingsPanel.GetComponent<Image>());
            //var settingsButtons = settingsPanel.GetComponentsInChildren<Image>();
            //var settingsTexts = settingsPanel.GetComponentsInChildren<Text>();
            //zerowy element to tło, nie zmieniam mu koloru
            //for (int i = 1; i < settingsButtons.Length; i++)
            //{
            //    images.Add(settingsButtons[i]);
            //}
            //foreach (Text text in settingsTexts)
            //{
            //    texts.Add(text);
            //}
            foreach (ButtonsLayout layout in layouts)
            {
                foreach (Image img in layout.GetComponentsInChildren<Image>())
                {
                    images.Add(img);
                    //if (img.color.a != 0f)
                    //{
                    //    if (img.tag == "TextImage")
                    //    {
                    //        img.color = textColor;
                    //    }
                    //    else
                    //    {
                    //        img.color = themeColor;
                    //    }
                    //}
                }
                foreach (Text text in layout.GetComponentsInChildren<Text>())
                {
                    texts.Add(text);
                }
            }

            foreach (Image img in images)
            {
                if (img.color.a != 0f)
                {
                    if (img.tag == "TextImage")
                    {
                        img.color = textColor;
                    }
                    else
                    {
                        //if (img.color.grayscale != cmpColor.grayscale)
                        //{
                        //    Color newColor = new Color(themeColor.r, themeColor.g, themeColor.b, themeColor.a);
                        //    float grayScaleRatio = cmpColor.grayscale / img.color.grayscale;
                        //    float h, s, v;
                        //    Color.RGBToHSV(newColor, out h, out s, out v);
                        //    v *= grayScaleRatio;
                        //    newColor = Color.HSVToRGB(h, s, v);
                        //    img.color = newColor;
                        //}
                        //else

                        if (img.tag != tagDontUseThemeColors)
                        {
                            img.color = themeColor;
                        }

                        if (img.tag == "ButtonLayoutOption")
                            imageColor = img.color;
                    }
                }
            }
            foreach (Text text in texts)
            {
                text.color = textColor;
            }
            images.Clear();
            texts.Clear();
        }
        /// <summary>
        /// Automatically creates buttons in settingsPanel
        /// </summary>
        void AddButtonsLayoutOptions()
        {
            ButtonsLayoutOptions[] options = new ButtonsLayoutOptions[layouts.Length];
            for (int i = 0; i < options.Length; i++)
                options[i] = layouts[i].option;
            foreach (ButtonsLayoutOptions option in options) // Enum.GetValues(typeof(ButtonsLayoutOptions)))
            {
                if (option == ButtonsLayoutOptions.XboxPad)
                    if (!includeXboxPadLayout)
                        continue;
                AddButtonsLayoutOption(option, buttonsLayout_Content);
            }
        }
        /// <summary>
        /// Creates and adds a layout choice button
        /// </summary>
        /// <param name="option">Layout option</param>
        /// <param name="parent">Parent</param>
        void AddButtonsLayoutOption(ButtonsLayoutOptions option, RectTransform parent)
        {
            GameObject button = CreateButtonsLayoutOption(option);
            button.transform.SetParent(parent);
        }
        /// <summary>
        /// Creates a layout choice button
        /// </summary>
        /// <param name="option">Layout option</param>
        GameObject CreateButtonsLayoutOption(ButtonsLayoutOptions option)
        {
            GameObject buttonGO = Instantiate(buttonsLayoutOptionPrefab);
            Button button = buttonGO.GetComponentInChildren<Button>();
            Image buttonImage = button.GetComponent<Image>();

            button.onClick.AddListener(delegate
            {
                Skin skin = new Skin(currentSkin);
                skin.skinData.layoutOption = option;
                Player.instance.SavePreferencedSkin(skin.skinData.game.ToString(), skin);
                SwitchButtonsLayout(option);
            });
            option_buttonImage_Dictionary.Add(option, buttonImage);
            buttonGO.GetComponentInChildren<Text>().text = Enum.GetName(typeof(ButtonsLayoutOptions), option);

            if (buttonPressedColor == Color.clear) buttonPressedColor = button.colors.pressedColor;
            if (buttonUnpressedColor == Color.clear) buttonUnpressedColor = button.colors.normalColor;
            if (imageColor == Color.clear) imageColor = buttonImage.color;

            return buttonGO;

        }

        void Start()
        {
            nameInputFieldPlaceholder = nameInputField.transform.Find("Placeholder").GetComponent<Text>();
            playerName = Player.instance.playerInput.clientData.name;
            //nameInputFieldPlaceholder.text = Player.instance.playerInput.clientData.name;
            //nameInputField.onEndEdit.AddListener(SetName);
            nameInputField.onEndEdit.AddListener(delegate
            {
                SetName(nameInputField.text);
            });
            serverNameDropdown.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            foreach (string name in System.Enum.GetNames(typeof(Consts.Server)))
            {
                Dropdown.OptionData option = new Dropdown.OptionData(name);
                options.Add(option);
            }
            serverNameDropdown.AddOptions(options);
            serverNameDropdown.onValueChanged.AddListener(OnSubmit);
            GamePadClient.Instance.RefreshServerNameAndIP(System.Enum.GetNames(typeof(Consts.Server))[serverNameDropdown.value]);

            settingsButtonInGame.onClick.AddListener(delegate { SwitchSettingsPanel(!settingsPanel.gameObject.activeSelf); });
            settingsButtonInSettingsPanel.onClick.AddListener(delegate { SwitchSettingsPanel(!settingsPanel.gameObject.activeSelf); });
            closeSettingsButton.onClick.AddListener(delegate { SwitchSettingsPanel(false); });
            infoButton.onClick.AddListener(ShowInfo);

            for (int i = 0; i < backgrounds.Length; i++)
            {
                game_sprite_Dictionary.Add((EnhancedDodoServer.Consts.Game)i, backgrounds[i]);
            }

            SwitchSettingsPanel(false);
            AddButtonsLayoutOptions();
            //SwitchButtonsLayout(startingLayout);

        }
        void ShowInfo()
        {
            currentButtonsLayout.ShowInfo(true, true, floatingTextFadeInTime, floatingTextLifeTime, floatingTextFadeOutTime);
        }
        void OnSubmit(int valueIndex)
        {
            //GamePadClient.Instance.ServerName = System.Enum.GetNames(typeof(Consts.Server))[valueIndex];
            GamePadClient.Instance.RefreshServerNameAndIP(System.Enum.GetNames(typeof(Consts.Server))[valueIndex]);
            //Log("Looking for a server...");
        }
        void SetName(string name)
        {
            Player.instance.SetClientName(name);
        }
        public string GetNameInputFieldText()
        {
            return nameInputField.text;
        }
    }
}