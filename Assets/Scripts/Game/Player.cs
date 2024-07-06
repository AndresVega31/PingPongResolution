using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PaddleController : NetworkBehaviour
{
    private Vector2 touchOffset;
    private bool dragging = false;
    private Vector2 lastPosition;
    private float lastAngle = 0f;
    private float lastAngle_ = 0f;
    private float maxIncrementoEstacionario = 1f;
    private float maxIncrementoArrastrado = 100f;
    private Vector2 direction;
    private bool angleGoal = false;
    private float maxAngle = 50f;

    private float baseSpeed = 7f;

    // Referencias a los límites
    public Transform WallTop;
    public Transform WallBottom;
    public Transform Goal;
    public Transform Mid;

    public Slider slider; // Referencia al Slider
   
    //Multiplayer
    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1);
    [SerializeField] private Transform Ball;

    void Start()
    {
        if (!IsOwner) return;

        if (OwnerClientId == 0)
        {
            Transform ball = Instantiate(Ball);
            ball.GetComponent<NetworkObject>().Spawn(true);
            transform.position = new Vector3(-8, 0, 0f);
        }

        if (OwnerClientId == 1)
        {
            transform.position = new Vector3(8, 0, 0f);
        }

    }
    void Update()
    {
        Debug.Log(OwnerClientId + ": " + randomNumber.Value);

        if (!IsOwner) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Detectar si el toque es sobre la raqueta
                    RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
                    if (hit.collider != null && hit.collider.gameObject == gameObject)
                    {
                        dragging = true;
                        touchOffset = (Vector2)transform.position - touchPosition;
                        lastPosition = transform.position;
                    }
                    break;

                case TouchPhase.Stationary:
                    lastAngle_ = 0;

                    if (Mathf.Abs(lastAngle_ - lastAngle) >= maxIncrementoEstacionario)
                    {
                        lastAngle += Mathf.Abs(lastAngle_ - lastAngle) * maxIncrementoEstacionario / (lastAngle_ - lastAngle);
                    }
                    else
                    {
                        lastAngle = lastAngle_;
                    }

                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, lastAngle));
                    break;

                case TouchPhase.Moved:
                    if (dragging)
                    {
                        //Borders of movement based on game objects
                        float right = Mid.position.x;
                        float left = Goal.position.x + 1.2f;
                        float top = WallTop.position.y;
                        float down = WallBottom.position.y;

                        // Move player with touch
                        Vector2 newPosition = touchPosition + touchOffset;

                        // Restrict movement
                        float clampedX = Mathf.Clamp(newPosition.x, left, right);
                        float clampedY = Mathf.Clamp(newPosition.y, down, top);
                        newPosition = new Vector2(clampedX, clampedY);

                        transform.position = Vector2.Lerp(transform.position, newPosition, baseSpeed * Time.deltaTime);

                        maxIncrementoArrastrado = slider.value; //Consigue valor del slider

                        // Calculate the direction of the movement
                        Vector2 direction_ = newPosition - lastPosition;


                        if (direction_.magnitude > 0)
                        {
                            direction = newPosition - lastPosition;
                        }

                        if ((direction != Vector2.zero) && (direction.magnitude > 0.1))
                        {
                            if (angleGoal)
                            {
                                lastAngle_ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                                if (lastAngle_ > maxAngle)
                                {
                                    lastAngle_ = maxAngle;
                                }
                                else if (lastAngle_ < -maxAngle)
                                {   
                                    lastAngle_ = -maxAngle;
                                }
                            }
                        }
                        else if (direction.magnitude < 0.1)
                        {
                            lastAngle_ = 0;
                        }

                        float absX = Mathf.Abs(direction.x);

                        
                        //Player can't rotate moving backwards
                        if ( direction.x < 0)
                        {
                            lastAngle_ = 0;
                        }
                        else if (absX < 0.15) //Player can't rotate with minimun movement in x
                        {
                            lastAngle_ = 0;
                        }

                        if (Mathf.Abs(lastAngle_ - lastAngle) >= (maxIncrementoArrastrado * Time.deltaTime))
                        {
                            lastAngle += Mathf.Abs(lastAngle_ - lastAngle) * (maxIncrementoArrastrado * Time.deltaTime) / (lastAngle_ - lastAngle);
                        }
                        else
                        {
                            lastAngle = lastAngle_;
                        }

                        if (lastAngle_ == lastAngle)
                        {
                            angleGoal = true;
                        }
                        else
                        {
                            angleGoal = false;
                        }

                        transform.rotation = Quaternion.Euler(new Vector3(0, 0, lastAngle));
                        lastPosition = newPosition;
                    }
                    break;

                case TouchPhase.Ended:
                    dragging = false;
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    break;
                case TouchPhase.Canceled:
                    dragging = false;
                    break;
            }
        }
    }
}
