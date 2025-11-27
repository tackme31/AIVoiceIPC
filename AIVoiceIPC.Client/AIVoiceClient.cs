using AIVoiceIPC.Core;
using System;
using System.IO.Pipes;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AIVoiceIPC.Client
{
    public class AIVoiceClient
    {
        private readonly NamedPipeClientStream _pipeClient;
        private StreamReader? _reader;
        private StreamWriter? _writer;

        public AIVoiceClient()
        {
            _pipeClient = new NamedPipeClientStream(".", Const.PipeName, PipeDirection.InOut);
            _reader = null;
            _writer = null;
        }

        private async Task ConnectToServerAsync()
        {
            if (!_pipeClient.IsConnected)
            {
                await _pipeClient.ConnectAsync(5000);

                _reader = new StreamReader(_pipeClient);
                _writer = new StreamWriter(_pipeClient) { AutoFlush = true };
            }
        }

        public async Task SpeakAsync(SpeakRequest request)
        {
            await ConnectToServerAsync();

            if (_writer is null || _reader is null)
            {
                return;
            }

            try
            {
                var serialized = JsonConvert.SerializeObject(request, Formatting.None);
                await _writer.WriteLineAsync("SPEAK:" + serialized);

                await _reader.ReadLineAsync();
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException("An error occurred while communicating with the server.", ex);
            }
        }

        public async Task StopAsync()
        {
            await ConnectToServerAsync();

            if (_writer is null || _reader is null)
            {
                return;
            }

            try
            {
                await _writer.WriteLineAsync("STOP");

                await _reader.ReadLineAsync();
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException("An error occurred while communicating with the server.", ex);
            }
        }

        public void Dispose()
        {
            _writer?.Dispose();
            _reader?.Dispose();
            _pipeClient?.Dispose();
        }
    }
}