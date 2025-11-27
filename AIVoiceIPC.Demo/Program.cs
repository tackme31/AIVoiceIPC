using AIVoiceIPC.Core;

var client = new AIVoiceIPC.Client.AIVoiceClient();

var request1 = new SpeakRequest
{
    PresetName = "琴葉 茜",
    Text = "こんにちは、マスター。",
    Emotion = new Emotion
    {
        Joy = 0.3,
        Anger = 0,
        Sadness = 0
    }
};

await client.SpeakAsync(request1);

var request2 = new SpeakRequest
{
    PresetName = "琴葉 葵",
    Text = "こんにちは、マスター",
    Emotion = new Emotion
    {
        Joy = 0,
        Anger = 0,
        Sadness = 0.5
    }
};

await client.SpeakAsync(request2);