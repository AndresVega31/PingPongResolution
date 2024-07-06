using UnityEngine;
using TMPro;

public class FPSDisplayTMP : MonoBehaviour
{
    public TMP_Text fpsText;  // Arrastra y suelta el componente de texto TMP aquí en el editor.

    private float deltaTime = 0.0f;

    void Update()
    {
        // Calcula el tiempo delta
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        // Calcula los FPS
        float fps = 1.0f / deltaTime;

        // Actualiza el texto con los FPS
        fpsText.text = string.Format("{0:0.} FPS", fps);
    }
}
