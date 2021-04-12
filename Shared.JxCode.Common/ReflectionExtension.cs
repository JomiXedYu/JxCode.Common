using System;
using System.Collections.Generic;
using System.Reflection;

namespace JxCode.Common
{
    public static class ReflectionExtension
    {
        /// <summary>
        /// 成员是否为属性
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static bool IsProperty(this MemberInfo _this)
        {
            return _this.MemberType == MemberTypes.Property;
        }
        /// <summary>
        /// 成员是否为字段
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static bool IsField(this MemberInfo _this)
        {
            return _this.MemberType == MemberTypes.Field;
        }
        /// <summary>
        /// 成员是否为方法
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static bool IsMethod(this MemberInfo _this)
        {
            return _this.MemberType == MemberTypes.Method;
        }
        /// <summary>
        /// 成员方法是否为运算符重载方法
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static bool IsOperator(this MethodInfo _this)
        {
            //必须是静态方法，并且名字对应
            if (_this.MemberType == MemberTypes.Method && _this.IsStatic)
            {

                List<string> lst = new List<string>(8)
                {
                    "op_Addition", "op_Subtraction", "op_Multiply", "op_Division", "op_UnaryNegation",
                    "op_Equality", "op_Inequality", "op_Implicit"
                };
                return lst.Contains(_this.Name);
            }
            else
                return false;
        }

        /// <summary>
        /// 获取所有非属性，非操作符重载的公共方法。
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static MethodInfo[] GetUsableMethods(this Type _this)
        {
            List<MethodInfo> mis = new List<MethodInfo>();
            MethodInfo[] methods = _this.GetMethods();
            foreach (MethodInfo item in methods)
            {
                if (item.IsProperty() || item.IsOperator()) continue;
                mis.Add(item);
            }
            return mis.ToArray();
        }
        /// <summary>
        /// 获取当前运行的程序集中所有标记为该特性的公用类型
        /// </summary>
        /// <param name="ass"></param>
        /// <param name="attrib"></param>
        /// <returns></returns>
        public static Type[] GetClassTypeByAttribute(Assembly ass, Type attrib)
        {
            Type[] ts = ass.GetTypes();
            List<Type> rst = new List<Type>();
            foreach (Type item in ts)
            {
                if (Attribute.IsDefined(item, attrib)) rst.Add(item);
            }
            return rst.ToArray();
        }
        /// <summary>
        /// 获取当前运行的程序集中所有标记为该特性的公用方法
        /// </summary>
        /// <param name="ass"></param>
        /// <param name="attrib"></param>
        /// <returns></returns>
        public static MethodInfo[] GetMethodByAttribute(Assembly ass, Type attrib)
        {
            Type[] ts = ass.GetTypes();
            List<MethodInfo> rst = new List<MethodInfo>();
            foreach (Type item in ts)
            {
                foreach (MethodInfo mi in item.GetMethods())
                {
                    if (Attribute.IsDefined(mi, attrib))
                        rst.Add(mi);
                }
            }
            return rst.ToArray();
        }
        /// <summary>
        /// 获取程序集中所有继承于某类型的公共类型
        /// </summary>
        /// <param name="ass"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static Type[] GetClassTypeByBase(Assembly ass, Type baseType)
        {
            Type[] ts = ass.GetTypes();
            List<Type> rst = new List<Type>();
            foreach (Type item in ts)
            {
                if (item.IsSubclassOf(baseType))
                {
                    rst.Add(item);
                }
            }
            return rst.ToArray();
        }
    }
}
