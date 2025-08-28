using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
	public static TitleScene instance;
	[SerializeField] private Button startBtn;
	[SerializeField] private Button endBtn;
	[SerializeField] private string sceneName;
	private void Awake()
	{
		if (instance == null) instance = this;
		else Destroy(gameObject);
		//DontDestroyOnLoad(gameObject);
		startBtn.onClick.AddListener(StartGame);
		endBtn.onClick.AddListener(EndGame);
	}

	private void StartGame()
	{
		SceneManager.LoadScene(sceneName);
	}

	private void EndGame()
	{ 
		Application.Quit();
	}

	public void BackToTitle()
	{
		SceneManager.LoadScene("Title");
	}
}
