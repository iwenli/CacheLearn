/*----------------------------------------------------------------
 *  Copyright (C) 2017 天下商机（txooo.com）版权所有
 * 
 *  文 件 名：DataCache
 *  所属项目：
 *  创建用户：张玉龙(HouWeiya)
 *  创建时间：2018/4/20 15:54:39
 *  
 *  功能描述：
 *          1、
 *          2、 
 * 
 *  修改标识：  
 *  修改描述：
 *  待 完 善：
 *          1、 
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheLearn.Cache
{
    public class DataCache
    {
        private static IDataCache _instance = null;
        /// <summary>  
        /// 静态实例，外部可直接调用  
        /// </summary>  
        public static IDataCache Instance
        {
            get
            {
                if (_instance == null) _instance = new RuntimeCache();//这里可以改变缓存类型  
                return _instance;
            }
        }
    }
}
