using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MoistureUpsetRemix.Common.Utils;

public static class DebugUtils
{
    extension<T>(IEnumerable<T> enumerable)
    {
        public string ToDebugString([CallerMemberName] string callerName = "", bool showIndex = true)
        {
            var msg = $"[{callerName}]\n";

            var count = 0;
            foreach (var item in enumerable)
            {
                if (showIndex)
                {
                    msg += $"[{count}]: ";
                    count++;
                }
                
                msg += item?.ToString();
                msg += '\n';
            }

            msg = msg.Trim();
            
            return msg;
        }
    }
}