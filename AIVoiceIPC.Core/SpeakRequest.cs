namespace AIVoiceIPC.Core
{
    public class SpeakRequest
    {
        public string PresetName { get; set; }
        public string Text { get; set; }
        public Emotion Emotion { get; set; }
    }

    public class Emotion
    {
        public double Joy { get; set; }
        public double Anger { get; set; }
        public double Sadness { get; set; }
    }
}
