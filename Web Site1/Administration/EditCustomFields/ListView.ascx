<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.EditCustomFields.ListView" %>
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
<script type="text/javascript">
function ExportSQL()
{
		return window.open('export.aspx?MODULE_NAME=<%= sMODULE_NAME %>','ExportSQL','width=1200,height=600,resizable=1,scrollbars=1');
}
</script>
<div id="divListView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="EditCustomFields" Title="EditCustomFields.LBL_MODULE_TITLE" EnableModuleLabel="false" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchBasic" Src="SearchBasic.ascx" %>
	<SplendidCRM:SearchBasic ID="ctlSearch" Runat="Server" />
	<br />
	<asp:Table SkinID="tabFrame" CssClass="h3Row" runat="server">
		<asp:TableRow>
			<asp:TableCell Wrap="false">
				<h3><asp:Image SkinID="h3Arrow" Runat="server" />&nbsp;<asp:Label ID="Label1" Text='<%# L10n.Term("EditCustomFields.LBL_CUSTOM_FIELDS") %>' runat="server" /></h3>
			</asp:TableCell>
			<asp:TableCell HorizontalAlign="Right">
				<asp:Button ID="btnExport" Visible='<%# SplendidCRM.Security.AdminUserAccess(m_sMODULE, "export") >= 0 %>' UseSubmitBehavior="false" OnClientClick="ExportSQL(); return false;"    CssClass="button" Text='<%# L10n.Term(".LBL_EXPORT_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_EXPORT_BUTTON_TITLE"  ) %>' Runat="server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>

	<p>
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>
	
	<SplendidCRM:SplendidGrid id="grdMain" AllowPaging="false" AllowSorting="true" EnableViewState="true" runat="server">
		<Columns>
			<asp:TemplateColumn  HeaderText="EditCustomFields.COLUMN_TITLE_NAME" SortExpression="NAME" ItemStyle-Width="22%" ItemStyle-CssClass="listViewTdLinkS1">
				<ItemTemplate>
					<asp:HyperLink Enabled='<%# SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0 %>' Text='<%# DataBinder.Eval(Container.DataItem, "NAME") %>' NavigateUrl='<%# "edit.aspx?ID=" + DataBinder.Eval(Container.DataItem, "ID") %>' CssClass="listViewTdLinkS1" Runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn     HeaderText="EditCustomFields.COLUMN_TITLE_LABEL"           DataField="LABEL"           SortExpression="LABEL"           ItemStyle-Width="22%" />
			<asp:BoundColumn     HeaderText="EditCustomFields.COLUMN_TITLE_DATA_TYPE"       DataField="DATA_TYPE"       SortExpression="DATA_TYPE"       ItemStyle-Width="10%" />
			<asp:BoundColumn     HeaderText="EditCustomFields.COLUMN_TITLE_MAX_SIZE"        DataField="MAX_SIZE"        SortExpression="MAX_SIZE"        ItemStyle-Width="10%" />
			<asp:BoundColumn     HeaderText="EditCustomFields.COLUMN_TITLE_REQUIRED_OPTION" DataField="REQUIRED_OPTION" SortExpression="REQUIRED_OPTION" ItemStyle-Width="10%" />
			<asp:BoundColumn     HeaderText="EditCustomFields.COLUMN_TITLE_DEFAULT_VALUE"   DataField="DEFAULT_VALUE"   SortExpression="DEFAULT_VALUE"   ItemStyle-Width="10%" />
			<asp:BoundColumn     HeaderText="EditCustomFields.COLUMN_TITLE_DROPDOWN"        DataField="EXT1"            SortExpression="EXT1"            ItemStyle-Width="10%" />
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
				<ItemTemplate>
					<span onclick="return confirm('<%= L10n.TermJavaScript(".NTC_DELETE_CONFIRMATION") %>')">
						<asp:ImageButton Visible='<%# SplendidCRM.Security.AdminUserAccess(m_sMODULE, "delete") >= 0 %>' CommandName="EditCustomFields.Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_DELETE") %>' SkinID="delete_inline" Runat="server" />
						<asp:LinkButton  Visible='<%# SplendidCRM.Security.AdminUserAccess(m_sMODULE, "delete") >= 0 %>' CommandName="EditCustomFields.Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_DELETE") %>' Runat="server" />
					</span>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
	
	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>


