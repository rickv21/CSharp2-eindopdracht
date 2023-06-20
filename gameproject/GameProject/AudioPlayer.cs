using Plugin.Maui.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameProject
{
    public class AudioPlayer
    {
        private AudioPlayer() {}

        /// <summary>
        /// Loads the given audio file in the resources/audio folder and returns the stream.
        /// </summary>
        /// <param name="name">The name of the audio file.</param>
        /// <returns>The stream of the audio file.</returns>
        public static Stream LoadAudio(string name)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream("GameProject.Resources.Audio." + name);
        }

        /// <summary>
        /// Plays the given audio.
        /// </summary>
        /// <param name="stream">The stream of the audio.</param>
        /// <param name="volume">The volume of the audio, betweet 0 and 1.</param>
        public static void PlaySound(Stream stream, int volume)
        {
            if (stream != null)
            {
                var audioPlayer = AudioManager.Current.CreatePlayer(stream);
                audioPlayer.Volume = volume;
                audioPlayer.Play();
            }
        }

    }
}
