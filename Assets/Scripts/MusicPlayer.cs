using Sanicball;
using Sanicball.Data;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Sanicball.UI;

namespace Sanicball
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicPlayer : MonoBehaviour
    {
        //public GUISkin skin;

        public MusicPlayerCanvas playerCanvasPrefab;
        public bool playerCanvasLobbyOffset = false;
        private MusicPlayerCanvas playerCanvas;

        public bool startPlaying = false;
        public bool fadeIn = false;

        public Song[] playlist;
        public AudioSource fastSource;

        [System.NonSerialized]
        public bool fastMode = false;

        private int currentSongID;
        private bool isPlaying;
        private string currentSongCredits;

        //Song credits
        private float timer = 0;

        private float slidePosition;
        private float slidePositionMax = 20;

        private AudioSource aSource;

        public void Play()
        {
            Play(playlist[currentSongID].name);
        }

        public void Play(string credits)
        {
            if (!ActiveData.GameSettings.music) return;
            playerCanvas.Show(credits);
            isPlaying = true;
            aSource.Play();
        }

        public void Pause()
        {
            aSource.Pause();
            isPlaying = false;
        }

        private void Start()
        {
            playerCanvas = Instantiate(playerCanvasPrefab);
            if (playerCanvasLobbyOffset) 
            {
                playerCanvas.lobbyOffset = true;
            }

            aSource = GetComponent<AudioSource>();

            slidePosition = slidePositionMax;
            ShuffleSongs();

            Sanicball.Logic.MatchManager mm = FindObjectOfType<Sanicball.Logic.MatchManager>();
            if (mm)
            {
                var players = mm.Players;
                foreach (var p in players)
                {
                    if (p.CtrlType != SanicballCore.ControlType.None)
                    {
                        if (!mm.InLobby)
                        {
                            if (p.CharacterId == 16)
                            {
                            
                                List<Song> play = playlist.ToList();
                                Song s = new Song();
                                s.name = "Thomas the Tank Engine";
                                s.clip = ActiveData.ThomasMusic;
                                play.Insert(0, s);
                                playlist = play.ToArray();
                            }
                            if (p.CharacterId == 17)
                            {

                                List<Song> play = playlist.ToList();
                                Song s = new Song();
                                s.name = "Super Saiyan 2.0";
                                s.clip = ActiveData.GokuMusic;
                                play.Insert(0, s);
                                playlist = play.ToArray();
                            }
                        }
                    }
                }
            }

            if (ActiveData.ESportsFullyReady)
            {
                Sanicball.Logic.MatchManager m = FindObjectOfType<Sanicball.Logic.MatchManager>();
                if (!m.InLobby) {
                    List<Song> p = playlist.ToList();
                    Song s = new Song();
                    s.name = "Skrollex - Bungee Ride";
                    s.clip = ActiveData.ESportsMusic;
                    p.Insert(0,s);
                    playlist = p.ToArray();
                }
            }
            else if (ActiveData.greenModeFullyReady)
            {
                Sanicball.Logic.MatchManager m = FindObjectOfType<Sanicball.Logic.MatchManager>();
                if (!m.InLobby)
                {
                    List<Song> p = playlist.ToList();
                    Song s = new Song();
                    s.name = "It's not easy being green - Kermit the Froggo";
                    s.clip = ActiveData.GreenMusic;
                    p.Insert(0, s);
                    playlist = p.ToArray();
                }
            }
            else if (ActiveData.blueModeFullyReady)
            {
                Sanicball.Logic.MatchManager m = FindObjectOfType<Sanicball.Logic.MatchManager>();
                if (!m.InLobby)
                {
                    List<Song> p = playlist.ToList();
                    Song s = new Song();
                    s.name = "I'm Blue - Eiffel 65";
                    s.clip = ActiveData.BlueMusic;
                    p.Insert(0, s);
                    playlist = p.ToArray();
                }
            }
            else if (ActiveData.redModeFullyReady)
            {
                Sanicball.Logic.MatchManager m = FindObjectOfType<Sanicball.Logic.MatchManager>();
                if (!m.InLobby)
                {
                    List<Song> p = playlist.ToList();
                    Song s = new Song();
                    s.name = "Our national anthem - Toad";
                    s.clip = ActiveData.RedMusic;
                    p.Insert(0, s);
                    playlist = p.ToArray();
                }
            }

            aSource.clip = playlist[0].clip;
            currentSongID = 0;
            isPlaying = aSource.isPlaying;
            if (startPlaying && ActiveData.GameSettings.music)
            {
                Play();
            }
            if (fadeIn)
            {
                aSource.volume = 0f;
            }
            if (!ActiveData.GameSettings.music)
            {
                fastSource.Stop();
            }
        }

        private void Update()
        {
            if (fadeIn && aSource.volume < 0.5f)
            {
                aSource.volume = Mathf.Min(aSource.volume + Time.deltaTime * 0.1f, 0.5f);
            }
            //If it's not playing but supposed to play, change song
            if ((!aSource.isPlaying || GameInput.IsChangingSong()) && isPlaying)
            {
                if (currentSongID < playlist.Length - 1)
                {
                    currentSongID++;
                }
                else
                {
                    currentSongID = 0;
                }
                aSource.clip = playlist[currentSongID].clip;
                slidePosition = slidePositionMax;
                Play();
            }
            //Timer
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }

            if (fastMode && fastSource.volume < 1)
            {
                fastSource.volume = Mathf.Min(1, fastSource.volume + Time.deltaTime * 0.25f);
                aSource.volume = 0.5f - fastSource.volume / 2;
            }
            if (!fastMode && fastSource.volume > 0)
            {
                fastSource.volume = Mathf.Max(0, fastSource.volume - Time.deltaTime * 0.5f);
                aSource.volume = 0.5f - fastSource.volume / 2;
            }
            if (timer > 0)
            {
                slidePosition = Mathf.Lerp(slidePosition, 0, Time.deltaTime * 4);
            }
            else
            {
                slidePosition = Mathf.Lerp(slidePosition, slidePositionMax, Time.deltaTime * 2);
            }
        }

        private void ShuffleSongs()
        {
            //Shuffle playlist using Fisher-Yates algorithm
            for (int i = playlist.Length; i > 1; i--)
            {
                int j = Random.Range(0, i);
                Song tmp = playlist[j];
                playlist[j] = playlist[i - 1];
                playlist[i - 1] = tmp;
            }
        }
    }

    [System.Serializable]
    public class Song
    {
        public string name;
        public AudioClip clip;
    }
}