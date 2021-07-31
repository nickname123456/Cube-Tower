using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasButtons : MonoBehaviour
{
    public Sprite musicOn, //Картинка вкл музыки
                musicOff; //Картинка выкл музыки

    private void Start()
    {
        if (PlayerPrefs.GetString("music") == "No" && gameObject.name == "music"){
            GetComponent<Image>().sprite = musicOff;
        }
    }


    public void RestartGame()
    {
        if (PlayerPrefs.GetString("music") != "No"){
            GetComponent<AudioSource>().Play;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void LoadInstagram()
    {
        if (PlayerPrefs.GetString("music") != "No"){
            GetComponent<AudioSource>().Play;
        }
        Application.OpenURL("https://www.google.ru");
    }

    public void MusicWork()
    {
        //Сейчас музыка выключена и ее надо включить
        if(PlayerPrefs.GetString("music") == "No"){
            GetComponent<AudioSource>().Play;
            PlayerPrefs.SetString("music", "Yes");
            GetComponent<Image>().sprite = musicOn;

        }else{ //Сейчас музыка включена и ее надо выключить
            PlayerPrefs.SetString("music", "No");
            GetComponent<Image>().sprite = musicOff;
        }
    }
}
