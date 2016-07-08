﻿using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc
{
    /// <summary>
    /// My custom Filter Config
    /// </summary>
    public class InstitutionFilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}