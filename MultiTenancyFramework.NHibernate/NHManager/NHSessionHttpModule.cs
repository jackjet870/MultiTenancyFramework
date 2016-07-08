﻿using MultiTenancyFramework;
using System;
using System.Web;

namespace MultiTenancyFramework.NHibernate.NHManager
{
    public class NHSessionHttpModule : IHttpModule
    {
        /// <summary>
        /// Constant key for storing the session in the HttpContext
        /// </summary>
        private HttpApplication _context;
        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += Application_BeginRequest;
            context.Error += Application_Error;
            _context = context;
        }

        private void Application_BeginRequest(object sender, EventArgs e)
        {
            _context.Context.AddOnRequestCompleted(x =>
            {
                if (x.Items.Contains(WebSessionStorage.CurrentSessionKey))
                {
                    var storageSet = NHSessionManager.CurrentSessions; //x.Items[WebSessionStorage.CurrentSessionKey] as Dictionary<string, WebSessionStorage>;

                    if (storageSet != null && storageSet.Count > 0)
                    {
                        foreach (var storage in storageSet.Values)
                        {
                            //Closes the session if there's any open session
                            if (storage != null && storage.Session != null)
                            {
                                var session = storage.Session;
                                if (session.Transaction != null && session.IsConnected && session.Transaction.IsActive && !session.Transaction.WasCommitted && !session.Transaction.WasRolledBack)
                                {
                                    session.Transaction.Rollback();
                                }

                                if (session.IsOpen) session.Close();
                                session.Dispose();
                                NHSessionManager.CloseStorage(storage.InstitutionCode);
                            }
                        }
                        storageSet.Clear();
                        x.Items.Remove(WebSessionStorage.CurrentSessionKey);
                    }
                }
            });
        }

        private void Application_Error(object sender, EventArgs e)
        {
            try
            {
                MyServiceLocator.GetInstance<ILogger>().Log(_context.Server.GetLastError());

                NHSessionManager.CloseStorage();
            }
            catch { }
            finally
            {
                if (_context.Context != null && _context.Context.Response != null)
                {
                    var baseUrl = ConfigurationHelper.GetSiteUrl() ?? _context.Context.Request.Url.Authority;
                    _context.Context.Response.Redirect($"{baseUrl}Error/?gl=1");
                }
            }
        }

    }
}