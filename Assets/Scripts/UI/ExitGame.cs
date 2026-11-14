using UnityEngine;

/// <summary>
/// Botón Exit. Llama a Quit() desde el OnClick
/// Funciona tanto en el editor como en la build
/// </summary>
public class ExitGame : MonoBehaviour
{
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // detiene el Play en el editor
#else
        Application.Quit();
#endif
    }
}