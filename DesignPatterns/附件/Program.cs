using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

namespace 单例_懒汉式
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //LazyManSingle lazy = LazyManSingle.GetLazyManSingle();
            //LazyManSingle lazy1 = LazyManSingle.GetLazyManSingle();

            //Console.WriteLine(lazy.GetHashCode());
            //Console.WriteLine(lazy1.GetHashCode());

            // 懒汉式在单线程是可以的,多线程是不安全的

            //for (int i = 0; i < 10; i++)
            //{
            //    new Thread(() => LazyManSingle.GetLazyManSingle()).Start();
            //}

            // 1.通过单例创建对象
            //LazyManSingle lazy1 = LazyManSingle.GetLazyManSingle();

            // 2.通过反射来破坏单例 ---> 通过反射,来调用私有的构造函数

            Type t = Type.GetType("单例_懒汉式.LazyManSingle");

            // 3.获取私有的构造函数
            ConstructorInfo[] cons = t.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

            // 4.执行构造函数
            LazyManSingle lazy1 = (LazyManSingle)cons[0].Invoke(null);

            // 创建完成第一个对象后,在私有的构造函数中,标记位已经被改成了true,
            // 通过反射拿到私有的标记位字段,并将其改成false

            FieldInfo fieldInfo = t.GetField("isOk", BindingFlags.NonPublic | BindingFlags.Static);
            //给字段赋值
            fieldInfo.SetValue("isOk", false);

            LazyManSingle lazy2 = (LazyManSingle)cons[0].Invoke(null);

            Console.WriteLine(lazy1.GetHashCode());
            Console.WriteLine(lazy2.GetHashCode());

            Console.ReadKey();
        }
    }

    /// <summary>
    /// 懒汉式,需要时创建,不会造成内存资源的浪费
    /// </summary>
    public class LazyManSingle
    {

        private static bool isOk = false;

        // 1.私有化构造函数
        private LazyManSingle()
        {
            lock (o)
            {
                if (isOk == false)
                {
                    isOk = true;
                }
                else
                {
                    throw new Exception("不要试图通过反射来对我的程序搞破坏");
                }

                // 如果if被执行,证明有反射来搞破坏
                //if (lazy != null)
                //{
                //    throw new Exception("不要试图通过反射来对我的程序搞破坏");
                //}
            }
        }

        // 2.声明静态字段,存储为一的对象实例
        // private static LazyManSingle lazy = new LazyManSingle();
        private static LazyManSingle lazy;

        // 创建锁
        private static object o = new object();

        // 3.通过方法,创建对象,并返回
        public static LazyManSingle GetLazyManSingle()
        {
            // lazy = new LazyManSingle(); 调用就创建

            // 判断是否有类的实例,没有创建,有返回
            // 通过加锁,来解决多线程下,单例不安全的问题
            // lock C#语法糖
            // Monitor.Enter() Monitor.Exit() 互斥锁 用来解决多线程的安全问题 第一个来的拿钥匙,执行完收回供第二个线程使用
            if (lazy == null) // 双重锁,节省锁的资源消耗
            {
                lock (o)
                {
                    if (lazy == null)
                    {
                        lazy = new LazyManSingle();
                        // 输出次数测试懒汉是否正常
                        Console.WriteLine("我被创建了一次");
                    }
                }
            }

            return lazy;
        }
    }
}
