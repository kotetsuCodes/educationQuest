using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance = null;

    public HashSet<string> CollectedChallengeWords = new HashSet<string>();
    public bool AllChallengesCollected = false;
    public bool ChallengeMode = false;
    public bool LevelCompleted = false;

    HashSet<string> challengeWordsInPlay = new HashSet<string>();
    LinkedList<ChallengeWord> challengeModeWords = new LinkedList<ChallengeWord>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void InitializeLevelManager(HashSet<string> possibleWords)
    {
        CollectedChallengeWords = new HashSet<string>();
        AllChallengesCollected = false;
        ChallengeMode = false;
        LevelCompleted = false;
        challengeWordsInPlay = new HashSet<string>();
        challengeModeWords = new LinkedList<ChallengeWord>();

        while (challengeWordsInPlay.Count < 5)
        {
            var randomWord = getRandomWord(possibleWords);

            if (!challengeWordsInPlay.Contains(randomWord))
                challengeWordsInPlay.Add(randomWord);
        }
    }

    private void Update()
    {
        Debug.Log("Level Manager Update() running");

        CheckForLevelCompletion();
        RunChallengeMode();
    }

    void RunChallengeMode()
    {
        if (LevelCompleted == false && ChallengeMode)
        {
            // select random challenge word
            if (!challengeModeWords.Any(w => w.IsCurrentWord))
            {
                ChallengeWord randomWord = getRandomChallengeModeWord(challengeModeWords);
                randomWord.IsCurrentWord = true;
                speakWord(randomWord);
            }

            // 
            var selectedWord = challengeModeWords.SingleOrDefault(c => c.IsSelected);

            // if word is not selected select one at random
            if (selectedWord == null)
                getRandomChallengeModeWord(challengeModeWords).IsSelected = true;

            // update coloring for text
            foreach (var node in challengeModeWords)
            {
                if (node.IsSelected)
                    node.ChallengeModeWordText.color = Color.cyan;
                else
                    node.ChallengeModeWordText.color = Color.white;
            }

            // Left
            if (Input.GetAxis("Horizontal") < -0.8f)
            {
                var selectedNode = challengeModeWords.Find(selectedWord);

                if (selectedNode.Previous != null)
                {
                    selectedNode.Previous.Value.IsSelected = true;
                    selectedWord.IsSelected = false;
                }
            }
            // right
            else if (Input.GetAxis("Horizontal") > 0.8f)
            {
                var selectedNode = challengeModeWords.Find(selectedWord);

                if (selectedNode.Next != null)
                {
                    selectedNode.Next.Value.IsSelected = true;
                    selectedWord.IsSelected = false;
                }
            }
            else if (Input.GetButtonDown("Submit"))
            {
                if (selectedWord.IsCurrentWord)
                {
                    WindowsVoice.speak("That is correct!");
                    Destroy(selectedWord.ChallengeModeWordText);
                    challengeModeWords.Remove(selectedWord);

                    // Level has been completed
                    LevelCompleted = !challengeModeWords.Any();
                }
                else
                {
                    WindowsVoice.speak("That is incorrect!");
                }
            }
        }
    }

    void CheckForLevelCompletion()
    {
        if (LevelCompleted)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            GameManager.instance.InitGame();
        }
    }

    public void AddCollectedItem()
    {
        // get random word from challenge items
        var randomWord = getRandomWord(challengeWordsInPlay);

        CollectedChallengeWords.Add(randomWord);
        challengeWordsInPlay.Remove(randomWord);

        GameManager.instance.DisplayCollectedItem(randomWord);

        AllChallengesCollected = !challengeWordsInPlay.Any();
    }

    public void BeginEndOfLevelChallenge()
    {
        Debug.Log("BeginEndofLevelChallenge was called");
        GameManager.instance.CharactersCanMove = false;

        for (var i = 0; i < CollectedChallengeWords.Count; i++)
        {
            GameObject challengeWordObj = new GameObject();
            Text challengeWordText = challengeWordObj.AddComponent<Text>();

            challengeWordText.text = CollectedChallengeWords.ElementAt(i);
            challengeWordText.fontSize = 32;
            challengeWordText.alignment = TextAnchor.MiddleCenter;
            challengeWordText.color = Color.white;
            challengeWordText.font = GameManager.instance.WordFont;

            var challengeWordTextRectTransform = challengeWordText.GetComponent<RectTransform>();

            challengeWordTextRectTransform.SetParent(GameManager.instance.Canvas.transform);

            challengeWordTextRectTransform.offsetMin = new Vector2(0, 0.5f);
            challengeWordTextRectTransform.offsetMax = new Vector2(0, 0.5f);
            challengeWordTextRectTransform.anchoredPosition = new Vector2((i + 1) * 80, 0);
            challengeWordTextRectTransform.localScale = new Vector3(1, 1, 1);
            challengeWordTextRectTransform.anchorMin = new Vector2(0, 0.5f);
            challengeWordTextRectTransform.anchorMax = new Vector2(0, 0.5f);
            challengeWordTextRectTransform.pivot = new Vector2(0, 0.5f);
            challengeWordTextRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 64);
            challengeWordTextRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 256);
            challengeWordObj.SetActive(true);

            challengeModeWords.AddLast(new ChallengeWord
            {
                Word = CollectedChallengeWords.ElementAt(i),
                ChallengeModeWordText = challengeWordText,
                IsCurrentWord = false,
                IsSelected = false
            });
        }

        challengeModeWords.First.Value.IsSelected = true;
        ChallengeWord randomWord = getRandomChallengeModeWord(challengeModeWords);
        speakWord(randomWord);

        ChallengeMode = true;
    }

    private void speakWord(ChallengeWord word)
    {
        WindowsVoice.speak($"Select the word {word.Word}");
    }

    private ChallengeWord getRandomChallengeModeWord(LinkedList<ChallengeWord> challengeModeWords)
    {
        // choose random word
        var randomWord = challengeModeWords.ElementAt(Random.Range(0, challengeModeWords.Count));
        randomWord.IsCurrentWord = true;
        return randomWord;
    }

    private string getRandomWord(HashSet<string> wordList)
    {
        System.Random random = new System.Random();

        var randomWord = wordList.ElementAt(random.Next(0, wordList.Count));

        return randomWord;
    }

    public class ChallengeWord
    {
        public string Word { get; set; }
        public bool IsSelected { get; set; }
        public bool IsCurrentWord { get; set; }
        public Text ChallengeModeWordText { get; set; }
    }
}
