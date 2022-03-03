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
		/// Creator初始化
		/// </summary>
		void Init();
		/// <summary>
		/// 建立範本
		/// </summary>
		void Create();
		/// <summary>
		/// 輸出範本
		/// </summary>
		void Output();
	}
}
