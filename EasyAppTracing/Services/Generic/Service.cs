using EasyAppTracing.Entities.TraceSettings;
using System;
using System.Text;
using static EasyAppTracing.Entities.Enums;

namespace EasyAppTracing.Services.Generic
{
    internal class Service
    {
        public string SetEmailContent(TraceFileType traceType, Settings settings, string targetMethod, string content)
        {
            StringBuilder message = new StringBuilder();

            string headerTraceType;
            switch (traceType)
            {
                case TraceFileType.InfoTracing:
                    headerTraceType = "Information Trace";
                    break;

                case TraceFileType.ErrorTracing:
                    headerTraceType = "Error Trace";
                    break;

                case TraceFileType.SuccessTracing:
                    headerTraceType = "Exit Trace";
                    break;

                default:
                    headerTraceType = "Information Trace";
                    break;
            }

            if (settings.GlobalSettings.EmailSettings.IsBodyHtml)
            {
                message.Append("<table style='width:100%'>");
                message.Append($"<tr><td colspan='4' align='center'><b>{headerTraceType}</b></td></tr>");

                message.Append("<tr><td><b>Tracing Id</b></td>");
                message.Append("<td><b>Target Method</b></td>");
                message.Append("<td><b>User</b></td>");
                message.Append("<td><b>DateTime</b></td></tr>");

                message.Append($"<tr><td>{settings.GlobalSettings.TracingId}</td>");
                message.Append($"<td>{targetMethod}</td>");
                message.Append($"<td>{Environment.UserName}</td>");
                message.Append($"<td>{DateTime.Now}</td></tr>");

                message.Append("<tr><td colspan='4' align='center'><b>Input Parameters</b></td></tr>");
                message.Append($"<tr><td colspan='4'>{content}</td></tr>");

                message.Append("</table>");
            }
            else
            {
                message.Append($"{headerTraceType}{Environment.NewLine}");
                message.Append($"Tracing Id: {settings.GlobalSettings.TracingId}{Environment.NewLine}");
                message.Append($"Target Method: {targetMethod}{Environment.NewLine}");
                message.Append($"User: {Environment.UserName}{Environment.NewLine}");
                message.Append($"DateTime: {DateTime.Now}{Environment.NewLine}");
                message.Append($"Input Parameters: {Environment.NewLine}");
                message.Append($"{content}");
            }

            return message.ToString();
        }
    }
}