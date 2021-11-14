/*
 * @Moudule     : ReflectionExtension
 * @Date        : 2020/12/04
 * @Author      : jx
 * @Link        : https://github.com/JomiXedYu
 * @Description : 
 */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace JxCode.Common
{
    public static class ReflectionExtension
    {
        static readonly List<string> operatorList = new List<string>()
        {
            "op_Addition", "op_Subtraction", "op_Multiply", "op_Division", "op_UnaryNegation",
            "op_Equality", "op_Inequality", "op_Implicit"
        };

        /// <summary>
        /// 成员方法是否为运算符重载方法
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static bool IsOperatorMethod(this MethodInfo _this)
        {
            //必须是静态方法，并且名字对应
            if (_this.MemberType == MemberTypes.Method && _this.IsStatic)
            {
                return operatorList.Contains(_this.Name);
            }
            else
                return false;
        }

        public static bool IsPropertyMethod(this MethodInfo _this)
        {
            return _this.Name.StartsWith("get_") || _this.Name.StartsWith("set_");
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
                if (item.IsOperatorMethod() || item.IsPropertyMethod())
                {
                    continue;
                }
                mis.Add(item);
            }
            return mis.ToArray();
        }

        /// <summary>
        /// 获取程序集中所有静态类中标记为该特性的静态方法
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MethodInfo[] GetMethodByAttribute(this Assembly assembly, Type attribute)
        {
            if (assembly == null) assembly = Assembly.GetCallingAssembly();

            Type[] ts = assembly.GetTypes();
            List<MethodInfo> rst = new List<MethodInfo>();
            foreach (Type _class in ts)
            {
                if (_class.IsAbstract && _class.IsSealed)
                {
                    foreach (MethodInfo mi in _class.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        if (Attribute.IsDefined(mi, attribute))
                            rst.Add(mi);
                    }
                }
            }
            return rst.ToArray();
        }

        /// <summary>
        /// 获取程序集中所有标记某特性的类型
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static Type[] GetClassTypeByAttribute(this Assembly assembly, Type attribute)
        {
            if (assembly == null) assembly = Assembly.GetCallingAssembly();
            Type[] ts = assembly.GetTypes();
            List<Type> rst = new List<Type>();
            foreach (Type item in ts)
            {
                if (item.IsDefined(attribute, true))
                {
                    rst.Add(item);
                }
            }
            return rst.ToArray();
        }

        /// <summary>
        /// 获取程序集中所有继承于某类型的类型
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static Type[] GetClassTypeByBase(this Assembly assembly, Type baseType)
        {
            if (assembly == null) assembly = Assembly.GetCallingAssembly();

            Type[] ts = assembly.GetTypes();
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
