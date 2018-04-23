using CacheLearn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadTest
{
    class Program
    {
        static void Main(string[] args)
        {

            while (true)
            {
                try
                {
                    //读取所有用户
                    var _list = UserInfo.GetAllUser();
                    System.Console.WriteLine("读取所有用户成功：{0}条".FormatWith(_list.Count));
                    if (_list != null)
                    {
                        foreach (var user in _list)
                        {
                            System.Console.WriteLine(user.ToString());
                        }
                    }

                    //读取性别下用户
                    _list.Clear();
                    _list = UserInfo.GetUserBySex(0);
                    System.Console.WriteLine();
                    System.Console.WriteLine("读取男性用户成功：{0}条".FormatWith(_list?.Count ?? 0));
                    if (_list != null)
                    {
                        foreach (var user in _list)
                        {
                            System.Console.WriteLine(user.ToString());
                        }
                    }

                    _list.Clear();
                    _list = UserInfo.GetUserBySex(1);
                    System.Console.WriteLine();
                    System.Console.WriteLine("读取女性用户成功：{0}条".FormatWith(_list?.Count ?? 0));
                    if (_list != null)
                    {
                        foreach (var user in _list)
                        {
                            System.Console.WriteLine(user.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex);
                }
                System.Console.ReadKey();
            }
        }


    }
}
