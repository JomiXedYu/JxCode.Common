using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace JxCode.Common
{
    public class DynamicType
    {
        public static T New<T>(Dictionary<string, Delegate> methods)
        {
            Type t = typeof(T);

            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("DynamicAssembly." + typeof(T).FullName),
                AssemblyBuilderAccess.Run);

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");

            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                name: "new" + t.Name,
                attr: TypeAttributes.Class | TypeAttributes.Public,
                parent: null,
                interfaces: new Type[] { t });

            //把匿名方法表存起来
            FieldBuilder fieldBuilder = typeBuilder.DefineField(
                fieldName: "_methodTable",
                type: typeof(Dictionary<string, Delegate>), 
                attributes: FieldAttributes.Private);


            MethodInfo[] methodInfos = t.GetMethods();

            for (int i = 0; i < methodInfos.Length; i++)
            {
                MethodInfo methodInfo = methodInfos[i];

                Type[] paramTypes = ParamInfoToTypes(methodInfo);

                MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                    name: methodInfo.Name,
                    attributes: MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual,
                    returnType: methodInfo.ReturnType,
                    parameterTypes: paramTypes);

                ILGenerator il = methodBuilder.GetILGenerator();

                //开始方法调用
                il.BeginScope();
                //实现接口的肯定是实例类，先加载this
                il.Emit(OpCodes.Ldarg_0);
                //字典对象和key压入栈
                il.Emit(OpCodes.Ldfld, fieldBuilder);
                il.Emit(OpCodes.Ldstr, methodInfo.Name);
                //获取的匿名方法压入栈
                il.Emit(OpCodes.Callvirt, typeof(Dictionary<string, Delegate>).GetMethod("get_Item"));
                //对应的实现
                Delegate @delegate = methods[methodInfo.Name];
                //调用委托的Invoke方法，需要在栈上加载委托对象和参数集 //因为委托实例中包含目标对象所以不用管
                //压入所有形参
                for (int j = 0; j < paramTypes.Length; j++)
                {
                    il.Emit(OpCodes.Ldarg_S, j + 1);
                }
                //调用委托
                il.Emit(OpCodes.Callvirt, @delegate.GetType().GetMethod("Invoke"));
                il.Emit(OpCodes.Ret);
                il.EndScope();
            }

            Type rtnType = typeBuilder.CreateType();
            object obj = Activator.CreateInstance(rtnType);
            //设置实例的方法表
            obj.GetType().GetField("_methodTable", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(obj, methods);
            return (T)obj;
        }

        public static Type[] ParamInfoToTypes(MethodInfo methodInfo)
        {
            ParameterInfo[] paramInfos = methodInfo.GetParameters();
            Type[] paramTypes = new Type[paramInfos.Length];
            for (int i = 0; i < paramInfos.Length; i++)
            {
                paramTypes[i] = paramInfos[i].ParameterType;
            }
            return paramTypes;
        }
    }
}
