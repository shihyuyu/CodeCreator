using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCreator.Tools
{
	public class CMDHelper
	{
		// 當找不到檔案或者拒絕訪問時出現的Win32錯誤碼
		const int ERROR_FILE_NOT_FOUND = 2;
		const int ERROR_ACCESS_DENIED = 5;

		const string CMD_CLEAR = "CLS";
		const string CMD_GO_TO_DIR = "CD";
		const string CMD_SHOW_FILE_LIST = "DIR /b";
		const string CMD_PRINT_TEXT_CONTENT = "TYPE";
		const string CMD_TO_UNICODE = "chcp 65001";

		public void PrintCommand(string path)
		{
			Process process = InitProcess();

			try
			{
				// 更改文字編碼
				// 清空ouput文字
				// 取得檔案清單
				string str = ExecuteCmd(CMD_CLEAR, $"{ CMD_SHOW_FILE_LIST } \"{ path }\" ");
				Console.WriteLine(str);

				var ary = (from s in str.Split('\n').ToList<string>()
						   where !string.IsNullOrWhiteSpace(s)
						   let v = CMD_PRINT_TEXT_CONTENT + " \"" + path + s.Replace(Environment.NewLine, "").Trim() + "\" "
						   select v).ToList();

				for (int i = 0; i < 3; i++) { ary.RemoveAt(0); }
				ary.Insert(0, CMD_TO_UNICODE);

				string all = ExecuteCmd(ary.ToArray());
				Console.WriteLine(all);
			}
			catch (Win32Exception e)
			{
				ExceptionHandle(e);
				process.Close();
			}
		}

		private Process InitProcess()
		{
			Process p = new Process();
			p.StartInfo.UseShellExecute = false;   //是否使用作業系統shell啟動 
			p.StartInfo.CreateNoWindow = true;   //是否在新視窗中啟動該程序的值 (不顯示程式視窗)
			p.StartInfo.RedirectStandardInput = true;  // 接受來自呼叫程式的輸入資訊 
			p.StartInfo.RedirectStandardOutput = true;  // 由呼叫程式獲取輸出資訊
			p.StartInfo.RedirectStandardError = true;  //重定向標準錯誤輸出
			p.StartInfo.FileName = "cmd.exe";

			return p;
		}

		public string ExecuteCmd(params string[] cmds)
		{
			string ret = "";
			using (Process p = InitProcess())
			{
				p.Start();

				string cmd = "";
				foreach (string c in cmds)
				{
					cmd += "& " + c + " ";
				}

				cmd = (cmd.Length > 0) ? cmd.Substring(1) + " & exit " : "exit";
				p.StandardInput.WriteLine(cmd);

				ret = p.StandardOutput.ReadToEnd();

				p.WaitForExit();
				p.Close();
			}

			return ret;
		}

		public void Test2()
		{
			Process process = new Process();
			process.StartInfo.UseShellExecute = false;   //是否使用作業系統shell啟動 
			process.StartInfo.CreateNoWindow = true;   //是否在新視窗中啟動該程序的值 (不顯示程式視窗)
			process.StartInfo.RedirectStandardInput = true;  // 接受來自呼叫程式的輸入資訊 
			process.StartInfo.RedirectStandardOutput = true;  // 由呼叫程式獲取輸出資訊
			process.StartInfo.RedirectStandardError = true;  //重定向標準錯誤輸出
			process.StartInfo.FileName = "cmd.exe";
			process.Start();                         // 啟動程式
			process.StandardInput.WriteLine("help & exit"); //向cmd視窗傳送輸入資訊
			process.StandardInput.AutoFlush = true;
			// 前面一個命令不管是否執行成功都執行後面(exit)命令，如果不執行exit命令，後面呼叫ReadToEnd()方法會假死
			//process.StandardInput.WriteLine("exit");

			StreamReader reader = process.StandardOutput;//獲取exe處理之後的輸出資訊
			string curLine = reader.ReadLine(); //獲取錯誤資訊到error
			while (!reader.EndOfStream)
			{
				if (!string.IsNullOrEmpty(curLine))
				{
					Console.WriteLine(curLine);
				}
				curLine = reader.ReadLine();
			}
			reader.Close(); //close程序

			process.WaitForExit();  //等待程式執行完退出程序
			process.Close();
		}

		/// <summary>
		/// 執行外部程式並將執行結果以字串方式傳回
		/// </summary>
		/// <param name="exeFile"></param>
		/// <param name="argument"></param>
		/// <returns></returns>
		public string Shell(string exeFile, string argument)
		{
			Process pShell = new Process();
			pShell.StartInfo.FileName = exeFile;
			pShell.StartInfo.Arguments = argument;
			//必須要設定以下兩個屬性才可將輸出結果導向
			pShell.StartInfo.UseShellExecute = false;
			pShell.StartInfo.RedirectStandardOutput = true;
			//不顯示任何視窗
			pShell.StartInfo.CreateNoWindow = true;
			//開始執行
			pShell.Start();
			pShell.WaitForExit();
			//將StdOUT的結果轉為字串, 其中StandardOutput屬性類別為StreamReader
			return pShell.StandardOutput.ReadToEnd();
		}

		private void ExceptionHandle(Win32Exception e)
		{
			if (e.NativeErrorCode == ERROR_FILE_NOT_FOUND)
			{
				Console.WriteLine(e.Message + ". 檢查檔案路徑.");
			}

			else if (e.NativeErrorCode == ERROR_ACCESS_DENIED)
			{
				Console.WriteLine(e.Message + ". 你沒有許可權操作檔案.");
			}
		}
	}
}
