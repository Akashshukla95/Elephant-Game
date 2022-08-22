using System;
using System.Collections.Generic;

namespace ElesJourney.Models
{
    [Serializable]
    public class DialogueData
    {
        public string name;
        public string dialogue;

    }

    [Serializable]
    public class DialogueModel
    {
        public List<DialogueData> dialogues;
    }
}


