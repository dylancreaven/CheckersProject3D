using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
	public static GameManager Instance { set; get; }

	public GameObject mainMenu;
	public GameObject hostMenu;
	public GameObject connectMenu;

	public InputField nameInput;

	public GameObject serverPrefab;
	public GameObject clientPrefab;
	
	private void Start () 
	{
		Instance = this;
		hostMenu.SetActive(false);
		connectMenu.SetActive(false);
		DontDestroyOnLoad (gameObject);
	}
	
	public void ConnectButton()
	{
		mainMenu.SetActive(false);
		connectMenu.SetActive(true);
	}

	public void HostServerButton()
	{
		string hostAddress = "127.0.0.1";

		try
		{
			Server s = Instantiate(serverPrefab).GetComponent<Server>();
			s.Init();

			Client c = Instantiate(clientPrefab).GetComponent<Client>();
			c.clientName = nameInput.text;
			c.isHost = true;
			if (c.clientName == "")
				c.clientName = "Host";
			c.ConnectToServer(hostAddress, 6321); 
		}
		catch (System.Exception e)
		{
			Debug.Log (e.Message);
		}
		
		mainMenu.SetActive(false);
		connectMenu.SetActive(false);
		hostMenu.SetActive(true);	
	}
	public void ConnectToServerButton()
	{
		string hostAddress = GameObject.Find("HostInput").GetComponent<InputField>().text;
		if (hostAddress == "")
			hostAddress = "127.0.0.1";
		
		try
		{
			Client c = Instantiate(clientPrefab).GetComponent<Client>();
			c.clientName = nameInput.text;
			if (c.clientName == "")
				c.clientName = "Client";
			c.ConnectToServer(hostAddress, 6321); 
			connectMenu.SetActive(false);
		}
		catch (System.Exception e)
		{
			Debug.Log(e.Message);
		}
	}
	public void BackButton()
	{
		mainMenu.SetActive (true);
		connectMenu.SetActive (false);
		hostMenu.SetActive (false);

		Server s = FindObjectOfType<Server>();
		if (s != null)
			Destroy(s.gameObject);
	
		Client c = FindObjectOfType<Client>();
		if (c != null)
			Destroy(c.gameObject);
	}

	public void StartGame()
	{
		SceneManager.LoadScene ("Game");
	}
}
