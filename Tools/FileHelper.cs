using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace CodeCreator
{
	public class FileHelper
	{
		public static readonly string TemplateDir = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "template");
		/// <summary>
		/// 取得Template檔案內容
		/// </summary>
		/// <param name="TemplateName"></param>
		/// <returns></returns>
		public static string GetTemplate(string TemplateName)
		{
			string fpath = Path.Combine(TemplateDir, TemplateName);
			if (!File.Exists(fpath)) { throw new Exception($"找不到Template檔案路徑，路徑名稱 : { fpath }"); }
			return ReadFile(fpath);
		}
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
		/// <summary>
		/// 寫入檔案內容(沒有檔案就新增，有檔案就續寫)
		/// </summary>
		/// <param name="FilePath"></param>
		/// <param name="Content"></param>
		public static void WriteFile(string FilePath, string Content)
		{
			if (string.IsNullOrEmpty(FilePath))
			{
				throw new Exception($"FilePath不可為空白");
			}

			string dir = Path.GetDirectoryName(FilePath);
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}

			File.WriteAllText(FilePath, Content);
		}

		/// <summary>
		/// 計算相對路徑
		/// </summary>
		/// <param name="basePath"></param>
		/// <param name="targetPath"></param>
		/// <returns></returns>
		public static string GetRelativePath(string basePath, string targetPath)
		{
			Uri baseUri = new Uri(basePath);
			Uri targetUri = new Uri(targetPath);
			return baseUri.MakeRelativeUri(targetUri).ToString().Replace(@"/", @"\");
		}
	}
}
