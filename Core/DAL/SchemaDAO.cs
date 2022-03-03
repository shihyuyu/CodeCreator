using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCreator.Core
{
	public class SchemaDAO
	{
		public SchemaDAO()
		{
			DBHelper.ConnectionString = "";
		}

		public SchemaDAO(string connstr)
		{
			DBHelper.ConnectionString = connstr;
		}

		/// <summary>
		/// 取得db
		/// </summary>
		/// <returns></returns>
		public List<string> QueryDataBase()
		{
			string strSQL = " select name from sys.databases ";
			return DBHelper.Query<string>(strSQL);
		}
		/// <summary>
		/// 取得db所有table
		/// </summary>
		/// <param name="dbname"></param>
		/// <returns></returns>
		public List<string> QueryTable(string dbname = "")
		{
			string strSQL = "select name from sysobjects where xtype = 'U'";
			return DBHelper.Query<string>(strSQL, new { DB_NAME = dbname });
		}
		/// <summary>
		/// 取得db所有schema
		/// </summary>
		/// <param name="dbname"></param>
		/// <param name="tablename"></param>
		/// <param name="show_syscolum"></param>
		/// <returns></returns>
		public List<SchemaVO> QuerySchema(string dbname = "", string tablename = "", bool show_syscolum = true)
		{
			string strSQL = @" SELECT  a.TABLE_CATALOG AS [DB_NAME]
													   ,b.TABLE_NAME AS [TABLE_NAME]
													   ,b.COLUMN_NAME AS [COLUMN_NAME]
													   ,b.DATA_TYPE AS [TYPE]
													   ,b.CHARACTER_MAXIMUM_LENGTH AS [LENGTH]
													   ,(CASE WHEN b.IS_NULLABLE='YES' THEN 'true' WHEN b.IS_NULLABLE='NO' THEN 'false' ELSE NULL END ) AS [ISNULL]
													   ,b.COLUMN_DEFAULT AS [DEFAULT]
													   ,( SELECT value FROM fn_listextendedproperty (NULL, 'schema', 'dbo', 'table', a.TABLE_NAME, 'column', default)
															WHERE name='MS_Description'  and objtype='COLUMN'  
															and objname Collate Chinese_Taiwan_Stroke_CI_AS=b.COLUMN_NAME 
														 ) AS [COLUMN_DESC]
												FROM INFORMATION_SCHEMA.TABLES  a
												LEFT JOIN INFORMATION_SCHEMA.COLUMNS b ON (a.TABLE_NAME=b.TABLE_NAME) 
												WHERE TABLE_TYPE='BASE TABLE' ";
			strSQL += (!string.IsNullOrEmpty(dbname)) ? " AND a.TABLE_CATALOG=@DB_NAME " : "";
			strSQL += (!string.IsNullOrEmpty(tablename)) ? " AND b.TABLE_NAME=@TABLE_NAME " : "";
			strSQL += (!show_syscolum) ? " AND UPPER(b.COLUMN_NAME) NOT IN ('MASK', 'CREATEBY','REVISEBY','DELETEBY','CREATETIME','REVISETIME','DELETETIME','CREATEGRP','REVISEGRP','DELETEGRP') " : "";
			strSQL += "ORDER BY b.TABLE_NAME, b.ORDINAL_POSITION ";

			return DBHelper.Query<SchemaVO>(strSQL, new { DB_NAME = dbname, TABLE_NAME = tablename });
		}
		
		/// <summary>
		/// 以SP取得db
		/// </summary>
		/// <returns></returns>
		public void QueryDataBaseBySP()
		{
			// --列出所有資料庫
			string sp = "sp_helpdb";
			var ret = DBHelper.StoredProcedure(sp);
		}
		/// <summary>
		/// 以SP取得db所有table
		/// </summary>
		/// <param name="dbname"></param>
		/// <returns></returns>
		public void QueryTableBySP(string table_type)
		{
			//--列出特定資料庫下的資料表，table_qualifier為資料庫名稱， table_type為表格類型，寫法為"'TABLE'" 或 "'VIEW'"
			string sp = "sp_tables";
			var ret = DBHelper.StoredProcedure(sp, new { table_type = table_type });
		}
		/// <summary>
		/// 以SP取得db所有schema
		/// </summary>
		/// <param name="dbname"></param>
		/// <param name="tablename"></param>
		/// <param name="show_syscolum"></param>
		/// <returns></returns>
		public List<dynamic> QuerySchemaBySP(string tablename)
		{
			// --列出指定資料庫及資料表下的所有欄位
			string sp = "sp_columns";
			var ret = DBHelper.StoredProcedure(sp, new { table_name = tablename }).ToList();
			return ret;
		}

		/// <summary>
		/// 取得table的PrimaryKey
		/// </summary>
		/// <param name="tablename">表格</param>
		/// <returns></returns>
		public List<dynamic> GetPrimaryKey(string tablename)
		{
			string strSQL = "EXEC sp_pkeys @tablename ";
			return DBHelper.Query(strSQL, new { tablename }).ToList();
		}

	}
}
