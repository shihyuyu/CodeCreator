using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCreator.Core
{
	public class SchemaVO
	{
		/// <summary>
		/// 表格名稱
		/// </summary>
		public string TABLE_NAME { get; set; }
		/// <summary>
		/// 表格描述
		/// </summary>
		public string TABLE_DESC { get; set; }
		/// <summary>
		/// 欄位名稱
		/// </summary>
		public string COLUMN_NAME { get; set; }
		/// <summary>
		/// 欄位描述
		/// </summary>
		public string COLUMN_DESC { get; set; }
		/// <summary>
		/// 類型
		/// </summary>
		public string TYPE { get; set; }
		/// <summary>
		/// 長度
		/// </summary>
		public int LENGTH { get; set; }
		/// <summary>
		/// 允許NULL
		/// </summary>
		public bool ISNULL { get; set; }
		/// <summary>
		/// PK欄位
		/// </summary>
		public bool ISPK { get; set; }
	}
}
