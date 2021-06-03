<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ConfigView.ascx.cs" Inherits="SplendidCRM.Administration.Twilio.ConfigView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script runat="server">
/**********************************************************************************************************************
 * SplendidCRM is a Customer Relationship Management program created by SplendidCRM Software, Inc. 
 * Copyright (C) 2005-2015 SplendidCRM Software, Inc. All rights reserved.
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
<div id="divEditView" runat="server">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" ShowRequired="true" EditView="true" Module="Twilio" Title="Twilio.LBL_TWILIO_SETTINGS" EnableModuleLabel="false" EnablePrint="false" EnableHelp="true" Runat="Server" />
	
	<p></p>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("Twilio.LBL_ACCOUNT_SID") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
				<asp:TextBox ID="ACCOUNT_SID" Size="40" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("Twilio.LBL_AUTH_TOKEN") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
				<asp:TextBox ID="AUTH_TOKEN" Size="40" Runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("Twilio.LBL_FROM_PHONE") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
				<asp:TextBox ID="FROM_PHONE" Size="40" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
			</asp:TableCell>
			<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label ID="Label2" Text='<%# L10n.Term("Twilio.LBL_LOG_INBOUND_MESSAGES") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
				<asp:CheckBox ID="LOG_INBOUND_MESSAGES" CssClass="checkbox" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("Twilio.LBL_MESSAGE_REQUEST_URL") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
				<asp:Label ID="MESSAGE_REQUEST_URL" Runat="server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<p></p>
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlFooterButtons" Visible="<%# !SplendidDynamic.StackedLayout(this.Page.Theme) && !PrintView %>" ShowRequired="false" Runat="Server" />
</div>

