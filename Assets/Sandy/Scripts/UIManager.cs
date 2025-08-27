using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //[SerializeField]private string sceneName;
	public Button backToTitelBtn;
	private void Awake()
	{
		backToTitelBtn.onClick.AddListener(LoadScene);
	}
	public void LoadScene()
    {
        SceneManager.LoadScene("Title");
    }
}
