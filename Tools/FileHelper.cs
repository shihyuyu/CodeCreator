using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCreator
{
	public class FileHelper
	{
		/// <summary>
		/// 讀取檔案路徑
		/// </summary>
		/// <param name="FilePath">檔案路徑</param>
		/// <returns></returns>
		public static string ReadFile(string FilePath)
		{
			if (string.IsNullOrEmpty(FilePath))
			{
				throw new Exception($"FilePath不可為空白");
			}

			if (!File.Exists(FilePath))
			{
				throw new Exception($"找不到檔案路徑：{ FilePath }");
			}

			string content = File.ReadAllText(FilePath);
			return content;
		}

		public static void WriteFile(string FilePath, string Content)
		{
			if (string.IsNullOrEmpty(FilePath))
			{
				throw new Exception($"FilePath不可為空白");
			}

			File.WriteAllText(FilePath, Content);
		}
	}
}
