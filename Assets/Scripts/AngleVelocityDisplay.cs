using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AngleVelocityDisplay : MonoBehaviour
{
    public TMP_Text MaxAngleText;  // Arrastra y suelta el componente de texto TMP aquí en el editor.

    private float deltaTime = 0.0f;

    private float maxIncrementoArrastrado = 100f;

    public Slider slider; // Referencia al Slider

    void Update()
    {
        maxIncrementoArrastrado = slider.value;

        // Calcula el tiempo delta
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        float fps = 1.0f / deltaTime;

        // Calcula los FPS
        float display = maxIncrementoArrastrado * Time.deltaTime;

        // Actualiza el texto con los FPS
        MaxAngleText.text = string.Format("{0:0.0}", display);
    }
}
