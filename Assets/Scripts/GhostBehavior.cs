using UnityEngine;

[RequireComponent(typeof(Ghost))]
public abstract class GhostBehavior : MonoBehaviour
{
    public Ghost ghost { get; private set; }
    public float duration;
    public float passedDuration;
    public float initialDuration;

    private void Awake()
    {
        ghost = GetComponent<Ghost>();
        this.enabled = false;
    }

    public void Enable()
    {
        Enable(initialDuration);
    }

    public virtual void Enable(float duration)
    {
        enabled = true;
        passedDuration = 0;
        this.duration = duration;
        CancelInvoke();
        Invoke(nameof(Disable), duration);
    }

    public virtual void Disable()
    {
        enabled = false;
        passedDuration = 0;
        CancelInvoke();
    }
    protected virtual void Update()
    {
        passedDuration += Time.deltaTime;
    }
    public float DurationRemaining()
    {
        if (!this.enabled)
            return 0;
        return duration - passedDuration;
    }
}
