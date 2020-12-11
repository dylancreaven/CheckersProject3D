 using UnityEngine;
 
 public class MusicManager : MonoBehaviour
 {
     public static MusicManager Instance = null;
     private AudioSource _audioSource;
     private void Awake()
     {
         if (Instance == null)
		{
			Instance = this;
		}
		//If an instance already exists, destroy whatever this object is to enforce the singleton.
		else if (Instance != this)
		{
			Destroy(gameObject);
		}
         DontDestroyOnLoad(transform.gameObject);
         _audioSource = GetComponent<AudioSource>();
     }
 
     public void PlayMusic()
     {
         if (_audioSource.isPlaying) return;
         _audioSource.Play();
     }

 
     public void StopMusic()
     {
         _audioSource.Stop();
     }
 }