using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class LiftScript : MonoBehaviour
{
    public bool startLift;
    
    [SerializeField] private DOTweenAnimation leftDoorAnimation;
    [SerializeField] private DOTweenAnimation rightDoorAnimation;

    [SerializeField] private string currentNumbers;
    [SerializeField] private TMP_InputField inputField;

    public static Action OnGameOver;

    private string codeNumber;

    private void Start()
    {
        if (startLift)
        {
            OpenDoors();
        }
        
        codeNumber = CodeManager.Instance.GetCode();
        Debug.Log(codeNumber);
    }
    
    [Button]
    public void OpenDoors()
    {
        leftDoorAnimation.DOPlayForward();
        rightDoorAnimation.DOPlayForward();
    }

    [Button]
    public void CloseDoors()
    {
        leftDoorAnimation.DOPlayBackwards();
        rightDoorAnimation.DOPlayBackwards();
    }

    public void AddNumber(int number)
    {
        if (startLift) return;
        if (inputField.text.Length >= 3) return;
        inputField.text += number.ToString();
    }

    public void RemoveNumber()
    {
        if (inputField.text.Length <= 0) return;
        inputField.text = inputField.text.Remove(inputField.text.Length - 1);
    }

    public void GameOver()
    {
        OnGameOver?.Invoke();
    }

    public void CheckCode()
    {
        if (startLift) return;
        if (inputField.text == codeNumber)
        {
            OpenDoors();
            inputField.text = "_";
        }
    }
}
