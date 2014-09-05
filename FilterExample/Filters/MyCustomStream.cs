using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace FilterExample.Filters
{
    public class MyCustomStream : Stream
    {

        private readonly Stream filter;
        private readonly MemoryStream cacheStream = new MemoryStream();

        public MyCustomStream(Stream filter)
        {
            this.filter = filter;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            cacheStream.Write(buffer, 0, count);
        }

        public override void Flush()
        {
            if (cacheStream.Length > 0)
            {
                var allScripts = new StringBuilder();
                string wholeHtmlDocument = Encoding.UTF8.GetString(cacheStream.ToArray(), 0, (int)cacheStream.Length);
                var regex = new Regex(@"<script[^>]*>(?<script>([^<]|<[^/])*)</script>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                //Remove all Script Tag
                wholeHtmlDocument = regex.Replace(wholeHtmlDocument, m => { allScripts.Append(m.Groups[0].Value); return "<!-- Removed Script -->"; });

                //Put all Script at the end
                if (allScripts.Length > 0)
                {
                    wholeHtmlDocument = wholeHtmlDocument.Replace("</html>", "<script type='text/javascript'>" + allScripts.ToString() + "</script></html>");
                }
                var buffer = Encoding.UTF8.GetBytes(wholeHtmlDocument);
                this.filter.Write(buffer, 0, buffer.Length);
                cacheStream.SetLength(0);
            }
            this.filter.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.filter.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.filter.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.filter.Read(buffer, offset, count);
        }

        public override bool CanRead
        {
            get { return this.filter.CanRead; }
        }

        public override bool CanSeek
        {
            get { return this.filter.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return this.filter.CanWrite; }
        }

        public override long Length
        {
            get { return this.filter.Length; }
        }

        public override long Position { get { return this.filter.Position; }
            set { this.filter.Position = value; }
        }
    }
}