using UnityEngine;

/// <summary>
/// This class showcases the PresentAnimator class callbacks.
/// </summary>
public class CallbackShowcase : MonoBehaviour
{
    // Reference to the PresentAnimator class
    private PresentAnimator presentAnimator;

    private void Start()
    {
        presentAnimator = GetComponent<PresentAnimator>();

        // We register our methods to the corresponding callbacks
        presentAnimator.IdleHandler += IdleHandler;
        presentAnimator.MouseOverHandler += MouseOverHandler;
        presentAnimator.OpeningHandler += OpeningHandler;
        presentAnimator.ClosingHandler += ClosingHandler;
    }

    public void IdleHandler()
    {
        print(this.name + " idling.");
    }

    public void MouseOverHandler()
    {
        print(this.name + " mouse over.");
    }

    public void OpeningHandler()
    {
        print(this.name + " opening.");
    }

    public void ClosingHandler()
    {
        print(this.name + " closing.");
    }
}
