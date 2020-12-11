using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }

  


    public void Start1v1Level()
    {
        SceneManager.LoadScene(1);
    }
    public void StartAILevel()
    {

        //SceneManager.LoadScene(2);
    }

    public void loadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void loadSettings()
    {
        SceneManager.LoadScene(3);
    }

	public void loadStats(){
		SceneManager.LoadScene(4);
	}
}
