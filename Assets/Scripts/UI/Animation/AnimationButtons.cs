using UnityEngine;

public class AnimationButtons : MonoBehaviour
{
    public AnimationManager animationManager;
    public AnimationManager reverseAnimation;

    public void TestingAnim()
    {
        animationManager.PlayGroup("Tes");
    }

    public void TestingReverse()
    {
        reverseAnimation.PlayGroup("Tes");
    }
}
