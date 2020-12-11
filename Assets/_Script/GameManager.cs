using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }

    private void Awake()
    {
        int y = SceneManager.GetActiveScene().buildIndex;
        if (y == 0)
        {

            if (Instance == null)
            {
                Instance = this;
            }
            //If an instance already exists, destroy whatever this object it is to enforce the singleton.
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        DontDestroyOnLoad(transform.gameObject);
    }


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
