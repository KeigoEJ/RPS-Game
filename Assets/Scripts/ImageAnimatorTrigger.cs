using UnityEngine;
using UnityEngine.UI;

public class ImageAnimatorTrigger : MonoBehaviour
{
    [Header("🎞 Image Animation Settings")]
    public Animator imageAnimator; // The animator attached to your image
    public string triggerName = "Enter"; // The trigger you want to fire

    [Header("⏱ Optional Delay")]
    public float delayBeforeTrigger = 0f; // Set if you want it to wait before triggering

    void Start()
    {
        if (imageAnimator == null)
        {
            Debug.LogError("❌ ImageAnimatorTrigger: No Animator assigned!");
            return;
        }

        if (delayBeforeTrigger > 0f)
            Invoke(nameof(TriggerAnimation), delayBeforeTrigger);
        else
            TriggerAnimation();
    }

    void TriggerAnimation()
    {
        imageAnimator.SetTrigger(triggerName);
    }
}
