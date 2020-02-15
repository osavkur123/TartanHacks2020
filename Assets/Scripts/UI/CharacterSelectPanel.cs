using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sanicball.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
    public class CharacterSelectionArgs : EventArgs
    {
        public int SelectedCharacter { get; set; }

        public CharacterSelectionArgs(int selectedCharacter)
        {
            SelectedCharacter = selectedCharacter;
        }
    }

    public class CharacterSelectPanel : MonoBehaviour
    {
        private const int COLUMN_COUNT = 4;

        [SerializeField]
        private RectTransform entryContainer = null;
        [SerializeField]
        private CharacterSelectEntry entryPrefab = null;
        [SerializeField]
        private Text characterNameLabel;

        [SerializeField]
        private float scrollSpeed = 1f;

        [SerializeField]
        private float normalIconSize = 64;
        [SerializeField]
        private float selectedIconSize = 96;

        private int selected = 0;
        private Data.CharacterInfo selectedChar;
        private float targetX = 0;
        private float targetY = 0;
        private List<CharacterSelectEntry> activeEntries = new List<CharacterSelectEntry>();

        [SerializeField]
        private Sprite cancelIconSprite;

        public event EventHandler<CharacterSelectionArgs> CharacterSelected;
        public event EventHandler CancelSelected;

        private IEnumerator Start()
        {
            var charList = ActiveData.Characters.OrderBy(a => a.tier).ToArray();
            if (ActiveData.GameSettings.eSportsReady)
            {
                charList = charList.Where(a => a.tier == SanicballCore.CharacterTier.Hyperspeed).ToArray();
            }

            else if (ActiveData.GameSettings.greenModeReady)
            {
                charList = charList.Where(a => a.tier == SanicballCore.CharacterTier.Odd).ToArray();
            }

            CharacterSelectEntry cancelEnt = Instantiate(entryPrefab);
            cancelEnt.IconImage.sprite = cancelIconSprite;
            cancelEnt.transform.SetParent(entryContainer.transform, false);
            activeEntries.Add(cancelEnt);

            for (int i = 0; i < charList.Length; i++)
            {
                if (!charList[i].hidden)
                {
                    CharacterSelectEntry characterEnt = Instantiate(entryPrefab);

                    characterEnt.Init(charList[i]);
                    characterEnt.transform.SetParent(entryContainer.transform, false);
                    activeEntries.Add(characterEnt);
                }
            }

            //Wait a single frame before selecting the first character.
            yield return null;
            Select(1);
        }

        public void Right()
        {
            if (selected < activeEntries.Count - 1) Select(selected + 1); else Select(0);
        }

        public void Left()
        {
            if (selected > 0) Select(selected - 1); else Select(activeEntries.Count - 1);
        }

        public void Up()
        {
            if (activeEntries.Count <= COLUMN_COUNT) return;

            int s = selected - COLUMN_COUNT;
            if (s < 0)
            {
                s += activeEntries.Count;
                //if (s < 0) s = activeEntries.Count - 1;
            }
            Select(s);
        }

        public void Down()
        {
            if (activeEntries.Count <= COLUMN_COUNT) return;

            int s = selected + COLUMN_COUNT;
            if (s > activeEntries.Count - 1)
            {
                s -= activeEntries.Count;
                //if (s > activeEntries.Count) s = 0;
            }
            Select(s);
        }

        private void Select(int newSelection)
        {
            selected = newSelection;
            selectedChar = activeEntries[selected].Character;
            if (ActiveData.GameSettings.blueModeReady)
            {
                selectedChar = activeEntries[1].Character;
            }
            if (ActiveData.GameSettings.blueModeReady && selected == 1)
            {
                selectedChar = activeEntries[13].Character;
            }

            if (selected == 0)
                characterNameLabel.text = "Leave match";
            else if (ActiveData.GameSettings.blueModeReady && selected != 1 && selected != 13)
                characterNameLabel.text = "NOT BLOO ENOUGH";
            else if (ActiveData.GameSettings.blueModeReady)
                characterNameLabel.text = "BLOO";
            else
                characterNameLabel.text = selectedChar.name;

            if (ActiveData.GameSettings.redModeReady)
            {
                selectedChar = activeEntries[2].Character;
            }

            if(ActiveData.GameSettings.numPlayers == 2)
            {
                selectedChar = activeEntries[1].Character;
                ActiveData.MatchSettings.SetAICharacter(1, 1);
            }
        }

        private void Update()
        {
            //Find the container's target X to center the selected character
            targetX = entryContainer.sizeDelta.x / 2 - activeEntries[selected].RectTransform.anchoredPosition.x;
            targetY = -entryContainer.sizeDelta.y / 2 - activeEntries[selected].RectTransform.anchoredPosition.y;

            if (!Mathf.Approximately(entryContainer.anchoredPosition.x, targetX))
            {
                float x = Mathf.Lerp(entryContainer.anchoredPosition.x, targetX, scrollSpeed * Time.deltaTime);
                entryContainer.anchoredPosition = new Vector2(x, entryContainer.anchoredPosition.y);
            }

            if (!Mathf.Approximately(entryContainer.anchoredPosition.y, targetY))
            {
                float y = Mathf.Lerp(entryContainer.anchoredPosition.y, targetY, scrollSpeed * Time.deltaTime);
                entryContainer.anchoredPosition = new Vector2(entryContainer.anchoredPosition.x, y);
            }

            //Resize all elements
            for (int i = 0; i < activeEntries.Count; i++)
            {
                var element = activeEntries[i];

                float targetSize = (i == selected) ? selectedIconSize : normalIconSize;

                if (!Mathf.Approximately(element.Size, targetSize))
                {
                    element.Size = Mathf.Lerp(element.Size, targetSize, scrollSpeed * Time.deltaTime);
                }
            }
        }

        public void Accept()
        {
            if (selected == 0)
            {
                if (CancelSelected != null)
                    CancelSelected(this, EventArgs.Empty);
            }
            else
            {
                if (CharacterSelected != null)
                    CharacterSelected(this, new CharacterSelectionArgs(Array.IndexOf(ActiveData.Characters, selectedChar)));
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}