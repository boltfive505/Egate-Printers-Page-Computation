using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Printers_Page_Computation
{
    public static class EnumExt
    {
        public static TEnum ToEnum<TEnum>(this string value) where TEnum : struct
        {
            return EnumExt.ToEnum(value, default(TEnum));
            //TEnum e = default(TEnum);
            //Enum.TryParse(value, true, out e);
            //return e;
        }

        public static TEnum ToEnum<TEnum>(this string value, TEnum defaultValue) where TEnum : struct
        {
            TEnum e = default(TEnum);
            bool flag = Enum.TryParse(value, true, out e);
            return flag ? e : defaultValue;
        }

        public static TEnum ToEnum<TEnum>(this long value) where TEnum : struct
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), value);
        }

        public static TEnum ToEnum<TEnum>(this int value) where TEnum : struct
        {
            //Enum.GetValues
            return (TEnum)Enum.ToObject(typeof(TEnum), value);
        }

        public static IEnumerable GetValuesExcept(Type enumType, params object[] exceptions)
        {
            return Enum.GetValues(enumType).OfType<object>().Except(exceptions);
        }
    }
}
