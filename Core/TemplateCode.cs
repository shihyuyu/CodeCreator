using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeCreator.Core;

namespace CodeCreator
{
	/// <summary>
	/// 命名規則說明
	/// 產生類型__副檔名
	/// </summary>
	public class TemplateCode
	{
		#region 屬性
		public enum TemplateType
		{
			Code_CSharp_Model,
			Code_CSharp_Controller,
			Code_CSharp_BussinessObject,
			Code_CSharp_DataAccessObject,
			View_HTML,
			DB_MSSQL_DDL,
			DB_MSSQL_DML,
			DB_MSSQL_DQL
		}
		/// <summary>
		/// Model.cs
		/// </summary>
		public const string Code_CSharp_Model = "Code_CSharp_Model";
		/// <summary>
		/// Controller.cs
		/// </summary>
		public const string Code_CSharp_Controller = "Code_CSharp_Controller";
		/// <summary>
		/// ValueObject
		/// </summary>
		public const string Code_CSharp_ValueObject = "Code_CSharp_ValueObject";
		/// <summary>
		/// BussinessObject
		/// </summary>
		public const string Code_CSharp_BussinessObject = "Code_CSharp_BussinessObject";
		/// <summary>
		/// DataAccessObject
		/// </summary>
		public const string Code_CSharp_DataAccessObject = "Code_CSharp_DataAccessObject";
		/// <summary>
		/// html
		/// </summary>
		public const string View_HTML = "View_HTML";
		/// <summary>
		/// create / drop / alter
		/// </summary>
		public const string DB_MSSQL_DDL = "DB_MSSQL_DDL";
		/// <summary>
		/// insert / delete / update
		/// </summary>
		public const string DB_MSSQL_DML = "DB_MSSQL_DML";
		/// <summary>
		/// select
		/// </summary>
		public const string DB_MSSQL_DQL = "DB_MSSQL_DQL";
		#endregion
		public enum SchemaSource {
			DataBase,
			Code,
			Document
		}
		#region public function
		/// <summary>
		/// 取得schema
		/// </summary>
		/// <param name="source">Schema的資料來源，可以是連線字串或檔案路徑</param>
		/// <returns></returns>
		public List<SchemaVO> GetSchema(SchemaSource input, SchemaSource output, string source="")
		{
			List<SchemaVO> schemas;

			// 輸入來源
			switch (input)
			{
				case SchemaSource.DataBase:
					schemas = CreateSchemaByDB(source);
					break;
				case SchemaSource.Code:
					schemas = CreateSchemaByCode(source);
					break;
				case SchemaSource.Document:
					schemas = CreateSchemaByDocument(source);
					break;
				default:
					throw new Exception("input格式未設定，無法輸讀取schema。");
			}

			// 輸出格式
			switch (output)
			{
				case SchemaSource.DataBase:
					return ConvertSchemaToDBStyle(schemas);
				case SchemaSource.Code:
					return ConvertSchemaToCodeStyle(schemas);
				case SchemaSource.Document:
					return ConvertSchemaToDocumentStyle(schemas);
				default:
					throw new Exception("output格式未設定，無法輸出schema。");
			}
		}

		public List<SchemaVO> ConvertSchemaToCodeStyle(List<SchemaVO> source)
		{
			SchemaDAO dao = new SchemaDAO();
			var list = (from item in source
						select new SchemaVO()
						{
							TABLE_NAME = item.TABLE_NAME
							,
							TABLE_DESC = item.TABLE_DESC
							,
							COLUMN_NAME = item.COLUMN_NAME
							,
							COLUMN_DESC = item.COLUMN_DESC
							,
							TYPE = Translator.MSSQL_To_CSharp_Type(Convert.ToString(item.TYPE))
							,
							LENGTH = item.LENGTH
							,
							ISNULL = Convert.ToBoolean(item.ISNULL)
							,
							ISPK = CheckIsPrimaryKey(item.TABLE_NAME, item.COLUMN_NAME)
						}
				).ToList<SchemaVO>();

			return list;
		}

		public List<SchemaVO> ConvertSchemaToDocumentStyle(List<SchemaVO> source)
		{
			return new List<SchemaVO>();
		}

		public List<SchemaVO> ConvertSchemaToDBStyle(List<SchemaVO> source)
		{
			var list = (from item in source
						select new SchemaVO()
						{
							TABLE_NAME = item.TABLE_NAME
							,
							TABLE_DESC = item.TABLE_DESC
							,
							COLUMN_NAME = item.COLUMN_NAME
							,
							COLUMN_DESC = item.COLUMN_DESC
							,
							TYPE = Translator.CSharpType_To_MSSQL_Type(Convert.ToString(item.TYPE))
							,
							LENGTH = item.LENGTH
							,
							ISNULL = Convert.ToBoolean(item.ISNULL)
							,
							ISPK = CheckIsPrimaryKey(item.TABLE_NAME, item.COLUMN_NAME)
						}
				).ToList<SchemaVO>();

			return list;
		}

		/// <summary>
		/// 讀取DB取得schema
		/// </summary>
		/// <param name="connstr">連線字串</param>
		/// <returns></returns>
		public List<SchemaVO> CreateSchemaByDB(string connstr)
		{
			string dbname = ""; 
			string tablename = ""; 
			bool show_syscolum = true;
			return new SchemaDAO(connstr).QuerySchema(dbname, tablename, show_syscolum);
		}
		/// <summary>
		/// 讀取model取得schema
		/// </summary>
		/// <returns></returns>
		public List<SchemaVO> CreateSchemaByCode(string codefile = "")
		{
			throw new NullReferenceException();
		}
		/// <summary>
		/// 讀取文件取得schema
		/// </summary>
		/// <returns></returns>
		public List<SchemaVO> CreateSchemaByDocument(string tablename = "")
		{
			throw new NullReferenceException();
		}
		#endregion

		#region private function
		/// <summary>
		/// 檢查欄位是否為table的PrimaryKey
		/// </summary>
		/// <param name="tablename"></param>
		/// <param name="columnname"></param>
		/// <returns></returns>
		private bool CheckIsPrimaryKey(string tablename, string columnname)
		{
			return new SchemaDAO().GetPrimaryKey(tablename).Where(col => col.COLUMN_NAME == columnname).Count() > 0;
		}
		#endregion
	}
}
