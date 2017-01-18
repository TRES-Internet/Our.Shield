﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Shield.Persistance.UmbracoAccess
{
    /// <summary>
    /// The Umbraco Access Journal Context.
    /// </summary>
    public class JournalContext : Bal.JournalContext
    {
        /// <summary>
        /// Gets the Id of the Journal Context.
        /// </summary>
        public override Guid Id { get { return ConfigurationContext.id; } }

        /// <summary>
        /// Reads the Journal from the database.
        /// </summary>
        /// <param name="page">
        /// The requested page of results to return.
        /// </param>
        /// <param name="itemsPerPage">
        /// The number of Journal(s) to return for the page.
        /// </param>
        /// <returns>
        /// Collection of Journal objects.
        /// </returns>
        public IEnumerable<Journal> Read(int page, int itemsPerPage)
        {
            return Read<Journal>(page, itemsPerPage);
        }

        /// <summary>
        /// Writes a Journal object to the database.
        /// </summary>
        /// <param name="model">
        /// The Journal object to write.
        /// </param>
        /// <returns>
        /// If successfull, returns true, otherwise false.
        /// </returns>
        public bool Write(Journal model)
        {
            return Write<Journal>(model);
        }
    }
}
