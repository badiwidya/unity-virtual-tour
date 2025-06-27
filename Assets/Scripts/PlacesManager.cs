using System;
using JetBrains.Annotations;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    [CanBeNull] public GameObject quizPanel;

    public NarrationPage[] narrationPages;
}

public class PlacesManager : MonoBehaviour
{
    public PlaceData[] allPlaces;
    public UIPositioner uiPositioner;

    public TMP_Text narrationDisplayText;

    public Button narrationPreviousButton;

    public Button narrationNextButton;

    public Button narrationContinueButton;

    public TMP_Text narrationContinueButtonText;

    private int _currentPlaceIndex = 0;
    private int _currentNarrationPageIndex = 0;

    void Start()
    {
        foreach (var place in allPlaces)
        {
            if (place.sphereObject != null) place.sphereObject.SetActive(false);
            if (place.narrationPanel != null) place.narrationPanel.SetActive(false);
            if (place.quizPanel != null) place.quizPanel.SetActive(false);
        }

        ShowPlace(0);
    }

    public void ShowPlace(int index)
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
        var currentPlace = allPlaces[_currentPlaceIndex];

        if (currentPlace.quizPanel != null) currentPlace.quizPanel.SetActive(false);

        if (currentPlace.narrationPanel == null) return;

        uiPositioner.PositionObjectInFrontOfPlayer(currentPlace.narrationPanel);
        currentPlace.narrationPanel.SetActive(true);
        UpdateNarrationContent();
    }

    public void ShowQuizForCurrentPlace()
    {
        var currentPlace = allPlaces[_currentPlaceIndex];
        if (currentPlace.quizPanel == null)
        {
            ShowNextPlace();
            return;
        }

        if (currentPlace.narrationPanel != null) currentPlace.narrationPanel.SetActive(false);

        uiPositioner.PositionObjectInFrontOfPlayer(currentPlace.quizPanel);
        currentPlace.quizPanel.SetActive(true);
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

        narrationContinueButtonText.text = currentPlace.quizPanel != null ? "Lanjut ke Kuis" : "Selanjutnya";

        if (Array.IndexOf(allPlaces, currentPlace) == allPlaces.Length - 1)
        {
            narrationContinueButtonText.text = "Kembali ke Awal";
        }
    }

    public void OnContinueButtonPressed()
    {
        var currentPlace = allPlaces[_currentPlaceIndex];
        if (currentPlace.quizPanel != null)
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
        if (currentPlace.quizPanel != null) currentPlace.quizPanel.SetActive(false);
    }
}