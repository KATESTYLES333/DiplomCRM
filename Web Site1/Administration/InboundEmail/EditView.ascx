<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditView.ascx.cs" Inherits="SplendidCRM.Administration.InboundEmail.EditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<SplendidCRM:InlineScript runat="server">
<script type="text/javascript">
// 11/23/2012 Paul.  Must place within InlineScript to prevent error. The Controls collection cannot be modified because the control contains code blocks (i.e. ).
function toggleUseSSL()
{
	var MAILBOX_SSL = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "MAILBOX_SSL").ClientID %>');
	var SERVICE     = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "SERVICE"    ).ClientID %>');
	var PORT        = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "PORT"       ).ClientID %>');
	if ( MAILBOX_SSL != null && PORT != null )
	{
		/*
		IMAP values. 
		if ( MAILBOX_SSL.checked )
			PORT.value = 993;
		else
			PORT.value = 143;
		*/
	}
}
</script>
</SplendidCRM:InlineScript>
<div id="divEditView" runat="server">
	<script runat="server">
	// 08/29/2009 Paul.  Module Popup logic is now dynamic. 
	</script>

	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="InboundEmail" EnablePrint="false" HelpName="EditView" EnableHelp="true" Runat="Server" />
	<p>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlDynamicButtons" Visible="<%# !PrintView %>" ShowRequired="true" Runat="Server" />
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<table ID="tblMain" class="tabEditView" runat="server">
					<tr>
						<th colspan="5"><h4><asp:Label Text='<%# L10n.Term("InboundEmail.LBL_BASIC") %>' runat="server" /></h4></th>
					</tr>
				</table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
	<p>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<table ID="tblOptions" class="tabEditView" runat="server">
					<tr>
						<th colspan="5"><h4><asp:Label Text='<%# L10n.Term("InboundEmail.LBL_EMAIL_OPTIONS") %>' runat="server" /></h4></th>
					</tr>
				</table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
	<SplendidCRM:DynamicButtons ID="ctlFooterButtons" Visible="<%# !PrintView %>" ShowRequired="false" Runat="Server" />
</div>


