using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{

    public Canvas Canvas;
    GameObject startMenuObj;
    readonly StartMenuOption[] menuOptions = new StartMenuOption[]
    {
        new StartMenuOption{ MenuText = "Start Game (Sight Words)", IsSelected = true },
        new StartMenuOption{ MenuText = "Start Game (Math)", IsSelected = false }
    };

    // Use this for initialization
    void Start()
    {
        startMenuObj = new GameObject();

        var panelImage = startMenuObj.AddComponent<Image>();
        panelImage.name = "MenuContainer";

        panelImage.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);

        var panelImageRectTransform = panelImage.GetComponent<RectTransform>();

        panelImageRectTransform.position = new Vector3(0, 0, -5.0f);

        panelImageRectTransform.SetParent(Canvas.transform);
        panelImageRectTransform.offsetMin = new Vector2(0, 0);
        panelImageRectTransform.offsetMax = new Vector2(0, 0);
        panelImageRectTransform.anchoredPosition = new Vector3(0, 0, -5.0f);
        panelImageRectTransform.localScale = new Vector3(16, 16, 1);
        panelImageRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        panelImageRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        panelImageRectTransform.pivot = new Vector2(0.5f, 0.5f);
        panelImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 38);
        panelImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 67);
        startMenuObj.SetActive(true);

        for (var i = 0; i < menuOptions.Length; i++)
        {
            var menuOption = menuOptions[i];

            GameObject menuOptionObj = new GameObject();
            var menuOptionText = menuOptionObj.AddComponent<Text>();

            menuOptionText.name = $"MenuOption_{menuOption.MenuText}";
            menuOptionText.font = GameManager.instance.DefaultFont;
            menuOptionText.fontSize = 64;
            menuOptionText.alignment = TextAnchor.MiddleCenter;
            menuOptionText.color = Color.white;

            var menuOptionTextObjRectTransform = menuOptionText.GetComponent<RectTransform>();

            menuOptionTextObjRectTransform.SetParent(panelImage.transform);

            menuOptionTextObjRectTransform.offsetMin = new Vector2(0, 0);
            menuOptionTextObjRectTransform.offsetMax = new Vector2(0, 0);

            if (i == 0)
                menuOptionTextObjRectTransform.anchoredPosition = new Vector2(0, i);
            else
                menuOptionTextObjRectTransform.anchoredPosition = new Vector2(0, i * 8);

            menuOptionTextObjRectTransform.localScale = new Vector3(0.06f, 0.06f, 1);
            menuOptionTextObjRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            menuOptionTextObjRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            menuOptionTextObjRectTransform.pivot = new Vector2(0.5f, 0.5f);
            menuOptionTextObjRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 52);
            menuOptionTextObjRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 541);

            menuOptionText.text = menuOption.MenuText;

            menuOptionObj.SetActive(true);

            menuOption.TextReference = menuOptionText;
        }

    }

    // Update is called once per frame
    void Update()
    {
        var selectedIndex = 0;

        for (var i = 0; i < menuOptions.Length; i++)
        {
            var currentOption = menuOptions[i];

            if (currentOption.IsSelected)
            {
                currentOption.TextReference.color = Color.blue;
                selectedIndex = i;
            }
            else
                currentOption.TextReference.color = Color.white;
        }

        // check if submit button was pressed
        if (Input.GetButtonUp("Submit"))
        {
            Destroy(startMenuObj);
            GameManager.instance.StartGame();
        }

        if (Input.GetAxis("Vertical") < 0.0f)
        {
            if (selectedIndex > 0)
            {
                menuOptions[selectedIndex].IsSelected = false;
                menuOptions[selectedIndex - 1].IsSelected = true;
            }
        }
        else if (Input.GetAxis("Vertical") > 0.0f)
        {
            if (selectedIndex < menuOptions.Length - 1)
            {
                menuOptions[selectedIndex].IsSelected = false;
                menuOptions[selectedIndex + 1].IsSelected = true;
            }
        }
    }

    public class StartMenuOption
    {
        public string MenuText { get; set; }
        public bool IsSelected { get; set; }
        public Text TextReference { get; set; }
    }
}
