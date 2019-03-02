using BookingApp.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BookingApp.Helpers
{
    /// <summary>
    /// Load/delete store proc/funct helper class
    /// </summary>
    static public class StoreProcFuncRepository
    {
        /// <summary>
        /// Search store procedures code as embeded ressources in
        /// namespace <c>Data.StoredProcedures</c> and must be with extension sql
        /// </summary>
        /// <param name="context">Db context</param>
        static public async Task LoadAllToDb(ApplicationDbContext context)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var storedProcedureResourceList = assembly.GetManifestResourceNames().Where(
                str => str.Contains("Data.StoredProcedures") && str.EndsWith(".sql")
                );

            foreach (var resourceName in storedProcedureResourceList)
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    await context.Database.ExecuteSqlCommandAsync(reader.ReadToEnd());
                }
            }
        }

        /// <summary>
        /// Delete all store proc/funct from db
        /// </summary>
        /// <param name="context">Db context</param>
        static public async Task DeleteAllFromDb(ApplicationDbContext context)
        {
            await context.Database.ExecuteSqlCommandAsync(
                  @"DECLARE @SPname VARCHAR(128)
                    DECLARE @SQL VARCHAR(254)
                    SELECT @SPname = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] = 'P' AND category = 0 ORDER BY [name])
                    WHILE @SPname is not null
                    BEGIN
                        SELECT @SQL = 'DROP PROCEDURE [dbo].[' + RTRIM(@SPname ) +']'
                        EXEC (@SQL)
                        PRINT 'Dropped Procedure: ' + @SPname
                        SELECT @SPname = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] = 'P' AND category = 0 AND [name] > @SPname ORDER BY [name])
                    END
                    DECLARE @FNname VARCHAR(128)
                    SELECT @FNname = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] IN (N'FN', N'IF', N'TF', N'FS', N'FT') AND category = 0 ORDER BY [name])
                    WHILE @FNname IS NOT NULL
                    BEGIN
                        SELECT @SQL = 'DROP FUNCTION [dbo].[' + RTRIM(@FNname ) +']'
                        EXEC (@SQL)
                        PRINT 'Dropped Function: ' + @FNname
                        SELECT @FNname = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] IN (N'FN', N'IF', N'TF', N'FS', N'FT') AND category = 0 AND [name] > @FNname ORDER BY [name])
                    END");
        }
    }
}
