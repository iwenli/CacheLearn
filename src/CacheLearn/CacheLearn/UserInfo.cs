using CacheLearn.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheLearn
{
    public class UserInfo
    {
        public long UserId { set; get; }
        public string UserName { set; get; }
        public string NickName { set; get; }
        public string HeadPic { set; get; }
        /// <summary>
        /// 0男 1女
        /// </summary>
        public int Sex { set; get; }

        public double Source { set; get; }
        public bool IsActivity { set; get; }


        public override string ToString()
        {
            return "用户id:{0},性别,{1},昵称{2}...".FormatWith(UserId, Sex == 0 ? "男" : "女", NickName);
        }

        public static void SetUser(UserInfo user)
        {
            try
            {
                //全部
                if (RedisHelper.Hash_Exist<UserInfo>("sales_user_all", "uid_" + user.UserId))
                {
                    RedisHelper.Hash_Remove("sales_user_all", "uid_" + user.UserId);
                }
                RedisHelper.Hash_Set<UserInfo>("sales_user_all", "uid_" + user.UserId, user);

                //按性别缓存
                if (RedisHelper.Hash_Exist<UserInfo>("sales_user_sex_" + user.Sex, "uid_" + user.UserId))
                {
                    RedisHelper.Hash_Remove("sales_user_sex_" + user.Sex, "uid_" + user.UserId);
                }
                RedisHelper.Hash_Set<UserInfo>("sales_user_sex_" + user.Sex, "uid_" + user.UserId, user);

                //分页用

                long isActivity = user.IsActivity ? 1 : 0;
                long longSource = (long)(user.Source * 100);
                var _source = isActivity << 56 | longSource;
                RedisHelper.SortedSet_Add("sales_user_source", user.UserId, _source);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 获取性别下的所有用户
        /// </summary>
        /// <param name="sex"></param>
        /// <returns></returns>
        public static List<UserInfo> GetUserBySex(int sex)
        {
            return RedisHelper.Hash_GetAll<UserInfo>("sales_user_sex_" + sex);
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        public static List<UserInfo> GetAllUser()
        {
            return RedisHelper.Hash_GetAll<UserInfo>("sales_user_all");
        }
    }
}
