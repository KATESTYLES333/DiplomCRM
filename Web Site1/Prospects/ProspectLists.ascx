<%@ Control CodeBehind="ProspectLists.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Prospects.ProspectLists" %>
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
function ProspectListPopup()
{
	return ModulePopup('ProspectLists', '<%= txtPROSPECT_LIST_ID.ClientID %>', null, 'ClearDisabled=1', true, null);
}
</script>
<input ID="txtPROSPECT_LIST_ID" type="hidden" Runat="server" />
<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
<SplendidCRM:ListHeader SubPanel="divProspectsProspectLists" Title="ProspectLists.LBL_MODULE_NAME" Runat="Server" />

<div id="divProspectsProspectLists" style='<%= "display:" + (CookieValue("divProspectsProspectLists") != "1" ? "inline" : "none") %>'>
	<asp:UpdatePanel UpdateMode="Conditional" runat="server">
		<ContentTemplate>
			<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
			<SplendidCRM:DynamicButtons ID="ctlDynamicButtons" Visible="<%# !PrintView %>" Runat="Server" />

			<asp:Panel ID="pnlNewRecordInline" Visible='<%# !Sql.ToBoolean(Application["CONFIG.disable_editview_inline"]) %>' Style="display:none" runat="server">
				<%@ Register TagPrefix="SplendidCRM" Tagname="NewRecord" Src="~/ProspectLists/NewRecord.ascx" %>
				<SplendidCRM:NewRecord ID="ctlNewRecord" Width="100%" EditView="EditView.Inline" ShowCancel="true" ShowHeader="false" ShowFullForm="true" ShowTopButtons="true" Runat="Server" />
			</asp:Panel>
		</ContentTemplate>
	</asp:UpdatePanel>

	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdSubPanelView" AllowPaging="<%# !PrintView %>" EnableViewState="true" runat="server">
		<Columns>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
				<ItemTemplate>
					<asp:ImageButton Visible='<%# SplendidCRM.Security.GetUserAccess("ProspectLists", "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ASSIGNED_USER_ID"))) >= 0 %>' CommandName="ProspectLists.Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PROSPECT_LIST_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_EDIT") %>' SkinID="edit_inline" Runat="server" />
					<asp:LinkButton  Visible='<%# SplendidCRM.Security.GetUserAccess("ProspectLists", "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ASSIGNED_USER_ID"))) >= 0 %>' CommandName="ProspectLists.Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PROSPECT_LIST_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_EDIT") %>' Runat="server" />
					&nbsp;
					<span onclick="return confirm('<%= L10n.TermJavaScript("ProspectLists.NTC_PROSPECT_REMOVE_PROSPECT_LISTS_CONFIRM") %>')">
						<asp:ImageButton Visible='<%# SplendidCRM.Security.GetUserAccess("ProspectLists", "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ASSIGNED_USER_ID"))) >= 0 %>' CommandName="ProspectLists.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PROSPECT_LIST_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_REMOVE") %>' SkinID="delete_inline" Runat="server" />
						<asp:LinkButton  Visible='<%# SplendidCRM.Security.GetUserAccess("ProspectLists", "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ASSIGNED_USER_ID"))) >= 0 %>' CommandName="ProspectLists.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PROSPECT_LIST_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_REMOVE") %>' Runat="server" />
					</span>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
</div>


