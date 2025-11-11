using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private AudioMixer auidoMixer;
    [SerializeField] private string nextScene;

    public GameObject settingsPanel; 
    void Start()
    {
        float musicVol = PlayerPrefs.GetFloat("musicVol", 1.0f);
        musicSlider.value = musicVol;
        SetMusicVolume(musicVol);

        float sfxVol = PlayerPrefs.GetFloat("sfxVol", 1.0f);
        sfxSlider.value = sfxVol;
        SetSFXVolume(sfxVol);
    }

    public void SetMusicVolume(float volume)
    {
        float decibels = Mathf.Log10(volume) * 20;
        auidoMixer.SetFloat("VolumenMusical", decibels);
        PlayerPrefs.SetFloat("musicVol", volume);
    }

    public void SetSFXVolume(float volume)
    {
        float decibels = Mathf.Log10(volume) * 20;
        auidoMixer.SetFloat("VolumenSFX", decibels);
        PlayerPrefs.SetFloat("sfxVol", volume);
    }

    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }

    public void Jugar()
    {
        SceneManager.LoadScene(nextScene);
    }

    public void Cerrar()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

}
