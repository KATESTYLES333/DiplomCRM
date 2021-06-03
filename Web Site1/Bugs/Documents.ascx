<%@ Control CodeBehind="Documents.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Bugs.Documents" %>
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
function DocumentPopup()
{
	return ModulePopup('Documents', '<%= txtDOCUMENT_ID.ClientID %>', null, 'ClearDisabled=1', true, null);
}
</script>
<input ID="txtDOCUMENT_ID" type="hidden" Runat="server" />
<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
<SplendidCRM:ListHeader SubPanel="divBugsDocuments" Title="Documents.LBL_MODULE_NAME" Runat="Server" />

<div id="divBugsDocuments" style='<%= "display:" + (CookieValue("divBugsDocuments") != "1" ? "inline" : "none") %>'>
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Button Visible='<%# SplendidCRM.Security.GetUserAccess("Documents", "edit") >= 0 %>' CommandName="Documents.Create" OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_NEW_BUTTON_LABEL"   ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_NEW_BUTTON_TITLE"   ) %>' AccessKey='<%# L10n.AccessKey(".LBL_NEW_BUTTON_KEY"   ) %>' Runat="server" />
		<asp:Button                                                     UseSubmitBehavior="false" OnClientClick="return DocumentPopup();"                 CssClass="button" Text='<%# "  " + L10n.Term(".LBL_SELECT_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_SELECT_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_SELECT_BUTTON_KEY") %>' Runat="server" />
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>

	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdSubPanelView" AllowPaging="<%# !PrintView %>" EnableViewState="true" runat="server">
		<Columns>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
				<ItemTemplate>
					<div style="DISPLAY: <%# Sql.ToString(DataBinder.Eval(Container.DataItem, "REVISION")) != Sql.ToString(DataBinder.Eval(Container.DataItem, "SELECTED_REVISION")) ? "inline" : "none" %>">
						<asp:ImageButton Visible='<%# SplendidCRM.Security.GetUserAccess("Bugs", "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "BUG_ASSIGNED_USER_ID"))) >= 0 %>' CommandName="Documents.GetLatest" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "DOCUMENT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_GET_LATEST") %>' SkinID="getLatestDocument" Runat="server" />
						<asp:LinkButton  Visible='<%# SplendidCRM.Security.GetUserAccess("Bugs", "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "BUG_ASSIGNED_USER_ID"))) >= 0 %>' CommandName="Documents.GetLatest" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "DOCUMENT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_GET_LATEST") %>' Runat="server" />
					</div>
					<asp:ImageButton Visible='<%# !bEditView && SplendidCRM.Security.GetUserAccess("Documents", "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ASSIGNED_USER_ID"))) >= 0 %>' CommandName="Documents.Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "DOCUMENT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_EDIT") %>' SkinID="edit_inline" Runat="server" />
					<asp:LinkButton  Visible='<%# !bEditView && SplendidCRM.Security.GetUserAccess("Documents", "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ASSIGNED_USER_ID"))) >= 0 %>' CommandName="Documents.Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "DOCUMENT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_EDIT") %>' Runat="server" />
					&nbsp;
					<span onclick="return confirm('<%= L10n.TermJavaScript(".NTC_DELETE_CONFIRMATION") %>')">
						<asp:ImageButton Visible='<%# SplendidCRM.Security.GetUserAccess("Bugs", "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "BUG_ASSIGNED_USER_ID"))) >= 0 %>' CommandName="Documents.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "DOCUMENT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_REMOVE") %>' SkinID="delete_inline" Runat="server" />
						<asp:LinkButton  Visible='<%# SplendidCRM.Security.GetUserAccess("Bugs", "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "BUG_ASSIGNED_USER_ID"))) >= 0 %>' CommandName="Documents.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "DOCUMENT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_REMOVE") %>' Runat="server" />
					</span>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
</div>

