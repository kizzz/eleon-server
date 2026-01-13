using System.Text;
using Eleon.McpCodexGateway.Host.Stdio;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Eleon.McpCodexGateway.Host.Stdio.Tests;

public sealed class StreamPipeTests
{
    [Fact]
    public async Task PipeAsync_CompletesWhenSourceEndsAndClosesDestination()
    {
        var sourcePayload = Encoding.UTF8.GetBytes("payload");
        await using var source = new MemoryStream(sourcePayload);
        await using var destination = new TrackingStream();
        var pipe = new StreamPipe(NullLogger<StreamPipe>.Instance);

        await pipe.PipeAsync(source, destination, completeDestinationOnCompletion: true, "stdin->codex", CancellationToken.None);

        Encoding.UTF8.GetString(destination.Captured.ToArray()).Should().Be("payload");
        destination.WasDisposed.Should().BeTrue();
    }

    [Fact]
    public async Task PipeAsync_DoesNotDisposeDestinationWhenFlagIsFalse()
    {
        var sourcePayload = Encoding.UTF8.GetBytes("keep-open");
        await using var source = new MemoryStream(sourcePayload);
        await using var destination = new TrackingStream();
        var pipe = new StreamPipe(NullLogger<StreamPipe>.Instance);

        await pipe.PipeAsync(source, destination, completeDestinationOnCompletion: false, "stdin->codex", CancellationToken.None);

        Encoding.UTF8.GetString(destination.Captured.ToArray()).Should().Be("keep-open");
        destination.WasDisposed.Should().BeFalse();
    }

    [Fact]
    public async Task PipeAsync_StopsOnCancellationWithoutThrowing()
    {
        await using var source = new BlockingStream();
        await using var destination = new TrackingStream();
        var pipe = new StreamPipe(NullLogger<StreamPipe>.Instance);
        using var cts = new CancellationTokenSource();

        var piping = pipe.PipeAsync(source, destination, completeDestinationOnCompletion: false, "stdin->codex", cts.Token);

        // Trigger cancellation while stream is blocked waiting to read more data.
        cts.Cancel();

        await piping; // should complete gracefully
        destination.Captured.Should().BeEmpty();
        destination.WasDisposed.Should().BeFalse();
    }

    private sealed class TrackingStream : Stream
    {
        private readonly List<byte> captured = new();

        public bool WasDisposed { get; private set; }

        public IReadOnlyList<byte> Captured => captured.ToArray();

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => captured.Count;

        public override long Position
        {
            get => captured.Count;
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count)
        {
            for (var i = 0; i < count; i++)
            {
                captured.Add(buffer[offset + i]);
            }
        }

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            captured.AddRange(buffer.ToArray());
            return ValueTask.CompletedTask;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                WasDisposed = true;
            }
        }
    }

    private sealed class BlockingStream : Stream
    {
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => 0;
        public override long Position
        {
            get => 0;
            set => throw new NotSupportedException();
        }

        public override void Flush() => throw new NotSupportedException();

        public override int Read(byte[] buffer, int offset, int count)
        {
            Thread.Sleep(Timeout.Infinite);
            return 0;
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<int>();
            cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));
            return new ValueTask<int>(tcs.Task);
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
