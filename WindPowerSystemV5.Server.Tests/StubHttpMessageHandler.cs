using System.Net;
using System.Text;

namespace WindPowerSystemV5.Server.Tests;

/// <summary>Returns queued canned responses and records the requested URLs and request bodies.</summary>
public class StubHttpMessageHandler : HttpMessageHandler
{
    private readonly Queue<(HttpStatusCode Status, string Json)> _responses = new();

    public List<string> RequestedUrls { get; } = new();

    public List<string> RequestBodies { get; } = new();

    public StubHttpMessageHandler Enqueue(string json, HttpStatusCode status = HttpStatusCode.OK)
    {
        _responses.Enqueue((status, json));
        return this;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        RequestedUrls.Add(request.RequestUri!.ToString());
        RequestBodies.Add(request.Content is null ? string.Empty : await request.Content.ReadAsStringAsync(cancellationToken));
        var (status, json) = _responses.Dequeue();
        return new HttpResponseMessage(status)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }
}
