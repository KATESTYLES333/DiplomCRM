/* Copyright (C) 2013 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */

var bSharedCalendar = false;
function ToggleSharedCalendar()
{
	try
	{
		bSharedCalendar = bSharedCalendar ? false : true;
		if ( bSharedCalendar )
			$("#divCalendar").find('span.fc-button-' + 'shared').addClass('fc-state-active');
		else
			$("#divCalendar").find('span.fc-button-' + 'shared').removeClass('fc-state-active');
		$("#divCalendar").fullCalendar('refetchEvents');
	}
	catch(e)
	{
		SplendidError.SystemMessage(SplendidError.FormatError(e, 'ToggleSharedCalendar'));
	}
}

function CalendarViewUI()
{
	this.YearMonthPattern      = L10n.Term('Calendar.YearMonthPattern');
	this.MonthDayPattern       = L10n.Term('Calendar.MonthDayPattern' );
	this.LongDatePattern       = L10n.Term('Calendar.LongDatePattern' );
	this.ShortTimePattern      = Security.USER_TIME_FORMAT();
	this.ShortDatePattern      = Security.USER_DATE_FORMAT();
	this.FirstDayOfWeek        = Sql.ToInteger(L10n.Term('Calendar.FirstDayOfWeek'));
	this.MonthNames            = L10n.GetListTerms('month_names_dom'      );
	this.AbbreviatedMonthNames = L10n.GetListTerms('short_month_names_dom');
	this.DayNames              = L10n.GetListTerms('day_names_dom'        );
	this.AbbreviatedDayNames   = L10n.GetListTerms('short_day_names_dom'  );

	if ( Sql.IsEmptyString(this.YearMonthPattern) || this.YearMonthPattern == 'Calendar.YearMonthPattern' ) this.YearMonthPattern = 'MMMM, yyyy';
	if ( Sql.IsEmptyString(this.MonthDayPattern ) || this.MonthDayPattern  == 'Calendar.MonthDayPattern'  ) this.MonthDayPattern  = 'MMMM dd';
	if ( Sql.IsEmptyString(this.LongDatePattern ) || this.LongDatePattern  == 'Calendar.LongDatePattern'  ) this.LongDatePattern  = 'dddd, MMMM dd, yyyy';
	if ( Sql.IsEmptyString(this.ShortTimePattern)                                                         ) this.ShortTimePattern = 'h:mm tt';
	if ( Sql.IsEmptyString(this.ShortDatePattern)                                                         ) this.ShortDatePattern = 'MM/dd/yyyy';
	if ( Sql.IsEmptyString(this.FirstDayOfWeek  ) || isNaN(this.FirstDayOfWeek)                           ) this.FirstDayOfWeek   = 0;
	if ( this.MonthNames            == null || this.MonthNames.length            == 0 ) this.MonthNames            = ['January','February','March','April','May','June','July','August','September','October','November','December'];
	if ( this.AbbreviatedMonthNames == null || this.AbbreviatedMonthNames.length == 0 ) this.AbbreviatedMonthNames = ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'];
	if ( this.DayNames              == null || this.DayNames.length              == 0 ) this.DayNames              = ['Sunday','Monday','Tuesday','Wednesday','Thursday','Friday','Saturday'];
	if ( this.AbbreviatedDayNames   == null || this.AbbreviatedDayNames.length   == 0 ) this.AbbreviatedDayNames   = ['Sun','Mon','Tue','Wed','Thu','Fri','Sat'];
}

CalendarViewUI.prototype.Clear = function(sLayoutPanel, sActionsPanel)
{
	try
	{
		SplendidUI_Clear(sLayoutPanel, sActionsPanel);
		SplendidUI_ModuleHeader(sLayoutPanel, sActionsPanel, 'Calendar', null);
		var divCalendar = document.createElement('div');
		divCalendar.id          = 'divCalendar';
		divCalendar.style.width = '100%'       ;
		var divMainLayoutPanel = document.getElementById(sLayoutPanel);
		divMainLayoutPanel.appendChild(divCalendar);
	}
	catch(e)
	{
		SplendidError.SystemMessage(SplendidError.FormatError(e, 'CalendarViewUI.Clear'));
	}
};

CalendarViewUI.prototype.Render = function(sLayoutPanel, sActionsPanel, callback, context)
{
	var sCalendarDefaultView = '';
	if ( window.localStorage )
		sCalendarDefaultView = localStorage['CalendarDefaultView'];
	else
		sCalendarDefaultView = getCookie('CalendarDefaultView');
	if ( Sql.IsEmptyString(sCalendarDefaultView) )
		sCalendarDefaultView = 'agendaDay';
	
	// 12/14/2014 Paul.  Header is too large on a mobile device. Remove the title. 
	var bIsMobile = isMobileDevice();
	if ( isMobileLandscape() )
		bIsMobile = false;

	var bgPage = chrome.extension.getBackgroundPage();
	// http://arshaw.com/fullcalendar/docs/
	var calendar = $('#divCalendar').fullCalendar
	({
		header:
		{
			  left  : 'agendaDay,agendaWeek,month {shared:ToggleSharedCalendar}'
			, center: (bIsMobile ? '' : 'title')
			, right : 'today prev,next'
		}
		, buttonText:
		{
			  prev    : '&nbsp;&#9668;&nbsp;'  // left triangle
			, next    : '&nbsp;&#9658;&nbsp;'  // right triangle
			, prevYear: '&nbsp;&lt;&lt;&nbsp;' // <<
			, nextYear: '&nbsp;&gt;&gt;&nbsp;' // >>
			, today   : L10n.Term("Calendar.LNK_VIEW_CALENDAR")
			, month   : L10n.Term("Calendar.LBL_MONTH"        )
			, week    : L10n.Term("Calendar.LBL_WEEK"         )
			, day     : L10n.Term("Calendar.LBL_DAY"          )
			, shared  : L10n.Term("Calendar.LBL_SHARED"       )
		}
		, titleFormat:
		{
			  month   : this.YearMonthPattern
			, week    : this.MonthDayPattern + "[ yyyy]{ '&#8212;' " + this.MonthDayPattern + ' yyyy}'
			, day     : this.LongDatePattern
		}
		, timeFormat:
		{
			  agenda  : this.ShortTimePattern + '{ - ' + this.ShortTimePattern + '}'
			, ''      : this.ShortTimePattern 
		}
		, columnFormat:
		{
			  month   : 'ddd'
			, week    : 'ddd '  + Trim(this.ShortDatePattern.replace('yyyy', '').replace(new RegExp(/\//g), ' ')).replace(new RegExp(/ /g), '/').replace('MM', 'M').replace('dd', 'd')
			, day     : 'dddd ' + Trim(this.ShortDatePattern.replace('yyyy', '').replace(new RegExp(/\//g), ' ')).replace(new RegExp(/ /g), '/').replace('MM', 'M').replace('dd', 'd')
		}
		, height             : ($(window).height() > 0 ? $(window).height() - 180 : 800)
		, defaultView        : sCalendarDefaultView
		, editable           : true
		, selectable         : true
		, selectHelper       : true
		, allDaySlot         : true
		, allDayText         : L10n.Term("Calendar.LBL_ALL_DAY")
		, slotMinutes        : 30
		, defaultEventMinutes: 60
		, firstHour          : Sql.ToInteger(bgPage.SplendidCache.Config('calendar.hour_start'))
		, firstDay           : this.FirstDayOfWeek       
		, monthNames         : this.MonthNames           
		, monthNamesShort    : this.AbbreviatedMonthNames
		, dayNames           : this.DayNames             
		, dayNamesShort      : this.AbbreviatedDayNames  
		, select: function(start, end, allDay)
		{
			// 02/24/2013 Paul.  Until we can support allDay events, when clicking on a day of the month, change to the day view. 
			if ( Math.round((end - start)/1000) == 0 )
			{
				var view = calendar.fullCalendar('getView');
				if ( view.name == 'month' )
				{
					calendar.fullCalendar('changeView', 'agendaDay');
					calendar.fullCalendar('gotoDate', start.getFullYear(), start.getMonth(), start.getDate());
					return;
				}
			}
			var $dialog = $('<div id="divNewAppointmentPopup"></div>');
			$dialog.dialog(
			{
				  modal    : true
				, resizable: true
				, width    : ($(window).height() > 0 ? 540 : 580)
				, height   : ($(window).height() > 0 ? 320 : 420)
				, title    : L10n.Term("Calendar.LNK_NEW_APPOINTMENT")
				, create: function(event, ui)
				{
					var TOTAL_SECONDS    = Math.round((end - start)/1000);
					var TOTAL_MINUTES    = Math.round(TOTAL_SECONDS/60);
					var DURATION_HOURS   = Math.floor(TOTAL_MINUTES/60);
					var DURATION_MINUTES = TOTAL_MINUTES % 60;
					// 03/10/2013 Paul.  Add ALL_DAY_EVENT. 
					var ALL_DAY_EVENT    = allDay;
					if ( ALL_DAY_EVENT )
					{
						DURATION_HOURS   = 24;
						DURATION_MINUTES = 0;
					}
					
					var divNewAppointmentPopup = document.getElementById('divNewAppointmentPopup');
					// sHTML += '<table class="tabEditView">';
					var tblEditView = document.createElement('table');
					tblEditView.className = 'tabEditView';
					divNewAppointmentPopup.appendChild(tblEditView);
					var col = document.createElement('col');
					tblEditView.appendChild(col);
					col.setAttribute('width', '20%');
					col = document.createElement('col');
					tblEditView.appendChild(col);
					col.setAttribute('width', '30%');
					col = document.createElement('col');
					tblEditView.appendChild(col);
					col.setAttribute('width', '20%');
					col = document.createElement('col');
					tblEditView.appendChild(col);
					col.setAttribute('width', '30%');
					var tbody = document.createElement('tbody');
					tblEditView.appendChild(tbody);
					
					var tr  = null;
					var td  = null;
					var txt = null;
					var spn = null;
					var nbsp = String.fromCharCode(160);
					
					// sHTML += '	<tr>';
					// sHTML += '		<td>';
					// sHTML += '			<input ID="divNewAppointmentPopup_radScheduleCall"    Name="grpSchedule" type="radio" class="radio" value="Calls" checked /> ' + L10n.Term("Calls.LNK_NEW_CALL"   );
					// sHTML += '		</td>';
					// sHTML += '		<td>';
					// sHTML += '			<input ID="divNewAppointmentPopup_radScheduleMeeting" Name="grpSchedule" type="radio" class="radio" value="Meetings"      /> ' + L10n.Term("Calls.LNK_NEW_MEETING");
					// sHTML += '		</td>';
					// sHTML += '		<td />';
					// sHTML += '		<td />';
					// sHTML += '	</tr>';
					tr = document.createElement('tr');
					tbody.appendChild(tr);
					td = document.createElement('td');
					tr.appendChild(td);

					var divNewAppointmentPopup_radScheduleCall = document.createElement('input');
					divNewAppointmentPopup_radScheduleCall.id           = 'divNewAppointmentPopup_radScheduleCall';
					divNewAppointmentPopup_radScheduleCall.name         = 'grpSchedule';
					divNewAppointmentPopup_radScheduleCall.type         = 'radio'      ;
					divNewAppointmentPopup_radScheduleCall.value        = 'Calls'      ;
					divNewAppointmentPopup_radScheduleCall.className    = 'radio'      ;
					td.appendChild(divNewAppointmentPopup_radScheduleCall);
					divNewAppointmentPopup_radScheduleCall.checked      = true;
					txt = document.createTextNode(L10n.Term('Calls.LNK_NEW_CALL') + nbsp + nbsp);
					td.appendChild(txt);
					
					td = document.createElement('td');
					tr.appendChild(td);
					var divNewAppointmentPopup_radScheduleMeeting = document.createElement('input');
					divNewAppointmentPopup_radScheduleMeeting.id        = 'divNewAppointmentPopup_radScheduleCall';
					divNewAppointmentPopup_radScheduleMeeting.name      = 'grpSchedule';
					divNewAppointmentPopup_radScheduleMeeting.type      = 'radio'      ;
					divNewAppointmentPopup_radScheduleMeeting.value     = 'Meetings'   ;
					divNewAppointmentPopup_radScheduleMeeting.className = 'radio'      ;
					td.appendChild(divNewAppointmentPopup_radScheduleMeeting);
					txt = document.createTextNode(L10n.Term('Calls.LNK_NEW_MEETING'));
					td.appendChild(txt);
					
					td = document.createElement('td');
					tr.appendChild(td);
					td = document.createElement('td');
					tr.appendChild(td);

					// sHTML += '	<tr>';
					// sHTML += '		<td colspan="4" height="5px"></td>';
					// sHTML += '	</tr>';
					tr = document.createElement('tr');
					tbody.appendChild(tr);
					td = document.createElement('td');
					tr.appendChild(td);
					td.setAttribute('colspan', '4'  );
					td.setAttribute('height' , '5px');
					
					// sHTML += '	<tr>';
					// sHTML += '		<td>';
					// sHTML += '			' + L10n.Term("Calls.LBL_SUBJECT") + ' <span class="required">' + L10n.Term(".LBL_REQUIRED_SYMBOL") + '</>';
					// sHTML += '		</td>';
					// sHTML += '		<td>';
					// sHTML += '			<input type="text" ID="divNewAppointmentPopup_txtNAME" size="30" maxlength="255" />';
					// sHTML += '		</td>';
					// sHTML += '		<td>';
					// sHTML += '			<span ID="divNewAppointmentPopup_reqNAME" class="required" style="display:none">' + L10n.Term(".ERR_REQUIRED_FIELD") + '</span>';
					// sHTML += '		</td>';
					// sHTML += '		<td />';
					// sHTML += '	</tr>';
					tr = document.createElement('tr');
					tbody.appendChild(tr);
					td = document.createElement('td');
					tr.appendChild(td);
					txt = document.createTextNode(L10n.Term('Calls.LBL_SUBJECT'));
					td.appendChild(txt);
					spn = document.createElement('span');
					spn.className = 'required';
					td.appendChild(spn);
					txt = document.createTextNode(L10n.Term('.LBL_REQUIRED_SYMBOL'));
					spn.appendChild(txt);
					td = document.createElement('td');
					tr.appendChild(td);
					var divNewAppointmentPopup_txtNAME = document.createElement('input');
					divNewAppointmentPopup_txtNAME.id            = 'divNewAppointmentPopup_txtNAME';
					divNewAppointmentPopup_txtNAME.type          = 'text' ;
					divNewAppointmentPopup_txtNAME.maxLength     = '255'  ;
					divNewAppointmentPopup_txtNAME.style.width   = '200px';
					td.appendChild(divNewAppointmentPopup_txtNAME);
					td = document.createElement('td');
					tr.appendChild(td);
					var divNewAppointmentPopup_reqNAME = document.createElement('span');
					divNewAppointmentPopup_reqNAME.id            = 'divNewAppointmentPopup_reqNAME';
					divNewAppointmentPopup_reqNAME.className     = 'required';
					divNewAppointmentPopup_reqNAME.style.display = 'none';
					td.appendChild(divNewAppointmentPopup_reqNAME);
					txt = document.createTextNode(nbsp + nbsp + L10n.Term('.ERR_REQUIRED_FIELD'));
					divNewAppointmentPopup_reqNAME.appendChild(txt);
					td = document.createElement('td');
					tr.appendChild(td);
					
					// sHTML += '	</tr>';
					// sHTML += '		<td>';
					// sHTML += '			' + L10n.Term("Calls.LBL_DATE_TIME") + ' <span class="required">' + L10n.Term(".LBL_REQUIRED_SYMBOL") + '</>';
					// sHTML += '		</td>';
					// sHTML += '		<td>';
					// sHTML += '			<input type="text" ID="divNewAppointmentPopup_txtDATE_START" size="15" readonly="true" />';
					// sHTML += '		</td>';
					// sHTML += '		<td />';
					// sHTML += '		<td />';
					// sHTML += '	</tr>';
					tr = document.createElement('tr');
					tbody.appendChild(tr);
					td = document.createElement('td');
					tr.appendChild(td);
					txt = document.createTextNode(L10n.Term('Calls.LBL_DATE_TIME'));
					td.appendChild(txt);
					spn = document.createElement('span');
					spn.className = 'required';
					td.appendChild(spn);
					txt = document.createTextNode(L10n.Term('.LBL_REQUIRED_SYMBOL'));
					spn.appendChild(txt);
					td = document.createElement('td');
					tr.appendChild(td);
					var divNewAppointmentPopup_txtDATE_START = document.createElement('input');
					divNewAppointmentPopup_txtDATE_START.id          = 'divNewAppointmentPopup_DATE_START';
					divNewAppointmentPopup_txtDATE_START.type        = 'text';
					divNewAppointmentPopup_txtDATE_START.readOnly    = true  ;
					divNewAppointmentPopup_txtDATE_START.style.width = '120px';
					td.appendChild(divNewAppointmentPopup_txtDATE_START);
					td = document.createElement('td');
					tr.appendChild(td);
					td = document.createElement('td');
					tr.appendChild(td);
					
					// sHTML += '	<tr>';
					// sHTML += '		<td>';
					// sHTML += '			' + L10n.Term("Calls.LBL_DURATION") + ' <span class="required">' + L10n.Term(".LBL_REQUIRED_SYMBOL") + '</>';
					// sHTML += '		</td>';
					// sHTML += '		<td>';
					// sHTML += '			<input type="text" ID="divNewAppointmentPopup_txtDURATION_HOURS"   size="5" />';
					// sHTML += '			<select ID="divNewAppointmentPopup_lstDURATION_MINUTES">';
					// sHTML += '				<option value="0">0</option>';
					// sHTML += '				<option value="15">15</option>';
					// sHTML += '				<option value="30">30</option>';
					// sHTML += '				<option value="45">45</option>';
					// sHTML += '			</select>';
					// sHTML += '		</td>';
					// sHTML += '		<td>';
					// sHTML += '			&nbsp;' + L10n.Term("Calls.LBL_HOURS_MINUTES");
					// sHTML += '		</td>';
					// sHTML += '		<td />';
					// sHTML += '	</tr>';
					tr = document.createElement('tr');
					tbody.appendChild(tr);
					td = document.createElement('td');
					tr.appendChild(td);
					txt = document.createTextNode(L10n.Term('Calls.LBL_DURATION'));
					td.appendChild(txt);
					spn = document.createElement('span');
					spn.className = 'required';
					td.appendChild(spn);
					txt = document.createTextNode(L10n.Term('.LBL_REQUIRED_SYMBOL'));
					spn.appendChild(txt);
					td = document.createElement('td');
					tr.appendChild(td);
					var divNewAppointmentPopup_txtDURATION_HOURS = document.createElement('input');
					divNewAppointmentPopup_txtDURATION_HOURS.id            = 'divNewAppointmentPopup_DURATION_HOURS';
					divNewAppointmentPopup_txtDURATION_HOURS.type          = 'text';
					divNewAppointmentPopup_txtDURATION_HOURS.style.width   = '25px';
					td.appendChild(divNewAppointmentPopup_txtDURATION_HOURS);
					txt = document.createTextNode(nbsp + nbsp);
					td.appendChild(txt);
					var divNewAppointmentPopup_lstDURATION_MINUTES = document.createElement('select');
					divNewAppointmentPopup_lstDURATION_MINUTES.id          = 'divNewAppointmentPopup_DURATION_MINUTES';
					td.appendChild(divNewAppointmentPopup_lstDURATION_MINUTES);
					txt = document.createTextNode(nbsp + nbsp + L10n.Term('Calls.LBL_HOURS_MINUTES') + nbsp + nbsp);
					td.appendChild(txt);
					
					for ( var nMinutes = 0; nMinutes < 60; nMinutes += 15 )
					{
						var opt = document.createElement('option');
						divNewAppointmentPopup_lstDURATION_MINUTES.appendChild(opt);
						opt.setAttribute('value', nMinutes.toString());
						opt.innerHTML = nMinutes.toString();
						if ( nMinutes == DURATION_MINUTES )
							opt.setAttribute('selected', 'selected');
					}
					td = document.createElement('td');
					tr.appendChild(td);
					
					// 03/10/2013 Paul.  Add ALL_DAY_EVENT. 
					//if ( ALL_DAY_EVENT )
					var chkALL_DAY_EVENT = document.createElement('input');
					chkALL_DAY_EVENT.id        = 'divNewAppointmentPopup_ALL_DAY_EVENT';
					chkALL_DAY_EVENT.type      = 'checkbox';
					chkALL_DAY_EVENT.className = 'checkbox';
					td.appendChild(chkALL_DAY_EVENT);
					// 03/10/2013 Paul.  The checked flag must be set after adding. 
					chkALL_DAY_EVENT.checked   = ALL_DAY_EVENT;
					chkALL_DAY_EVENT.onclick   = function()
					{
						if ( chkALL_DAY_EVENT.checked )
						{
							divNewAppointmentPopup_lstDURATION_MINUTES.selectedIndex = 0;
							divNewAppointmentPopup_txtDURATION_HOURS.value = 24;
							divNewAppointmentPopup_txtDATE_START.value = $.fullCalendar.formatDate(start, context.ShortDatePattern);
						}
						else
						{
							divNewAppointmentPopup_txtDATE_START.value = $.fullCalendar.formatDate(start, context.ShortDatePattern) + ' ' + $.fullCalendar.formatDate(start, context.ShortTimePattern);
							divNewAppointmentPopup_txtDURATION_HOURS.value = 1;
						}
					};
					txt = document.createTextNode(nbsp + nbsp + L10n.Term('Calls.LBL_ALL_DAY'));
					td.appendChild(txt);
					td = document.createElement('td');
					tr.appendChild(td);

					// sHTML += '	<tr>';
					// sHTML += '		<td>';
					// sHTML += '			' + L10n.Term("Calendar.LBL_REPEAT_TAB") + '</>';
					// sHTML += '		</td>';
					// sHTML += '		<td />';
					// sHTML += '		<td />';
					// sHTML += '		<td />';
					// sHTML += '	</tr>';
					tr = document.createElement('tr');
					tbody.appendChild(tr);
					td = document.createElement('td');
					tr.appendChild(td);
					var h4 = document.createElement('h4');
					td.appendChild(h4);
					txt = document.createTextNode(L10n.Term('Calendar.LBL_REPEAT_TAB'));
					h4.appendChild(txt);
					h4.style.marginTop = '10px';
					td = document.createElement('td');
					tr.appendChild(td);
					td = document.createElement('td');
					tr.appendChild(td);
					td = document.createElement('td');
					tr.appendChild(td);

					// sHTML += '	<tr>';
					// sHTML += '		<td>';
					// sHTML += '			' + L10n.Term("Calls.LBL_REPEAT_TYPE") + '</>';
					// sHTML += '		</td>';
					// sHTML += '		<td>';
					// sHTML += '			<input type="text" ID="divNewAppointmentPopup_txtREPEAT_TYPE" size="30" maxlength="255" />';
					// sHTML += '		</td>';
					// sHTML += '		<td>';
					// sHTML += '			' + L10n.Term("Calendar.LBL_REPEAT_END_AFTER") + '</>';
					// sHTML += '		</td>';
					// sHTML += '		<td>';
					// sHTML += '			<input type="text" ID="divNewAppointmentPopup_txtREPEAT_COUNT" size="30" maxlength="255" />';
					// sHTML += '		</td>';
					// sHTML += '	</tr>';
					tr = document.createElement('tr');
					tbody.appendChild(tr);
					td = document.createElement('td');
					tr.appendChild(td);
					txt = document.createTextNode(L10n.Term('Calls.LBL_REPEAT_TYPE'));
					td.appendChild(txt);
					td = document.createElement('td');
					tr.appendChild(td);
					var divNewAppointmentPopup_lstREPEAT_TYPE = document.createElement('select');
					divNewAppointmentPopup_lstREPEAT_TYPE.id = 'divNewAppointmentPopup_lstREPEAT_TYPE';
					td.appendChild(divNewAppointmentPopup_lstREPEAT_TYPE);
					
					try
					{
						var sLIST_NAME = 'repeat_type_dom';
						var arrLIST = L10n.GetList(sLIST_NAME);
						if ( arrLIST != null )
						{
							var lst = divNewAppointmentPopup_lstREPEAT_TYPE;
							var opt = document.createElement('option');
							lst.appendChild(opt);
							opt.setAttribute('value', '');
							opt.innerHTML = L10n.Term('.LBL_NONE');
							for ( var i = 0; i < arrLIST.length; i++ )
							{
								var opt = document.createElement('option');
								lst.appendChild(opt);
								opt.setAttribute('value', arrLIST[i]);
								opt.innerHTML = L10n.ListTerm(sLIST_NAME, arrLIST[i]);
							}
						}
					}
					catch(e)
					{
						alert(e.Message);
					}
					
					td = document.createElement('td');
					tr.appendChild(td);
					var divNewAppointmentPopup_divREPEAT_COUNT_LABEL = document.createElement('div');
					divNewAppointmentPopup_divREPEAT_COUNT_LABEL.id = 'divNewAppointmentPopup_divREPEAT_COUNT_LABEL';
					divNewAppointmentPopup_divREPEAT_COUNT_LABEL.style.display = 'none';
					td.appendChild(divNewAppointmentPopup_divREPEAT_COUNT_LABEL);
					txt = document.createTextNode(L10n.Term('Calendar.LBL_REPEAT_END_AFTER'));
					divNewAppointmentPopup_divREPEAT_COUNT_LABEL.appendChild(txt);
					
					td = document.createElement('td');
					tr.appendChild(td);
					var divNewAppointmentPopup_divREPEAT_COUNT = document.createElement('div');
					divNewAppointmentPopup_divREPEAT_COUNT.id = 'divNewAppointmentPopup_divREPEAT_COUNT';
					divNewAppointmentPopup_divREPEAT_COUNT.style.display = 'none';
					td.appendChild(divNewAppointmentPopup_divREPEAT_COUNT);
					var divNewAppointmentPopup_txtREPEAT_COUNT = document.createElement('input');
					divNewAppointmentPopup_txtREPEAT_COUNT.id            = 'divNewAppointmentPopup_txtREPEAT_COUNT';
					divNewAppointmentPopup_txtREPEAT_COUNT.type          = 'text' ;
					divNewAppointmentPopup_txtREPEAT_COUNT.style.width   = '25px';
					divNewAppointmentPopup_divREPEAT_COUNT.appendChild(divNewAppointmentPopup_txtREPEAT_COUNT);
					txt = document.createTextNode(nbsp + nbsp + L10n.Term('Calendar.LBL_REPEAT_OCCURRENCES'));
					divNewAppointmentPopup_divREPEAT_COUNT.appendChild(txt);

					// sHTML += '	<tr>';
					// sHTML += '		<td>';
					// sHTML += '			' + L10n.Term("Calendar.LBL_REPEAT_INTERVAL") + '</>';
					// sHTML += '		</td>';
					// sHTML += '		<td>';
					// sHTML += '			<input type="text" ID="divNewAppointmentPopup_txtREPEAT_INTERVAL" size="30" maxlength="255" />';
					// sHTML += '		</td>';
					// sHTML += '		<td>';
					// sHTML += '			' + L10n.Term("Calls.LBL_REPEAT_UNTIL") + '</>';
					// sHTML += '		</td>';
					// sHTML += '		<td>';
					// sHTML += '			<input type="text" ID="divNewAppointmentPopup_txtREPEAT_UNTIL" size="30" maxlength="255" />';
					// sHTML += '		</td>';
					// sHTML += '	</tr>';
					tr = document.createElement('tr');
					tbody.appendChild(tr);
					td = document.createElement('td');
					tr.appendChild(td);
					var divNewAppointmentPopup_divREPEAT_INTERVAL_LABEL = document.createElement('div');
					divNewAppointmentPopup_divREPEAT_INTERVAL_LABEL.id = 'divNewAppointmentPopup_divREPEAT_INTERVAL_LABEL';
					divNewAppointmentPopup_divREPEAT_INTERVAL_LABEL.style.display = 'none';
					td.appendChild(divNewAppointmentPopup_divREPEAT_INTERVAL_LABEL);
					txt = document.createTextNode(L10n.Term('Calendar.LBL_REPEAT_INTERVAL'));
					divNewAppointmentPopup_divREPEAT_INTERVAL_LABEL.appendChild(txt);
					td = document.createElement('td');
					tr.appendChild(td);
					var divNewAppointmentPopup_divREPEAT_INTERVAL = document.createElement('div');
					divNewAppointmentPopup_divREPEAT_INTERVAL.id = 'divNewAppointmentPopup_divREPEAT_INTERVAL';
					divNewAppointmentPopup_divREPEAT_INTERVAL.style.display = 'none';
					td.appendChild(divNewAppointmentPopup_divREPEAT_INTERVAL);
					var divNewAppointmentPopup_txtREPEAT_INTERVAL = document.createElement('input');
					divNewAppointmentPopup_txtREPEAT_INTERVAL.id            = 'divNewAppointmentPopup_txtREPEAT_INTERVAL';
					divNewAppointmentPopup_txtREPEAT_INTERVAL.type          = 'text' ;
					divNewAppointmentPopup_txtREPEAT_INTERVAL.style.width   = '25px';
					divNewAppointmentPopup_divREPEAT_INTERVAL.appendChild(divNewAppointmentPopup_txtREPEAT_INTERVAL);
					
					td = document.createElement('td');
					tr.appendChild(td);
					var divNewAppointmentPopup_divREPEAT_UNTIL_LABEL = document.createElement('div');
					divNewAppointmentPopup_divREPEAT_UNTIL_LABEL.id = 'divNewAppointmentPopup_divREPEAT_UNTIL_LABEL';
					divNewAppointmentPopup_divREPEAT_UNTIL_LABEL.style.display = 'none';
					td.appendChild(divNewAppointmentPopup_divREPEAT_UNTIL_LABEL);
					txt = document.createTextNode(L10n.Term('Calls.LBL_REPEAT_UNTIL'));
					divNewAppointmentPopup_divREPEAT_UNTIL_LABEL.appendChild(txt);
					td = document.createElement('td');
					tr.appendChild(td);
					var divNewAppointmentPopup_divREPEAT_UNTIL = document.createElement('div');
					divNewAppointmentPopup_divREPEAT_UNTIL.id = 'divNewAppointmentPopup_divREPEAT_UNTIL';
					divNewAppointmentPopup_divREPEAT_UNTIL.style.display = 'none';
					td.appendChild(divNewAppointmentPopup_divREPEAT_UNTIL);
					var divNewAppointmentPopup_txtREPEAT_UNTIL = document.createElement('input');
					divNewAppointmentPopup_txtREPEAT_UNTIL.id            = 'divNewAppointmentPopup_txtREPEAT_UNTIL';
					divNewAppointmentPopup_txtREPEAT_UNTIL.type          = 'text';
					divNewAppointmentPopup_txtREPEAT_UNTIL.style.width   = '80px';
					divNewAppointmentPopup_divREPEAT_UNTIL.appendChild(divNewAppointmentPopup_txtREPEAT_UNTIL);
					var sDATE_FORMAT = Security.USER_DATE_FORMAT();
					var sTIME_FORMAT = Security.USER_TIME_FORMAT();
					sDATE_FORMAT = sDATE_FORMAT.replace('yyyy', 'yy');
					sDATE_FORMAT = sDATE_FORMAT.replace('MM'  , 'mm');
					//var bAMPM        = (sTIME_FORMAT.indexOf('t') >= 0) || (sTIME_FORMAT.indexOf('T') >= 0);
					//$('#' + divNewAppointmentPopup_txtREPEAT_UNTIL.id).datetimepicker( { dateFormat: sDATE_FORMAT, timeFormat: sTIME_FORMAT, ampm: bAMPM } );
					$('#' + divNewAppointmentPopup_txtREPEAT_UNTIL.id).datepicker( { dateFormat: sDATE_FORMAT } );
					
					// sHTML += '	<tr>';
					// sHTML += '		<td>';
					// sHTML += '			' + L10n.Term("Calls.LBL_REPEAT_DOW") + '</>';
					// sHTML += '		</td>';
					// sHTML += '		<td colspan="3">';
					// sHTML += '			<input type="checkbox" ID="divNewAppointmentPopup_chkSunday"    value="0" class="checkbox" />';
					// sHTML += '			<input type="checkbox" ID="divNewAppointmentPopup_chkMonday"    value="1" class="checkbox" />';
					// sHTML += '			<input type="checkbox" ID="divNewAppointmentPopup_chkTuesday"   value="2" class="checkbox" />';
					// sHTML += '			<input type="checkbox" ID="divNewAppointmentPopup_chkWednesday" value="3" class="checkbox" />';
					// sHTML += '			<input type="checkbox" ID="divNewAppointmentPopup_chkThursday"  value="4" class="checkbox" />';
					// sHTML += '			<input type="checkbox" ID="divNewAppointmentPopup_chkFriday"    value="5" class="checkbox" />';
					// sHTML += '			<input type="checkbox" ID="divNewAppointmentPopup_chkSaturday"  value="6" class="checkbox" />';
					// sHTML += '		</td>';
					// sHTML += '	</tr>';
					tr = document.createElement('tr');
					tbody.appendChild(tr);
					td = document.createElement('td');
					tr.appendChild(td);
					var divNewAppointmentPopup_divREPEAT_DOW_LABEL = document.createElement('div');
					divNewAppointmentPopup_divREPEAT_DOW_LABEL.id = 'divNewAppointmentPopup_divREPEAT_DOW_LABEL';
					divNewAppointmentPopup_divREPEAT_DOW_LABEL.style.display = 'none';
					td.appendChild(divNewAppointmentPopup_divREPEAT_DOW_LABEL);
					txt = document.createTextNode(L10n.Term('Calls.LBL_REPEAT_DOW'));
					divNewAppointmentPopup_divREPEAT_DOW_LABEL.appendChild(txt);
					td = document.createElement('td');
					tr.appendChild(td);
					td.setAttribute('colspan', '3'  );
					td.style.whiteSpace = 'nowrap';
					var divNewAppointmentPopup_divREPEAT_DOW = document.createElement('div');
					divNewAppointmentPopup_divREPEAT_DOW.id = 'divNewAppointmentPopup_divREPEAT_DOW';
					divNewAppointmentPopup_divREPEAT_DOW.style.display = 'none';
					td.appendChild(divNewAppointmentPopup_divREPEAT_DOW);

					arrLIST = L10n.GetList('short_day_names_dom');
					var divNewAppointmentPopup_chkSunday = document.createElement('input');
					divNewAppointmentPopup_chkSunday.id        = 'divNewAppointmentPopup_chkSunday';
					divNewAppointmentPopup_chkSunday.type      = 'checkbox';
					divNewAppointmentPopup_chkSunday.className = 'checkbox';
					divNewAppointmentPopup_chkSunday.value     = '0';
					divNewAppointmentPopup_divREPEAT_DOW.appendChild(divNewAppointmentPopup_chkSunday);
					txt = document.createTextNode(nbsp + arrLIST[0] + nbsp + nbsp);
					divNewAppointmentPopup_divREPEAT_DOW.appendChild(txt);

					var divNewAppointmentPopup_chkMonday = document.createElement('input');
					divNewAppointmentPopup_chkMonday.id        = 'divNewAppointmentPopup_chkMonday';
					divNewAppointmentPopup_chkMonday.type      = 'checkbox';
					divNewAppointmentPopup_chkMonday.className = 'checkbox';
					divNewAppointmentPopup_chkMonday.value     = '1';
					divNewAppointmentPopup_divREPEAT_DOW.appendChild(divNewAppointmentPopup_chkMonday);
					txt = document.createTextNode(nbsp + arrLIST[1] + nbsp + nbsp);
					divNewAppointmentPopup_divREPEAT_DOW.appendChild(txt);

					var divNewAppointmentPopup_chkTuesday = document.createElement('input');
					divNewAppointmentPopup_chkTuesday.id        = 'divNewAppointmentPopup_chkTuesday';
					divNewAppointmentPopup_chkTuesday.type      = 'checkbox';
					divNewAppointmentPopup_chkTuesday.className = 'checkbox';
					divNewAppointmentPopup_chkTuesday.value     = '2';
					divNewAppointmentPopup_divREPEAT_DOW.appendChild(divNewAppointmentPopup_chkTuesday);
					txt = document.createTextNode(nbsp + arrLIST[2] + nbsp + nbsp);
					divNewAppointmentPopup_divREPEAT_DOW.appendChild(txt);

					var divNewAppointmentPopup_chkWednesday = document.createElement('input');
					divNewAppointmentPopup_chkWednesday.id        = 'divNewAppointmentPopup_chkWednesday';
					divNewAppointmentPopup_chkWednesday.type      = 'checkbox';
					divNewAppointmentPopup_chkWednesday.className = 'checkbox';
					divNewAppointmentPopup_chkWednesday.value     = '3';
					divNewAppointmentPopup_divREPEAT_DOW.appendChild(divNewAppointmentPopup_chkWednesday);
					txt = document.createTextNode(nbsp + arrLIST[3] + nbsp + nbsp);
					divNewAppointmentPopup_divREPEAT_DOW.appendChild(txt);

					var divNewAppointmentPopup_chkThursday = document.createElement('input');
					divNewAppointmentPopup_chkThursday.id        = 'divNewAppointmentPopup_chkThursday';
					divNewAppointmentPopup_chkThursday.type      = 'checkbox';
					divNewAppointmentPopup_chkThursday.className = 'checkbox';
					divNewAppointmentPopup_chkThursday.value     = '4';
					divNewAppointmentPopup_divREPEAT_DOW.appendChild(divNewAppointmentPopup_chkThursday);
					txt = document.createTextNode(nbsp + arrLIST[4] + nbsp + nbsp);
					divNewAppointmentPopup_divREPEAT_DOW.appendChild(txt);

					var divNewAppointmentPopup_chkFriday = document.createElement('input');
					divNewAppointmentPopup_chkFriday.id        = 'divNewAppointmentPopup_chkFriday';
					divNewAppointmentPopup_chkFriday.type      = 'checkbox';
					divNewAppointmentPopup_chkFriday.className = 'checkbox';
					divNewAppointmentPopup_chkFriday.value     = '5';
					divNewAppointmentPopup_divREPEAT_DOW.appendChild(divNewAppointmentPopup_chkFriday);
					txt = document.createTextNode(nbsp + arrLIST[5] + nbsp + nbsp);
					divNewAppointmentPopup_divREPEAT_DOW.appendChild(txt);

					var divNewAppointmentPopup_chkSaturday = document.createElement('input');
					divNewAppointmentPopup_chkSaturday.id        = 'divNewAppointmentPopup_chkSaturday';
					divNewAppointmentPopup_chkSaturday.type      = 'checkbox';
					divNewAppointmentPopup_chkSaturday.className = 'checkbox';
					divNewAppointmentPopup_chkSaturday.value     = '6';
					divNewAppointmentPopup_divREPEAT_DOW.appendChild(divNewAppointmentPopup_chkSaturday);
					txt = document.createTextNode(nbsp + arrLIST[6] + nbsp + nbsp);
					divNewAppointmentPopup_divREPEAT_DOW.appendChild(txt);
					
					// sHTML += '	<tr>';
					// sHTML += '		<td></td>';
					// sHTML += '		<td>';
					// sHTML += '			<input type="button" ID="divNewAppointmentPopup_btnSave"   value="' + "  " + L10n.Term(".LBL_SAVE_BUTTON_LABEL"  ) + "  " + '" title="' + L10n.Term(".LBL_SAVE_BUTTON_TITLE"  ) + '" class="button" />';
					// sHTML += '			<input type="button" ID="divNewAppointmentPopup_btnCancel" value="' + "  " + L10n.Term(".LBL_CANCEL_BUTTON_LABEL") + "  " + '" title="' + L10n.Term(".LBL_CANCEL_BUTTON_TITLE") + '" class="button" />';
					// sHTML += '		</td>';
					// sHTML += '	</tr>';
					tr = document.createElement('tr');
					tbody.appendChild(tr);
					td = document.createElement('td');
					tr.appendChild(td);
					td = document.createElement('td');
					tr.appendChild(td);
					var divNewAppointmentPopup_btnSave = document.createElement('input');
					divNewAppointmentPopup_btnSave.id          = 'divNewAppointmentPopup_btnSave';
					divNewAppointmentPopup_btnSave.type        = 'button';
					divNewAppointmentPopup_btnSave.className   = 'button';
					divNewAppointmentPopup_btnSave.value       = L10n.Term('.LBL_SAVE_BUTTON_LABEL');
					divNewAppointmentPopup_btnSave.title       = L10n.Term('.LBL_SAVE_BUTTON_TITLE');
					divNewAppointmentPopup_btnSave.style.marginTop = '10px';
					td.appendChild(divNewAppointmentPopup_btnSave);
					txt = document.createTextNode(nbsp);
					td.appendChild(txt);
					var divNewAppointmentPopup_btnCancel = document.createElement('input');
					divNewAppointmentPopup_btnCancel.id        = 'divNewAppointmentPopup_btnSave';
					divNewAppointmentPopup_btnCancel.type      = 'button';
					divNewAppointmentPopup_btnCancel.className = 'button';
					divNewAppointmentPopup_btnCancel.value     = L10n.Term('.LBL_CANCEL_BUTTON_LABEL');
					divNewAppointmentPopup_btnCancel.title     = L10n.Term('.LBL_CANCEL_BUTTON_TITLE');
					td.appendChild(divNewAppointmentPopup_btnCancel);
					td = document.createElement('td');
					tr.appendChild(td);
					td = document.createElement('td');
					tr.appendChild(td);
					
					// sHTML += '	<tr>';
					// sHTML += '		<td></td>';
					// sHTML += '		<td><div id="divNewAppointmentPopup_divError" class="error"></div></td>';
					// sHTML += '	</tr>';
					tr = document.createElement('tr');
					tbody.appendChild(tr);
					td = document.createElement('td');
					tr.appendChild(td);
					td = document.createElement('td');
					tr.appendChild(td);
					td.setAttribute('colspan', '3'  );
					var divNewAppointmentPopup_divError = document.createElement('div');
					divNewAppointmentPopup_divError.id        = 'divNewAppointmentPopup_divError';
					divNewAppointmentPopup_divError.className = 'error';
					td.appendChild(divNewAppointmentPopup_divError);
					
					// 03/10/2013 Paul.  Add ALL_DAY_EVENT. 
					if ( ALL_DAY_EVENT )
						divNewAppointmentPopup_txtDATE_START.value         = $.fullCalendar.formatDate(start, context.ShortDatePattern);
					else
						divNewAppointmentPopup_txtDATE_START.value         = $.fullCalendar.formatDate(start, context.ShortDatePattern) + ' ' + $.fullCalendar.formatDate(start, context.ShortTimePattern);
					divNewAppointmentPopup_txtDURATION_HOURS.value   = DURATION_HOURS.toString();
					divNewAppointmentPopup_lstDURATION_MINUTES.value = DURATION_MINUTES.toString();

					divNewAppointmentPopup_lstREPEAT_TYPE.onchange = function(e)
					{
						// 03/30/2013 Paul.  Chrome, Firefox and Safari do not like to hide and show table rows or columns.  Use divisions for the cell contents instead. 
						var REPEAT_TYPE = divNewAppointmentPopup_lstREPEAT_TYPE.options[divNewAppointmentPopup_lstREPEAT_TYPE.options.selectedIndex].value;
						divNewAppointmentPopup_divREPEAT_INTERVAL_LABEL.style.display = (REPEAT_TYPE != ''       ? 'inline' : 'none');
						divNewAppointmentPopup_divREPEAT_INTERVAL.style.display       = (REPEAT_TYPE != ''       ? 'inline' : 'none');
						divNewAppointmentPopup_divREPEAT_COUNT_LABEL.style.display    = (REPEAT_TYPE != ''       ? 'inline' : 'none');
						divNewAppointmentPopup_divREPEAT_COUNT.style.display          = (REPEAT_TYPE != ''       ? 'inline' : 'none');
						divNewAppointmentPopup_divREPEAT_INTERVAL_LABEL.style.display = (REPEAT_TYPE != ''       ? 'inline' : 'none');
						divNewAppointmentPopup_divREPEAT_INTERVAL.style.display       = (REPEAT_TYPE != ''       ? 'inline' : 'none');
						divNewAppointmentPopup_divREPEAT_UNTIL_LABEL.style.display    = (REPEAT_TYPE != ''       ? 'inline' : 'none');
						divNewAppointmentPopup_divREPEAT_UNTIL.style.display          = (REPEAT_TYPE != ''       ? 'inline' : 'none');
						divNewAppointmentPopup_divREPEAT_DOW_LABEL.style.display      = (REPEAT_TYPE == 'Weekly' ? 'inline' : 'none');
						divNewAppointmentPopup_divREPEAT_DOW.style.display            = (REPEAT_TYPE == 'Weekly' ? 'inline' : 'none');
					}
					
					divNewAppointmentPopup_txtNAME.onkeypress = function(e)
					{
						return RegisterEnterKeyPress(e, 'divNewAppointmentPopup_btnSave');
					};
					divNewAppointmentPopup_btnSave.onclick = function()
					{
						divNewAppointmentPopup_txtNAME.value = $.trim(divNewAppointmentPopup_txtNAME.value);
						divNewAppointmentPopup_reqNAME.style.display = (divNewAppointmentPopup_txtNAME.value.length > 0) ? 'none' : 'inline';
						if ( divNewAppointmentPopup_txtNAME.value.length > 0 )
						{
							var MODULE_NAME      = divNewAppointmentPopup_radScheduleMeeting.checked ? 'Meetings' : 'Calls';
							var DURATION_HOURS   = Sql.ToInteger(divNewAppointmentPopup_txtDURATION_HOURS.value);
							var DURATION_MINUTES = divNewAppointmentPopup_lstDURATION_MINUTES.options[divNewAppointmentPopup_lstDURATION_MINUTES.options.selectedIndex].value;
							if ( isNaN(DURATION_MINUTES) )
								DURATION_MINUTES = 0;
							if ( isNaN(DURATION_HOURS) )
								DURATION_HOURS = 0;
							if ( chkALL_DAY_EVENT.checked )
							{
								DURATION_HOURS   = 24;
								DURATION_MINUTES = 0;
							}
							
							var REPEAT_TYPE     = divNewAppointmentPopup_lstREPEAT_TYPE.options[divNewAppointmentPopup_lstREPEAT_TYPE.options.selectedIndex].value;
							var REPEAT_COUNT    = Sql.ToInteger(divNewAppointmentPopup_txtREPEAT_COUNT.value);
							var REPEAT_INTERVAL = Sql.ToInteger(divNewAppointmentPopup_txtREPEAT_INTERVAL.value);
							// 04/25/2013 Paul.  ToJsonDate requires a date value not a string. 
							var REPEAT_UNTIL    = ToJsonDate($('#' + divNewAppointmentPopup_txtREPEAT_UNTIL.id).datepicker('getDate'));
							var REPEAT_DOW      = '';
							if ( REPEAT_TYPE == 'Weekly' )
							{
								if ( divNewAppointmentPopup_chkSunday.checked    ) REPEAT_DOW += '0';
								if ( divNewAppointmentPopup_chkMonday.checked    ) REPEAT_DOW += '1';
								if ( divNewAppointmentPopup_chkTuesday.checked   ) REPEAT_DOW += '2';
								if ( divNewAppointmentPopup_chkWednesday.checked ) REPEAT_DOW += '3';
								if ( divNewAppointmentPopup_chkThursday.checked  ) REPEAT_DOW += '4';
								if ( divNewAppointmentPopup_chkFriday.checked    ) REPEAT_DOW += '5';
								if ( divNewAppointmentPopup_chkSaturday.checked  ) REPEAT_DOW += '6';
							}
							
							var row = new Object();
							row. NAME            = divNewAppointmentPopup_txtNAME.value;
							row.DATE_TIME        = ToJsonDate(start);
							row.DURATION_HOURS   = DURATION_HOURS   ;
							row.DURATION_MINUTES = DURATION_MINUTES ;
							row.ALL_DAY_EVENT    = chkALL_DAY_EVENT.checked;
							row.DIRECTION        = 'Outbound'       ;
							row.STATUS           = 'Planned'        ;
							row.ASSIGNED_USER_ID = Security.USER_ID();
							row.TEAM_ID          = Security.TEAM_ID();
							row.REPEAT_TYPE      = REPEAT_TYPE      ;
							row.REPEAT_COUNT     = REPEAT_COUNT     ;
							row.REPEAT_INTERVAL  = REPEAT_INTERVAL  ;
							row.REPEAT_UNTIL     = REPEAT_UNTIL     ;
							row.REPEAT_DOW       = REPEAT_DOW       ;
							var bgPage = chrome.extension.getBackgroundPage();
							bgPage.UpdateModule(MODULE_NAME, row, null, function(status, message)
							{
								try
								{
									if ( status == 1 || status == 3 )
									{
										$dialog.dialog('close');
										calendar.fullCalendar('refetchEvents');
									}
									else
									{
										var divNewAppointmentPopup_divError = document.getElementById('divNewAppointmentPopup_divError');
										divNewAppointmentPopup_divError.innerHTML =message;
									}
								}
								catch(e)
								{
									var divNewAppointmentPopup_divError = document.getElementById('divNewAppointmentPopup_divError');
									divNewAppointmentPopup_divError.innerHTML =message;
								}
							}, context);
						}
					};
					divNewAppointmentPopup_btnCancel.onclick = function()
					{
						$dialog.dialog('close');
					};
				}
				, open: function(event, ui)
				{
					var divNewAppointmentPopup_txtNAME = document.getElementById('divNewAppointmentPopup_txtNAME');
					divNewAppointmentPopup_txtNAME.focus();
				}
				, close: function(event, ui)
				{
					$dialog.dialog('destroy');
					var divPopup = document.getElementById('divNewAppointmentPopup');
					divPopup.parentNode.removeChild(divPopup);
					calendar.fullCalendar('unselect');
				}
			});
		}
		, eventClick: function(event)
		{
			if ( event.url !== undefined && event.url != null )
			{
				//if ( event.url.indexOf('google.com/calendar') >= 0 )
				//	window.open(event.url, 'gcalevent', 'width=700,height=600');
				// 01/07/2014 Paul.  Use ../../ instead of ~/ so that raw URL will work as expected. 
				if ( event.url.substr(0, 6) == '../../' )
				{
					var sURL = event.url.replace('../../', '~/');
					var arrURL = sURL.split('/');
					if ( arrURL.length > 2 )
					{
						var sMODULE_NAME = arrURL[1];
						var sID          = arrURL[2].replace('view.aspx?ID=', '');
						var oDetailViewUI = new DetailViewUI();
						oDetailViewUI.Load(sLayoutPanel, sActionsPanel, sMODULE_NAME, sID, function(status, message)
						{
							if ( status == 1 )
							{
								SplendidError.SystemMessage('');
							}
							else
							{
								SplendidError.SystemMessage(message);
							}
						});
					}
				}
			}
			return false;
		}
		, eventDrop: function(event, dayDelta, minuteDelta, allDay, revertFunc, jsEvent, ui, view)
		{
			var TOTAL_SECONDS    = Math.round((event.end - event.start)/1000);
			var TOTAL_MINUTES    = Math.round(TOTAL_SECONDS/60);
			var DURATION_HOURS   = Math.floor(TOTAL_MINUTES/60);
			var DURATION_MINUTES = TOTAL_MINUTES % 60;
			var row = new Object();
			row.ID               = event.id               ;
			row.DATE_TIME        = ToJsonDate(event.start);
			row.DURATION_HOURS   = DURATION_HOURS         ;
			row.DURATION_MINUTES = DURATION_MINUTES       ;
			var bgPage = chrome.extension.getBackgroundPage();
			bgPage.UpdateModule(event.MODULE_NAME, row, event.id, function(status, message)
			{
				try
				{
					if ( status == 1 || status == 3 )
					{
					}
					else
					{
						revertFunc();
						SplendidError.SystemMessage(message);
					}
				}
				catch(e)
				{
					revertFunc();
					SplendidError.SystemAlert(e, 'UpdateModule');
				}
			}, context);
		}
		, eventResize: function(event, dayDelta, minuteDelta, revertFunc, jsEvent, ui, view)
		{
			var TOTAL_SECONDS    = Math.round((event.end - event.start)/1000);
			var TOTAL_MINUTES    = Math.round(TOTAL_SECONDS/60);
			var DURATION_HOURS   = Math.floor(TOTAL_MINUTES/60);
			var DURATION_MINUTES = TOTAL_MINUTES % 60;
			var row = new Object();
			row.ID               = event.id               ;
			row.DATE_TIME        = ToJsonDate(event.start);
			row.DURATION_HOURS   = DURATION_HOURS         ;
			row.DURATION_MINUTES = DURATION_MINUTES       ;
			var bgPage = chrome.extension.getBackgroundPage();
			bgPage.UpdateModule(event.MODULE_NAME, row, event.id, function(status, message)
			{
				try
				{
					if ( status == 1 || status == 3 )
					{
					}
					else
					{
						revertFunc();
						SplendidError.SystemMessage(message);
					}
				}
				catch(e)
				{
					revertFunc();
					SplendidError.SystemAlert(e, 'UpdateModule');
				}
			}, context);
		}
		, loading: function(bool)
		{
		}
		, viewDisplay: function(view)
		{
			try
			{
				// 02/22/2013 Paul.  Save the last view. 
				if ( window.localStorage )
					localStorage['CalendarDefaultView'] = view.name;
				else
					setCookie('CalendarDefaultView', view.name, 180);
			}
			catch(e)
			{
				// 03/10/2013 Paul.  IE9 is throwing an out-of-memory error. Just ignore the error. 
				//if ( window.localStorage.remainingSpace !== undefined )
				//	alert('remainingSpace = ' + window.localStorage.remainingSpace);
				SplendidError.SystemLog('CalendarDefaultView: ' + e.message);
			}
		}
	});
	var sGoogleHolidayURL = bgPage.SplendidCache.Config('GoogleCalendar.HolidayCalendars');
	if ( !Sql.IsEmptyString(sGoogleHolidayURL) )
	{
		var arrGoogleHolidayURL = sGoogleHolidayURL.split(',');
		for ( var i = 0; i < arrGoogleHolidayURL.length; i++ )
		{
			// 01/30/2014 Paul.  Chrome and Firefox require that the protocol match.  IE seems to be more flexible. 
			if ( window.location.protocol == 'https:' )
				arrGoogleHolidayURL[i] = arrGoogleHolidayURL[i].replace('http:', window.location.protocol);
			calendar.fullCalendar('addEventSource', arrGoogleHolidayURL[i]);
		}
	}
	calendar.fullCalendar('addEventSource', function(start, end, cbCalendar)
	{
		var dtDATE_START      = ToJsonDate(start);
		var dtDATE_END        = ToJsonDate(end  );
		var gASSIGNED_USER_ID = (bSharedCalendar ? '' : Security.USER_ID());
		var bgPage = chrome.extension.getBackgroundPage();
		bgPage.CalendarView_GetCalendar(dtDATE_START, dtDATE_END, gASSIGNED_USER_ID, function(status, message)
		{
			if ( status == 1 )
			{
				var rows = message;
				var events = new Array();
				for ( var i in rows )
				{
					var row = rows[i];
					var event = new Object();
					event.id          = row.ID;
					event.title       = row.STATUS + ': ' + row.NAME;
					event.MODULE_NAME = row.ACTIVITY_TYPE;
					event.start       = FromJsonDate(row.DATE_START).getTime()/1000;
					event.end         = FromJsonDate(row.DATE_END  ).getTime()/1000;
					event.editable    = true;
					// 02/20/2013 Paul.  Must set allDay in order for event to appear on agenda view. 
					// 03/10/2013 Paul.  Add ALL_DAY_EVENT. 
					event.allDay      = Sql.ToBoolean(row.ALL_DAY_EVENT);
					// 03/10/2013 Paul.  We set duration to 24 hours for all day events for iCal synching, but it makes FullCalendar span days in the Week view. 
					if ( event.allDay )
						event.end = event.start;
					// 01/07/2014 Paul.  Use ../../ instead of ~/ so that raw URL will work as expected. 
					event.url         = '../../' + row.ACTIVITY_TYPE + '/view.aspx?ID=' + row.ID;
					events.push(event);
				}
				cbCalendar(events);
				callback(1, null);
			}
			else
			{
				callback(status, message);
			}
		}, context);
	});
};

CalendarViewUI.prototype.Load = function(sLayoutPanel, sActionsPanel, callback)
{
	try
	{
		var bgPage = chrome.extension.getBackgroundPage();
		bgPage.AuthenticatedMethod(function(status, message)
		{
			if ( status == 1 )
			{
				bgPage.Terminology_LoadModule('Calendar', function(status, message)
				{
					bgPage.Terminology_LoadModule('Calls', function(status, message)
					{
						if ( status == 0 || status == 1 )
						{
							// 12/06/2014 Paul.  LayoutMode is used on the Mobile view. 
							ctlActiveMenu.ActivateTab('Calendar', null, 'CalendarView');
							this.Render(sLayoutPanel, sActionsPanel, callback, this);
							// 03/10/2013 Paul.  Always load the global layout cache if it has not been loaded. 
							SplendidUI_Cache(function(status, message)
							{
								if ( status == 2 )
								{
									SplendidError.SystemMessage(message);
								}
							});
						}
						else
						{
							callback(status, message);
						}
					}, this);
				}, this);
			}
			else
			{
				callback(-1, message);
			}
		}, this);
	}
	catch(e)
	{
		callback(-1, SplendidError.FormatError(e, 'CalendarViewUI.Load'));
	}
};


