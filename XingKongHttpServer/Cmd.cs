using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XingKongHttpServer
{
    class Cmd
    {
        private System.Diagnostics.Process p;

        public void Start()
        {
            if (p != null)
            {
                Stop();
            }
            p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();
        }

        public void Input(string input)
        {
            Console.WriteLine(input);
            p.StandardInput.WriteLine(input);
        }

        public void Stop()
        {
            if (p != null)
            {
                p.StandardInput.WriteLine("exit");
                p.WaitForExit();//等待程序执行完退出进程
                p.Close();
                p = null;
            }
        }
    }
}
