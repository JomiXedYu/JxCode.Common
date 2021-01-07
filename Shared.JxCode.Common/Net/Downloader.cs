using System;
using System.IO;
using System.Net;
using System.Threading;


namespace JxCode.Net
{
    public sealed class Downloader : IDisposable
    {
        public enum DownloadState
        {
            None,
            Download,
            Pause,
            Finish,
        }
        //目标文件地址
        public string TargetURL { get; private set; }
        //目标文件大小
        public long TargetLength { get; private set; }
        //下载的本地文件路径
        public string LocalFilePath { get; private set; }
        //已经下载的本地文件大小
        public long DownloadedLength { get; private set; }
        //下载速度
        public int Speed { get; private set; }
        private long lastDownloadLength;
        private Timer timer;

        private DownloadState state = DownloadState.None;
        public DownloadState State
        {
            get => this.state;
            private set
            {
                this.state = value;
                this.StateHandler?.Invoke(value);
            }
        }

        //状态委托
        public event Action<DownloadState> StateHandler;

        //下载时的文件名
        private string downloadingName = string.Empty;
        //下载时的额外后缀名
        private const string DOWNLOADING_FILE_EXT = ".jxdown";

        private Thread downloadThread = null;
        //设置下载任务
        public void SetTask(string targetURL, string localPath)
        {
            this.Reset();
            this.TargetURL = targetURL;
            this.LocalFilePath = localPath;
        }
        //重置
        private void Reset()
        {
            this.State = DownloadState.None;
            this.DownloadedLength = 0;
            this.lastDownloadLength = 0;
            this.Speed = 0;
        }
        //开始
        public void Start()
        {
            if (this.State == DownloadState.Download)
                return;

            this.State = DownloadState.Download;

            if (this.downloadThread == null)
            {
                this.downloadThread = new Thread(this.DownloadThread);
            }

            //开启下载线程
            this.downloadThread.IsBackground = false;
            this.downloadThread.Start(this);

            //开启速度计时器线程
            if (this.timer == null)
            {
                this.timer = new Timer((sender) =>
                {
                    Downloader self = (Downloader)sender;
                    var distance = self.DownloadedLength - self.lastDownloadLength;
                    self.Speed = (int)distance;
                    self.lastDownloadLength = self.DownloadedLength;
                }, this, -1, 0);
            }

            this.timer.Change(0, 1000);
        }
        //下载线程
        private void DownloadThread(object _this)
        {
            Downloader self = (Downloader)_this;

            self.downloadingName = self.LocalFilePath + DOWNLOADING_FILE_EXT;

            if (File.Exists(self.LocalFilePath))
            {
                //TODO 下载完成
                self.State = DownloadState.Finish;
                return;
            }
            //获取长度
            self.TargetLength = Http.GetHttpFileLenght(self.TargetURL);
            //获取远程文件流
            HttpWebRequest httpWebRequest = HttpWebRequest.Create(self.TargetURL) as HttpWebRequest;


            //本地文件流
            FileStream fs = null;
            if (File.Exists(self.downloadingName))
            {
                //断点续传
                fs = new FileStream(self.downloadingName, FileMode.Open);
                self.DownloadedLength = fs.Length;
                //移动指针
                fs.Seek(fs.Length, SeekOrigin.Current);
                //请求续传
                httpWebRequest.AddRange(fs.Length);
            }
            else
            {
                //创建新文件
                fs = new FileStream(self.downloadingName, FileMode.Create);
            }

            //下载
            Stream downStream = httpWebRequest.GetResponse().GetResponseStream();

            const int chunkSize = 409600; // 400kb
            byte[] buf = new byte[chunkSize];

            int size = 0;

            while (self.state == DownloadState.Download && (size = downStream.Read(buf, 0, chunkSize)) > 0)
            {
                //写文件流
                fs.Write(buf, 0, size);
                fs.Flush();
                self.DownloadedLength += size;
            }

            fs.Close();
            downStream.Close();

            self.Finish();
        }

        private void Finish()
        {
            File.Move(this.downloadingName, this.LocalFilePath);
            this.State = DownloadState.Finish;
        }

        public void Pause()
        {
            this.State = DownloadState.Pause;
            this.timer.Change(-1, 0);
        }

        public void Dispose()
        {
            if (this.downloadThread != null)
            {
                this.downloadThread.Abort();
                this.downloadThread = null;
            }
            this.timer.Dispose();
            this.timer = null;
            GC.SuppressFinalize(this);
        }
    }

}
