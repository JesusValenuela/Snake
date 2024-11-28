using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;

public class SnakeHead : MonoBehaviour
{
    enum Direction
    {
        left,
        up,
        down,
        right
    }

    Direction direction;

    public List<Transform> tail = new List<Transform>();

    public float frameRate = 0.16f;
    public float step = 0.32f;

    public GameObject TailPrefab;

    public Vector2 horizontalRange;
    public Vector2 verticalRange;

    public UnityEngine.UI.Text scoreText; // Referencia al componente de texto Score
    public Button restartButton; // Botón de reinicio
    private int score = 0; // Contador de puntos

    public UnityEngine.UI.Text gameOverText; // Referencia al componente de texto GameOver



    // Start is called before the first frame update
    void Start()
    {
        gameOverText.gameObject.SetActive(false); // Desctiva el texto GameOver
        InvokeRepeating("Move", frameRate, frameRate);
        UpdateScore(); // Actualizar el puntaje al inicio
        restartButton.gameObject.SetActive(false); // Oculta el botón al inicio
        restartButton.onClick.AddListener(RestartGame); // Vincula el botón con la función
    }

    void Move()
    {
        Lastpos = transform.position;

        Vector3 nextPos = Vector3.zero;
        if (direction == Direction.up)
            nextPos = Vector3.up;
        else if (direction == Direction.down)
            nextPos = Vector3.down;
        else if (direction == Direction.left)
            nextPos = Vector3.left;
        else if (direction == Direction.right)
            nextPos = Vector3.right;
        nextPos *= step;
        transform.position += nextPos;

        MoveTail();
    }

    Vector3 Lastpos;
    void MoveTail()
    {
        for (int i = 0; i < tail.Count; i++)
        {
            Vector3 temp = tail[i].position;
            tail[i].position = Lastpos;
            Lastpos = temp;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && direction != Direction.down)
            direction = Direction.up;
        else if (Input.GetKeyDown(KeyCode.DownArrow) && direction != Direction.up)
            direction = Direction.down;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && direction != Direction.right)
            direction = Direction.left;
        else if (Input.GetKeyDown(KeyCode.RightArrow) && direction != Direction.left)
            direction = Direction.right;
    }

    void UpdateScore()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Block"))
        {
            gameOverText.gameObject.SetActive(true); // Activa el texto GameOver
            CancelInvoke("Move"); // Detener el movimiento
            enabled = false; // Deshabilitar este script para evitar más actualizaciones
            GameOver(); // Llama a GameOver si choca con un muro o su cola

            // Reiniciar el juego después de 3 segundos
            //Invoke("RestartGame", 3f);
        }

        else if (col.CompareTag("Food"))
        {
            // Agregar tail al cuerpo
            tail.Add(Instantiate(TailPrefab, tail[tail.Count - 1].position, Quaternion.identity).transform);

            //Nueva comida sin empalmar con Head y Tail
            Vector2 newPosition;
            bool positionValid;
            do
            {
                positionValid = true;
                newPosition = new Vector2(UnityEngine.Random.Range(horizontalRange.x - 3, verticalRange.y + 3), UnityEngine.Random.Range(horizontalRange.x + 3, verticalRange.y - 3));


                // Si la nueva comida choca con Head
                if (newPosition == (Vector2)transform.position)
                {
                    positionValid = false;
                    continue;
                }

                // Si la nueva comida choca con alguna parte de la cola
                foreach (Transform segment in tail)
                {
                    if (newPosition == (Vector2)segment.position)
                    {
                        positionValid = false;
                        break;
                    }
                }

            } while (!positionValid);

            col.transform.position = newPosition;

            // Incrementar velocidad
            frameRate = Mathf.Max(0.05f, frameRate - 0.002f); // Reduce frameRate hasta un límite inferior
            CancelInvoke("Move");
            InvokeRepeating("Move", frameRate, frameRate);

            // Incrementar puntos
            score += 10;
            UpdateScore(); // Actualizar el puntaje mostrado
        }
    }

    public void RestartGame()
    {
        // Reinicia la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void GameOver()
    {
        // Muestra el botón y detiene el juego
        restartButton.gameObject.SetActive(true);
        CancelInvoke("Move"); // Detiene el movimiento de la serpiente
    }
}
