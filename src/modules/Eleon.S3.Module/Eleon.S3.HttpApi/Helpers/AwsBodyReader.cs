using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EleonS3.HttpApi.Helpers;
public static class AwsSigV4ChunkedDecoder
{
    /// <summary>
    /// Decodes an AWS SigV4 streaming (chunked) request body into the original payload bytes.
    /// Handles chunk headers like "1C;chunk-signature=...", binary data, and trailer headers after the 0 chunk.
    /// </summary>
    public static async Task<byte[]> DecodeAsync(Stream body)
    {
        if (body == null) throw new ArgumentNullException(nameof(body));

        using var output = new MemoryStream();

        while (true)
        {
            // 1) Read a header line terminated by CRLF (e.g. "1C;chunk-signature=abcd...").
            var header = await ReadLineAsync(body).ConfigureAwait(false);
            if (header is null)
                throw new EndOfStreamException("Unexpected end of stream while reading chunk header.");

            if (header.Length == 0)
                continue; // tolerate extra blank lines

            // 2) End marker starts with "0"
            if (header.Length > 0 && header[0] == '0')
            {
                // After the final 0-sized chunk, there may be trailer headers.
                // Consume trailer lines until a blank line.
                while (true)
                {
                    var trailer = await ReadLineAsync(body).ConfigureAwait(false);
                    if (trailer is null) break;           // end of stream
                    if (trailer.Length == 0) break;       // blank line ends trailers
                }
                break; // done
            }

            // 3) Parse the chunk size (hex until ';' or end-of-line).
            var semi = header.IndexOf(';');
            var sizeHex = semi >= 0 ? header[..semi] : header;

            if (!int.TryParse(sizeHex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var size) || size < 0)
                throw new InvalidDataException($"Invalid chunk header: '{header}'");

            // 4) Read exactly <size> bytes of chunk data.
            await CopyExactAsync(body, output, size).ConfigureAwait(false);

            // 5) Expect CRLF after the chunk data.
            await ExpectCrlfAsync(body).ConfigureAwait(false);
        }

        return output.ToArray();
    }

    // Reads a CRLF-terminated line (without the CRLF) as UTF-8 text.
    // Returns null only if stream ends before any bytes are read for this line.
    private static async Task<string?> ReadLineAsync(Stream stream)
    {
        using var ms = new MemoryStream();
        bool sawAny = false;

        while (true)
        {
            int b = await ReadByteAsync(stream).ConfigureAwait(false);
            if (b == -1)
                return sawAny ? Encoding.UTF8.GetString(ms.ToArray()) : null;

            sawAny = true;

            if (b == '\n')
            {
                // Trim an optional preceding '\r'
                var buf = ms.ToArray();
                int len = buf.Length;
                if (len > 0 && buf[len - 1] == (byte)'\r')
                    len--;
                return Encoding.UTF8.GetString(buf, 0, len);
            }

            ms.WriteByte((byte)b);
        }
    }

    // Reads exactly 'size' bytes from source to dest or throws EndOfStreamException.
    private static async Task CopyExactAsync(Stream source, Stream dest, int size)
    {
        var buffer = new byte[Math.Min(8192, size)];
        int remaining = size;
        while (remaining > 0)
        {
            int toRead = Math.Min(buffer.Length, remaining);
            int read = await source.ReadAsync(buffer.AsMemory(0, toRead)).ConfigureAwait(false);
            if (read == 0)
                throw new EndOfStreamException("Unexpected end of chunked body.");
            await dest.WriteAsync(buffer.AsMemory(0, read)).ConfigureAwait(false);
            remaining -= read;
        }
    }

    // Ensures the next two bytes are CRLF.
    private static async Task ExpectCrlfAsync(Stream stream)
    {
        int c1 = await ReadByteAsync(stream).ConfigureAwait(false);
        int c2 = await ReadByteAsync(stream).ConfigureAwait(false);
        if (c1 != '\r' || c2 != '\n')
            throw new InvalidDataException("Missing CRLF after chunk data.");
    }

    // Async single-byte read helper (since Stream has no ReadByteAsync).
    private static async Task<int> ReadByteAsync(Stream stream)
    {
        byte[] one = new byte[1];
        int n = await stream.ReadAsync(one.AsMemory(0, 1)).ConfigureAwait(false);
        return n == 0 ? -1 : one[0];
    }
}
