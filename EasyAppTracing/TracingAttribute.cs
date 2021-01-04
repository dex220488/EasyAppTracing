using ArxOne.MrAdvice.Advice;
using EasyAppTracing.Entities.TraceSettings;
using Newtonsoft.Json;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.Email;
using System;
using System.Collections.Generic;
using System.IO;

namespace EasyAppTracing
{
    public class TracingAttribute : BaseLoggingAdviseAttribute
    {
        private readonly string serilogFilePath;
        private readonly ILogger fileTracing;
        private readonly ILogger emailTracing;
        private readonly ILogger elasticSearchTracing;
        private readonly Settings settings;

        public TracingAttribute()
        {
            settings = new Settings();

            string infoTracingFilePath = settings.GetPath(Entities.Enums.TraceFileType.InfoTracing);
            string errorTracingFilePath = settings.GetPath(Entities.Enums.TraceFileType.ErrorTracing);
            serilogFilePath = settings.GetPath(Entities.Enums.TraceFileType.Serilog);

            fileTracing = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(infoTracingFilePath, Serilog.Events.LogEventLevel.Information, rollingInterval: RollingInterval.Day, shared: true)
                .WriteTo.File(errorTracingFilePath, Serilog.Events.LogEventLevel.Error, rollingInterval: RollingInterval.Day, shared: true)
                .Destructure.ToMaximumStringLength(settings.GlobalSettings.FileSettings.MaxStringLength)
                .CreateLogger();

            emailTracing = new LoggerConfiguration()
                 .WriteTo.Email(new EmailConnectionInfo
                 {
                     IsBodyHtml = true,
                     FromEmail = settings.GlobalSettings.EmailSettings.FromEmail,
                     ToEmail = settings.GlobalSettings.EmailSettings.ToEmail,
                     MailServer = settings.GlobalSettings.EmailSettings.SmtpServer,
                     EmailSubject = settings.GlobalSettings.TracingId
                 },
                batchPostingLimit: 1)
                .Destructure.ToMaximumStringLength(settings.GlobalSettings.EmailSettings.MaxStringLength)
                .CreateLogger();

            elasticSearchTracing = new LoggerConfiguration()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(settings.GlobalSettings.ElasticSearchSettings.NodeUri))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                    IndexFormat = $"{settings.GlobalSettings.TracingId}-{DateTime.Now:yyyy-MM-dd}"
                })
                .CreateLogger();
        }

        public override void OnEntry(MethodAdviceContext context)
        {
            Serilog.Debugging.SelfLog.Enable(msg => WriteSerilogFile(msg));

            if (settings.GlobalSettings.EnableInformationTrace)
            {
                var inputParams = JsonConvert.SerializeObject(context.Arguments, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

                if (settings.GlobalSettings.FileSettings.Enable)
                {
                    fileTracing.Information($"\r\n### OnEntry Method: {context.TargetMethod}; User: {Environment.UserName}; DateTime: {DateTime.Now}");
                    fileTracing.Information(inputParams);
                }

                if (settings.GlobalSettings.EmailSettings.Enable)
                {
                    //
                }

                if (settings.GlobalSettings.ElasticSearchSettings.Enable)
                {
                    elasticSearchTracing.Information($"\r\n### OnEntry Method: {context.TargetMethod}; User: {Environment.UserName}; DateTime: {DateTime.Now}");
                    elasticSearchTracing.Information(inputParams);
                }
            }
        }

        public override void OnException(MethodAdviceContext context, Exception ex)
        {
            Serilog.Debugging.SelfLog.Enable(msg => WriteSerilogFile(msg));

            if (settings.GlobalSettings.EnableErrorTrace)
            {
                var inputParams = JsonConvert.SerializeObject(context.Arguments, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                var exceptionsDetails = JsonConvert.SerializeObject(FlattenHierarchyException(ex), new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

                if (settings.GlobalSettings.FileSettings.Enable)
                {
                    fileTracing.Error($"\r\n### OnException Method: {context.TargetMethod}; User: {Environment.UserName}; DateTime: {DateTime.Now}");
                    fileTracing.Error(inputParams);
                    fileTracing.Error($"Exception Message => {ex.Message} StackTrace => {ex.StackTrace}");
                    fileTracing.Error("Inner Exceptions Details");
                    fileTracing.Error(exceptionsDetails);
                }

                if (settings.GlobalSettings.EmailSettings.Enable)
                {
                    //
                }

                if (settings.GlobalSettings.ElasticSearchSettings.Enable)
                {
                    elasticSearchTracing.Error($"\r\n### OnException Method: {context.TargetMethod}; User: {Environment.UserName}; DateTime: {DateTime.Now}");
                    elasticSearchTracing.Error(inputParams);
                    elasticSearchTracing.Error($"Exception Message => {ex.Message} StackTrace => {ex.StackTrace}");
                    elasticSearchTracing.Error("Inner Exceptions Details");
                    elasticSearchTracing.Error(exceptionsDetails);
                }
            }
        }

        public override void OnSuccess(MethodAdviceContext context)
        {
            Serilog.Debugging.SelfLog.Enable(msg => WriteSerilogFile(msg));

            if (context.HasReturnValue)
            {
                if (context.ReturnValue != null)
                {
                    if (settings.GlobalSettings.EnableInformationTrace)
                    {
                        var outputParams = JsonConvert.SerializeObject(context.ReturnValue, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

                        if (settings.GlobalSettings.FileSettings.Enable)
                        {
                            fileTracing.Information($"\r\n### OnExit Method: {context.TargetMethod}; User: {Environment.UserName}; DateTime: {DateTime.Now}");
                            fileTracing.Information(outputParams);
                        }

                        if (settings.GlobalSettings.EmailSettings.Enable)
                        {
                            //
                        }

                        if (settings.GlobalSettings.ElasticSearchSettings.Enable)
                        {
                            elasticSearchTracing.Information($"\r\n### OnExit Method: {context.TargetMethod}; User: {Environment.UserName}; DateTime: {DateTime.Now}");
                            elasticSearchTracing.Information(outputParams);
                        }
                    }
                }
            }
        }

        public void Write(string targetMethod, string traceMessage, bool isError = false)
        {
            if (settings.GlobalSettings.FileSettings.Enable)
            {
                if (isError)
                {
                    fileTracing.Error($"\r\n### OnWrite Method: {targetMethod}; User: {Environment.UserName}; DateTime: {DateTime.Now}");
                    fileTracing.Error($"Message : {traceMessage}");
                }
                else
                {
                    fileTracing.Information($"\r\n### OnWrite Method: {targetMethod}; User: {Environment.UserName}; DateTime: {DateTime.Now}");
                    fileTracing.Information($"Message : {traceMessage}");
                }
            }

            if (settings.GlobalSettings.EmailSettings.Enable)
            {
                //
            }

            if (settings.GlobalSettings.ElasticSearchSettings.Enable)
            {
                if (isError)
                {
                    elasticSearchTracing.Error($"\r\n### OnWrite Method: {targetMethod}; User: {Environment.UserName}; DateTime: {DateTime.Now}");
                    elasticSearchTracing.Error($"Message : {traceMessage}");
                }
                else
                {
                    elasticSearchTracing.Information($"\r\n### OnWrite Method: {targetMethod}; User: {Environment.UserName}; DateTime: {DateTime.Now}");
                    elasticSearchTracing.Information($"Message : {traceMessage}");
                }
            }
        }

        private void WriteSerilogFile(string message)
        {
            try
            {
                using (StreamWriter writer = File.AppendText(serilogFilePath))
                {
                    writer.WriteLine(message);
                }
            }
            catch
            {
                // empty for unhandle exceptions
            }
        }

        private IEnumerable<Exception> FlattenHierarchyException(Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }
            else
            {
                var innerException = ex;
                do
                {
                    yield return innerException;
                    innerException = innerException.InnerException;
                } while (innerException != null);
            }
        }
    }
}