using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace iris_n2n_launcher.Utils;

public class EdgeUdpManage : IDisposable
{
    private const string DefaultAddress = "127.0.0.1";
    private const string DefaultKey = "n2n";
    private const int MaxTagValue = 1000;

    private readonly string _address;
    private readonly int _port;
    private readonly string _key;
    private readonly UdpClient _udpClient;
    private int _tag;
    private readonly List<dynamic> _resultBuffer = new();

    public EdgeUdpManage(int port, string address = DefaultAddress, string key = DefaultKey)
    {
        _port = port;
        _address = address;
        _key = key;
        _udpClient = new UdpClient { Client = { ReceiveTimeout = 2000 } };
    }

    private string GetNextTag()
    {
        var tagStr = _tag.ToString();
        _tag = (_tag + 1) % MaxTagValue;
        return tagStr;
    }

    private (string Tag, string Message) PrepareCommand(string msgType, string cmdLine)
    {
        var tagStr = GetNextTag();
        var options = new List<string> { tagStr };

        if (!string.IsNullOrEmpty(_key))
        {
            options.Add("1"); 
            options.Add(_key);
        }

        var optionsStr = string.Join(":", options);
        var message = $"{msgType} {optionsStr} {cmdLine}";

        return (tagStr, message);
    }

    private bool ProcessResponse(string expectedTag)
    {
        bool seenBegin = false;
        Exception? error = null;
        try
        {
            while (true)
            {
                var endPoint = new IPEndPoint(IPAddress.Any, _port);
                var receiveBytes = _udpClient.Receive(ref endPoint);
                var data = System.Text.Encoding.UTF8.GetString(receiveBytes);
                var jsonData = JsonConvert.DeserializeObject<dynamic>(data);

                if (jsonData!._tag != expectedTag)
                {
                    continue;
                }

                switch (jsonData._type.ToString())
                {
                    case "error":
                        error = new Exception($"Error: {jsonData.error}");
                        break;

                    case "replacing":
                        continue;

                    case "subscribe":
                        return true;

                    case "begin":
                        seenBegin = true;
                        continue;

                    case "end":
                        if (error != null) throw error;
                        return true;

                    case "row":
                        if (seenBegin)
                        {
                            jsonData._tag = null;
                            jsonData._type = null;
                            _resultBuffer.Add(jsonData);
                        }
                        break;

                    default:
                        throw new Exception($"Unknown data type {jsonData._type} from edge");
                }
            }
        }
        catch
        {
            return true;
        }
    }

    public IReadOnlyList<dynamic> GetReceivedData() => _resultBuffer.AsReadOnly();

    private void SendCommand(string message)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(message);
        _udpClient.Send(bytes, bytes.Length, _address, _port);
    }

    public bool ExecuteCommand(string msgType, string cmdLine)
    {
        var (tag, message) = PrepareCommand(msgType, cmdLine);
        SendCommand(message);
        return ProcessResponse(tag);
    }

    public bool Read(string cmdLine)
    {
        _resultBuffer.Clear();
        return ExecuteCommand("r", cmdLine);
    }

    public bool Write(string cmdLine)
    {
        _resultBuffer.Clear();
        return ExecuteCommand("w", cmdLine);
    }

    public bool Subscribe(string cmdLine)
    {
        _resultBuffer.Clear();
        return ExecuteCommand("s", cmdLine);
    }

    public dynamic ReadEvent()
    {
        _udpClient.Client.ReceiveTimeout = 3600000;
        var endPoint = new IPEndPoint(IPAddress.Any, _port);
        var receiveBytes = _udpClient.Receive(ref endPoint);
        var data = System.Text.Encoding.UTF8.GetString(receiveBytes);
        var jsonData = JsonConvert.DeserializeObject<dynamic>(data);

        if (jsonData!._type != "event")
        {
            throw new Exception($"Unexpected data type {jsonData._type} from edge");
        }

        jsonData._tag = null;
        jsonData._type = null;
        return jsonData;
    }

    public void Dispose()
    {
        _udpClient?.Dispose();
    }
}