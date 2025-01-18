using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

#if USE_ADDRESSABLES
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

namespace Yarn.Unity
{
    public class TextLineProvider : LineProviderBehaviour
    {
        /// <summary>Specifies the language code to use for text content
        /// for this <see cref="TextLineProvider"/>.
        [Language]
        public string textLanguageCode = System.Globalization.CultureInfo.CurrentCulture.Name;


        public AudioSource audioSource;


        public override LocalizedLine GetLocalizedLine(Yarn.Line line)
        {
            int sound = PlayerPrefs.GetInt("sound");
            if (sound==1) //If Sound Toggle was on when game started
            {
                int currentline;
                string cleanID = line.ID.Substring(5); //line.id = line:005 => extract only the number
                if (Int32.TryParse(cleanID, out currentline))
                {
                    LoadAndPlaySound(currentline);
                }
            }
            
            var text = YarnProject.GetLocalization(textLanguageCode).GetLocalizedString(line.ID);
            


            return new LocalizedLine()
            {
                TextID = line.ID,
                RawText = text,
                Substitutions = line.Substitutions,
                Metadata = YarnProject.lineMetadata.GetMetadata(line.ID),
            };
        }

        public override void PrepareForLines(IEnumerable<string> lineIDs)
        {
            // No-op; text lines are always available
        }

        public override bool LinesAvailable => true;

        public override string LocaleCode => textLanguageCode;


        private void LoadAndPlaySound(int fileID)
        {

            // Stop any previously playing sound
            audioSource.Stop();

            // Load the AudioClip from the Resources folder
            AudioClip audioClip = Resources.Load<AudioClip>($"{fileID}");

            if (audioClip == null)
            {
                Debug.LogError("Failed to load MP3 file: " +  fileID);
            }
            else
            {
                // Assign the AudioClip to the AudioSource
                audioSource.clip = audioClip;

                // Play the sound
                StartCoroutine(Play_Sound());
            }
        }

        IEnumerator Play_Sound()
        {
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
        }
    }
}
