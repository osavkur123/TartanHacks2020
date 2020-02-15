using UnityEngine;
using System.Collections.Generic;

namespace Sanicball
{
    public class redMode : MonoBehaviour
    {
        public Texture2D screenOverlay;
        public Texture2D solidWhite;
        private Color currentColor = new Color(1f, 0, 0, 0.2f);
        private bool timerOn = false;
        private float timer = 1f;
        bool started = false;
        private bool screenOverlayEnabled = false;
        const float COLOR_TIME = 60.0f / 110.0f;
        private float colorTimer = COLOR_TIME;

        void Start()
        {
        }

        public void StartTheShit()
        {
            timerOn = true;
        }

        AudioSource music;

        private void Start4Real()
        {
            started = true;
            screenOverlayEnabled = true;
            music = FindObjectOfType<MusicPlayer>().GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (timerOn)
            {
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    timerOn = false;
                    Start4Real();
                }
            }

            if (screenOverlayEnabled)
            {
                colorTimer -= Time.deltaTime;
                if (colorTimer <= 0)
                {
                    currentColor = new Color(1f, 0, 0, Random.Range(0f, 0.3f));
                    colorTimer += COLOR_TIME;
                }
            }
        }

        private void OnGUI()
        {
            Rect getRekt = new Rect(0, 0, Screen.width, Screen.height);
            if (screenOverlayEnabled)
            {
                //Background
                GUIStyle colorStyle = new GUIStyle();
                colorStyle.normal.background = solidWhite;
                colorStyle.stretchWidth = true;
                colorStyle.stretchHeight = true;
                GUI.backgroundColor = currentColor;
                GUI.Box(getRekt, "", colorStyle);
                GUI.backgroundColor = Color.white;

                //Overlay
                GUIStyle mlgStyle = new GUIStyle();
                mlgStyle.normal.background = screenOverlay;
                mlgStyle.stretchWidth = true;
                mlgStyle.stretchHeight = true;
                GUI.Box(getRekt, "", mlgStyle);
            }
        }
    }
}