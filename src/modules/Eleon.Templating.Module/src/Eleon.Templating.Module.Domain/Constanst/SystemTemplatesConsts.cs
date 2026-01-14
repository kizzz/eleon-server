using Common.Module.Constants;
using Eleon.Templating.Module.Constants;
using Eleon.Templating.Module.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.Templating.Module.Eleon.Templating.Module.Domain.Constanst
{
  public static class SystemTemplatesConsts
  {
    public static Dictionary<string, Template> ActionTemplates = new()
    {
      {
          TemplatingDomainConstants.ActionSendSystemLog,
          new Template(Guid.Empty)
          {
            Name = TemplatingDomainConstants.ActionSendSystemLog,
            Type = TemplateType.Action,
            TemplateContent = """
            {
              "SystemLogLevel": 0,
              "Message": "Info system log"
            }
            """,
            Format = TextFormat.Json,
            TemplateId = "SendSystemLog",
            IsSystem = true,
          }
      },
      {
          TemplatingDomainConstants.ActionBankFaxJobWithTemplating,
          new Template(Guid.Empty)
          {
            Name = TemplatingDomainConstants.ActionBankFaxJobWithTemplating,
            Type = TemplateType.Action,
            TemplateContent = """
            {
              "BankCode": "BLLI",
              "DataSourceKey": "SFTP",
              "ReportRecepients": "vladymir.ogorodnytsky@eleonsoft.com",
              "EnablePostProcess": false,
              "RunDry": false,
              "DelayInMinutes": "0",
              "UseTemplatingModule": true,
              "HtmlReportHeaderTemplateKey": "BankFaxJobReportHTMLHeader",
              "HtmlReportBodyTemplateKey": "BankFaxJobReportHTMLBody",
              "CsvReportHeaderTemplateKey": "BankFaxJobReportCSVHeader",
              "CsvReportBodyTemplateKey": "BankFaxJobReportCSVBody",
            }
            """,
            Format = TextFormat.Json,
            TemplateId = "BankFaxJob",
            IsSystem = true,
          }
      },
      {
          TemplatingDomainConstants.ActionSendMailNotification,
          new Template(Guid.Empty)
          {
            Name = TemplatingDomainConstants.ActionSendMailNotification,
            Type = TemplateType.Action,
            TemplateContent = """
            { 
              "recipients": [ 
                { "type": 2, "recipientAddress": "vladymir.ogorodnytsky@eleonsoft.com" } 
              ], 
              "message": "<p>See the attached report.</p>", 
              "type": { 
                "Type": "Email", 
                "isHtml": true, 
                "subject": "Daily Report", 
                "attachments": { 
                  "report.txt": " SGVsbG8gd29ybGQuCkkgaGF2ZSBlbmNvZGVkIHRoaXMgbWVzc2FnZSBoZXJlOiBodHRwczovL3d3dy5iYXNlNjRlbmNvZGUub3JnLw== " 
                } 

              }, 

              "priority": 0, 

              "runImmidiate": true 

            } 
             

            """,
            Format = TextFormat.Json,
            TemplateId = "SendNotification",
            IsSystem = true,
          }
      },
      {
          TemplatingDomainConstants.ActionSendTelegram,
          new Template(Guid.Empty)
          {
            Name = TemplatingDomainConstants.ActionSendTelegram,
            Type = TemplateType.Action,
            TemplateContent = """
            { 
              "recipients": [ 
                { "type": 2, "recipientAddress": "vladymir.ogorodnytsky@eleonsoft.com" } 
              ], 
              "message": "Telegram Notification", 
              "type": { 
                "Type": "Social", 
                "Platform": "telegram",
                "ChannelId": "586191940"

              }, 

              "priority": 0, 

              "runImmidiate": true 

            } 
            """,
            Format = TextFormat.Json,
            TemplateId = "SendNotification",
            IsSystem = true,
          }
      },
      {
          TemplatingDomainConstants.ActionSendMessageNotification,
          new Template(Guid.Empty)
          {
            Name = TemplatingDomainConstants.ActionSendMessageNotification,
            Type = TemplateType.Action,
            TemplateContent = """
            { 
              "recipients": [{ 
                "refId": "9fe1dadd-ad2b-bbfd-92ad-3a1ddba6e04d", 
                "type": 0 
              }], 
              "message": "Message for SA", 
              "priority": 1, 
              "runImmidiate": true, 
              "type": { 
                  "Type": "Message", 
                  "isLocalizedData": true, 

                  "isRedirectEnabled": true, 
                  "redirectUrl": "/home",  
                  "dataParams": ["Eleonsoft", "2025"], 
                  "applicationName": "Admin" 
                } 
            } 
            
            """,
            Format = TextFormat.Json,
            TemplateId = "SendNotification",
            IsSystem = true,
          }
      }
    };

    public static Dictionary<string, Template> NotificationTemplates = new()
    {
        {
            TemplatingDomainConstants.NotificationEmail,
            new Template(Guid.Empty)
            {
                Name = TemplatingDomainConstants.NotificationEmail,
                Type = TemplateType.Notification,
                TemplateContent = "<b>💬 {logLevel}</b> <code>{priority}</code>{endline}" +
"<b>🕒</b> {time}{endline}" +
"<b>Message:</b> {message}{endline}" +
"<b>Exception:</b> <code>{exception_message}</code>{endline}" +
"<pre>{extraProperties}</pre>",
                RequiredPlaceholders = "logLevel;priority;endline;time;message;exception_message;extraProperties",
                IsSystem = true
            }
        },
        {
            TemplatingDomainConstants.NotificationTelegram,
            new Template(Guid.Empty)
            {
                Name = TemplatingDomainConstants.NotificationTelegram,
                Format = TextFormat.Scriban,
                Type = TemplateType.Notification,
                TemplateContent = """
{{- $lvl = logLevel | string.downcase -}}
{{- $icon = "ℹ️" -}}
{{- if $lvl == "critical" -}}{{ $icon = "❌" -}}
{{- else if $lvl == "warning" -}}{{ $icon = "⚠️" -}}
{{- end -}}

<b>{{$icon}} {{ logLevel }}</b> <code>{{ priority }}</code>{{ endline }}
<b>🕒</b> {{ time }}{{ endline }}
<b>💬 Message:</b> {{ message }}{{ endline }}
<b>⚠️ Exception:</b> <code>{{ exception_message }}</code>{{ endline }}
<pre>{{ extraProperties }}</pre>
""",
                RequiredPlaceholders = "logLevel;priority;endline;time;message;exception_message;extraProperties",
                IsSystem = true
            }
        },
        {
            TemplatingDomainConstants.TwoFAEmail,
            new Template(Guid.Empty)
            {
                Name = TemplatingDomainConstants.TwoFAEmail,
                Type = TemplateType.Notification,
                TemplateContent = @"
            <div style=""font-family: Arial, sans-serif; color: #333;"">
                <p>Hi,</p>
                <p>Please use the following One-Time Password (OTP) to proceed:</p>

                <p style=""font-size: 20px; font-weight: bold; color: #2c3e50;"">{code}</p>

                <p>For your security, please do not share this code with anyone.</p>

                <p>If you did not request this, please contact our support team immediately.</p>

                <p>
                    Best regards,<br>
                    Eleonsoft
                </p>
            </div>
            ",
                RequiredPlaceholders = "code;",
                IsSystem = true
            }
        },
      {
            TemplatingDomainConstants.TwoFaSMS,
            new Template(Guid.Empty)
            {
                Name = TemplatingDomainConstants.TwoFaSMS,
                Type = TemplateType.Notification,
                TemplateContent = "Dear {CustomerName}, your order {OrderNumber} has been confirmed. Total amount: {TotalAmount}.",
                RequiredPlaceholders = "CustomerName;OrderNumber;TotalAmount",
                IsSystem = true
            }
        },
      {
            TemplatingDomainConstants.BankFaxJobReportHtmlHeader,
            new Template(Guid.Empty)
            {
                Name = TemplatingDomainConstants.BankFaxJobReportHtmlHeader,
                Type = TemplateType.Notification,
                TemplateContent = """
<tr>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{AccountNo}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{ClientName}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{Valuedate}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{FinalValuationDate}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{FxOptionValuedate}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{B/S}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{Instrument}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{Call/Put}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{Ccypair}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{FXNearAmountmaj}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{FXNearAmountmin}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{FXFarAmountmaj}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{FXFarAmountmin}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{Amountmaj}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{Amountmin}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{SpotonDeal}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{Pips}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{FXNearFinalRate}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{FXFarFinalRate}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{Strike}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{TradeDate}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{DeliveryDate}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{PremiumCurrency}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{Premium}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{Bank}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{BarrierType}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{Barrier}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{Del/Ndf}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{TradeStatus}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{TradeId}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{Buy/Sell}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{BCCY}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{SCCY}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{LEGID}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{DMID}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{Updated}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{ExtraInfo}</b></th>
  <th style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px"><b>{ErrorMSG}</b></th>
</tr>
""",
                IsSystem = true
            }
        },
      {
            TemplatingDomainConstants.BankFaxJobReportHtmlBody,
            new Template(Guid.Empty)
            {
                Name = TemplatingDomainConstants.BankFaxJobReportHtmlBody,
                Type = TemplateType.Notification,
                TemplateContent = """
<tr>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{AccountNo}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{ClientName}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{Valuedate}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{FinalValuationDate}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{FxOptionValuedate}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{B/S}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{Instrument}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{Call/Put}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{Ccypair}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{FXNearAmountmaj}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{FXNearAmountmin}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{FXFarAmountmaj}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{FXFarAmountmin}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{Amountmaj}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{Amountmin}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{SpotonDeal}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{Pips}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{FXNearFinalRate}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{FXFarFinalRate}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{Strike}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{TradeDate}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{DeliveryDate}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{PremiumCurrency}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{Premium}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{Bank}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{BarrierType}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{Barrier}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{Del/Ndf}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{TradeStatus}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{TradeId}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{Buy/Sell}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{BCCY}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{SCCY}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{LEGID}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{DMID}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{Updated}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{ExtraInfo}</td>
  <td style="border-left:none;width:35px;color:black;font-size:8px;font-family:Arial;text-align:center;vertical-align:top;border-top:0.5px solid lightgrey;border-right:0.5px solid lightgrey;border-bottom:0.5px solid lightgrey;padding-top:1px;padding-right:1px;padding-left:1px">{ErrorMSG}</td>
</tr>
""",
                IsSystem = true
            }
        },
      {
            TemplatingDomainConstants.BankFaxJobReportCsvHeader,
            new Template(Guid.Empty)
            {
                Name = TemplatingDomainConstants.BankFaxJobReportCsvHeader,
                Type = TemplateType.Notification,
                TemplateContent = "{AccountNo},{ClientName},{Valuedate},{FinalValuationDate},{FxOptionValuedate},{B/S},{Instrument},{Call/Put},{Ccypair},{FXNearAmountmaj},{FXNearAmountmin},{FXFarAmountmaj},{FXFarAmountmin},{Amountmaj},{Amountmin},{SpotonDeal},{Pips},{FXNearFinalRate},{FXFarFinalRate},{Strike},{TradeDate},{DeliveryDate},{PremiumCurrency},{Premium},{Bank},{BarrierType},{Barrier},{Del/Ndf},{TradeStatus},{TradeId},{Buy/Sell},{BCCY},{SCCY},{LEGID},{DMID},{Updated},{ExtraInfo},{ErrorMSG}",
                IsSystem = true
            }
      },
            {
            TemplatingDomainConstants.BankFaxJobReportCsvBody,
            new Template(Guid.Empty)
            {
                Name = TemplatingDomainConstants.BankFaxJobReportCsvBody,
                Type = TemplateType.Notification,
                TemplateContent = "\"{AccountNo}\",\"{ClientName}\",\"{Valuedate}\",\"{FinalValuationDate}\",\"{FxOptionValuedate}\",\"{B/S}\",\"{Instrument}\",\"{Call/Put}\",\"{Ccypair}\",\"{FXNearAmountmaj}\",\"{FXNearAmountmin}\",\"{FXFarAmountmaj}\",\"{FXFarAmountmin}\",\"{Amountmaj}\",\"{Amountmin}\",\"{SpotonDeal}\",\"{Pips}\",\"{FXNearFinalRate}\",\"{FXFarFinalRate}\",\"{Strike}\",\"{TradeDate}\",\"{DeliveryDate}\",\"{PremiumCurrency}\",\"{Premium}\",\"{Bank}\",\"{BarrierType}\",\"{Barrier}\",\"{Del/Ndf}\",\"{TradeStatus}\",\"{TradeId}\",\"{Buy/Sell}\",\"{BCCY}\",\"{SCCY}\",\"{LEGID}\",\"{DMID}\",\"{Updated}\",\"{ExtraInfo}\",\"{ErrorMSG}\"",
                IsSystem = true
            }
        }
    };
  }
}
