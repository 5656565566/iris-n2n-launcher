using System.Net;
using System.Net.Sockets;
using DnsClient;
using Microsoft;
using STUN;
using STUN.Client;
using STUN.Enums;
using STUN.Proxy;
using STUN.StunResult;

namespace iris_n2n_launcher.Utils;

internal class StunHelper
{
    private const int Port = 3478;
    private readonly LookupClient _dnsClient = new();

    public async Task<NatType?> Stun3489TestAsync(string serverHostname)
    {
        Verify.Operation(StunServer.TryParse(serverHostname, out StunServer? server), @"Wrong STUN Server!");

        var serverIp = await GetServerIpAsync(server.Hostname, null);
        var localEndPoint = new IPEndPoint(IPAddress.Any, 0);

        using var udpClient = new UdpClient(localEndPoint);

        if (serverIp == null)
        {
            return null;
        }

        using var client = new StunClient3489(new IPEndPoint(serverIp, Port), localEndPoint);

        try
        {
            await client.QueryAsync();
            return client.State.NatType;
        }
        catch
        {
            return NatType.Unknown;
        }
    }

    public async Task<StunResult5389?> Stun5780TestAsync(string serverHostname)
    {
        Verify.Operation(StunServer.TryParse(serverHostname, out StunServer? server), @"Wrong STUN Server!");

        var serverIp = await GetServerIpAsync(server.Hostname, null);
        var localEndPoint = new IPEndPoint(IPAddress.Any, 0);

        using var udpClient = new UdpClient(localEndPoint);

        if (serverIp == null)
        {
            return null;
        }

        using var client = new StunClient5389UDP(new IPEndPoint(serverIp, server.Port), localEndPoint);

        try
        {
            await client.QueryAsync();
            return client.State;
        }
        catch
        {
            return null;
        }
    }

    private async Task<IPAddress?> GetServerIpAsync(string hostname, IPEndPoint? localEndPoint)
    {
        try
        {
            if (localEndPoint?.AddressFamily == AddressFamily.InterNetworkV6)
            {
                try
                {
                    var aaaaResult = await _dnsClient.QueryAsync(hostname, QueryType.AAAA);
                    if (aaaaResult.Answers.AaaaRecords().Any())
                    {
                        return aaaaResult.Answers.AaaaRecords().First().Address;
                    }
                }
                catch { }
            }

            var aResult = await _dnsClient.QueryAsync(hostname, QueryType.A);
            if (aResult.Answers.ARecords().Any())
            {
                return aResult.Answers.ARecords().First().Address;
            }

            if (localEndPoint?.AddressFamily != AddressFamily.InterNetworkV6)
            {
                var aaaaResult = await _dnsClient.QueryAsync(hostname, QueryType.AAAA);
                if (aaaaResult.Answers.AaaaRecords().Any())
                {
                    return aaaaResult.Answers.AaaaRecords().First().Address;
                }
            }

            return null;
        }
        catch { return null; }
    }
}