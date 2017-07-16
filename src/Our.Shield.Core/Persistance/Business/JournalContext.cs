﻿namespace Our.Shield.Core.Persistance.Business
{
    using Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;

    /// <summary>
    /// The Journal context
    /// </summary>
    public class JournalContext : DbContext
    {
        private IEnumerable<IJournal> GetListingResults(int? environmentId, string appId, int page, int itemsPerPage, Type type, out int totalPages)
        {
            totalPages = 0;

            var sql = new Sql();
            sql.Select("*").From<Data.Dto.Journal>(Syntax);

            if (environmentId.HasValue)
            { 
                sql.Where<Data.Dto.Journal>(j => j.EnvironmentId == environmentId, Syntax);
            }
            
            if (appId != null)
            {
                sql.Where<Data.Dto.Journal>(j => j.AppId == appId, Syntax);
            }

            sql.OrderByDescending<Data.Dto.Journal>(j => j.Datestamp, Syntax);

            try
            {
                var results = Database.Page<Data.Dto.Journal>(page, itemsPerPage, sql);
                totalPages = (int) results.TotalPages;

                var typedRecords = results.Items
                    .Select(x =>
                        {
                            try
                            {
                                var journal = JsonConvert.DeserializeObject(x.Value, type) as Journal;
                                journal.AppId = x.AppId;
                                journal.Datestamp = x.Datestamp;
                                journal.EnvironmentId = x.EnvironmentId;

                                return journal;
                            }
                            catch (JsonSerializationException jEx)
                            {
                                LogHelper.Error(typeof(JournalContext), $"Error Deserialising journal for environment with Id: {environmentId}; Record Id: {x.Id}; Type:{type}", jEx);
                                return null;
                            }
                            catch (Exception selectEx)
                            {
                                LogHelper.Error(typeof(JournalContext), $"An error occured getting journals for environment with Id: {environmentId}; Record Id: {x.Id};", selectEx);
                                return null;
                            }

                        })
                    .Where(x => x != null);

                return typedRecords;
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(JournalContext), $"Error getting journals for environment with Id: {environmentId}; App Id: {appId}", ex);
            }
            
            return Enumerable.Empty<IJournal>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="environmentId"></param>
        /// <param name="appId"></param>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="type"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public IEnumerable<IJournal> List(int environmentId, string appId, int page, int itemsPerPage, Type type, out int totalPages) => 
            GetListingResults(environmentId, appId, page, itemsPerPage, type, out totalPages);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="environmentId"></param>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="type"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public IEnumerable<IJournal> List(int environmentId, int page, int itemsPerPage, Type type, out int totalPages) => 
            GetListingResults(environmentId, null, page, itemsPerPage, type, out totalPages);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="environmentId"></param>
        /// <param name="appId"></param>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public IEnumerable<T> List<T>(int environmentId, string appId, int page, int itemsPerPage, out int totalPages) where T : IJournal => 
            GetListingResults(environmentId, appId, page, itemsPerPage, typeof(T), out totalPages).Select(x => (T)x);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="type"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public IEnumerable<IJournal> FetchAll(int page, int itemsPerPage, Type type, out int totalPages) => 
            GetListingResults(null, null, page, itemsPerPage, type, out totalPages);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public IEnumerable<T> FetchAll<T>(int page, int itemsPerPage, out int totalPages) where T : IJournal =>
            GetListingResults(null, null, page, itemsPerPage, typeof(T), out totalPages).Select(x => (T)x);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="environmentId"></param>
        /// <param name="appId"></param>
        /// <param name="journal"></param>
        /// <returns></returns>
        public bool Write(int environmentId, string appId, IJournal journal)
        {
            try
            {
                Database.Insert(new Data.Dto.Journal()
                {
                    EnvironmentId = environmentId,
                    AppId = appId,
                    Datestamp = DateTime.UtcNow,
                    Value = JsonConvert.SerializeObject(journal)
                });
                return true;
            }
            catch(Exception ex)
            {
                LogHelper.Error(typeof(JournalContext), $"Error writing Journal for environment Id: {environmentId} app Id: {appId}", ex);
            }
            return false;
        }
    }
}
