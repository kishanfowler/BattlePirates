using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Audio;

public class VolumeUIController : MonoBehaviour
{
    public UIDocument UIDocument;
    public AudioMixer AudioMixer;

    void OnEnable()
    {
        var root = UIDocument.rootVisualElement;
        var slider = root.Q<Slider>("volume-slider");

        slider.RegisterValueChangedCallback(evt =>
        {
            AudioMixer.SetFloat("MusicVolume", evt.newValue);
        });

        // Optional: Initialize slider from current volume
        if (AudioMixer.GetFloat("MusicVolume", out float currentVolume))
        {
            slider.value = currentVolume;
        }
    }
}