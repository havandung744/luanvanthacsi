using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace luanvanthacsi.Data.Extentions
{
    public static class ObjectExtentions
    {
        public static T As<T>(this object source)
        {
            if (source == null)
            {
                return default;
            }
            var result = Activator.CreateInstance<T>();
            foreach (var item in typeof(T).GetProperties())
            {
                if (source.GetType().GetProperty(item.Name) != null)
                    item.SetValue(result, source.GetType().GetProperty(item.Name).GetValue(source) ?? null);
            }
            return result;
        }

        public static void Update(this object targetObject, object sourceObject, Dictionary<string, string> propertyFields = null)
        {
            if (propertyFields == null)
            {
                foreach (var item in targetObject.GetType().GetProperties())
                {
                    var info2 = sourceObject.GetType().GetProperty(item.Name);
                    if (info2 != null && item.PropertyType == info2.PropertyType)
                    {
                        item.SetValue(targetObject, info2.GetValue(sourceObject) ?? null);
                    }
                }
            }
            else
            {
                Type targetType = targetObject.GetType();
                Type sourceType = sourceObject.GetType();
                foreach (var item in propertyFields)
                {
                    var targetProperty = targetType.GetProperty(item.Key);
                    if (targetProperty != null)
                    {
                        var sourceProperty = sourceType.GetProperty(item.Value);
                        if (sourceProperty != null && targetProperty.PropertyType == sourceProperty.PropertyType)
                        {
                            try
                            {
                                var value = sourceProperty.GetValue(sourceObject);
                                targetProperty.SetValue(targetObject, value);
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }
                    }
                }
            }
        }

        public static void UpdateIgnoreNull(this object targetObject, object sourceObject, Dictionary<string, string> propertyFields = null)
        {
            if (propertyFields == null)
            {
                foreach (var item in targetObject.GetType().GetProperties())
                {
                    var info2 = sourceObject.GetType().GetProperty(item.Name);
                    if (info2 != null && item.PropertyType == info2.PropertyType)
                    {
                        var value = info2.GetValue(sourceObject);
                        if (value != null)
                        {
                            item.SetValue(targetObject, value);
                        }
                    }
                }
            }
            else
            {
                Type targetType = targetObject.GetType();
                Type sourceType = sourceObject.GetType();
                foreach (var item in propertyFields)
                {
                    var targetProperty = targetType.GetProperty(item.Key);
                    if (targetProperty != null)
                    {
                        var sourceProperty = sourceType.GetProperty(item.Value);
                        if (sourceProperty != null && targetProperty.PropertyType == sourceProperty.PropertyType)
                        {
                            try
                            {
                                var value = sourceProperty.GetValue(sourceObject);
                                if (value != null)
                                {
                                    targetProperty.SetValue(targetObject, value);
                                }
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }
                    }
                }
            }
        }

        public static void Trim(this object targetObject)
        {
            try
            {
                foreach (var item in targetObject.GetType().GetProperties().Where(c => c.PropertyType == typeof(string)))
                {
                    var currentValue = item.GetValue(targetObject) as string;
                    if (currentValue.IsNotNullOrEmpty() && (currentValue[0] == ' ' || currentValue[currentValue.Length - 1] == ' '))
                    {
                        item.SetValue(targetObject, currentValue.Trim());
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// EncodeHtml object cho object
        /// </summary>
        /// <param name="targetObject"></param>
        public static T EncodeHtml<T>(T targetObject)
        {
            try
            {
                T tempObject = (T)Activator.CreateInstance(targetObject.GetType());
                tempObject.Update(targetObject);
                foreach (var item in tempObject.GetType().GetProperties().Where(c => c.PropertyType == typeof(string)))
                {
                    var currentValue = item.GetValue(tempObject) as string;
                    if (currentValue.IsNotNullOrEmpty())
                    {
                        item.SetValue(tempObject, HttpUtility.HtmlEncode(currentValue));
                    }
                }
                return tempObject;
            }
            catch (Exception)
            {
                return targetObject;
            }
        }

        /// <summary>
        /// EncodeHtml object cho List va object
        /// </summary>
        /// <param name="targetObject"></param>
        public static List<T> EncodeHtml<T>(List<T> targetObject)
        {
            try
            {
                List<T> tempObject = new List<T>();
                foreach (var item in targetObject as System.Collections.IList)
                {
                    tempObject.Add(EncodeHtml(item).As<T>());
                }
                return tempObject;
            }
            catch (Exception)
            {
                return targetObject;
            }
        }

        public static void SetValue<T>(this T obj, string nameProperty, object value)
        {
            try
            {
                var info = obj.GetType().GetProperty(nameProperty);
                CultureInfo culture1 = CultureInfo.CreateSpecificCulture("vi-VN");
                DateTime dateTime;
                if (value is DBNull)
                {
                    return;
                }
                if (info != null)
                {
                    if (info.PropertyType == typeof(int))
                    {
                        value = new Regex(@"[^\d]+").Replace(value.ToString(), "");
                        info.SetValue(obj, Convert.ToInt32(value));
                    }
                    else if (info.PropertyType == typeof(int?))
                    {
                        if (value == null || value?.ToString() == "null" || value?.ToString() == "")
                        {
                            info.SetValue(obj, null);
                        }
                        else
                        {
                            value = new Regex(@"[^\d]+").Replace(value.ToString(), "");
                            info.SetValue(obj, Convert.ToInt32(value));
                        }
                    }
                    else if (info.PropertyType == typeof(decimal))
                    {
                        info.SetValue(obj, Convert.ToDecimal(value));
                    }
                    else if (info.PropertyType == typeof(decimal?))
                    {
                        if (value.IsNullOrEmpty() || value?.ToString() == "null")
                        {
                            info.SetValue(obj, null);
                        }
                        else
                        {
                            info.SetValue(obj, Convert.ToDecimal(value));
                        }
                    }
                    //else if (info.PropertyType == typeof(DateTime))
                    //{
                    //    DateTime.TryParseExact(value.ToString(), "dd/MM/yyyy", culture1, DateTimeStyles.None, out dateTime);
                    //    info.SetValue(obj, dateTime);
                    //}
                    else if (info.PropertyType == typeof(DateTime?))
                    {
                        if (value.IsNullOrEmpty() || value?.ToString() == "null")
                        {
                            info.SetValue(obj, null);
                        }
                        else
                        {
                            DateTime.TryParse(value.ToString(), culture1, DateTimeStyles.None, out dateTime);
                            info.SetValue(obj, dateTime);
                        }
                    }
                    else if (info.PropertyType == typeof(bool))
                    {
                        info.SetValue(obj, Convert.ToBoolean(value));
                    }
                    else if (info.PropertyType == typeof(bool?))
                    {
                        if (value.IsNotNullOrEmpty())
                        {
                            info.SetValue(obj, Convert.ToBoolean(value.ToString()));
                        }
                        else
                        {
                            info.SetValue(obj, null);
                        }
                    }
                    else if (info.PropertyType.IsEnum)
                    {
                        if (Enum.TryParse(info.PropertyType, value?.ToString(), true, out object convertValue))
                        {
                            info.SetValue(obj, convertValue);
                        }

                    }
                    else
                    {
                        info.SetValue(obj, value);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static object GetValue<T>(this T obj, string nameProperty)
        {
            var info = obj.GetType().GetProperty(nameProperty);
            if (info == null)
            {
                return null;
            }
            return info.GetValue(obj);
        }

        public static object GetValueField(this Type obj, string fieldName)
        {
            var info = obj.GetField(fieldName);
            if (info == null)
            {
                return null;
            }
            return info.GetValue(obj);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static void Add<T>(this IList<PropertyInfo> propertyInfos, Expression<Func<T, bool>> property)
        {
            propertyInfos.Add<T, bool>(property);
        }
        public static void Add<T>(this IList<PropertyInfo> propertyInfos, Expression<Func<T, string>> property)
        {
            propertyInfos.Add<T, string>(property);
        }
        public static void Add<T>(this IList<PropertyInfo> propertyInfos, Expression<Func<T, int>> property)
        {
            propertyInfos.Add<T, int>(property);
        }
        public static void Add<T>(this IList<PropertyInfo> propertyInfos, Expression<Func<T, decimal>> property)
        {
            propertyInfos.Add<T, decimal>(property);
        }
        public static void Add<T>(this IList<PropertyInfo> propertyInfos, Expression<Func<T, decimal?>> property)
        {
            propertyInfos.Add<T, decimal?>(property);
        }
        public static void Add<T>(this IList<PropertyInfo> propertyInfos, Expression<Func<T, DateTime?>> property)
        {
            propertyInfos.Add<T, DateTime?>(property);
        }
        public static void Add<T>(this IList<PropertyInfo> propertyInfos, Expression<Func<T, int?>> property)
        {
            propertyInfos.Add<T, int?>(property);
        }

        public static void Add<T, TR>(this IList<PropertyInfo> propertyInfos, Expression<Func<T, TR>> property)
        {
            MemberExpression memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("'property' không có body");
            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null || propertyInfo.ReflectedType == null)
                throw new ArgumentException(string.Format("Expression '{0}' can't be cast to an Operand.", property));
            propertyInfos.Add(propertyInfo);
        }

        public static void Add<T>(this ICollection<string> propertyInfos, Expression<Func<T, bool>> property)
        {
            propertyInfos.Add<T, bool>(property);
        }
        public static void Add<T>(this ICollection<string> propertyInfos, Expression<Func<T, string>> property)
        {
            propertyInfos.Add<T, string>(property);
        }
        public static void Add<T>(this ICollection<string> propertyInfos, Expression<Func<T, int>> property)
        {
            propertyInfos.Add<T, int>(property);
        }
        public static void Add<T>(this ICollection<string> propertyInfos, Expression<Func<T, decimal>> property)
        {
            propertyInfos.Add<T, decimal>(property);
        }
        public static void Add<T>(this ICollection<string> propertyInfos, Expression<Func<T, decimal?>> property)
        {
            propertyInfos.Add<T, decimal?>(property);
        }
        public static void Add<T>(this ICollection<string> propertyInfos, Expression<Func<T, DateTime?>> property)
        {
            propertyInfos.Add<T, DateTime?>(property);
        }
        public static void Add<T>(this ICollection<string> propertyInfos, Expression<Func<T, int?>> property)
        {
            propertyInfos.Add<T, int?>(property);
        }

        public static void Add<T, TR>(this ICollection<string> propertyInfos, Expression<Func<T, TR>> property)
        {
            MemberExpression memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("'property' không có body");
            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null || propertyInfo.ReflectedType == null)
                throw new ArgumentException(string.Format("Expression '{0}' can't be cast to an Operand.", property));
            propertyInfos.Add(propertyInfo.Name);
        }

        public static bool Contains<T, TR>(this IList<PropertyInfo> propertyInfos, Expression<Func<T, TR>> property)
        {
            MemberExpression memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("'property' không có body");
            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null || propertyInfo.ReflectedType == null)
                return false;
            if (propertyInfos.Contains(propertyInfo))
            {
                return true;
            }
            return false;
        }

        public static string GenerateGuid() => Guid.NewGuid().ToString("N");

        public static void AddRangeWithCheckExist(this HashSet<string> source, IEnumerable<string> addItems)
        {
            foreach (var item in addItems)
            {
                if (!source.Contains(item))
                {
                    source.Add(item);
                }
            }
        }

        public static bool IsNullOrEmpty(this object input)
        {
            if (input == null || input.ToString().Length == 0 || input.ToString().Trim().Length == 0)
            {
                return true;
            }
            return false;
        }
        public static bool IsNotNullOrEmpty(this object input)
        {
            if (input == null || input.ToString().Length == 0 || input.ToString().Trim().Length == 0)
            {
                return false;
            }
            return true;
        }
        public static bool IsNotNullOrEmptyOrNotDBNull(this object input)
        {
            if (input == null || input == "null" || input == DBNull.Value || input.ToString().Length == 0 || input.ToString().Trim().Length == 0)
            {
                return false;
            }
            return true;
        }
        public static DateTime? ConvertCultureVN(this string input)
        {
            CultureInfo culture1 = CultureInfo.CreateSpecificCulture("vi-VN");
            DateTime dateTime;
            string[] font = { "dd/MM/yyyy", "dd-MM-yyyy", "d/M/yyyy", "d-M-yyyy" };
            if (string.IsNullOrEmpty(input))
                return null;
            DateTime.TryParseExact(input.Trim(), font, culture1, DateTimeStyles.None, out dateTime);
            if (dateTime == new DateTime())
                dateTime = DateTime.Now;
            return dateTime;
        }
        public static void Add(this List<KeyValuePair<int, int>> list, int i, int j)
        {
            list.Add(new KeyValuePair<int, int>(i, j));
        }

        /// <summary>
        /// Chuyển object về số nguyên. Nếu obj null hoặc empty hoặc không chuyển được trả về 0
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int ToInt(this object obj)
        {
            if (obj == null || obj.IsNullOrEmpty())
            {
                return 0;
            }
            try
            {
                int value;
                int.TryParse(obj.ToString(), out value);
                return value;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Kiểm tra chuối ký tự có thể chuyển thành số hay không kiểu 0.000,0
        /// </summary>
        /// <param name="pText">Chuỗi ký tự</param>
        /// <returns>True nếu chuối ký tự là số. False nếu ngược lại</returns>
        public static bool IsNumberFormatVN(this string pText)
        {
            if (pText.IsNullOrEmpty())
            {
                return false;
            }
            Regex regex = new Regex(@"^[-+]?[0-9.]*\,?[0-9]+$");
            return regex.IsMatch(pText);
        }

        public static void SetValue<T>(this DataRow row, string key, List<T> list, T item, List<KeyValuePair<int, int>> errors, int i, int j)
        {
            if (list.Contains(item))
            {
                row[key] = item;
            }
            else
            {
                errors.Add(i, j);
            }
        }

        public static void SetRowValue<T>(this DataRow row, string key, ICollection<T> list, T item, List<KeyValuePair<int, int>> errors, int i, int j)
        {
            if (list.Contains(item))
            {
                row[key] = item;
            }
            else
            {
                errors.Add(i, j);
            }
        }

        public static bool EqualValues<T>(T a1, T a2)
        {
            return EqualityComparer<T>.Default.Equals(a1, a2);
        }
        //public static List<KeyValuePair<string, string>> ToListKeyValuePair(this Dictionary<string, ISelectItem> sources, bool customDisplay = false)
        //{
        //    if (sources == null)
        //    {
        //        return new List<KeyValuePair<string, string>>();
        //    }
        //    else
        //    {
        //        if (customDisplay)
        //        {
        //            return sources.Select(c => new KeyValuePair<string, string>(c.Key, c.Value.GetCustomDisplay())).ToList();
        //        }
        //        else
        //        {
        //            return sources.Select(c => new KeyValuePair<string, string>(c.Key, c.Value.GetDisplay())).ToList();
        //        }
        //    }
        //}

        public static Decimal ToDecimal(this object obj)
        {
            CultureInfo culture1 = CultureInfo.CreateSpecificCulture("vi-VN");
            if (obj == null || obj.IsNullOrEmpty())
            {
                return 0;
            }
            decimal value;
            Decimal.TryParse(obj.ToString(), NumberStyles.Number, culture1, out value);
            return value;
        }

        /// <summary>
        /// Hàm chuyển số x dạng decimal về chuỗi theo format 
        /// Ví dụ:
        ///     x >= 100: 1000 => 1.000
        ///     x < 100: 50 => 50,00
        ///              3,5 => 3,5
        ///              3,55 => 3,55
        /// </summary>
        /// <param name="d_value"></param>
        /// <returns></returns>
        public static string ToDecimalFormated(this decimal? d_value, int numberAfterComma = 2)
        {
            if (d_value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            var valueAfterComma = Math.Pow(10, numberAfterComma).ToInt();
            if (d_value > 0m && d_value < 100m && ((int)d_value * valueAfterComma) % valueAfterComma == 0)
            {
                // Yêu cầu chỉ có 2 số 0 sau dấu phẩy
                return d_value?.ToString("N2");
            }
            else
            {
                var strValue = d_value?.ToString("N" + numberAfterComma);
                if (strValue != null && strValue.Contains(','))
                {
                    return strValue.TrimEnd('0').TrimEnd(',');
                }
                else
                {
                    return strValue;
                }
            }
        }
        public static string ToDecimalFormated(this decimal d_value, int numberAfterComma = 2)
        {
            if (d_value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            var valueAfterComma = Math.Pow(10, numberAfterComma).ToInt();
            if (d_value > 0m && d_value < 100m && ((int)d_value * valueAfterComma) % valueAfterComma == 0)
            {
                // Yêu cầu chỉ có 2 số 0 sau dấu phẩy
                return d_value.ToString("N2");
            }
            else
            {
                var strValue = d_value.ToString("N" + numberAfterComma);
                if (strValue != null && strValue.Contains(','))
                {
                    return strValue.TrimEnd('0').TrimEnd(',');
                }
                else
                {
                    return strValue;
                }
            }
        }

        public static string ToDecimalFormated1(this decimal? d_value, int numberAfterComma = 1)
        {
            if (d_value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            var valueAfterComma = Math.Pow(10, numberAfterComma).ToInt();
            if (d_value > 0m && d_value < 100m && ((int)d_value * valueAfterComma) % valueAfterComma == 0)
            {
                // Yêu cầu chỉ có 2 số 0 sau dấu phẩy
                return d_value?.ToString("N1");
            }
            else
            {
                var strValue = d_value?.ToString("N" + numberAfterComma);
                if (strValue != null && strValue.Contains(','))
                {
                    return strValue.TrimEnd('0').TrimEnd(',');
                }
                else
                {
                    return strValue;
                }
            }
        }
        public static string ToDecimalFormated3(this decimal? d_value, int numberAfterComma = 3)
        {
            if (d_value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            var valueAfterComma = Math.Pow(10, numberAfterComma).ToInt();
            if (d_value > 0m && d_value < 100m && ((int)d_value * valueAfterComma) % valueAfterComma == 0)
            {
                // Yêu cầu chỉ có 3 số 0 sau dấu phẩy
                return d_value?.ToString("N3");
            }
            else
            {
                var strValue = d_value?.ToString("N" + numberAfterComma);
                if (strValue != null && strValue.Contains(','))
                {
                    return strValue.TrimEnd('0').TrimEnd(',');
                }
                else
                {
                    return strValue;
                }
            }
        }

        public static string ToDecimalUnFormat(this decimal? d_value)
        {
            if (d_value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            var strValue = d_value?.ToString("N");
            if (strValue != null && strValue.Contains(','))
            {
                return strValue.TrimEnd('0').TrimEnd(',');
            }
            else
            {
                return strValue;
            }
        }

        public static string ToDecimalUnFormat(this decimal d_value)
        {
            if (d_value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            string value = d_value.ToString();
            if (value.Contains(","))
            {
                return value.TrimEnd('0').TrimEnd(',');
            }
            else
            {
                return value;
            }
        }

        public static string ToDecimalUnFormatDot(this decimal? d_value)
        {
            var value = d_value.ToDecimalUnFormat();
            if (value.Contains(","))
            {
                return value.Replace(",", ".");
            }
            return value;
        }

        public static string ToDecimalUnFormatDot(this decimal d_value)
        {
            var value = d_value.ToDecimalUnFormat();
            if (value.Contains(","))
            {
                return value.Replace(",", ".");
            }
            return value;
        }

        public static void AddExist(this Dictionary<string, List<string>> dictionary, string key, string value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key].Add(value);
            }
            else
            {
                dictionary.Add(key, new List<string> { value });
            }
        }

        public static string DecimalToIntXml(this decimal? d_value)
        {
            if (d_value.IsNullOrEmpty())
            {
                return "0";
            }
            else
            {
                return d_value.ToString().Split(',')[0].Replace(".", "");
            }
        }
        public static string DecimalToFormatTyLeXml(this decimal? d_value)
        {
            if (d_value.IsNullOrEmpty())
            {
                return "0";
            }
            else
            {
                return d_value.ToString().Replace(".", "").Replace(",", ".");
            }
        }

        public static string GetDescription<T>(this T enumValue)
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                return null;

            var description = enumValue.ToString();
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    description = ((DescriptionAttribute)attrs[0]).Description;
                }
                else
                {
                    var displayAttrs = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), true);
                    if (displayAttrs != null && displayAttrs.Length > 0)
                    {
                        description = ((DisplayAttribute)displayAttrs[0]).Name;
                    }
                }
            }

            return description;
        }

        public static string GetDescription(this object enumValue)
        {
            var description = enumValue.ToString();
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    description = ((DescriptionAttribute)attrs[0]).Description;
                }
                else
                {
                    var displayAttrs = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), true);
                    if (displayAttrs != null && displayAttrs.Length > 0)
                    {
                        description = ((DisplayAttribute)displayAttrs[0]).Name;
                    }
                }
            }

            return description;
        }

        public static int EnumToInt<T>(this T enumObj)
        {
            if (!typeof(T).IsEnum)
                throw new InvalidEnumArgumentException("Method is only supported enum");
            return Convert.ToInt32(enumObj);
        }

        public static string EnumToString<T>(this T enumObj)
        {
            if (!typeof(T).IsEnum)
                throw new InvalidEnumArgumentException("Method is only supported enum");
            return Convert.ToInt32(enumObj).ToString();
        }

        public static string GetFriendlyUrl(this string str)
        {
            str = str.ToLower();
            str = Regex.Replace(str, @"[áàảạãăắẳằặẵâấầẩậẫ]", "a");
            str = Regex.Replace(str, @"[eéèẻẹẽêếềểệễ]", "e");
            str = Regex.Replace(str, @"[oõóòỏọôốồổộỗơớờởỡợ]", "o");
            str = Regex.Replace(str, @"[uủùúụưứừửựũữ]", "u");
            str = Regex.Replace(str, @"[iíìỉịĩ]", "i");
            str = Regex.Replace(str, @"[yýỳỷỵỹ]", "y");
            str = Regex.Replace(str, @"[đ]", "d");
            str = Regex.Replace(str, @"[^a-zA-Z0-9]", "-");
            str = Regex.Replace(str, "-+", "-");
            str = Regex.Replace(str, "^[-]", "");
            str = Regex.Replace(str, "[-]$", "");

            return str;
        }

        public static string GetBasicString(this string str)
        {
            str = str.ToLower();
            str = Regex.Replace(str, @"[áàảạãăắẳằặẵâấầẩậẫ]", "a");
            str = Regex.Replace(str, @"[eéèẻẹẽêếềểệễ]", "e");
            str = Regex.Replace(str, @"[oõóòỏọôốồổộỗơớờởỡợ]", "o");
            str = Regex.Replace(str, @"[uủùúụưứừửựũữ]", "u");
            str = Regex.Replace(str, @"[iíìỉịĩ]", "i");
            str = Regex.Replace(str, @"[yýỳỷỵỹ]", "y");
            str = Regex.Replace(str, @"[đ]", "d");
            str = Regex.Replace(str, @"[^a-zA-Z0-9]", "_");
            str = Regex.Replace(str, "_+", "_");
            str = Regex.Replace(str, "^[_]", "");
            str = Regex.Replace(str, "[_]$", "");

            return str;
        }

        public static string GetNonVietnameseString(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return str;
            }
            str = str.GetBasicString();
            str = Regex.Replace(str, "[_]+", " ");
            return GetUpperFirstWord(str);
        }

        public static string GetNormalName(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return str;
            }
            str = Regex.Replace(str, "[ ]+", " ");
            return GetUpperFirstWord(str);
        }

        public static string GetUpperFirstWord(string str)
        {
            if (str.IsNullOrEmpty())
            {
                return str;
            }
            var strArr = str.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in strArr)
            {
                var charArr = item.ToCharArray();
                charArr[0] = Char.ToUpper(charArr[0]);
                stringBuilder.Append(string.Concat(charArr[0].ToString().ToUpper(), item.AsSpan(1)) + " ");
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static IEnumerable<T> SelectManyRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            var result = source.SelectMany(selector);
            if (!result.Any())
            {
                return result;
            }
            return result.Concat(result.SelectManyRecursive(selector));
        }

        public static T GetAttribute<T>(this MemberInfo member, bool isRequired) where T : Attribute
        {
            var attribute = member.GetCustomAttributes(typeof(T), false).SingleOrDefault();

            if (attribute == null && isRequired)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The {0} attribute must be defined on member {1}",
                        typeof(T).Name,
                        member.Name));
            }

            return (T)attribute;
        }

        public static string GetPropertyDisplayName<T>(Expression<Func<T, object>> propertyExpression)
        {
            var memberInfo = GetPropertyInformation(propertyExpression.Body);
            if (memberInfo == null)
            {
                throw new ArgumentException(
                    "No property reference expression was found.",
                    "propertyExpression");
            }

            var attr = memberInfo.GetAttribute<DisplayAttribute>(false);
            if (attr == null)
            {
                return memberInfo.Name;
            }

            return attr.Name;
        }

        public static string GetPropertyDisplayName<T>(string propertyName)
        {
            var memberInfo = GetPropertyInformation<T>(propertyName);
            if (memberInfo == null)
            {
                throw new ArgumentException(
                    "No property reference expression was found.",
                    "propertyExpression");
            }

            var attr = memberInfo.GetAttribute<DisplayAttribute>(false);
            if (attr == null)
            {
                return memberInfo.Name;
            }

            return attr.Name;
        }

        public static MemberInfo GetPropertyInformation(Expression propertyExpression)
        {
            Debug.Assert(propertyExpression != null, "propertyExpression != null");
            MemberExpression memberExpr = propertyExpression as MemberExpression;
            if (memberExpr == null)
            {
                UnaryExpression unaryExpr = propertyExpression as UnaryExpression;
                if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                {
                    memberExpr = unaryExpr.Operand as MemberExpression;
                }
            }

            if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
            {
                return memberExpr.Member;
            }

            return null;
        }

        public static MemberInfo GetPropertyInformation<T>(string propertyName)
        {
            Debug.Assert(propertyName != null, "propertyName != null");
            var propery = typeof(T).GetProperty(propertyName);
            return propery as MemberInfo;
        }

        public static async Task CopyFileAsync(string sourceFile, string destinationFile, FileMode fileMode, CancellationToken cancellationToken)
        {
            var fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
            var bufferSize = 65536;

            using var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, fileOptions);
            using var destinationStream = new FileStream(destinationFile, fileMode, FileAccess.Write, FileShare.None, bufferSize, fileOptions);
            await sourceStream.CopyToAsync(destinationStream, bufferSize, cancellationToken)
                                       .ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}
