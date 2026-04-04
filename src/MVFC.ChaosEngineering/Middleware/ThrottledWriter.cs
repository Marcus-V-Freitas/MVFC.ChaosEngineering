namespace MVFC.ChaosEngineering.Middleware;

/// <summary>Helper class for writing data to a stream in throttled chunks.</summary>
internal static class ThrottledWriter
{
    /// <summary>Writes the provided bytes to the destination stream in chunks, with a delay between them.</summary>
    /// <param name="destination">The destination stream.</param>
    /// <param name="bytes">The data to write.</param>
    /// <param name="chunkSize">The maximum number of bytes to write per chunk.</param>
    /// <param name="chunkDelay">The delay between writes.</param>
    /// <param name="ct">The cancellation token.</param>
    internal static async Task WriteAsync(
        Stream destination,
        byte[] bytes,
        int chunkSize,
        TimeSpan chunkDelay,
        CancellationToken ct)
    {
        var offset = 0;
        while (offset < bytes.Length)
        {
            var count = Math.Min(chunkSize, bytes.Length - offset);
            await destination.WriteAsync(bytes.AsMemory(offset, count), ct).ConfigureAwait(false);
            await destination.FlushAsync(ct).ConfigureAwait(false);
            offset += count;

            if (offset < bytes.Length)
                await Task.Delay(chunkDelay, ct).ConfigureAwait(false);
        }
    }
}
