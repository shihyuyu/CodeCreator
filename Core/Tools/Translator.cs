using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCreator.Core
{
	public class Translator
	{
		/// <summary>
		/// MSSQL型別轉C#型別
		/// </summary>
		/// <param name="dbType"></param>
		/// <returns></returns>
		public static string MSSQL_To_CSharp_Type(string dbType)
		{
			if (string.IsNullOrEmpty(dbType)) { dbType = ""; }

			switch (dbType.ToUpper())
			{
				// 長度為 4 個 bytes；介於 -2,147,483,648 與 2,147,483,647 間的整數
				case "INT": 									return "int";
				// 長度為 2 個 bytes；介於 -32,768 與 32,767 間的整數
				case "SMALLINT": 						return "int";
				// 長度為 1 個 bytes；介於 0 與 255 間的整數
				case "TINYINT": 							return "int";
				// 長度為 8 個 bytes；介於 -2^63 與 2^63-1 間的整數
				case "BIGINT": 								return "Int64";
				// 只佔用一個位元，且不允許存放 NULL 值
				case "BIT": 									return "bool";
				// 使用 2 到 17 個 bytes 來儲存資料，可儲存的值介於 -1038-1 與 1038-1 之間；p 用來定義小數點兩邊可以被儲存的位數總數目，而 s 代表小數點右邊的有效位數（s < p）；p的預設值為 18 而 s 的預設值為0
				case "DECIMAL": 						return "int";
				// 與 DECIMAL[(p[,s])] 同
				case "NUMERIC": 						return "int";
				// n 是儲存 float 數字的小數位數，介於 1 與 53 間；若 n 介於 1 與 24 間，儲存大小為 4 個 bytes，有效位數為 7 位數；若 n 介於 25 與 53 間，儲存大小為 8 個 bytes，有效位數為 15 位數
				case "FLOAT": 								return "double";
				// 長度為 4 個 bytes；介於 -3.4E-38 與 3.4E+38 間的浮點數；與 FLOAT(24) 相同
				case "REAL": 								return "";
				// 長度為 8 個 bytes；介於 1/1/1753 與 12/31/9999 間的日期時間
				case "DATETIME": 						return "DateTime";
				// 長度為 4 個 bytes；介於 1/1/1900 與 6/6/2079間的日期時間
				case "SMALLDATETIME": 			return "DateTime";
				// 固定長度為 n 的字元型態，n 必須介於 1 與 8000 之間
				case "CHAR": 								return "char";
				// 與 CHAR 相同，只是若輸入的資料小於 n，資料庫不會自動補空格，因此為變動長度之字串
				case "VARCHAR": 						return "string";
				// 用來儲存大量的（可高達兩億個位元組）字元資料，儲存空間以 8k 為單位動態增加
				case "TEXT": 									return "string";
				// 與 CHAR 相同，只是每一個字元為兩個 bytes 的 unicode，且 n 最大為 4000
				case "NCHAR": 							return "string";
				// 與 VARCHAR 相同，只是每一個字元為兩個 bytes 的 unicode，且 n 最大為 4000
				case "NVARCHAR": 					return "string";
				// 和 TEXT 雷同，只是儲存的是 Unicode 資料
				case "NTEXT": 								return "string";
				// 固定長度為 n+4 個 bytes； n 為 1 到 8000 的值，輸入的值必需符合兩個條件：（1） 每一個值皆為 0-9、a-f 的值；（2）每一個值的前面必須有 0X
				case "BINARY": 							return "byte[]";
				// 與 BINARY 相同，只是若輸入的資料小於 n，資料庫不會自動補 0，因此長度為變動的
				case "VARBINARY": 					return "byte[]";
				// 和 TEXT 雷同，只是儲存的是影像資料
				case "IMAGE": 								return "";
				// 長度為 8 個 bytes 的整數，小數點的精確度取四位
				case "MONEY": 							return "int";
				// 長度為 4 個 bytes 的整數，小數點的精確度取四位
				case "SMALLMONEY": 				return "int";
				// 8 bytes 的 16 進位值
				case "TIMESTAMP": 					return "TimeSpan";
				//// 16 bytes 的 16 進位值
				//case "UNIQUEIDENTIFIER": 		return "";
				//// 此資料型別可儲存 text、ntext、timestamp 與 sql_variant 以外的各種 SQL Server 支援的資料型別。
				//case "SQL_VARIANT":				return "";
				//// 查詢結果的資料集 (註：2005)
				//case "CURSOR":							return "";
				//// 表格型式的資料 (註：2005)
				//case "TABLE":								return "";
				default: 											return dbType;
			}
		}
		/// <summary>
		/// C#型別轉MSSQL型別
		/// </summary>
		/// <param name="dbType"></param>
		/// <returns></returns>
		public static string CSharpType_To_MSSQL_Type(string csharpType, int length=0)
		{
			switch (csharpType)
			{
				case "int":					if (length == 0) { return "INT"; }
													else if (length <= 2) { return "TINYINT"; }
													else { return "INT"; }
				case "Int64":				return "BIGINT";
				case "double":			return "FLOAT";
				case "bool":				return "BIT";
				case "char":				if (length == 1) { return "CHAR"; } 
													else { return "VARCHAR"; }
				case "string":			if (length == 1 || length == 3 || length == 6 || length == 36) { return "NCHAR"; }
													else if (length <= 4000) { return "NVARCHAR"; }
													else if (length == 0) { return "NTEXT"; }
													else { return "NVARCHAR"; }
				case "byte[]":			if (length != 0) { return "BINARY"; } 
													else { return "VARBINARY"; }	
				case "DateTime":		return "DATETIME";
				case "TimeSpan":	return "TIMESTAMP";
				default:						return (!string.IsNullOrEmpty(csharpType)) ? csharpType.ToUpper() : "NVARCHAR";
			}
		}
	}
}
