using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class ImageCycler : MonoBehaviour {
    private RawImage _rawImage;
    private Timer _timer = new Timer(5);
    [SerializeField] private Texture[] textures;
    private int _index;

    private void Awake() {
        _rawImage = GetComponent<RawImage>();
        _timer.Start();
        _index = 0;
    }

    private void Update() {
        if (_timer.Finished()) {
            _timer.Start();
            CycleImage();
        }
    }

    private void CycleImage() {
        _index++;
        if (_index == textures.Length) {
            _index = 0;
        }

        _rawImage.texture = textures[_index];
    }
}
