using AI.Talk;
using AI.Talk.Editor.Api;
using AIVoiceIPC.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AIVoiceIPC.Server
{
    internal class Program
    {
        private static readonly TtsControl TtsControl = new TtsControl();
        private static readonly int WaitCheckInterval = 500;
        private static readonly int WaitTimeout = 15 * 1000;

        static async Task Main(string[] args)
        {
            InitializeEditorHost();

            var tasks = new List<Task>();

            // クライアントの接続を待ち続ける
            while (true)
            {
                var pipeServer = new NamedPipeServerStream(Const.PipeName, PipeDirection.InOut, 10, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

                Console.WriteLine("Waiting for client connection...");

                try
                {
                    await pipeServer.WaitForConnectionAsync();
                    Console.WriteLine("Client connected.");

                    // 新しい接続を処理するタスクを開始
                    _ = HandleClientAsync(pipeServer);
                }
                catch (IOException)
                {
                    // 接続エラー処理
                    // reader/writerのdisposeの関係で毎回発生するため一旦無視する
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
            }

            async Task HandleClientAsync(NamedPipeServerStream ps)
            {
                using (var reader = new StreamReader(ps))
                using (var writer = new StreamWriter(ps) { AutoFlush = true })
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        Console.WriteLine($"Data received: {line}");

                        var (command, payload) = ParseRequest(line);
                        try
                        {
                            await RunCommand(command, payload);
                            await writer.WriteLineAsync("OK");
                        }
                        catch (Exception ex)
                        {
                            await writer.WriteLineAsync("ERROR: " + ex.Message);
                        }
                    }
                }

                Console.WriteLine("Client disconnected.");
                ps.Disconnect();
            }

            async Task RunCommand(string command, string payload)
            {
                switch (command)
                {
                    case "SPEAK":
                        var request = JsonConvert.DeserializeObject<SpeakRequest>(payload);
                        await SpeakAsync(request);
                        break;
                    case "STOP":
                        await StopAsync();
                        break;
                }
            }

            (string command, string payload) ParseRequest(string input)
            {
                var m = Regex.Match(input, @"^(?<command>[^:]+):?(?<payload>.*)$");
                return (m.Groups["command"].Value, m.Groups["payload"].Value);
            }
        }

        private static void InitializeEditorHost()
        {
            var availableHosts = TtsControl.GetAvailableHostNames();
            if (!availableHosts.Any())
            {
                throw new Exception("利用可能なホストが存在しません。");
            }

            try
            {
                var host = availableHosts.First();
                TtsControl.Initialize(host);

                if (TtsControl.Status == HostStatus.NotRunning)
                {
                    TtsControl.StartHost();
                }

                TtsControl.Connect();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                throw;
            }
        }

        private static void EnsureEditorConnected()
        {
            try
            {
                if (TtsControl.Status == HostStatus.NotConnected)
                {
                    TtsControl.Connect();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                throw;
            }
        }

        private static async Task SpeakAsync(SpeakRequest request)
        {
            EnsureEditorConnected();

            try
            {
                await WaitForStatusAsync(HostStatus.Idle);

                TtsControl.CurrentVoicePresetName = request.PresetName;
                ChangeStyle(request);

                TtsControl.Text = request.Text;
                TtsControl.Play();

                await WaitForStatusAsync(HostStatus.Idle);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }

        private static async Task StopAsync()
        {
            EnsureEditorConnected();

            if (TtsControl.Status == HostStatus.Idle)
            {
                return;
            }

            try
            {
                TtsControl.Stop();

                await WaitForStatusAsync(HostStatus.Idle);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }

        private static void ChangeStyle(SpeakRequest request)
        {
            var presetValue = TtsControl.GetVoicePreset(request.PresetName);
            var preset = JsonConvert.DeserializeObject<VoicePreset>(presetValue);

            preset.Styles["J"].Value = request.Emotion.Joy;
            preset.Styles["A"].Value = request.Emotion.Anger;
            preset.Styles["S"].Value = request.Emotion.Sadness;

            var newPreset = JsonConvert.SerializeObject(preset);

            TtsControl.SetVoicePreset(newPreset);
        }

        private static async Task WaitForStatusAsync(HostStatus status)
        {
            var startTime = DateTime.Now;

            while (TtsControl.Status != status)
            {
                if ((DateTime.Now - startTime).TotalMilliseconds > WaitTimeout)
                {
                    throw new TimeoutException();
                }

                await Task.Delay(WaitCheckInterval);
            }
        }
    }
}
