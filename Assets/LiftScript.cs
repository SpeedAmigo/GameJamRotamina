using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LiftScript : MonoBehaviour
{
    [SerializeField] private DOTweenAnimation leftDoorAnimation;
    [SerializeField] private DOTweenAnimation rightDoorAnimation;

    [SerializeField] private string currentNumbers;
    [SerializeField] private TMP_InputField inputField;

    private int codeNumber = 123;
    
    

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
        if (inputField.text.Length >= 3) return;
        inputField.text += number.ToString();
    }

    public void RemoveNumber()
    {
        if (inputField.text.Length <= 0) return;
        inputField.text = inputField.text.Remove(inputField.text.Length - 1);
    }

    public void CheckCode()
    {
        if (inputField.text == codeNumber.ToString())
        {
            OpenDoors();
            inputField.text = "_";
        }
    }
}
