<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditView.ascx.cs" Inherits="SplendidCRM.ProspectLists.EditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<asp:UpdatePanel UpdateMode="Conditional" runat="server">
	<ContentTemplate>
		<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
		<%-- 03/16/2016 Paul.  HeaderButtons must be inside UpdatePanel in order to display errors. --%>
		<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
		<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" ShowRequired="true" EditView="true" Module="ProspectLists" EnablePrint="false" HelpName="EditView" EnableHelp="true" Runat="Server" />

		<asp:HiddenField ID="LAYOUT_EDIT_VIEW" Runat="server" />
		<asp:Table SkinID="tabForm" runat="server">
			<asp:TableRow>
				<asp:TableCell>
					<table ID="tblMain" class="tabEditView" runat="server">
					</table>
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>

		<asp:Panel ID="pnlDynamicSQL" runat="server">
			<br />
			<%@ Register TagPrefix="SplendidCRM" Tagname="QueryBuilder" Src="~/Reports/QueryBuilder.ascx" %>
			<%-- 10/27/2017 Paul.  Add Accounts as email source. --%>
			<SplendidCRM:QueryBuilder ID="ctlQueryBuilder" Modules="Contacts,Leads,Prospects,Users,Accounts" UserSpecific="false" ShowRelated="true" PrimaryKeyOnly="true" Runat="Server" />
		</asp:Panel>

		<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
		<%-- 03/16/2016 Paul.  HeaderButtons must be inside UpdatePanel in order to display errors. --%>
		<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
		<SplendidCRM:DynamicButtons ID="ctlFooterButtons" Visible="<%# !SplendidDynamic.StackedLayout(this.Page.Theme) && !PrintView %>" ShowRequired="false" Runat="Server" />
	</ContentTemplate>
</asp:UpdatePanel>

	<div id="divEditSubPanel">
		<asp:PlaceHolder ID="plcSubPanel" Runat="server" />
	</div>
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
 


