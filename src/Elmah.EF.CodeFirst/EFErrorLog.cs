using System;
using System.Collections;
using System.Data.Common;
using System.Configuration;
using System.Runtime.CompilerServices;
using IDictionary = System.Collections.IDictionary;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace Elmah.EF.CodeFirst
{
    public class EFErrorLog : ErrorLog
    {
        private readonly string _connectionStringOrName;

        public EFErrorLog(IDictionary config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            var connectionStringOrName = config.Find("connectionStringOrName", string.Empty);

            //
            // If there is no connection string to use then throw an 
            // exception to abort construction.
            //

            if (string.IsNullOrWhiteSpace(connectionStringOrName) == true)
                throw new ApplicationException("connectionStringOrName is missing for the SQL error log.");

            this._connectionStringOrName = connectionStringOrName;

            var appName = config.Find("applicationName", string.Empty);

            if (appName.Length > 60)
            {
                throw new ApplicationException(string.Format(
                    "Application name is too long. Maximum length allowed is {0} characters.",
                    60.ToString("N0")));
            }

            ApplicationName = appName;
        }

        public override ErrorLogEntry GetError(string id)
        {

            if (id == null) throw new ArgumentNullException("id");
            if (id.Length == 0) throw new ArgumentException(null, "id");

            Guid errorGuid;

            try
            {
                errorGuid = new Guid(id);
            }
            catch (FormatException e)
            {
                throw new ArgumentException(e.Message, "id", e);
            }

            string errorXml;

            using (var context = new ElmahDbContext(this._connectionStringOrName))
            {
                errorXml = context.Error
                    .Where(p => p.Application == this.ApplicationName && p.ErrorId == errorGuid)
                    .Select(p => p.AllXml)
                    .FirstOrDefault();
            }

            if (string.IsNullOrWhiteSpace(errorXml) == true)
            {
                return null;
            }

            var error = ErrorXml.DecodeString(errorXml);
            return new ErrorLogEntry(this, id, error);
        }

        public override int GetErrors(int pageIndex, int pageSize, IList errorEntryList)
        {
            if (pageIndex < 0) throw new ArgumentOutOfRangeException("pageIndex", pageIndex, null);
            if (pageSize < 0) throw new ArgumentOutOfRangeException("pageSize", pageSize, null);


            int totalCnt = 0;
            using (var context = new ElmahDbContext(this._connectionStringOrName))
            {
                var datas = context.Error.AsQueryable();
                datas = datas.Where(p => p.Application == this.ApplicationName);
                datas = datas.OrderByDescending(p => p.TimeUtc)
                    .ThenByDescending(p => p.Sequence);

                totalCnt = datas.Count();
                datas = datas.Skip(pageIndex * pageSize).Take(pageSize);

                var lst = datas.Select(p => new
                {
                    Application = p.Application,
                    ErrorId = p.ErrorId,
                    Host = p.Host,
                    Message = p.Message,
                    Source = p.Source,
                    StatusCode = p.StatusCode,
                    TimeUtc = p.TimeUtc,
                    Type = p.Type,
                    User = p.User
                }).ToList();
                lst.ForEach(p =>
                {
                    var entry = new ErrorLogEntry(this, p.ErrorId.ToString(), new Error()
                    {
                        ApplicationName = p.Application,
                        HostName = p.Host,
                        Message = p.Message,
                        Source = p.Source,
                        StatusCode = p.StatusCode,
                        Time = p.TimeUtc,
                        Type = p.Type,
                        User = p.User
                    });
                    errorEntryList.Add(entry);
                });
            }

            return totalCnt;
        }

        public override string Log(Error error)
        {
            if (error == null)
                throw new ArgumentNullException("error");

            var errorXml = ErrorXml.EncodeString(error);
            var id = Guid.NewGuid();

            var log = new ELMAH_Error
            {
                Application = ApplicationName,
                Host = error.HostName,
                AllXml = errorXml,
                ErrorId = id,
                Message = error.Message,
                Source = error.Source,
                StatusCode = error.StatusCode,
                TimeUtc = error.Time.ToUniversalTime(),
                Type = error.Type,
                User = error.User
            };
            using (var context = new ElmahDbContext(this._connectionStringOrName))
            {
                context.Error.Add(log);
                context.SaveChanges();
            }
            return id.ToString();
        }
    }
}
