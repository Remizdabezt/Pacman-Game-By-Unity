using UnityEngine;

[DefaultExecutionOrder(-10)]
[RequireComponent(typeof(Movement))]
public class Ghost : MonoBehaviour
{
    public Rigidbody2D rigidBody { get; private set; }
    public Movement movement { get; private set; }
    public GhostHome home { get; private set; }
    public GhostScatter scatter { get; private set; }
    public GhostChase chase { get; private set; }
    public GhostFrightened frightenned { get; private set; }
    public GhostBehavior initialBehavior;
    public Transform target;
    public int points = 200;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        movement = GetComponent<Movement>();
        home = GetComponent<GhostHome>();
        scatter = GetComponent<GhostScatter>();
        chase = GetComponent<GhostChase>();
        frightenned = GetComponent<GhostFrightened>();
    }

    public void ResetState()
    {
        gameObject.SetActive(true);
        movement.ResetState();

        frightenned.Disable();
        chase.Disable();
        scatter.Enable();

        if (home != initialBehavior)
        {
            home.Disable();
        }

        if (initialBehavior != null)
        {
            initialBehavior.Enable();
        }
    }

    public void SetPosition(Vector3 position)
    {
        rigidBody.position = position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            if (frightenned.enabled)
            {
                FindObjectOfType<GameManager>().GhostEaten(this);
            }
            else
            {
                FindObjectOfType<GameManager>().PacmanEaten();
            }
        }
    }

}
