using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class LiftScript : MonoBehaviour
{
    [SerializeField] private DOTweenAnimation leftDoorAnimation;
    [SerializeField] private DOTweenAnimation rightDoorAnimation;

    [SerializeField] private string currentNumbers;

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
        Debug.Log(number);
        
        currentNumbers += number.ToString();
    }

    public void RemoveNumber()
    {
        
    }
}
