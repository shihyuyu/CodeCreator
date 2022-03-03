using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeCreator
{
	public class ReflactionHelper
	{
		private static Assembly _assembly = null;
        private static Type _type = null;

		public static void Load(string path)
		{
            _assembly = Assembly.LoadFile(path);
            _type = _assembly.GetType();
        }
        /// <summary>
        /// 獲取程序集所有類型
        /// </summary>
        public static Type[] Types { get { return _assembly.GetTypes(); } }
        /// <summary>
        /// 獲取所有方法
        /// </summary>
        public static MethodInfo[] Methods { get { return _type.GetMethods(); } }
        /// <summary>
        /// 獲取所有屬性
        /// </summary>
        public static PropertyInfo[] Propertys { get { return _type.GetProperties(); } }
        /// <summary>
        /// 獲取所有字段
        /// </summary>
        public static FieldInfo[] Fields { get { return _type.GetFields(); } }


        /// <summary>
        /// 建立實體
        /// </summary>
        /// <returns></returns>
        public T CreateInstance<T>(string type)
        {
            // 獲取類型
            Type dalType = _assembly.GetType(type);
            // 創建對象
            return (T)Activator.CreateInstance(dalType);
        }
        public T CreateGenericInstance<T>(string type)
        {
            // 獲取類型
            Type dalType = _assembly.GetType(type);
            // 獲取泛型類型
            var newType = dalType.MakeGenericType(typeof(T));
            // 創建泛型對象
            return (T)Activator.CreateInstance(newType);
        }

        public Type GetType(string TypeName) 
        {
            return _assembly.GetType(TypeName);
        }
        public MethodInfo GetMethodInfo(string MethodName)
        {
            return _type.GetMethod(MethodName);
        }
        public PropertyInfo GetPropertyInfo(string PropertyName)
        {
            return _type.GetProperty(PropertyName);
        }
        public FieldInfo GetFieldInfo(string FieldName)
        {
            return _type.GetField(FieldName);
        }
        public Attribute GetAttribute(Type AttributeName)
        {
            //Type AttributeName = typeof(DisplayNameAttribute)
            return _type.GetCustomAttribute(AttributeName);
        }
    }
}
