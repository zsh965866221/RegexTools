using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WPF_Homework_2016
{
    static class LogController
    {
        private static string LogFileName = "log.txt";
        private static ConcurrentQueue<string> LogStringQueue;
        private static FileStream fs;
        private static StreamWriter sw;
        private static StringBuilder sb;
        private static ThreadStart aThreadStart;
        private static Thread aThread;

        static LogController()
        {
            try
            {
                fs = new FileStream(LogFileName, FileMode.Create);
                sw = new StreamWriter(fs);
                aThreadStart=new ThreadStart(WriteLog);
                aThread=new Thread(aThreadStart);
                aThread.IsBackground = true;
                aThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
            LogStringQueue=new ConcurrentQueue<string>();
            sb=new StringBuilder();
        }

        static void WriteLog()
        {
            while (true)
            {
                while (!LogStringQueue.IsEmpty)
                {
                    sb.Clear();
                    try
                    {
                        sb.Append(DateTime.Now.ToString());
                        sb.Append(" : ");
                        string alogString = null;
                        LogStringQueue.TryDequeue(out alogString);
                        sb.Append(alogString);
                        sw.WriteLine(sb.ToString());
                        sw.Flush();
                    }
                    catch (Exception ex)
                    {
                    }
                }
                aThread.Suspend();
            }
            
        }

        public static void Clear()
        {
            WriteLog();
            sw.Close();
            fs.Close();
        }

        public static void Log(string aLog)
        {
            LogStringQueue.Enqueue(aLog);
            if ((aThread.ThreadState & ThreadState.Suspended) != 0)
            {
                aThread.Resume();
            }

        }
    }
}
