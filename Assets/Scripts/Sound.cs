using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Internal;

namespace Assets.Scripts
{
    [Serializable]
    public class Sound
    {
        public AudioClip Clip;

        public String Name;

        [Range(0, 1f)]
        public Single Volume = 1f;

        [Range(.1f, 3f)]
        public Single Pitch = 1f;

        [HideInInspector]
        public AudioSource source;
    }
}
