using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class AnswerData
{
    public string answerText;
    public bool isCorrect;
}

[System.Serializable]
public class QuizData
{
    [TextArea(3, 5)] public string questionText;

    public AnswerData[] answers = new AnswerData[3];
}

[System.Serializable]
public class NarrationPage
{
    [TextArea(5, 10)] public string narrationText;
}

[System.Serializable]
public class PlaceData
{
    public string placeName;
    public GameObject sphereObject;
    public GameObject narrationPanel;

    public bool hasQuiz;
    public QuizData quizData;

    public NarrationPage[] narrationPages;
}

public class PlacesManager : MonoBehaviour
{
    public PlaceData[] allPlaces;
    public UIPositioner uiPositioner;

    [Header("Referensi Komponen Panel Narasi")]
    public TMP_Text narrationDisplayText;

    public Button narrationPreviousButton;
    public Button narrationNextButton;
    public Button narrationContinueButton;
    public TMP_Text narrationContinueButtonText;

    [Header("Referensi Komponen Panel Kuis")]
    public GameObject quizPanelObject;
    public TMP_Text questionText;
    public Button[] answerButtons = new Button[3];

    [Header("Referensi Transisi")]
    public Image fadeImage;
    public float fadeDuration = 0.5f;
    
    private int _currentPlaceIndex = 0;
    private int _currentNarrationPageIndex = 0;
    private readonly Color[] _originalButtonColors = new Color[3];

    void Start()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            _originalButtonColors[i] = answerButtons[i].GetComponent<Image>().color;
        }

        foreach (var place in allPlaces)
        {
            if (place.sphereObject != null) place.sphereObject.SetActive(false);
            if (place.narrationPanel != null) place.narrationPanel.SetActive(false);
        }

        if (quizPanelObject != null) quizPanelObject.SetActive(false);

        ShowPlace(0);
    }

    private void ShowPlace(int index)
    {
        if (index < 0 || index >= allPlaces.Length) return;

        HideCurrentPlace();

        _currentPlaceIndex = index;
        _currentNarrationPageIndex = 0;

        var currentPlace = allPlaces[_currentPlaceIndex];
        if (currentPlace.sphereObject != null)
        {
            currentPlace.sphereObject.SetActive(true);
        }

        ShowNarrationForCurrentPlace();
    }

    public void ShowNextPlace()
    {
        var nextIndex = _currentPlaceIndex + 1;
        if (nextIndex >= allPlaces.Length) nextIndex = 0;
        ShowPlace(nextIndex);
    }

    public void ShowNarrationForCurrentPlace()
    {
        if (quizPanelObject != null) quizPanelObject.SetActive(false);

        var currentPlace = allPlaces[_currentPlaceIndex];

        if (currentPlace.narrationPanel == null) return;

        uiPositioner.PositionObjectInFrontOfPlayer(currentPlace.narrationPanel);
        currentPlace.narrationPanel.SetActive(true);
        UpdateNarrationContent();
    }

    public void ShowQuizForCurrentPlace()
    {
        var currentPlace = allPlaces[_currentPlaceIndex];
        if (!currentPlace.hasQuiz)
        {
            ShowNextPlace();
            return;
        }

        if (currentPlace.narrationPanel != null) currentPlace.narrationPanel.SetActive(false);

        questionText.text = currentPlace.quizData.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponent<Image>().color = _originalButtonColors[i];
            answerButtons[i].GetComponentInChildren<TMP_Text>().text = currentPlace.quizData.answers[i].answerText;
            answerButtons[i].interactable = true;

            answerButtons[i].onClick.RemoveAllListeners();
            var answerIndex = i;
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(answerIndex));
        }

        uiPositioner.PositionObjectInFrontOfPlayer(quizPanelObject);
        quizPanelObject.SetActive(true);
    }

    public void OnAnswerSelected(int selectedIndex)
    {
        var currentPlace = allPlaces[_currentPlaceIndex];

        var isCorrect = currentPlace.quizData.answers[selectedIndex].isCorrect;

        if (isCorrect)
        {
            StartCoroutine(CorrectAnswerFeedback(answerButtons[selectedIndex]));
        }
        else
        {
            StartCoroutine(IncorrectAnswerFeedback(answerButtons[selectedIndex]));
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator IncorrectAnswerFeedback(Button selectedButton)
    {
        var originalText = selectedButton.GetComponentInChildren<TMP_Text>().text;

        selectedButton.GetComponent<Image>().color = Color.red;
        selectedButton.GetComponentInChildren<TMP_Text>().text = "SALAH";
        selectedButton.interactable = false;

        yield return new WaitForSeconds(1.5f);

        var buttonIndex = Array.IndexOf(answerButtons, selectedButton);
        selectedButton.GetComponent<Image>().color = _originalButtonColors[buttonIndex];
        selectedButton.GetComponentInChildren<TMP_Text>().text = originalText;
        selectedButton.interactable = true;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator CorrectAnswerFeedback(Button selectedButton)
    {
        foreach (var btn in answerButtons)
        {
            btn.interactable = false;
        }

        selectedButton.GetComponent<Image>().color = Color.green;
        selectedButton.GetComponentInChildren<TMP_Text>().text = "BENAR";

        yield return new WaitForSeconds(1.5f);

        ShowNextPlace();
    }

    public void ShowNextNarrationPage()
    {
        var currentPlace = allPlaces[_currentPlaceIndex];
        if (_currentNarrationPageIndex >= currentPlace.narrationPages.Length - 1) return;
        _currentNarrationPageIndex++;
        UpdateNarrationContent();
    }

    public void ShowPreviousNarrationPage()
    {
        if (_currentNarrationPageIndex <= 0) return;
        _currentNarrationPageIndex--;
        UpdateNarrationContent();
    }

    private void UpdateNarrationContent()
    {
        var currentPlace = allPlaces[_currentPlaceIndex];

        narrationDisplayText.text = currentPlace.narrationPages[_currentNarrationPageIndex].narrationText;

        narrationPreviousButton.interactable = (_currentNarrationPageIndex > 0);
        narrationNextButton.interactable = (_currentNarrationPageIndex < currentPlace.narrationPages.Length - 1);

        narrationContinueButton.gameObject.SetActive(_currentNarrationPageIndex ==
                                                     currentPlace.narrationPages.Length - 1);

        narrationContinueButtonText.text = currentPlace.hasQuiz ? "Lanjut ke Kuis" : "Selanjutnya";

        if (Array.IndexOf(allPlaces, currentPlace) == allPlaces.Length - 1)
        {
            narrationContinueButtonText.text = "Kembali ke Awal";
        }
    }

    public void OnContinueButtonPressed()
    {
        var currentPlace = allPlaces[_currentPlaceIndex];
        if (currentPlace.hasQuiz)
        {
            ShowQuizForCurrentPlace();
        }
        else
        {
            ShowNextPlace();
        }
    }

    private void HideCurrentPlace()
    {
        if (allPlaces.Length == 0) return;
        var currentPlace = allPlaces[_currentPlaceIndex];
        if (currentPlace.sphereObject != null) currentPlace.sphereObject.SetActive(false);
        if (currentPlace.narrationPanel != null) currentPlace.narrationPanel.SetActive(false);
        if (currentPlace.hasQuiz && quizPanelObject != null) quizPanelObject.SetActive(false);
    }
    
    private IEnumerator Fade(float targetAlpha)
    {
        var startAlpha = fadeImage.color.a;
        var elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            var newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, targetAlpha);
    }

    private IEnumerator TransitionToPlace(int index)
    {
        yield return StartCoroutine(Fade(1f));

        ShowPlace(index);

        yield return StartCoroutine(Fade(0f));
    }
}