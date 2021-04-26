using SystemBase;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonComponent : GameComponent
{
    public void StartGame()
    {
        Debug.Log("hey! start!");
        SceneManager.LoadScene("AdvancedEndScene");
        //SceneManager.LoadScene("Main");
    }

    public void EndGame()
    {
        Application.Quit();
    }

    public void StartInstruction()
    {
        SceneManager.LoadScene("Instruction");
    }
}
