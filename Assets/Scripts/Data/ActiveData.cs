using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SanicballCore;
using UnityEngine;

namespace Sanicball.Data
{
    public class ActiveData : MonoBehaviour
    {
        #region Fields

        public List<RaceRecord> raceRecords = new List<RaceRecord>();

        //Pseudo-singleton pattern - this field accesses the current instance.
        private static ActiveData instance;

        //This data is saved to a json file
        private GameSettings gameSettings = new GameSettings();

        private KeybindCollection keybinds = new KeybindCollection();
        private MatchSettings matchSettings = MatchSettings.CreateDefault();

        //This data is set from the editor and remains constant
        [Header("Static data")]
        [SerializeField]
        private StageInfo[] stages;

        [SerializeField]
        private CharacterInfo[] characters;

        [SerializeField]
        private GameJoltInfo gameJoltInfo;

        [SerializeField]
        private GameObject christmasHat;
        [SerializeField]
        private Material eSportsTrail;
        [SerializeField]
        private GameObject eSportsHat;
        [SerializeField]
        private AudioClip eSportsMusic;
        [SerializeField]
        private AudioClip greenMusic;
        [SerializeField]
        private AudioClip blueMusic;
        [SerializeField]
        private AudioClip redMusic;
        [SerializeField]
        private ESportMode eSportsPrefab;
        [SerializeField]
        private AudioClip thomasMusic;
        [SerializeField]
        private AudioClip gokuMusic;
        private greenMode greenModePrefab;
        [SerializeField]
        private blueMode blueModePrefab;
        [SerializeField]
        private redMode redModePrefab;

        #endregion Fields

        #region Properties

        public static GameSettings GameSettings { get { return instance.gameSettings; } }
        public static KeybindCollection Keybinds { get { return instance.keybinds; } }
        public static MatchSettings MatchSettings { get { return instance.matchSettings; } set { instance.matchSettings = value; } }
        public static List<RaceRecord> RaceRecords { get { return instance.raceRecords; } }

        public static StageInfo[] Stages { get { return instance.stages; } }
        public static CharacterInfo[] Characters { get { return instance.characters; } }
        public static GameJoltInfo GameJoltInfo { get { return instance.gameJoltInfo; } }
        public static GameObject ChristmasHat { get { return instance.christmasHat; } }
        public static Material ESportsTrail {get{return instance.eSportsTrail;}}
        public static GameObject ESportsHat {get{return instance.eSportsHat;}}
        public static AudioClip ESportsMusic {get{return instance.eSportsMusic;}}
        public static AudioClip GreenMusic { get { return instance.greenMusic; } }
        public static AudioClip BlueMusic { get { return instance.blueMusic; } }
        public static AudioClip RedMusic { get { return instance.redMusic; } }
        public static ESportMode ESportsPrefab {get{return instance.eSportsPrefab;}}
        public static AudioClip ThomasMusic { get { return instance.thomasMusic; } }
        public static AudioClip GokuMusic { get { return instance.gokuMusic; } }
        public static greenMode GreenModePrefab { get { return instance.greenModePrefab; } }
        public static blueMode BlueModePrefab { get { return instance.blueModePrefab; } }
        public static redMode RedModePrefab { get { return instance.redModePrefab; } }
        
        public static bool ESportsFullyReady {
            get {
                bool possible = false;
                if (GameSettings.eSportsReady)
                {
                    Sanicball.Logic.MatchManager m = FindObjectOfType<Sanicball.Logic.MatchManager>();
                    if (m)
                    {
                        var players = m.Players;
                        foreach (var p in players) {
                            if (p.CtrlType != SanicballCore.ControlType.None) {
                                if (p.CharacterId == 13) 
                                {
                                    possible = true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
                return possible;
            }
        }

        public static bool greenModeFullyReady
        {
            get
            {
                bool possible = false;
                if (GameSettings.greenModeReady)
                {
                    possible = true;
                    Sanicball.Logic.MatchManager m = FindObjectOfType<Sanicball.Logic.MatchManager>();
                    if (m)
                    {
                        var players = m.Players;
                        foreach (var p in players)
                        {
                            if (p.CtrlType != SanicballCore.ControlType.None)
                            {
                                if (p.CharacterId == 15)
                                {
                                    ActiveData.Characters[15].stats.rollSpeed = 500;
                                    ActiveData.Characters[15].stats.airSpeed = 300;
                                    ActiveData.Characters[15].stats.jumpHeight = 0;
                                    ActiveData.Characters[15].stats.grip = 200;
                                }
                            }
                        }
                    }
                }
                return possible;
            }
        }

        public static bool blueModeFullyReady
        {
            get
            {
                bool possible = false;
                if (GameSettings.blueModeReady)
                {
                    possible = true;
                }
                return possible;
            }
        }

        public static bool redModeFullyReady
        {
            get
            {
                bool possible = false;
                if (GameSettings.redModeReady)
                {
                    possible = true;
                }
                return possible;
            }
        }

        #endregion Properties

        #region Unity functions

        //Make sure there is never more than one GameData object
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            LoadAll();
            gameJoltInfo.Init();
        }

        private void OnApplicationQuit()
        {
            SaveAll();
        }

        #endregion Unity functions

        #region Saving and loading

        public void LoadAll()
        {
            Load("GameSettings.json", ref gameSettings);
            Load("GameKeybinds.json", ref keybinds);
            Load("MatchSettings.json", ref matchSettings);
            Load("Records.json", ref raceRecords);
        }

        public void SaveAll()
        {
            Save("GameSettings.json", gameSettings);
            Save("GameKeybinds.json", keybinds);
            Save("MatchSettings.json", matchSettings);
            Save("Records.json", raceRecords);
        }

        private void Load<T>(string filename, ref T output)
        {
            string fullPath = Application.persistentDataPath + "/" + filename;
            if (File.Exists(fullPath))
            {
                //Load file contents
                string dataString;
                using (StreamReader sr = new StreamReader(fullPath))
                {
                    dataString = sr.ReadToEnd();
                }
                //Deserialize from JSON into a data object
                try
                {
                    var dataObj = JsonConvert.DeserializeObject<T>(dataString);
                    //Make sure an object was created, this would't end well with a null value
                    if (dataObj != null)
                    {
                        output = dataObj;
                        Debug.Log(filename + " loaded successfully.");
                    }
                    else
                    {
                        Debug.LogError("Failed to load " + filename + ": file is empty.");
                    }
                }
                catch (JsonException ex)
                {
                    Debug.LogError("Failed to parse " + filename + "! JSON converter info: " + ex.Message);
                }
            }
            else
            {
                Debug.Log(filename + " has not been loaded - file not found.");
            }
        }

        private void Save(string filename, object objToSave)
        {
            var data = JsonConvert.SerializeObject(objToSave);
            using (StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/" + filename))
            {
                sw.Write(data);
            }
            Debug.Log(filename + " saved successfully.");
        }

        #endregion Saving and loading
    }
}
