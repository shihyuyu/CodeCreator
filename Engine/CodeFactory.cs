using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeCreator.Core;

namespace CodeCreator
{
	public class CodeFactory
	{
		/// <summary>
		/// 類別設定完成控制點
		/// </summary>
		private bool _IsFactorySettingSuccess = false;
		/// <summary>
		/// 讀取的範本文字內容
		/// </summary>
		private string _TemplateFileBody = "";
		/// <summary>
		/// 輸出檔案名稱
		/// </summary>
		private string _OuputFileName = "";
		/// <summary>
		/// 程式工廠model
		/// </summary>
		public class CodeFactoryModel
		{
			/// <summary>
			/// 
			/// </summary>
			public string Title = "";
			/// <summary>
			/// 
			/// </summary>
			public string Body = "";
		}
		/// <summary>
		/// 程式工廠model清單
		/// </summary>
		public List<CodeFactoryModel> CodeFactoryModelList = new List<CodeFactoryModel>();

		public CodeFactory(string FactoryType)
		{
			string TemplateFileName = "";
			switch (FactoryType)
			{
				case TemplateCode.Code_CSharp_BussinessObject:
					TemplateFileName = "BOTemplate.txt";
					_OuputFileName = "{#TemplateName#}BO.cs";
					break;
				case TemplateCode.Code_CSharp_Controller:
					TemplateFileName = "ControllerTemplate.txt";
					_OuputFileName = "{#TemplateName#}Controller.cs";
					break;
				case TemplateCode.Code_CSharp_DataAccessObject:
					TemplateFileName = "DAOTemplate.txt";
					_OuputFileName = "{#TemplateName#}DAO.cs";
					break;
				case TemplateCode.Code_CSharp_Model:
					TemplateFileName = "ModelTemplate.txt";
					_OuputFileName = "{#TemplateName#}.cs";
					break;
				case TemplateCode.Code_CSharp_ValueObject:
					TemplateFileName = "VOTemplate.txt";
					_OuputFileName = "{#TemplateName#}VO.cs";
					break;
				case TemplateCode.DB_MSSQL_DDL:
					TemplateFileName = "CreateTableTemplate.txt";
					_OuputFileName = "{#TemplateName#}_CreateTable.sql";
					break;
				case TemplateCode.DB_MSSQL_DML:
					TemplateFileName = "";
					break;
				case TemplateCode.DB_MSSQL_DQL:
					TemplateFileName = "";
					break;
				default:
					TemplateFileName = "";
					break;
			}

			// 初始化ModelList資料
			CodeFactoryModelList = new List<CodeFactoryModel>();
			// 是否已設定工廠
			_IsFactorySettingSuccess = (!string.IsNullOrEmpty(TemplateFileName));
			// 讀取範本檔案
			_TemplateFileBody = FileHelper.GetTemplate(TemplateFileName);
		}

		/// <summary>
		/// 檔案輸出
		/// </summary>
		public void Print(string DirPath = "")
		{
			foreach (var item in CodeFactoryModelList)
			{
				string dir = (!string.IsNullOrEmpty(DirPath)) ? DirPath + "\\" : "";
				FileHelper.WriteFile($"D:\\{ dir }{ _OuputFileName.Replace("{#TemplateName#}", item.Title) }", item.Body);
			}
		}
		/// <summary>
		/// 建一個Model的程式碼
		/// </summary>
		/// <param name="tb"></param>
		/// <param name="colList">
		/// colList.Name 欄位名稱
		/// colList.Description 欄位描述
		/// colList.Type 欄位型別
		/// </param>
		/// <returns></returns>
		public string CreateModel(string tb, List<SchemaVO> colList)
		{
			// 檢核
			if (!_IsFactorySettingSuccess) throw new Exception("未指定CodeFactory類別，無法建立程式碼");

			// 產生內容
			string temp = "/// <summary>" + "\r\n\t\t" +
										"/// {#Description#}" + "\r\n\t\t" +
										"/// </summary>" + "\r\n\t\t" +
										"public {#Type#} {#Name#} { get; set; }" + "\r\n\t\t";

			string code = "";
			foreach (var col in colList)
			{
				code += temp.Replace("{#Name#}", col.COLUMN_NAME).Replace("{#Description#}", col.COLUMN_DESC).Replace("{#Type#}", col.TYPE);
			}

			string templateCode = _TemplateFileBody.Replace("{#ClassName#}", tb).Replace("{#PropertyName#}", code).Replace("{#PublicMethodName#}", "").Replace("{#PrivateMethodName#}", "");

			return templateCode;
		}
		/// <summary>
		/// 建多個Model的程式碼
		/// </summary>
		/// <returns></returns>
		public List<CodeFactoryModel> CreateModels()
		{
			// 檢核
			if (!_IsFactorySettingSuccess) throw new Exception("未指定CodeFactory類別，無法建立程式碼");

			List<CodeFactoryModel> models = new List<CodeFactoryModel>();
			// 讀取資料庫格式
			var data = new TemplateCode().GetSchema(TemplateCode.SchemaSource.DataBase, TemplateCode.SchemaSource.Code);

			var tblist = (from tb in data select tb.TABLE_NAME).Distinct();
			foreach (var tb in tblist)
			{
				var colList = data.Where(item => item.TABLE_NAME == Convert.ToString(tb)).ToList();
				models.Add(new CodeFactoryModel() { Title = tb, Body = CreateModel(tb, colList) });
			}

			CodeFactoryModelList = models;

			return models;
		}
		/// <summary>
		/// 建一個BO的程式碼
		/// </summary>
		/// <param name="tb"></param>
		/// <param name="colList">
		/// colList.Name 欄位名稱
		/// colList.Description 欄位描述
		/// colList.Type 欄位型別
		/// </param>
		/// <returns></returns>
		public string CreateBO(string tb, List<SchemaVO> colList)
		{
			// 檢核
			if (!_IsFactorySettingSuccess) throw new Exception("未指定CodeFactory類別，無法建立程式碼");

			string ClassName = tb;
			string ModelName = tb;
			string tablename = tb;
			string insert_column = "";
			string insert_parameter = "";
			string where_condition = "";
			string primarykey = "";
			string update_column = "";

			foreach (var col in colList)
			{
				insert_column += $", {col.COLUMN_NAME} ";
				insert_parameter += $", @{col.COLUMN_NAME} ";

				if (col.ISPK)
				{
					primarykey += $"and {col.COLUMN_NAME}=@{col.COLUMN_NAME}";
				}
				else
				{
					update_column += $", {col.COLUMN_NAME}=@{col.COLUMN_NAME} ";
					where_condition += $"and {col.COLUMN_NAME}=@{col.COLUMN_NAME} ";
				}
			}

			primarykey = (primarykey.Length>3) ? primarykey.Substring(3) : primarykey;
			insert_column = (insert_column.Length>0) ? insert_column.Substring(1) : insert_column;
			insert_parameter = (insert_parameter.Length > 0) ? insert_parameter.Substring(1) : insert_parameter;
			where_condition = (where_condition.Length > 0) ? where_condition.Substring(3) : where_condition;
			update_column = (update_column.Length > 0) ? update_column.Substring(1) : update_column;

			string templateCode = _TemplateFileBody.Replace("{#ClassName#}", ClassName)
				.Replace("{#ModelName#}", ModelName)
				.Replace("{#tablename#}", tablename)
				.Replace("{#primarykey#}", primarykey)
				.Replace("{#where condition#}", where_condition)
				.Replace("{#insert column#}", insert_column)
				.Replace("{#insert parameter#}", insert_parameter)
				.Replace("{#update column#}", update_column);

			return templateCode;
		}
		/// <summary>
		/// 建多個BO的程式碼
		/// </summary>
		/// <returns></returns>
		public List<CodeFactoryModel> CreateBOs()
		{
			// 檢核
			if (!_IsFactorySettingSuccess) throw new Exception("未指定CodeFactory類別，無法建立程式碼");

			List<CodeFactoryModel> models = new List<CodeFactoryModel>();
			// 讀取資料庫格式
			var data = new TemplateCode().GetSchema(TemplateCode.SchemaSource.DataBase, TemplateCode.SchemaSource.Code);

			var tblist = (from tb in data select tb.TABLE_NAME).Distinct();
			foreach (var tb in tblist)
			{
				var colList = data.Where(item => item.TABLE_NAME == Convert.ToString(tb)).ToList();
				models.Add(new CodeFactoryModel() { Title = tb, Body = CreateBO(tb, colList) });
			}
			// 存到Temp資料
			CodeFactoryModelList = models;

			return models;
		}
		/// <summary>
		/// 建一個MVC_Controller的程式碼
		/// </summary>
		/// <param name="tb"></param>
		/// <param name="colList">
		/// colList.Name 欄位名稱
		/// colList.Description 欄位描述
		/// colList.Type 欄位型別
		/// </param>
		/// <returns></returns>
		public string CreateMVC_Controller(string tb, List<SchemaVO> colList)
		{
			// 檢核
			if (!_IsFactorySettingSuccess) throw new Exception("未指定CodeFactory類別，無法建立程式碼");

			string ClassName = tb;
			string ModelName = tb;

			string templateCode = _TemplateFileBody.Replace("{#ClassName#}", ClassName)
				.Replace("{#ModelName#}", ModelName);

			return templateCode;
		}
		/// <summary>
		/// 建多個MVC_Controller的程式碼
		/// </summary>
		/// <returns></returns>
		public List<CodeFactoryModel> CreateMVC_Controllers()
		{
			// 檢核
			if (!_IsFactorySettingSuccess) throw new Exception("未指定CodeFactory類別，無法建立程式碼");

			List<CodeFactoryModel> models = new List<CodeFactoryModel>();
			// 讀取資料庫格式
			var data = new TemplateCode().GetSchema(TemplateCode.SchemaSource.DataBase, TemplateCode.SchemaSource.Code);

			var tblist = (from tb in data select tb.TABLE_NAME).Distinct();
			foreach (var tb in tblist)
			{
				var colList = data.Where(item => item.TABLE_NAME == Convert.ToString(tb)).ToList();
				models.Add(new CodeFactoryModel() { Title = tb, Body = CreateBO(tb, colList) });
			}
			// 存到Temp資料
			CodeFactoryModelList = models;

			return models;
		}
		/// <summary>
		/// 建一個CreateTableSQL的程式碼
		/// </summary>
		/// <param name="tb"></param>
		/// <param name="colList">
		/// colList.Name 欄位名稱
		/// colList.Description 欄位描述
		/// colList.Type 欄位型別
		/// </param>
		/// <returns></returns>
		public string CreateTableSQL(string tb, List<SchemaVO> colList)
		{
			// 檢核
			if (!_IsFactorySettingSuccess) throw new Exception("未指定CodeFactory類別，無法建立程式碼");

			string TableName = tb;
			string pk_column = "";
			string column = "";
			string constraint = "";
			foreach (var col in colList)
			{
				if (col.ISPK)
				{
					column += $"[{ col.COLUMN_NAME }] [{ col.TYPE }]({ col.LENGTH }) NOT NULL, " + "\r\n\t";
					pk_column += $", [{ col.COLUMN_NAME }] ASC";
				}
				else
				{
					if (col.TYPE == "INT" || col.TYPE == "TINYINT" || col.TYPE == "FLOAT" || col.TYPE == "DATETIME" || col.TYPE == "BIGINT" || col.TYPE == "BIT")
					{
						// 不需要設定長度類型
						column += $"[{ col.COLUMN_NAME }] [{ col.TYPE }] { (col.ISNULL ? "NULL" : "NOT NULL") }, " + "\r\n\t";
					}
					else
					{
						column += $"[{ col.COLUMN_NAME }] [{ col.TYPE }]({ col.LENGTH }) { (col.ISNULL ? "NULL" : "NOT NULL") }, " + "\r\n\t";
					}
						
				}

				constraint += $"ALTER TABLE [dbo].[{ col.TABLE_NAME }] ADD  CONSTRAINT [DF_{ col.TABLE_NAME }_{ col.COLUMN_NAME }]  DEFAULT ({ "''" }) FOR [{ col.COLUMN_NAME }] " + "\r\n" + "GO" + "\r\n";
			}

			pk_column = (pk_column.Length > 0) ? pk_column.Substring(1) : pk_column;

			string templateCode = _TemplateFileBody.Replace("{#DBName#}", "SalesMapV2")
				.Replace("{#TableName#}", TableName)
				.Replace("{#CreateTime#}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
				.Replace("{#ColumnName#}", column)
				.Replace("{#CONSTRAINT#}", constraint)
				.Replace("{#ColumnPKName#}", pk_column);

			return templateCode;
		}
		/// <summary>
		/// 建多個CreateTableSQLs的程式碼
		/// </summary>
		/// <returns></returns>
		public List<CodeFactoryModel> CreateTableSQLs()
		{
			// 檢核
			if (!_IsFactorySettingSuccess) throw new Exception("未指定CodeFactory類別，無法建立程式碼");

			List<CodeFactoryModel> models = new List<CodeFactoryModel>();
			// 讀取資料庫格式
			//var data = new TemplateCode().GetSchema(TemplateCode.SchemaSource.DataBase);
			var data = new TemplateCode().GetSchema(TemplateCode.SchemaSource.DataBase, TemplateCode.SchemaSource.DataBase);

			var tblist = (from tb in data select tb.TABLE_NAME).Distinct();
			foreach (var tb in tblist)
			{
				var colList = data.Where(item => item.TABLE_NAME == Convert.ToString(tb)).ToList();
				models.Add(new CodeFactoryModel() { Title = tb, Body = CreateTableSQL(tb, colList) });
			}
			// 存到Temp資料
			CodeFactoryModelList = models;

			return models;
		}
	}
}
