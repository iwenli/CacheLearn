using CacheLearn;
using CacheLearn.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Test1
{
    class Program
    {
        static Stopwatch stopwatch = new Stopwatch();

        static void Main(string[] args)
        {
            stopwatch.Restart();
            var _count = 100000;
            for (int i = 0; i < _count; i++)
            {
                var user = new UserInfo()
                {
                    UserId = i,
                    UserName = "名称" + Get(i),
                    HeadPic = "头像" + Get(i),
                    NickName = "昵称" + Get(i),
                    Sex = i % 2,
                    Source = 1,
                    IsActivity = i % 2 == 0
                };
                try
                {
                    UserInfo.SetUser(user);
                    System.Console.WriteLine("用户{0}缓存添加完成".FormatWith(user.UserId));
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("用户{0}缓存添加异常,异常：{1}".FormatWith(user.UserId, ex));
                    System.Console.ReadKey();
                    continue;
                }
            }
            System.Console.WriteLine("创建缓存完成,共计:{0}条，耗时{1}毫秒".FormatWith(_count),stopwatch.ElapsedMilliseconds);
            System.Console.ReadKey();
        }

        public static long Get(int i)
        {
            return 1 + i;
        }

    }
}
