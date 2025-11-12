using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public class UIButtonSafeAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Animator animator;
    private bool isReady = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        // Delay one frame to ensure animator reactivates properly
        StartCoroutine(InitializeAnimator());
    }

    System.Collections.IEnumerator InitializeAnimator()
    {
        yield return null; // wait one frame
        if (animator != null && animator.isActiveAndEnabled)
        {
            animator.Rebind(); // reset all internal states safely
            animator.Update(0f);
            isReady = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isReady || animator == null || !animator.isActiveAndEnabled) return;
        animator.ResetTrigger("End");
        animator.SetTrigger("Hover");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isReady || animator == null || !animator.isActiveAndEnabled) return;
        animator.ResetTrigger("Hover");
        animator.SetTrigger("End");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isReady || animator == null || !animator.isActiveAndEnabled) return;
        animator.SetTrigger("Click");
    }

    void OnDisable()
    {
        isReady = false;
    }
}
