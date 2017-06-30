﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;

namespace Shield.Core.Operation
{
    public class ServiceHandler
    {
        private static readonly Lazy<ServiceHandler> _instance = new Lazy<ServiceHandler>(() => new ServiceHandler());

        private ServiceHandler()
        {
        }

        // accessor for instance
        public static ServiceHandler Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        /// <summary>
        /// Services types that you can monitor
        /// </summary>
        public enum ServiceType
        {
            AuditService,
            ContentService,
            ContentTypeService,
            DataTypeService,
            DomainService,
            EntityService,
            ExternalLoginService,
            FileService,
            LocalizationService,
            LocalizedTextService,
            MacroService,
            MediaService,
            MemberGroupService,
            PackagingService,
            PublicAccessService,
            RelationService,
            RepositoryService,
            ServerRegistrationService,
            TagService,
            TaskService,
            UserService
        }        


        private class Watcher
        {
            public int id;
            ServiceType type;
            public Func<int, ServiceType, IEnumerable<object>> request;
        }
    }
}
