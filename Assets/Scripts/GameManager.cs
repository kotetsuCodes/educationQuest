using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;

    public float MaxPlayerHealth = 5.0f;
    public float PlayerHealth = 5.0f;

    public int StartingPlayerCoins = 0;
    public int PlayerCoins = 0;

    public Image LeftHeartHalf;
    public Image RightHeartHalf;

    public Canvas Canvas;
    public bool CharactersCanMove = true;

    AudioSource audioSource;

    public AudioClip itemSpellOutClip;

    public Font DefaultFont;
    public Font WordFont;

    public Sprite CoinSprite;

    public Dictionary<int, HashSet<string>> challengeWords = new Dictionary<int, HashSet<string>>
    {
        { 0, new HashSet<string> { "if", "is", "him", "rip", "fit", "pin", "and", "be", "you", "an", "bad", "can", "had", "cat", "ran"} },
        { 1, new HashSet<string> { "ox", "log", "dot", "top", "hot", "lot", "fox", "dog", "us", "sun", "but", "fun", "bus", "run", "eat", "put", "one" } },
        { 2, new HashSet<string> { "with", "away", "find", "sing", "yet", "wet", "web", "leg", "pen", "hen", "me", "my", "add", "pass" } },
        { 3, new HashSet<string> { "up", "bug", "mud", "nut", "hug", "tub", "full", "pull", "take", "small", "give", "every" } },
        { 4, new HashSet<string> { "does", "here", "am", "at", "sat", "man", "dad", "mat", "ran", "sad", "van", "mad", "on", "got", "fox", "pop",  } },
        { 5, new HashSet<string> { "not", "hop", "her", "now", "that", "then", "this", "them", "with", "bath", "blue", "water", "live" } },
        { 6, new HashSet<string> { "where", "their", "good", "hold", "many", "friend", "little", "today", "hibernate", "classify", "block", "clock", "would", "musical" } },
        //{ 5, new string[] {   } },
        //{ 6, new string[] { "", "", "", "", "" } },
        //{ 7, new string[] { "", "", "", "", "" } },
        //{ 8, new string[] { "", "", "", "", "" } },
        //{ 9, new string[] { "", "", "", "", "" } },
    };

    Image panelImage;

    Image[] healthPoints = new Image[10];

    readonly Image coinImage;
    Text coinValueText;



    // Use this for initialization
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        WordFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

        audioSource = GetComponent<AudioSource>();

        Debug.Log("GameManager start was called");
        Canvas = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<Canvas>();
        BuildUI();

        InitGame();
    }

    // Update is called once per frame
    void Update()
    {
        // if (coinValueText == null)
        // coinValueText = GameObject.FindGameObjectWithTag("CoinValueText").GetComponent<Text>();

        // coinValueText.text = PlayerCoins.ToString();
        CheckForGameOver();
    }

    void CheckForGameOver()
    {
        if (PlayerHealth <= 0)
        {
            GameOver();
        }
    }

    public void InitGame()
    {
        Debug.Log("InitGame was called");

        PlayerHealth = MaxPlayerHealth;
        PlayerCoins = StartingPlayerCoins;

        CalculateHealthSprites(MaxPlayerHealth);
        UpdateCoinUI(StartingPlayerCoins);

        LevelManager.instance.InitializeLevelManager(challengeWords[SceneManager.GetActiveScene().buildIndex]);
    }

    void BuildUI()
    {
        var panelImageObj = new GameObject();

        panelImage = panelImageObj.AddComponent<Image>();
        panelImage.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        panelImage.type = Image.Type.Sliced;
        panelImage.name = "HUDContainer";

        panelImage.color = new Color(0, 0, 0, 0.5f);

        var panelImageRectTransform = panelImage.GetComponent<RectTransform>();

        panelImageRectTransform.SetParent(Canvas.transform);
        panelImageRectTransform.offsetMin = new Vector2(0, 0);
        panelImageRectTransform.offsetMax = new Vector2(0, 0);
        panelImageRectTransform.anchoredPosition = new Vector2(0, 0);
        panelImageRectTransform.localScale = new Vector2(1, 1);
        panelImageRectTransform.anchorMin = new Vector2(0, 1);
        panelImageRectTransform.anchorMax = new Vector2(1, 1);
        panelImageRectTransform.pivot = new Vector2(0.5f, 1);
        panelImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30);
        panelImageObj.SetActive(true);
    }

    public void DisplayCollectedItem(string collectedItem)
    {
        CharactersCanMove = false;

        GameObject collectedItemObj = new GameObject();
        Text collectedItemText = collectedItemObj.AddComponent<Text>();

        collectedItemText.fontSize = 64;
        collectedItemText.alignment = TextAnchor.MiddleCenter;
        collectedItemText.color = Color.white;
        collectedItemText.font = WordFont;

        var collectedItemRectTransform = collectedItemText.GetComponent<RectTransform>();

        collectedItemRectTransform.SetParent(Canvas.transform);

        collectedItemRectTransform.offsetMin = new Vector2(0, 0);
        collectedItemRectTransform.offsetMax = new Vector2(0, 0);
        collectedItemRectTransform.anchoredPosition = new Vector2(0, 0);
        collectedItemRectTransform.localScale = new Vector3(1, 1, 1);
        collectedItemRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        collectedItemRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        collectedItemRectTransform.pivot = new Vector2(0.5f, 0.5f);
        collectedItemRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 72);
        collectedItemRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 256);
        collectedItemObj.SetActive(true);

        StartCoroutine(DisplayCollectedItem(collectedItemText, collectedItem));
    }

    IEnumerator DisplayCollectedItem(Text itemTextObj, string item)
    {
        audioSource.clip = itemSpellOutClip;

        foreach (var character in item)
        {
            itemTextObj.text += character;
            WindowsVoice.speak(character.ToString());
            yield return new WaitForSeconds(1.0f);
            audioSource.Play();
            yield return new WaitForSeconds(1.0f);
        }

        WindowsVoice.speak(itemTextObj.text);

        yield return new WaitForSeconds(1.5f);

        Destroy(itemTextObj);
        CharactersCanMove = true;
    }

    public void GameOver()
    {
        // fade screen to black etc.
        SceneManager.LoadScene(0);
        InitGame();
    }

    public void PlayerCoinPickup()
    {
        ++PlayerCoins;
        UpdateCoinUI(PlayerCoins);
    }

    public void CalculateHealthSprites(float health)
    {
        foreach (var hp in healthPoints.Where(h => h != null))
            hp.enabled = false;

        for (var i = 0; i < (health / 0.5); i++)
        {
            var anchoredPositionX = 0.0f;

            if (i % 2 == 0)
            {
                healthPoints[i] = Instantiate<Image>(LeftHeartHalf);
                anchoredPositionX = (i + 1) * 16 + 0.0f;
            }
            else
            {
                healthPoints[i] = Instantiate<Image>(RightHeartHalf);
                anchoredPositionX = i * 16 + 0.1f;
            }

            healthPoints[i].transform.SetParent(panelImage.transform);

            var rectTransform = healthPoints[i].GetComponent<RectTransform>();
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.anchoredPosition = new Vector2(anchoredPositionX, 0);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 16);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 16);

            healthPoints[i].enabled = true;
        }
    }

    public void UpdateCoinUI(int coins)
    {
        if (coinValueText == null)
        {
            GameObject coinImageObj = new GameObject();
            var coinImage = coinImageObj.AddComponent<Image>();
            coinImage.sprite = CoinSprite;

            var coinImageRectTransform = coinImage.GetComponent<RectTransform>();

            coinImageRectTransform.SetParent(panelImage.transform);
            coinImageRectTransform.offsetMin = new Vector2(1, 0.5f);
            coinImageRectTransform.offsetMax = new Vector2(1, 0.5f);
            coinImageRectTransform.anchoredPosition = new Vector2(-30, 0);
            coinImageRectTransform.localScale = new Vector3(1, 1, 1);
            coinImageRectTransform.anchorMin = new Vector2(1, 0.5f);
            coinImageRectTransform.anchorMax = new Vector2(1, 0.5f);
            coinImageRectTransform.pivot = new Vector2(1, 0.5f);
            coinImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 16);
            coinImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 16);
            coinImageObj.SetActive(true);

            // Build Text UI

            GameObject coinTextObj = new GameObject();
            coinValueText = coinTextObj.AddComponent<Text>();

            coinValueText.name = "CoinCount";
            coinValueText.fontSize = 24;
            coinValueText.alignment = TextAnchor.MiddleRight;
            coinValueText.color = Color.white;
            coinValueText.font = DefaultFont;

            var coinTextObjRectTransform = coinValueText.GetComponent<RectTransform>();

            coinTextObjRectTransform.SetParent(panelImage.transform);

            coinTextObjRectTransform.offsetMin = new Vector2(1, 0.5f);
            coinTextObjRectTransform.offsetMax = new Vector2(1, 0.5f);
            coinTextObjRectTransform.anchoredPosition = new Vector2(-10, 0);
            coinTextObjRectTransform.localScale = new Vector3(1, 1, 1);
            coinTextObjRectTransform.anchorMin = new Vector2(1, 0.5f);
            coinTextObjRectTransform.anchorMax = new Vector2(1, 0.5f);
            coinTextObjRectTransform.pivot = new Vector2(1, 0.5f);
            coinTextObjRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 32);
            coinTextObjRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 64);
            coinTextObj.SetActive(true);
        }

        coinValueText.text = coins.ToString();
    }
}
