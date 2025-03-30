using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPF_Frame.Base
{
    public class LogBase
    {
        public static string rootPath = @"log"; //根路径
        public static string rootSonPath = $@"{rootPath}\{DateTime.Now.ToString("yyyy-MM-dd")}"; //当天目录路径
        public static string ImagePath = $@"{rootSonPath}\图像"; //创建当天目录图像
        public static string LogPath = $@"{rootSonPath}\日志"; //创建当天日志路径
        public static string ImageNGPath = $@"{ImagePath}\NG"; //创建当天NG图像路径
        public static string ImageOKPath = $@"{ImagePath}\OK"; //创建当天OK图像路径

        //线程安全
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        /// <summary>
        /// 检查和创建文件目录
        /// </summary>
        public void CreateFileDirectory()
        {
            if (rootSonPath != $@"{rootPath}\{DateTime.Now.ToString("yyyy-MM-dd")}")
            {
                rootSonPath = $@"{rootPath}\{DateTime.Now.ToString("yyyy-MM-dd")}"; //当天目录路径
                ImagePath = $@"{rootSonPath}\图像"; //创建当天目录图像
                LogPath = $@"{rootSonPath}\日志"; //创建当天日志路径
                ImageNGPath = $@"{ImagePath}\NG"; //创建当天NG图像路径
                ImageOKPath = $@"{ImagePath}\OK"; //创建当天OK图像路径
            }

            //第一步: 判断根目录是否被创建 
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath); //创建目录, 相对路径
            }

            // 第二步: 在更目录中创建当天的子目录
            if (!Directory.Exists(rootSonPath))
            {
                Directory.CreateDirectory(rootSonPath); //创建当天目录
            }

            if (!Directory.Exists(ImagePath))
            {
                Directory.CreateDirectory(ImagePath); //创建当天目录图像
            }

            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath); //创建当天日志路径 
            }

            if (!Directory.Exists(ImageOKPath))
            {
                Directory.CreateDirectory(ImageOKPath); //创建当天OK图像路径
            }

            if (!Directory.Exists(ImageNGPath))
            {
                Directory.CreateDirectory(ImageNGPath); //创建当天NG图像路径
            }
        }


        public async Task WriteLog(string log)
        {
            CreateFileDirectory();
            await _semaphore.WaitAsync(); // 等待信号
            try
            {
                string path = $@"{LogPath}\操作日志.log";
                using (FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write))
                {
                    byte[] data = Encoding.UTF8.GetBytes($"{DateTime.Now}: {log}\r\n"); //文本转成二进制

                    await fileStream.WriteAsync(data, 0, data.Length);
                }
            }
            finally
            {
                _semaphore.Release(); // 释放信号
            }
        }

        /// <summary>
        /// 读日志最后4kb
        /// </summary>
        public async Task<string> ReadLog()
        {
            string path = Path.Combine(LogPath, "操作日志.log");
            const int bufferSize = 4096; // 4KB
            try
            {
                if (!File.Exists(path))
                {
                    return string.Empty; // 空文件直接返回
                }

                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    long fileLength = fs.Length;
                    if (fileLength == 0)
                    {
                        return string.Empty; // 空文件直接返回
                    }

                    // 计算起始位置（确保不越界）
                    long startPosition = Math.Max(0, fileLength - bufferSize); //文件总长度 - 4KB 处
                    int bytesToRead = (int)Math.Min(bufferSize, fileLength);

                    // 定位并读取
                    fs.Seek(startPosition, SeekOrigin.Begin);
                    byte[] buffer = new byte[bytesToRead];
                    int bytesRead = await fs.ReadAsync(buffer, 0, bytesToRead);

                    // 处理编码（自动检测BOM头，UTF-8优先）
                    using (MemoryStream ms = new MemoryStream(buffer, 0, bytesRead))
                    using (StreamReader reader =
                           new StreamReader(ms, Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
                    {
                        return await reader.ReadToEndAsync(); // 自动处理编码和换行符
                    }
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                throw;
            }
        }
    }
}