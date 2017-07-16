﻿namespace Our.Shield.Core.Persistance.Business
{
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Reflection;
    using Umbraco.Core.Logging;

    /// <summary>
    /// The Configuration Context.
    /// </summary>
    public class ConfigurationContext : DbContext
    {

        /// <summary>
        /// 
        /// </summary>
        internal class ShouldSerializeContractResolver : DefaultContractResolver
        {
            public static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();
 
            /// <summary>
            /// 
            /// </summary>
            /// <param name="member"></param>
            /// <param name="memberSerialization"></param>
            /// <returns></returns>
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);
 
                if (property.PropertyName.Equals(nameof(IConfiguration.Enable), StringComparison.InvariantCultureIgnoreCase) || 
                    property.PropertyName.Equals(nameof(IConfiguration.LastModified), StringComparison.InvariantCultureIgnoreCase))
                {
                    property.ShouldSerialize = instance =>
                    {
                        return false;
                    };
                }
                return property;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="environmentId"></param>
        /// <param name="appId"></param>
        /// <param name="type"></param>
        /// <param name="defaultConfiguration"></param>
        /// <returns></returns>
        public IConfiguration Read(int environmentId, string appId, Type type, IConfiguration defaultConfiguration)
        {
            var config = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(defaultConfiguration), type) as Configuration;
            config.LastModified = null;
            config.Enable = false;
            try
            {
                var record = Database.SingleOrDefault<Data.Dto.Configuration>(
                    "WHERE " + nameof(Data.Dto.Configuration.EnvironmentId) + " = @0 AND " +
                    nameof(Data.Dto.Configuration.AppId) + " = @1",
                    environmentId, appId);
                if (record != null && !string.IsNullOrEmpty(record.Value))
                {
                    config = JsonConvert.DeserializeObject(record.Value, type) as Configuration;
                    config.LastModified = record.LastModified;
                    config.Enable = record.Enable;
                }
            }
            catch (JsonSerializationException jEx)
            {
                LogHelper.Error(typeof(ConfigurationContext), $"Error Deserialising configuration with environmentId: {environmentId} for appId: {appId}; to type:{type}", jEx);
                return defaultConfiguration;
            }
            catch(Exception ex)
            {
                LogHelper.Error(typeof(ConfigurationContext), $"Error reading configuration with environmentId: {environmentId} for appId: {appId}; to type:{type}", ex);
                return defaultConfiguration;
            }

            return config;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="environmentId"></param>
        /// <param name="appId"></param>
        /// <param name="defaultConfiguration"></param>
        /// <returns></returns>
        public T Read<T>(int environmentId, string appId, T defaultConfiguration) where T : IConfiguration
        {
            return (T) Read(environmentId, appId, typeof(T), defaultConfiguration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="environmentId"></param>
        /// <param name="appId"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool Write(int environmentId, string appId, IConfiguration config)
        {
            try
            {
                var record = Database.SingleOrDefault<Data.Dto.Configuration>(
                    "WHERE " + 
                    nameof(Data.Dto.Configuration.EnvironmentId) + " = @0 AND " +
                    nameof(Data.Dto.Configuration.AppId) + " = @1",
                    environmentId, appId);

                var value = JsonConvert.SerializeObject(config, Formatting.None, 
                            new JsonSerializerSettings { ContractResolver = new ShouldSerializeContractResolver() });

                if (record == null)
                {
                    record = new Data.Dto.Configuration
                    {
                        EnvironmentId = environmentId,
                        AppId = appId,
                        Enable = config.Enable,
                        LastModified = DateTime.UtcNow,
                        Value = value
                    };                                 
                    Database.Insert(record);
                }
                else
                {
                    record.Enable = config.Enable;
                    record.LastModified = DateTime.UtcNow;
                    record.Value = value;
                    Database.Update(record);
                }
                return true;
            }
            catch(Exception ex)
            {
                LogHelper.Error(typeof(ConfigurationContext), $"Error writing configuration with environmentId: {environmentId} for appId: {appId}", ex);
            }
            return false;
        }
    }
}
