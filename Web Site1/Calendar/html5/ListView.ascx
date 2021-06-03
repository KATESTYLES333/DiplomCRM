<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Calendar.html5.ListView" %>
<script runat="server">
/**********************************************************************************************************************
 * SplendidCRM is a Customer Relationship Management program created by SplendidCRM Software, Inc. 
 * Copyright (C) 2005-2011 SplendidCRM Software, Inc. All rights reserved.
 * 
 * This program is free software: you can redistribute it and/or modify it under the terms of the 
 * GNU Affero General Public License as published by the Free Software Foundation, either version 3 
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
 * See the GNU Affero General Public License for more details.
 * 
 * You should have received a copy of the GNU Affero General Public License along with this program. 
 * If not, see <http://www.gnu.org/licenses/>. 
 * 
 * You can contact SplendidCRM Software, Inc. at email address support@splendidcrm.com. 
 * 
 * In accordance with Section 7(b) of the GNU Affero General Public License version 3, 
 * the Appropriate Legal Notices must display the following words on all interactive user interfaces: 
 * "Copyright (C) 2005-2011 SplendidCRM Software, Inc. All rights reserved."
 *********************************************************************************************************************/
</script>
<div id="divListView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Calendar" Title="Calendar.LBL_MODULE_TITLE" EnableModuleLabel="false" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />
	
	<%@ Register TagPrefix="SplendidCRM" Tagname="RestUtils" Src="~/_controls/RestUtils.ascx" %>
	<SplendidCRM:RestUtils Runat="Server" />

	<div id="divError" class="error"></div>
	<div id="divCalendar" style="width: 100%" align="center"></div>

	<SplendidCRM:InlineScript runat="server">
		<script type="text/javascript">
// 11/06/2013 Paul.  Make sure to JavaScript escape the text as the various languages may introduce accents. 
TERMINOLOGY['.LBL_NONE'                      ] = '<%# Sql.EscapeJavaScript(L10n.Term(".LBL_NONE"                      )) %>';
TERMINOLOGY['Calendar.LNK_VIEW_CALENDAR'     ] = '<%# Sql.EscapeJavaScript(L10n.Term("Calendar.LNK_VIEW_CALENDAR"     )) %>';
TERMINOLOGY['Calendar.LBL_MONTH'             ] = '<%# Sql.EscapeJavaScript(L10n.Term("Calendar.LBL_MONTH"             )) %>';
TERMINOLOGY['Calendar.LBL_WEEK'              ] = '<%# Sql.EscapeJavaScript(L10n.Term("Calendar.LBL_WEEK"              )) %>';
TERMINOLOGY['Calendar.LBL_DAY'               ] = '<%# Sql.EscapeJavaScript(L10n.Term("Calendar.LBL_DAY"               )) %>';
TERMINOLOGY['Calendar.LBL_SHARED'            ] = '<%# Sql.EscapeJavaScript(L10n.Term("Calendar.LBL_SHARED"            )) %>';
TERMINOLOGY['Calendar.LBL_ALL_DAY'           ] = '<%# Sql.EscapeJavaScript(L10n.Term("Calendar.LBL_ALL_DAY"           )) %>';
TERMINOLOGY['Calendar.YearMonthPattern'      ] = '<%# Sql.EscapeJavaScript(DateTimeFormat.YearMonthPattern             ) %>';
TERMINOLOGY['Calendar.MonthDayPattern'       ] = '<%# Sql.EscapeJavaScript(DateTimeFormat.MonthDayPattern              ) %>';
TERMINOLOGY['Calendar.LongDatePattern'       ] = '<%# Sql.EscapeJavaScript(DateTimeFormat.LongDatePattern              ) %>';
TERMINOLOGY['Calendar.FirstDayOfWeek'        ] = '<%# (int) DateTimeFormat.FirstDayOfWeek       %>';
TERMINOLOGY['Calendar.LNK_NEW_APPOINTMENT'   ] = '<%# Sql.EscapeJavaScript(L10n.Term("Calendar.LNK_NEW_APPOINTMENT"   )) %>';
TERMINOLOGY['Calls.LNK_NEW_CALL'             ] = '<%# Sql.EscapeJavaScript(L10n.Term("Calls.LNK_NEW_CALL"             )) %>';
TERMINOLOGY['Calls.LNK_NEW_MEETING'          ] = '<%# Sql.EscapeJavaScript(L10n.Term("Calls.LNK_NEW_MEETING"          )) %>';
TERMINOLOGY['Calls.LBL_SUBJECT'              ] = '<%# Sql.EscapeJavaScript(L10n.Term("Calls.LBL_SUBJECT"              )) %>';
TERMINOLOGY['Calls.LBL_DATE_TIME'            ] = '<%# Sql.EscapeJavaScript(L10n.Term("Calls.LBL_DATE_TIME"            )) %>';
TERMINOLOGY['Calls.LBL_DURATION'             ] = '<%# Sql.EscapeJavaScript(L10n.Term("Calls.LBL_DURATION"             )) %>';
TERMINOLOGY['Calls.LBL_HOURS_MINUTES'        ] = '<%# Sql.EscapeJavaScript(L10n.Term("Calls.LBL_HOURS_MINUTES"        )) %>';
TERMINOLOGY['Calls.LBL_ALL_DAY'              ] = '<%# Sql.EscapeJavaScript(L10n.Term("Calls.LBL_ALL_DAY"              )) %>';
TERMINOLOGY['.LBL_REQUIRED_SYMBOL'           ] = '<%# Sql.EscapeJavaScript(L10n.Term(".LBL_REQUIRED_SYMBOL"           )) %>';
TERMINOLOGY['.ERR_REQUIRED_FIELD'            ] = '<%# Sql.EscapeJavaScript(L10n.Term(".ERR_REQUIRED_FIELD"            )) %>';
TERMINOLOGY['.LBL_SAVE_BUTTON_LABEL'         ] = '<%# Sql.EscapeJavaScript(L10n.Term(".LBL_SAVE_BUTTON_LABEL"         )) %>';
TERMINOLOGY['.LBL_SAVE_BUTTON_TITLE'         ] = '<%# Sql.EscapeJavaScript(L10n.Term(".LBL_SAVE_BUTTON_TITLE"         )) %>';
TERMINOLOGY['.LBL_CANCEL_BUTTON_LABEL'       ] = '<%# Sql.EscapeJavaScript(L10n.Term(".LBL_CANCEL_BUTTON_LABEL"       )) %>';
TERMINOLOGY['.LBL_CANCEL_BUTTON_TITLE'       ] = '<%# Sql.EscapeJavaScript(L10n.Term(".LBL_CANCEL_BUTTON_TITLE"       )) %>';
TERMINOLOGY_LISTS['month_names_dom'          ] = ['<%# String.Join("', '", Sql.EscapeJavaScript(DateTimeFormat.MonthNames           )) %>'];
TERMINOLOGY_LISTS['short_month_names_dom'    ] = ['<%# String.Join("', '", Sql.EscapeJavaScript(DateTimeFormat.AbbreviatedMonthNames)) %>'];
TERMINOLOGY_LISTS['day_names_dom'            ] = ['<%# String.Join("', '", Sql.EscapeJavaScript(DateTimeFormat.DayNames             )) %>'];
TERMINOLOGY_LISTS['short_day_names_dom'      ] = ['<%# String.Join("', '", Sql.EscapeJavaScript(DateTimeFormat.AbbreviatedDayNames  )) %>'];
TERMINOLOGY_LISTS['repeat_type_dom'          ] = ['Daily', 'Weekly', 'Monthly', 'Yearly'];
TERMINOLOGY['.repeat_type_dom.Daily'         ] = '<%# Sql.EscapeJavaScript(L10n.Term(".repeat_type_dom.Daily"         )) %>';
TERMINOLOGY['.repeat_type_dom.Weekly'        ] = '<%# Sql.EscapeJavaScript(L10n.Term(".repeat_type_dom.Weekly"        )) %>';
TERMINOLOGY['.repeat_type_dom.Monthly'       ] = '<%# Sql.EscapeJavaScript(L10n.Term(".repeat_type_dom.Monthly"       )) %>';
TERMINOLOGY['.repeat_type_dom.Yearly'        ] = '<%# Sql.EscapeJavaScript(L10n.Term(".repeat_type_dom.Yearly"        )) %>';

TERMINOLOGY['Calendar.LBL_REPEAT_TAB'        ] = '<%# Sql.EscapeJavaScript(L10n.Term("Calendar.LBL_REPEAT_TAB"        )) %>';
TERMINOLOGY['Calendar.LBL_REPEAT_END_AFTER'  ] = '<%# Sql.EscapeJavaScript(L10n.Term("Calendar.LBL_REPEAT_END_AFTER"  )) %>';
TERMINOLOGY['Calendar.LBL_REPEAT_OCCURRENCES'] = '<%# Sql.EscapeJavaScript(L10n.Term("Calendar.LBL_REPEAT_OCCURRENCES")) %>';
TERMINOLOGY['Calendar.LBL_REPEAT_INTERVAL'   ] = '<%# Sql.EscapeJavaScript(L10n.Term("Calendar.LBL_REPEAT_INTERVAL"   )) %>';
TERMINOLOGY['Calls.LBL_REPEAT_TYPE'          ] = '<%# Sql.EscapeJavaScript(L10n.Term("Calls.LBL_REPEAT_TYPE"          )) %>';
TERMINOLOGY['Calls.LBL_REPEAT_UNTIL'         ] = '<%# Sql.EscapeJavaScript(L10n.Term("Calls.LBL_REPEAT_UNTIL"         )) %>';
TERMINOLOGY['Calls.LBL_REPEAT_DOW'           ] = '<%# Sql.EscapeJavaScript(L10n.Term("Calls.LBL_REPEAT_DOW"           )) %>';

CONFIG['calendar.hour_start'                 ] = '<%# Sql.EscapeJavaScript(Application["CONFIG.calendar.hour_start"            ]) %>';
CONFIG['GoogleCalendar.HolidayCalendars'     ] = '<%# Sql.EscapeJavaScript(Application["CONFIG.GoogleCalendar.HolidayCalendars"]) %>';

background.CalendarView_GetCalendar = function(dtDATE_START, dtDATE_END, gASSIGNED_USER_ID, callback, context)
{
	var xhr = CreateSplendidRequest('Rest.svc/GetCalendar?DATE_START=' + encodeURIComponent(dtDATE_START) + '&DATE_END=' + encodeURIComponent(dtDATE_END)  + '&ASSIGNED_USER_ID=' + encodeURIComponent(gASSIGNED_USER_ID), 'GET');
	xhr.onreadystatechange = function()
	{
		if ( xhr.readyState == 4 )
		{
			GetSplendidResult(xhr, function(result)
			{
				try
				{
					if ( result.status == 200 )
					{
						if ( result.d !== undefined )
						{
							callback.call(context||this, 1, result.d.results);
						}
						else
						{
							callback.call(context||this, -1, xhr.responseText);
						}
					}
					else
					{
						if ( result.ExceptionDetail !== undefined )
							callback.call(context||this, -1, result.ExceptionDetail.Message);
						else
							callback.call(context||this, -1, xhr.responseText);
					}
				}
				catch(e)
				{
					callback.call(context||this, -1, SplendidError.FormatError(e, 'CalendarView_GetCalendar'));
				}
			}, context||this);
		}
	}
	try
	{
		xhr.send();
	}
	catch(e)
	{
		if ( e.number != -2146697208 )
			callback.call(context||this, -1, SplendidError.FormatError(e, 'CalendarView_GetCalendar'));
	}
};

$(document).ready(function()
{
	var oCalendarViewUI = new CalendarViewUI();
	oCalendarViewUI.Render(null, null, function(status, message)
	{
	}, oCalendarViewUI);
});
		</script>
	</SplendidCRM:InlineScript>
</div>


