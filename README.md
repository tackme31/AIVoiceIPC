# AIVoiceIPC
A.I.VOICEのエディタを.NET上から呼び出すためのIPCサーバー/クライアントです。
公式が提供するAPIが.NET Frameworkのみを対象としているため、IPCを使って.NETアプリケーションからの実行を実現しています。

## 必要要件
- A.I. VOICE
  - **A.I. Voice2は不可**
- .NET Framework 4.8.1 SDK

## セットアップ
1. A.I.VOICEをインストール・アクティベーション
2. `%PROGRAMFILES%/AI/AIVoice/AIVoiceEditor`内の以下のDLLファイルを、ソリューションの/libフォルダにコピー
    - AI.Talk.dll
    - AI.Talk.Editor.Api.dll
    - AI.Framework.dll
3. AIVoiceIPC.Server（コンソールアプリ）を実行

## 使い方

```csharp
var client = new AIVoiceIPC.Client.AIVoiceClient();

var request = new SpeakRequest
{
    PresetName = "琴葉 葵",
    Text = "これはA.I.VOICEのエディタを.NET上から呼び出すためのIPCサーバー/クライアントです。",
    Emotion = new Emotion
    {
        Joy = 0.3,
        Anger = 0,
        Sadness = 0
    }
};

// 非同期で読み上げ
_ = client.SpeakAsync(request);

await Task.Delay(1000);

// 読み上げを停止
await client.StopAsync();
```

## 作者
- Takumi Yamada ([@tackme31](https://x.com/tackme31))