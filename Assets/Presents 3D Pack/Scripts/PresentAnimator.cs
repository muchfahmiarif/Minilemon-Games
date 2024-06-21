using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
#if UNITY_EDITOR
[CanEditMultipleObjects]
#endif
public class PresentAnimator : MonoBehaviour
{
    // Animation Component
    Animator animatorComponent;

    // Parent for all instantiated FX
    public GameObject FXRoot;

    public GameObject[] ContainedItems;
    public bool IsPreloadContainedItems = true;
    private GameObject[] containedItemsPool;

    // Animation names shown in the editor
    // Used to assign specific animation to each present state
    // Those names must match the animations listed in the Animation Component
    public enum AnimNames { Idle, Breath, Jump, Open, Explode, Close }

    // Idle state variables
    // Animation to be played during the Idle state
    public AnimNames IdleAnim;
    // FX / Particle systems to be played during the Idle state
    public ParticleSystem[] IdleFX;
    // Toggle all Idle FX to be pre-loaded / instantiated when this script starts
    // if disabled, they will be instantiated when needed
    public bool IsPreLoadIdleFX = true;
    // FX playback delay in seconds
    public float IdleFXDelay = 0f;
    // Toggle the FX to be played as loops
    public bool IsIdleFXLoop;
    // FX pool (per present), keep a reference of all Idle FX already loaded
    // FX are not destroyed, they are simply stopped, this way they can be re-started if needed without any instantiation
    private ParticleSystem[] IdleFXPool;
    public Action IdleHandler;

    // Mouse Over state variables
    public bool EnableMouseOver;
    public AnimNames MouseOverAnim;
    // This is the cross-fade duration between the Mouse Over and Idle animation
    // Used for a smoother transition between the 2 animations
    public float MouseOverFadeOut = 0.5f;
    public ParticleSystem[] MouseOverFX;
    public bool IsPreLoadMouseOverFX = true;
    public float MouseOverFXDelay = 0f;
    public bool IsMouseOverFXLoop;
    private ParticleSystem[] MouseOverFXPool;
    public Action MouseOverHandler;

    // Clicked state variables
    public bool EnableClick;
    public AnimNames ClickedAnim;
    public ParticleSystem[] ClickedFX;
    public bool IsPreLoadClickedFX = true;
    public float ClickedFXDelay = 0f;
    public bool IsClickedFXLoop;
    private ParticleSystem[] ClickedFXPool;
    public Action OpeningHandler;

    // Close Back state variables
    public bool EnableCloseBack;
    public bool ClickToCloseBack;
    public bool AutoCloseBack;
    public float CloseBackDelay = 1f;
    public ParticleSystem[] CloseBackFX;
    public bool IsPreLoadCloseBackFX = true;
    public float CloseBackFXDelay = 0f;
    public bool IsCloseBackFXLoop;
    private ParticleSystem[] CloseBackFXPool;
    public Action ClosingHandler;

    // Present states to handle animation and FX playback
    private enum presentStates { Idle, MouseOver, Opening, Closing }
    // Only 1 state is active at a time
    private presentStates _presentState;
    private presentStates presentState
    {
        set
        {
            _presentState = value;

            switch (value)
            {
                case presentStates.Idle:
                    StopFX(MouseOverFXPool);
                    StopFX(CloseBackFXPool);
                    animatorComponent.CrossFade(IdleAnim.ToString(), MouseOverFadeOut);
                    StartCoroutine(PlayFX(IdleFX, IdleFXDelay, IsIdleFXLoop, IdleFXPool));
                    if (IdleHandler != null) IdleHandler();
                    break;
                case presentStates.MouseOver:
                    StopFX(IdleFXPool);
                    StopFX(CloseBackFXPool);
                    animatorComponent.Play(MouseOverAnim.ToString());
                    StartCoroutine(PlayFX(MouseOverFX, MouseOverFXDelay, IsMouseOverFXLoop, MouseOverFXPool));
                    if (MouseOverHandler != null) MouseOverHandler();
                    break;
                case presentStates.Opening:
                    StopFX(IdleFXPool);
                    StopFX(MouseOverFXPool);
                    animatorComponent.Play(ClickedAnim.ToString());
                    StartCoroutine(PlayFX(ClickedFX, ClickedFXDelay, IsClickedFXLoop, ClickedFXPool));
                    if (OpeningHandler != null) OpeningHandler();
                    break;
                case presentStates.Closing:
                    StopFX(ClickedFXPool);
                    animatorComponent.Play(AnimNames.Close.ToString());
                    StartCoroutine(PlayFX(CloseBackFX, CloseBackFXDelay, IsCloseBackFXLoop, CloseBackFXPool));
                    if (ClosingHandler != null) ClosingHandler();
                    break;
                default:
                    break;
            }
        }
        get
        {
            return _presentState;
        }
    }

    private void Start()
    {
        // FX pre-loading
        if (IsPreLoadIdleFX) PreloadFX(IdleFX, IsIdleFXLoop, ref IdleFXPool);
        if (IsPreLoadMouseOverFX) PreloadFX(MouseOverFX, IsMouseOverFXLoop, ref MouseOverFXPool);
        if (IsPreLoadClickedFX) PreloadFX(ClickedFX, IsClickedFXLoop, ref ClickedFXPool);
        if (IsPreLoadCloseBackFX) PreloadFX(CloseBackFX, IsCloseBackFXLoop, ref CloseBackFXPool);

        // Contained items pre-load
        if (IsPreloadContainedItems) PreloadGameObjects(ContainedItems, ref containedItemsPool);

        animatorComponent = GetComponent<Animator>();

        //AnimationClip openedClip = animationComponent.GetClip(AnimNames.Open.ToString());
        //AnimationEvent openedEvent = new AnimationEvent();
        //openedEvent.time = openedClip.length - 0.01f;
        //openedEvent.functionName = "IsOpened";
        //AnimationUtility.SetAnimationEvents(openedClip, new AnimationEvent[] { openedEvent });
        

        // default starting state
        presentState = presentStates.Idle;
    }

    /// <summary>
    /// Instantiate and initialize FX into a pool
    /// </summary>
    /// <param name="fxs">FX to be played</param>
    /// <param name="isLoop">Are the FX to be played as loops</param>
    /// <param name="fxPool">FX pool</param>
    void PreloadFX(ParticleSystem[] fxs, bool isLoop, ref ParticleSystem[] fxPool)
    {
        if (fxs == null || fxs.Length == 0) return;

        if (fxPool == null)
        {
            fxPool = new ParticleSystem[fxs.Length];

            for (int i = 0; i < fxs.Length; i++)
            {
                if (fxs[i] != null)
                {
                    fxPool[i] = Instantiate(fxs[i]);

                    if (FXRoot != null)
                    {
                        fxPool[i].transform.SetParent(FXRoot.transform);
                    }
                    else
                    {
                        fxPool[i].transform.SetParent(this.transform);
                    }

                    fxPool[i].transform.localRotation = Quaternion.identity;
                    fxPool[i].transform.localPosition = Vector3.zero;

                    var main = fxPool[i].main;
                    main.loop = isLoop;
                }
            }
        }
    }

    /// <summary>
    /// Playback FX, preload them first if needed
    /// </summary>
    /// <param name="fxs">FX to be played</param>
    /// <param name="delay">FX playback delay in seconds</param>
    /// <param name="isLoop">Are the FX to be played as loops</param>
    /// <param name="fxPool">FX pool</param>
    IEnumerator PlayFX(ParticleSystem[] fxs, float delay, bool isLoop, ParticleSystem[] fxPool)
    {
        if (fxs == null || fxs.Length == 0) yield break;

        if (fxPool == null)
        {
            PreloadFX(fxs, isLoop, ref fxPool);
        }

        if (delay > 0f) yield return new WaitForSeconds(delay);

        for (int i = 0; i < fxPool.Length; i++)
        {
            fxPool[i].Play();
        }
    }

    /// <summary>
    /// Stop FX
    /// </summary>
    void StopFX(ParticleSystem[] fxs)
    {
        if (fxs == null || fxs.Length == 0) return;

        for (int i = 0; i < fxs.Length; i++)
        {
            if (fxs[i] != null) fxs[i].Stop();
        }
    }

    private void OnMouseEnter()
    {
        if (EnableMouseOver && presentState == presentStates.Idle)
        {
            presentState = presentStates.MouseOver;
        }
    }

    private void OnMouseOver()
    {
        if (EnableMouseOver && presentState == presentStates.Idle)
        {
            presentState = presentStates.MouseOver;
        }
    }

    private void OnMouseExit()
    {
        if (EnableMouseOver && presentState == presentStates.MouseOver)
        {
            presentState = presentStates.Idle;
        }
    }

    private void OnMouseDown()
    {
        if (EnableClick)
        {
            if (presentState != presentStates.Opening)
            {
                presentState = presentStates.Opening;
            }
            else
            {
                if (EnableCloseBack && ClickToCloseBack)
                    presentState = presentStates.Closing;
            }
        }
    }

    /// <summary>
    /// Called from the animation at the end of the opening sequence
    /// </summary>
    public void IsOpened()
    {
        ActivateGameObjects(ContainedItems, ref containedItemsPool);
        StartCoroutine(CloseBackDelayed());
    }

    /// <summary>
    /// Delayed call of the closing state
    /// </summary>
    IEnumerator CloseBackDelayed()
    {
        if (!EnableCloseBack || !AutoCloseBack) yield break;
        if (CloseBackDelay > 0f) yield return new WaitForSeconds(CloseBackDelay);
        presentState = presentStates.Closing;
    }

    /// <summary>
    /// Called from the animation at the end of the closing sequence
    /// </summary>
    public void IsClosed()
    {
        presentState = presentStates.Idle;
    }

    /// <summary>
    /// Preload game object array into a pool, position them and disable them.
    /// </summary>
    void PreloadGameObjects(GameObject[] gameObjects, ref GameObject[] gameObjectsPool)
    {
        if (gameObjects == null || gameObjects.Length == 0) return;

        gameObjectsPool = new GameObject[gameObjects.Length];

        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (gameObjects[i] != null)
            {
                gameObjectsPool[i] = Instantiate<GameObject>(gameObjects[i]);
                gameObjectsPool[i].transform.SetParent(this.transform);
                gameObjectsPool[i].transform.position = Vector3.zero;
                gameObjectsPool[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// Activate game objects from the Game Object pool.
    /// </summary>
    void ActivateGameObjects(GameObject[] gameObjects, ref GameObject[] gameObjectsPool)
    {
        if (gameObjects == null || gameObjects.Length == 0) return;

        if (gameObjectsPool == null || gameObjectsPool.Length == 0) PreloadGameObjects(gameObjects, ref gameObjectsPool);

        for (int i = 0; i < gameObjectsPool.Length; i++)
        {
            if (gameObjectsPool[i] != null)
            {
                gameObjectsPool[i].SetActive(true);
            }
        }
    }
}
