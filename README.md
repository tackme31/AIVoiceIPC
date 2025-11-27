# AIVoiceIPC
A.I.VOICEのエディタを.NET上から呼び出すためのIPCサーバー/クライアントです。
公式が提供するAPIが.NET Frameworkのみを対象としているため、IPCを使って.NETアプリケーションからの実行を実現しています。

本IPCサーバーはローカル環境向けに設計されています。ネットワーク越しに公開する構成は推奨しません。

## 必要要件
- A.I. VOICE
  - ※A.I.Voice2 は Editor API を提供していないためサポート対象外です。
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

## 注意事項
このライブラリは、A.I.VOICE Editor API のために独自に開発されたラッパーです。
A.I.VOICE およびその API の一部は一切含まれていません。
本ライブラリを使用するには、A.I.VOICE 製品の正規ライセンスを所持し、A.I.VOICE Editor API 利用規約を遵守する必要があります。
本ライブラリは、ライセンス未取得のユーザーに対して API へのアクセスを提供するものではありません。

A.I.VOICE Editor APIの利用規約については、以下を参照ください。

- [A.I.VOICE Editor API — A.I.VOICE Editor 1.4.11 ドキュメント](https://aivoice.jp/manual/editor/api.html#termsandconditions)

## 作者
- Takumi Yamada ([@tackme31](https://x.com/tackme31))