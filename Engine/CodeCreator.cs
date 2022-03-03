using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCreator
{
    public class CodeCreator : ICreator
    {

		public void Init()
		{
		}

		public void Create()
        {
			CodeFactory factory = new CodeFactory(TemplateCode.Code_CSharp_Model);
			var li = factory.CreateModels();
			factory.Print();
		}
		public void Output()
		{
			throw new NotImplementedException();
		}

	}
}
