using UnityEngine;
using TMPro;
using Unity.Netcode;

public class Ball : NetworkBehaviour
{
    private Rigidbody2D rb;
    public Rigidbody2D PlayerRB;
    private Vector2 momento;
    private Vector2 NewVelocidad;
    private float alpha;
    private PaddleController Player;
    private int Player1Score;

    public TextMeshProUGUI Player1ScoreText;

    void Start()
    {
        Player = GetComponent<PaddleController>();
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.velocity = new Vector2(5f, 5f);

        Player1Score = 0;
    }

    Vector4 MultiplyVectorByMatrix(Vector4 vector, Matrix4x4 matrix)
    {
        Vector4 result = new Vector4();
        for (int col = 0; col < 4; col++)
        {
            result[col] =
                vector[0] * matrix[0, col] +
                vector[1] * matrix[1, col] +
                vector[2] * matrix[2, col] +
                vector[3] * matrix[3, col];
        }
        return result;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Goal")
        {
            Player1Score++;

            Player1ScoreText.text = Player1Score.ToString();
        }
    }
}
