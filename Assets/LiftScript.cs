using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class LiftScript : MonoBehaviour
{
    [SerializeField] private DOTweenAnimation leftDoorAnimation;
    [SerializeField] private DOTweenAnimation rightDoorAnimation;

    [Button]
    private void OpenDoors()
    {
        leftDoorAnimation.DOPlayForward();
        rightDoorAnimation.DOPlayForward();
    }

    [Button]
    private void CloseDoors()
    {
        leftDoorAnimation.DOPlayBackwards();
        rightDoorAnimation.DOPlayBackwards();
    }
}
