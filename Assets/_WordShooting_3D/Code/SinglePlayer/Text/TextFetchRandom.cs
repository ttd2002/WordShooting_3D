using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TextFetchRandom : MonoBehaviour
{
    private const string wordApiUrl = "https://random-word-api.vercel.app/api?words=500";
    private const string paragraphApiUrl = "https://baconipsum.com/api/?type=all-meat&paras=1&format=text";

    private string[] words;
    private List<string> paragraphs = new List<string>();

    public IEnumerator FetchRandomWordsAndParagraphsOnce(System.Action<string[]> wordCallback, System.Action<List<string>> paragraphCallback, int numberOfParagraphs)
    {
        // Fetch words if not already fetched
        if (words == null)
        {
            UnityWebRequest wordRequest = UnityWebRequest.Get(wordApiUrl);
            yield return wordRequest.SendWebRequest();

            if (wordRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to fetch random words: " + wordRequest.error);
                wordCallback(null);
            }
            else
            {
                string jsonResult = wordRequest.downloadHandler.text;
                words = ParseWordsFromJson(jsonResult);
                wordCallback(words);
            }
        }
        else
        {
            wordCallback(words);
        }

        // Fetch multiple paragraphs
        for (int i = 0; i < numberOfParagraphs; i++)
        {
            UnityWebRequest paragraphRequest = UnityWebRequest.Get(paragraphApiUrl);
            yield return paragraphRequest.SendWebRequest();

            if (paragraphRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to fetch paragraph: " + paragraphRequest.error);
                paragraphCallback(null);
                yield break;
            }
            else
            {
                string paragraphResult = paragraphRequest.downloadHandler.text;
                string shortParagraph = GetFirstTenWords(paragraphResult);
                paragraphs.Add(shortParagraph);
            }
        }

        paragraphCallback(paragraphs);
    }

    public virtual string GetRandomWord()
    {
        if (words == null || words.Length == 0) return "hello";
        int randomIndex = Random.Range(0, words.Length);
        return words[randomIndex];
    }

    public virtual string GetRandomParagraph()
    {
        if (paragraphs.Count == 0) return "Hello, nice to meet you";
        int randomIndex = Random.Range(0, paragraphs.Count);
        return paragraphs[randomIndex];
    }

    private string[] ParseWordsFromJson(string json)
    {
        json = json.Trim('[', ']');
        string[] wordArray = json.Split(new string[] { "\",\"" }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < wordArray.Length; i++)
        {
            wordArray[i] = wordArray[i].Trim('"');
        }
        return wordArray;
    }

    private string GetFirstTenWords(string paragraph)
    {
        string[] words = paragraph.Split(' ');
        if (words.Length > 5)
        {
            return string.Join(" ", words, 0, 5);
        }
        return paragraph;
    }
}