using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;

namespace ExecutiveOffice.EDT.GlobalNotesService.Extensions
{
    public static class LoggerExtensions
    {
		private const string AuditLog = "AuditLog";
		private const string SystemLog = "SystemLog";
		private const string FileOperationsAuditLog = "FileOperationsAuditLog";

        
        public static ILoggerFactory AuditInfo(this ILoggerFactory loggerFactory, IEnumerable<FileInfo> filesToLog, params object[] args)
        {
            if (filesToLog == null) throw new ArgumentNullException(nameof(filesToLog));
            filesToLog.Select(f => loggerFactory.AuditInfo($" + {f.Name}"));
            return loggerFactory;
        }

        public static ILoggerFactory AuditInfo(this ILoggerFactory loggerFactory, string message, params object[] args)
        {
            loggerFactory.CreateLogger(AuditLog).LogInformation(message, args);
            return loggerFactory;
        }

        public static ILoggerFactory AuditFileOperationInfo(this ILoggerFactory loggerFactory, string message, params object[] args)
        {
            loggerFactory.CreateLogger(AuditLog).LogInformation(message, args);
            loggerFactory.CreateLogger(FileOperationsAuditLog).LogInformation(message, args);
            return loggerFactory;
        }

        public static ILoggerFactory AuditWarn(this ILoggerFactory loggerFactory, string message, params object[] args)
        {
            loggerFactory.CreateLogger(AuditLog).LogWarning(message, args);
            return loggerFactory;
        }

        public static ILoggerFactory AuditError(this ILoggerFactory loggerFactory, string message, params object[] args)
        {
            loggerFactory.CreateLogger(AuditLog).LogError(message, args);
            return loggerFactory;
        }

        public static ILoggerFactory AuditError(this ILoggerFactory loggerFactory, Exception ex, string message, params object[] args)
        {
            loggerFactory.CreateLogger(AuditLog).LogError(ex,message, args);
            return loggerFactory;
        }

        public static ILoggerFactory SystemInfo(this ILoggerFactory loggerFactory, string message, params object[] args)
        {
            loggerFactory.CreateLogger(SystemLog).LogInformation(message, args);
            return loggerFactory;
        }

        public static ILoggerFactory SystemWarn(this ILoggerFactory loggerFactory, string message, params object[] args)
        {
            loggerFactory.CreateLogger(SystemLog).LogWarning(message, args);
            return loggerFactory;
        }

        public static ILoggerFactory SystemError(this ILoggerFactory loggerFactory, string message, params object[] args)
        {
            loggerFactory.CreateLogger(SystemLog).LogError(message, args);
            return loggerFactory;
        }

        public static ILoggerFactory SystemError(this ILoggerFactory loggerFactory, Exception ex, string message, params object[] args)
        {
            loggerFactory.CreateLogger(SystemLog).LogError(ex, message, args);
            return loggerFactory;
        }
    }
}
