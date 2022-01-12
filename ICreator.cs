using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCreator
{
	public interface ICreator
	{
		/// <summary>
		/// 建立類別
		/// </summary>
		void Create();
		/// <summary>
		/// 輸出檔案
		/// </summary>
		void Output();
	}
}
