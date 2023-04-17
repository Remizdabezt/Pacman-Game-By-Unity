using System.Collections;
using UnityEngine;

public class GhostHome : GhostBehavior
{
    public Transform inside;
    public Transform outside;
    public float Elapsed { get; private set; }
    private void OnDisable()
    {
        if (this.gameObject.activeSelf)
        {
            StartExit(0);
        }
    }

    private void OnEnable()
    {
        if (this.gameObject.activeSelf)
        {
            StopAllCoroutines();
        }
        this.Elapsed = 0;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (this.enabled && collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            this.ghost.movement.SetDirection(-this.ghost.movement.direction, true);
        }
    }

    public void StartExit(float elapsed)
    {
        StartCoroutine(ExitTransition(elapsed));
    }
    private IEnumerator ExitTransition(float elapsed)
    {
        this.ghost.movement.SetDirection(Vector2.up, true);
        this.ghost.movement.rigidbody.isKinematic = true;
        this.ghost.movement.enabled = false;

        Vector3 position = this.transform.position;
        float duration1 = 0.5f;
        float duration2 = 1f;

        while (elapsed <= duration1)
        {
            Vector3 newPosition = Vector3.Lerp(position, this.inside.position, elapsed / duration1);
            newPosition.z = position.z;
            this.ghost.transform.position = newPosition;
            elapsed += Time.deltaTime;
            this.Elapsed = elapsed;
            yield return null;
        }
        while (elapsed <= (duration2 + 0.35f))
        {
            Vector3 newPosition = Vector3.Lerp(this.inside.position, this.outside.position, (elapsed - 0.5f) / (duration2 - 0.5f));
            newPosition.z = position.z;
            this.ghost.transform.position = newPosition;
            elapsed += Time.deltaTime;
            this.Elapsed = elapsed;
            yield return null;
        }
        this.ghost.movement.SetDirection(new Vector2(Random.value < 0.5 ? -1f : 1f, 0f), true);
        this.ghost.movement.rigidbody.isKinematic = false;
        this.ghost.movement.enabled = true;
        this.Elapsed = 0;
    }

}
