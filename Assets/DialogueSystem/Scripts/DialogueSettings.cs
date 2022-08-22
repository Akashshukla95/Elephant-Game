using UnityEngine;

namespace K12.CommonDialogueSystem
{
    public class DialogueSettings : MonoBehaviour
    {
        // NOTE: The values are via the inspector!!
        [Header("Dialogue Settings")]
        public Font dialogueSystemFont = null;
        public bool canFadeSpeakerIn;
        public float fadeSpeakerDuration;
        public float dialogueTextDelay;
        public bool autoPlay;
    }
}