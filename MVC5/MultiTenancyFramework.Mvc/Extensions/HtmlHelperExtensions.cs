﻿using MultiTenancyFramework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Routing;

namespace System.Web.Mvc.Html
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString MyEnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum?>> expression)
           where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return htmlHelper.MyEnumDropDownListFor(expression, optionalLabel: null, htmlAttributes: null);
        }

        public static MvcHtmlString MyEnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum?>> expression, object htmlAttributes)
           where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return htmlHelper.MyEnumDropDownListFor(expression, optionalLabel: null, htmlAttributes: htmlAttributes);
        }

        public static MvcHtmlString MyEnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum?>> expression, string optionalLabel, object htmlAttributes)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var enumType = typeof(TEnum);
            if (enumType.IsNullable())
            {
                enumType = Nullable.GetUnderlyingType(enumType);
            }
            if (!enumType.IsEnum)
            {
                throw new NotSupportedException("TEnum must be an enumerated (enum) type");
            }

            var list = MultiTenancyFramework.EnumHelper.GetEnumNames(enumType);
            return htmlHelper.MyEnumDropDownListFor(expression, list, optionalLabel, htmlAttributes);
        }

        public static MvcHtmlString MyEnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum?>> expression, IEnumerable enumList, string optionalLabel, object htmlAttributes)
               where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var enumType = typeof(TEnum);
            //if (enumType.IsNullable())
            //{
            //    enumType = Nullable.GetUnderlyingType(enumType);
            //}
            if (!enumType.IsEnum)
            {
                throw new NotSupportedException("TEnum must be an enumerated (enum) type");
            }

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            if (metadata == null)
            {
                throw new InvalidOperationException("Could not get metadata from expression");
            }
            if (metadata.ModelType == null)
            {
                throw new InvalidOperationException("Null value for metadata's ModelType");
            }
            Enum currentValue = metadata.Model as Enum;

            string expressionName = ExpressionHelper.GetExpressionText(expression);
            string expressionFullName =
                htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionName);
            if (currentValue == null && !string.IsNullOrWhiteSpace(expressionName))
            {
                currentValue = htmlHelper.ViewData.Eval(expressionName) as Enum;
            }
            if (currentValue == null && !string.IsNullOrWhiteSpace(expressionFullName))
            {
                currentValue = GetValueFromModel(htmlHelper, expressionFullName, metadata.ModelType) as Enum;
            }

            if (currentValue == null && string.IsNullOrWhiteSpace(optionalLabel))
            {
                optionalLabel = "Choose an item...";
            }
            bool recastToNV = false;
            foreach (var item in enumList)
            {
                if (item.GetType() == typeof(MultiTenancyFramework.EnumHelper.NV))
                {
                    recastToNV = false;
                }
                else if (item.GetType() == enumType)
                {
                    recastToNV = true;
                }
                else
                {
                    throw new NotSupportedException("enumList must be an enumerated (enum) type OR or tyoe 'MultiTenancyFramework.EnumHelper.NV'");
                }
                break; // we're testing just the first; hopefully that will do.
            }
            SelectList selectList;
            if (recastToNV)
            {
                selectList = new SelectList(MultiTenancyFramework.EnumHelper.GetEnumNames(enumType, enumList), "Value", "Name", currentValue);
            }
            else
            {
                selectList = new SelectList(enumList, "Value", "Name", currentValue);
            }

            var attr = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (attr.ContainsKey("class"))
            {
                if (!attr["class"].ToString().Contains("form-control"))
                {
                    attr["class"] = "form-control " + attr["class"];
                }
            }
            else
            {
                attr["class"] = "form-control";
            }
            if (metadata.IsRequired)
            {
                attr["required"] = "required";
                return htmlHelper.DropDownListFor(expression, selectList, optionalLabel, attr);
            }
            return htmlHelper.DropDownListFor(expression, selectList, optionalLabel, htmlAttributes);
        }

        /// <summary>
        /// This is most useful when the enums are manually added to a list; i.e. when you want to list only specific enum items
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="selectList"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString MyEnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum?>> expression, IEnumerable<SelectListItem> selectList, object htmlAttributes)
                   where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var enumType = typeof(TEnum);
            var items = from item in selectList
                        select new SelectListItem
                        {
                            Value = item.Text,
                            Text = MultiTenancyFramework.EnumHelper.GetEnumName(enumType, item.Text),
                            Selected = item.Selected,
                            Disabled = item.Disabled,
                            Group = item.Group,
                        };
            return htmlHelper.DropDownListFor(expression, items, htmlAttributes);
        }


        #region Non-Nullables

        public static MvcHtmlString MyEnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression)
           where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return htmlHelper.MyEnumDropDownListFor(expression, optionalLabel: null, htmlAttributes: null);
        }

        public static MvcHtmlString MyEnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, object htmlAttributes)
           where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return htmlHelper.MyEnumDropDownListFor(expression, optionalLabel: null, htmlAttributes: htmlAttributes);
        }

        public static MvcHtmlString MyEnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, string optionalLabel, object htmlAttributes)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var enumType = typeof(TEnum);
            if (enumType.IsNullable())
            {
                enumType = Nullable.GetUnderlyingType(enumType);
            }
            if (!enumType.IsEnum)
            {
                throw new NotSupportedException("TEnum must be an enumerated (enum) type");
            }

            var list = MultiTenancyFramework.EnumHelper.GetEnumNames(enumType);
            return htmlHelper.MyEnumDropDownListFor(expression, list, optionalLabel, htmlAttributes);
        }

        public static MvcHtmlString MyEnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, IEnumerable enumList, string optionalLabel, object htmlAttributes)
               where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var enumType = typeof(TEnum);
            //if (enumType.IsNullable())
            //{
            //    enumType = Nullable.GetUnderlyingType(enumType);
            //}
            if (!enumType.IsEnum)
            {
                throw new NotSupportedException("TEnum must be an enumerated (enum) type");
            }

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            if (metadata == null)
            {
                throw new InvalidOperationException("Could not get metadata from expression");
            }
            if (metadata.ModelType == null)
            {
                throw new InvalidOperationException("Null value for metadata's ModelType");
            }
            Enum currentValue = metadata.Model as Enum;

            string expressionName = ExpressionHelper.GetExpressionText(expression);
            string expressionFullName =
                htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionName);
            if (currentValue == null && !string.IsNullOrWhiteSpace(expressionName))
            {
                currentValue = htmlHelper.ViewData.Eval(expressionName) as Enum;
            }
            if (currentValue == null && !string.IsNullOrWhiteSpace(expressionFullName))
            {
                currentValue = GetValueFromModel(htmlHelper, expressionFullName, metadata.ModelType) as Enum;
            }

            if (currentValue == null && string.IsNullOrWhiteSpace(optionalLabel))
            {
                optionalLabel = "Choose an item...";
            }
            bool recastToNV = false;
            foreach (var item in enumList)
            {
                if (item.GetType() == typeof(MultiTenancyFramework.EnumHelper.NV))
                {
                    recastToNV = false;
                }
                else if (item.GetType() == enumType)
                {
                    recastToNV = true;
                }
                else
                {
                    throw new NotSupportedException("enumList must be an enumerated (enum) type OR or tyoe 'MultiTenancyFramework.EnumHelper.NV'");
                }
                break; // we're testing just the first; hopefully that will do.
            }
            SelectList selectList;
            if (recastToNV)
            {
                selectList = new SelectList(MultiTenancyFramework.EnumHelper.GetEnumNames(enumType, enumList), "Value", "Name", currentValue);
            }
            else
            {
                selectList = new SelectList(enumList, "Value", "Name", currentValue);
            }

            var attr = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (attr.ContainsKey("class"))
            {
                if (!attr["class"].ToString().Contains("form-control"))
                {
                    attr["class"] = "form-control " + attr["class"];
                }
            }
            else
            {
                attr["class"] = "form-control";
            }
            if (metadata.IsRequired)
            {
                attr["required"] = "required";
                return htmlHelper.DropDownListFor(expression, selectList, optionalLabel, attr);
            }
            return htmlHelper.DropDownListFor(expression, selectList, optionalLabel, htmlAttributes);
        }

        /// <summary>
        /// This is most useful when the enums are manually added to a list; i.e. when you want to list only specific enum items
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="selectList"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString MyEnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, IEnumerable<SelectListItem> selectList, object htmlAttributes)
                   where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var enumType = typeof(TEnum);
            var items = from item in selectList
                        select new SelectListItem
                        {
                            Value = item.Text,
                            Text = MultiTenancyFramework.EnumHelper.GetEnumName(enumType, item.Text),
                            Selected = item.Selected,
                            Disabled = item.Disabled,
                            Group = item.Group,
                        };
            return htmlHelper.DropDownListFor(expression, items, htmlAttributes);
        }

        #endregion

        public static IDictionary<string, object> MergeHtmlAttributes(this HtmlHelper helper, object htmlAttributesObject, object defaultHtmlAttributesObject)
        {
            var concatKeys = new string[] { "class" };

            var htmlAttributesDict = htmlAttributesObject as IDictionary<string, object>;
            var defaultHtmlAttributesDict = defaultHtmlAttributesObject as IDictionary<string, object>;

            RouteValueDictionary htmlAttributes = (htmlAttributesDict != null)
                ? new RouteValueDictionary(htmlAttributesDict)
                : HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributesObject);
            RouteValueDictionary defaultHtmlAttributes = (defaultHtmlAttributesDict != null)
                ? new RouteValueDictionary(defaultHtmlAttributesDict)
                : HtmlHelper.AnonymousObjectToHtmlAttributes(defaultHtmlAttributesObject);

            foreach (var item in htmlAttributes)
            {
                if (concatKeys.Contains(item.Key))
                {
                    defaultHtmlAttributes[item.Key] = (defaultHtmlAttributes[item.Key] != null)
                        ? string.Format("{0} {1}", defaultHtmlAttributes[item.Key], item.Value)
                        : item.Value;
                }
                else
                {
                    defaultHtmlAttributes[item.Key] = item.Value;
                }
            }

            return defaultHtmlAttributes;
        }

        private static object GetValueFromModel(HtmlHelper htmlHelper, string key, Type destinationType)
        {
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(key, out modelState))
            {
                if (modelState.Value != null)
                {
                    if (destinationType.IsNullable())
                    {
                        destinationType = Nullable.GetUnderlyingType(destinationType);
                    }
                    destinationType = destinationType.GetEnumUnderlyingType();
                    return modelState.Value.ConvertTo(destinationType, null /* culture */);
                }
            }
            return null;
        }
    }
}
