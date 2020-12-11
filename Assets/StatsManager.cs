using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StatsManager : MonoBehaviour
{
    public Text whiteWins;
    public Text blackWins;
    private void Start() {

       if(PlayerPrefs.HasKey("WhiteWins")) 
        {
            whiteWins.text= (PlayerPrefs.GetInt("WhiteWins")).ToString();
        }   
        if(PlayerPrefs.HasKey("BlackWins")) 
        {
            blackWins.text= (PlayerPrefs.GetInt("BlackWins")).ToString();
        }   

    }

    public void ResetStats(){
        PlayerPrefs.SetInt("WhiteWins",0);
        PlayerPrefs.SetInt("BlackWins",0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //reload current scene
        

    }
      


}
