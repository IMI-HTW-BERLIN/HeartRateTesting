using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.ResponseTypes;
using UnityEngine;
using Utils;

namespace LocalServer
{
    public class MiBand2Client : MonoBehaviour
    {
        [SerializeField] private bool hideWindow = true;

        public static event Action<HeartRateResponse> OnHeartRateChange;
        public static event Action<bool> OnDeviceConnectionChange;


        private TcpClient _client;
        private bool _serverResponseReceived = true;

        private const float CONNECTION_RETRY_INTERVAL = 5f;

        private BinaryWriter _binaryWriter;

        private bool _lastHeartRateWasZero;

        private bool _isMeasuring;

        public void StartMiBand() => StartCoroutine(Initialize());

        public void StopMiBand()
        {
            if (_isMeasuring)
            {
                _isMeasuring = false;
                SendCommand(Consts.Command.StopMeasurement);
            }

            BackgroundServer.StopServer();
            _client?.Dispose();
        }

        private IEnumerator Initialize()
        {
            BackgroundServer.StartServer(hideWindow);
            // Short delay for server to start.
            yield return new WaitForSeconds(2);
            StartCoroutine(ConnectToSever());
            yield return new WaitForSeconds(2);
            yield return ConnectToBand();
            yield return StartMeasurement();
        }

        private IEnumerator ConnectToSever()
        {
            try
            {
                _client = new TcpClient("localhost", Consts.ServerData.PORT);
            }
            catch (SocketException)
            {
                StartCoroutine(ConnectToServerAfterDelay());
            }

            yield return ListenForResponse();
        }

        private IEnumerator ConnectToServerAfterDelay()
        {
            yield return new WaitForSeconds(CONNECTION_RETRY_INTERVAL);
            yield return ConnectToSever();
        }

        private IEnumerator ConnectToBand()
        {
            SendCommand(Consts.Command.ConnectBand);
            yield return new WaitUntil(() => _serverResponseReceived);
            SendCommand(Consts.Command.AuthenticateBand);
            yield return new WaitUntil(() => _serverResponseReceived);
        }

        private IEnumerator StartMeasurement()
        {
            SendCommand(Consts.Command.SubscribeToHeartRateChange);
            yield return new WaitUntil(() => _serverResponseReceived);
            SendCommand(Consts.Command.StartMeasurement);
            yield return new WaitUntil(() => _serverResponseReceived);
            _isMeasuring = true;
        }

        private void SendCommand(Consts.Command command)
        {
            if (!_serverResponseReceived)
                return;
            _serverResponseReceived = false;
            using (BinaryWriter writer = new BinaryWriter(_client.GetStream(), Encoding.UTF8, true))
                writer.Write((int) command);
        }

        private IEnumerator ListenForResponse()
        {
            using (BinaryReader reader = new BinaryReader(_client.GetStream(), Encoding.UTF8, true))
            {
                Task<string> task = reader.ReadStringAsync();
                yield return new WaitUntil(() => task.IsCompleted);
                string data = task.Result;

                ServerResponse response = ServerResponse.FromJson(data);
                switch (response.Data)
                {
                    case Exception exception:
                        throw exception;
                    case HeartRateResponse heartRateResponse:
                        // If last two measurements were zero, restart.
                        if (heartRateResponse.HeartRate == 0)
                        {
                            if (_lastHeartRateWasZero)
                                StartCoroutine(RestartMeasurement());
                            _lastHeartRateWasZero = true;
                        }

                        OnHeartRateChange?.Invoke(heartRateResponse);
                        break;
                    case DeviceConnectionResponse connectionResponse:
                        OnDeviceConnectionChange?.Invoke(connectionResponse.IsConnected);
                        break;
                }

                _serverResponseReceived = true;
                yield return ListenForResponse();
            }
        }

        private IEnumerator RestartMeasurement()
        {
            SendCommand(Consts.Command.StopMeasurement);
            yield return new WaitUntil(() => _serverResponseReceived);
            SendCommand(Consts.Command.StartMeasurement);
            yield return new WaitUntil(() => _serverResponseReceived);
        }
    }
}