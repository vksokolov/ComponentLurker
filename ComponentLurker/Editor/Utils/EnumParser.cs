using System;
using System.Collections.Generic;
using System.Linq;

namespace ComponentLurker.Utils
{
    public static class EnumParser<TEnum> where TEnum : Enum
    {
        private static readonly Dictionary<string, TEnum> SerializedValues;
        
        public static IReadOnlyCollection<TEnum> GetValues() => SerializedValues.Values;

        static EnumParser()
        {
            SerializedValues = Enum.GetNames(typeof(TEnum))
                .ToDictionary(x => x,
                    x => (TEnum)Enum.Parse(typeof(TEnum), x), StringComparer.OrdinalIgnoreCase);
        }


        public static TEnum Convert(string value)
        {
            if (SerializedValues.TryGetValue(value, out var enumValue)) return enumValue;

            throw new Exception("Enum " + typeof(TEnum) + " doesn't have a case for value = " + value);
        }

        public static TEnum Convert(string value, TEnum defaultValue)
        {
            return SerializedValues.TryGetValue(value, out var enumValue) ? enumValue : defaultValue;
        }

        public static List<TEnum> Convert(IEnumerable<string> values)
        {
            return values.Select(Convert).ToList();
        }

        public static bool TryConvert(string value, out TEnum result)
        {
            if (SerializedValues.TryGetValue(value, out var enumValue))
            {
                result = enumValue;
                return true;
            }

            result = default;
            return false;
        }
    }
}