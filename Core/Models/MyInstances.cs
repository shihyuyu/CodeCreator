using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCreator
{
	public class MyInstances
	{
		/// <summary>
		/// 名稱
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 描述
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// 屬性
		/// </summary>
		public List<MyProperties> properties;
	}
}