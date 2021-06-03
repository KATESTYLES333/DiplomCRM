<%@ Control CodeBehind="PipelineByMonthByOutcome.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Dashboard.SilverlightCharts.PipelineByMonthByOutcome" %>
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
<div id="divDashboardPipelineByMonthByOutcome">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ChartDatePicker" Src="~/_controls/ChartDatePicker.ascx" %>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DashletHeader" Src="~/_controls/DashletHeader.ascx" %>
	<SplendidCRM:DashletHeader ID="ctlDashletHeader" Title="Dashboard.LBL_YEAR_BY_OUTCOME" DivEditName="outcome_by_month_edit2" ShowCommandTitles="true" Runat="Server" />
	<p>
	<div ID="outcome_by_month_edit2" style="DISPLAY: <%= bShowEditDialog ? "inline" : "none" %>">
		<asp:Table SkinID="tabFrame" HorizontalAlign="Center" CssClass="chartForm" runat="server">
			<asp:TableRow>
				<asp:TableCell VerticalAlign="top" Wrap="false"><b><%# L10n.Term("Dashboard.LBL_YEAR") %></b><br /><span class="dateFormat">(yyyy)</span></asp:TableCell>
				<asp:TableCell VerticalAlign="top">
					<asp:TextBox ID="txtYEAR" MaxLength="10" size="12" Runat="server" />
				</asp:TableCell>
				<asp:TableCell VerticalAlign="top" Wrap="false"><b><%# L10n.Term("Dashboard.LBL_USERS") %></b></asp:TableCell>
				<asp:TableCell VerticalAlign="top">
					<asp:ListBox ID="lstASSIGNED_USER_ID" DataValueField="ID" DataTextField="USER_NAME" SelectionMode="Multiple" Rows="3" Runat="server" />
				</asp:TableCell>
				<asp:TableCell VerticalAlign="top" HorizontalAlign="Right">
					<asp:Button ID="btnSubmit" CommandName="Submit" OnCommand="Page_Command"                         CssClass="button" Text='<%# "  " + L10n.Term(".LBL_SELECT_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_SELECT_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_SELECT_BUTTON_KEY") %>' runat="server" />
					<asp:Button ID="btnCancel" UseSubmitBehavior="false" OnClientClick="toggleDisplay('outcome_by_month_edit2'); return false;" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_CANCEL_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_CANCEL_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_CANCEL_BUTTON_KEY") %>' runat="server" />
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
	</div>
	</p>
	<p align="center">
<%@ Register TagPrefix="SplendidCRM" Tagname="PipelineByMonthByOutcome" Src="~/Opportunities/xaml2/PipelineByMonthByOutcome.ascx" %>
<script type="text/xaml" id="xamlPipelineByMonthByOutcome2"><?xml version="1.0"?>
<SplendidCRM:PipelineByMonthByOutcome CHART_LENGTH="10" Visible="<%# SplendidCRM.Crm.Config.enable_silverlight() %>" Runat="Server" />
</script>
	<div id="hostPipelineByMonthByOutcome2" style="width: 800x; height: 400px; padding-bottom: 2px;" align="center"></div>
<SplendidCRM:InlineScript runat="server">
		<script type="text/javascript">
		Silverlight.createObjectEx({
			source: "<%= Application["rootURL" ] %>ClientBin/SilverlightContainer.xap",
			parentElement: document.getElementById("hostPipelineByMonthByOutcome2"),
			id: "SilverlightControl",
			properties:
			{
				width: "800",
				height: "400",
				version: "3.0",
				enableHtmlAccess: "true",
				isWindowless: "true" /* 05/08/2010 Paul.  The isWindowless allows HTML to appear over a silverlight app. */
			},
			events:
			{
			},
			initParams: "xamlContent=xamlPipelineByMonthByOutcome2",
			context: "none"
		});
		</script>
</SplendidCRM:InlineScript>
	</p>
	<span class="chartFootnote">
		<p align="center"><%# L10n.Term("Dashboard.LBL_MONTH_BY_OUTCOME_DESC") %></p>
		<p align="right"><i><%# L10n.Term("Dashboard.LBL_CREATED_ON") + T10n.FromServerTime(DateTime.Now).ToString() %></i></p>
	</span>
</div>


