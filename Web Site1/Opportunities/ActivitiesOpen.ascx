<%@ Control CodeBehind="Activities.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Opportunities.Activities" %>
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
<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
<SplendidCRM:ListHeader SubPanel="divOpportunitiesActivitiesOpen" Title="Activities.LBL_OPEN_ACTIVITIES" Runat="Server" />

<div id="divOpportunitiesActivitiesOpen" style='<%= "display:" + (CookieValue("divOpportunitiesActivitiesOpen") != "1" ? "inline" : "none") %>'>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlDynamicButtonsOpen" Visible="<%# !PrintView %>" Runat="Server" />
	
	<asp:Panel ID="pnlNewRecordInlineTask" Visible='<%# !Sql.ToBoolean(Application["CONFIG.disable_editview_inline"]) %>' Style="display:none" runat="server">
		<%@ Register TagPrefix="SplendidCRM" Tagname="NewRecordTask" Src="~/Tasks/NewRecord.ascx" %>
		<SplendidCRM:NewRecordTask ID="ctlNewRecordTask" Width="100%" EditView="EditView.Inline" ShowCancel="true" ShowHeader="false" ShowFullForm="true" ShowTopButtons="true" Runat="Server" />
	</asp:Panel>
	<asp:Panel ID="pnlNewRecordInlineCall" Visible='<%# !Sql.ToBoolean(Application["CONFIG.disable_editview_inline"]) %>' Style="display:none" runat="server">
		<%@ Register TagPrefix="SplendidCRM" Tagname="NewRecordCall" Src="~/Calls/NewRecord.ascx" %>
		<SplendidCRM:NewRecordCall ID="ctlNewRecordCall" Width="100%" EditView="EditView.Inline" ShowCancel="true" ShowHeader="false" ShowFullForm="true" ShowTopButtons="true" Runat="Server" />
	</asp:Panel>
	<asp:Panel ID="pnlNewRecordInlineMeeting" Visible='<%# !Sql.ToBoolean(Application["CONFIG.disable_editview_inline"]) %>' Style="display:none" runat="server">
		<%@ Register TagPrefix="SplendidCRM" Tagname="NewRecordMeeting" Src="~/Meetings/NewRecord.ascx" %>
		<SplendidCRM:NewRecordMeeting ID="ctlNewRecordMeeting" Width="100%" EditView="EditView.Inline" ShowCancel="true" ShowHeader="false" ShowFullForm="true" ShowTopButtons="true" Runat="Server" />
	</asp:Panel>
	
	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchView" Src="~/_controls/SearchView.ascx" %>
	<SplendidCRM:SearchView ID="ctlSearchViewOpen" Module="Activities" SearchMode="SearchSubpanel" IsSubpanelSearch="true" ShowSearchTabs="false" ShowDuplicateSearch="false" ShowSearchViews="false" Visible="false" Runat="Server" />
	
	<SplendidCRM:SplendidGrid id="grdOpen" SkinID="grdSubPanelView" AllowPaging="<%# !PrintView %>" EnableViewState="true" runat="server">
		<Columns>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Center">
				<ItemTemplate>
					<SplendidCRM:DynamicImage ImageSkinID='<%# DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE") %>' runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="Activities.LBL_LIST_CLOSE" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Center">
				<ItemTemplate>
					<asp:HyperLink Visible='<%# SplendidCRM.Security.GetUserAccess(Sql.ToString(DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE")), "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ACTIVITY_ASSIGNED_USER_ID"))) >= 0 %>' NavigateUrl='<%# "~/" + DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE") + "/edit.aspx?id=" + DataBinder.Eval(Container.DataItem, "ACTIVITY_ID") + "&Status=Close" + "&PARENT_ID=" + gID.ToString() %>' Runat="server">
						<asp:Image SkinID="close_inline" AlternateText='<%# L10n.Term("Activities.LBL_LIST_CLOSE") %>' Runat="server" />
					</asp:HyperLink>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn  HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
				<ItemTemplate>
					<asp:HyperLink Visible='<%# SplendidCRM.Security.GetUserAccess(Sql.ToString(DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE")), "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ACTIVITY_ASSIGNED_USER_ID"))) >= 0 %>' NavigateUrl='<%# "~/" + DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE") + "/edit.aspx?id=" + DataBinder.Eval(Container.DataItem, "ACTIVITY_ID") %>' CssClass="listViewTdToolsS1" Runat="server">
						<asp:Image SkinID="edit_inline" AlternateText='<%# L10n.Term(".LNK_EDIT") %>' Runat="server" />&nbsp;<%# L10n.Term(".LNK_EDIT") %>
					</asp:HyperLink>
					&nbsp;
					<span onclick="return confirm('<%= L10n.TermJavaScript(".NTC_DELETE_CONFIRMATION") %>')">
						<asp:ImageButton Visible='<%# SplendidCRM.Security.GetUserAccess(Sql.ToString(DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE")), "delete", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ACTIVITY_ASSIGNED_USER_ID"))) >= 0 %>' CommandName="Activities.Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ACTIVITY_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_DELETE") %>' SkinID="delete_inline" Runat="server" />
						<asp:LinkButton  Visible='<%# SplendidCRM.Security.GetUserAccess(Sql.ToString(DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE")), "delete", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ACTIVITY_ASSIGNED_USER_ID"))) >= 0 %>' CommandName="Activities.Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ACTIVITY_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_DELETE") %>' Runat="server" />
					</span>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
</div>

<SplendidCRM:ListHeader SubPanel="divOpportunitiesActivitiesHistory" Title="Activities.LBL_HISTORY" Visible="false" Runat="Server" />

<div id="divOpportunitiesActivitiesHistory" visible="false" runat="server">
	<SplendidCRM:DynamicButtons ID="ctlDynamicButtonsHistory" Visible="<%# !PrintView %>" Runat="Server" />
	
	<asp:Panel ID="pnlNewRecordInlineNote" Visible='<%# !Sql.ToBoolean(Application["CONFIG.disable_editview_inline"]) %>' Style="display:none" runat="server">
		<%@ Register TagPrefix="SplendidCRM" Tagname="NewRecordNote" Src="~/Notes/NewRecord.ascx" %>
		<SplendidCRM:NewRecordNote ID="ctlNewRecordNote" Width="100%" EditView="EditView.Inline" ShowCancel="true" ShowHeader="false" ShowFullForm="true" ShowTopButtons="true" Runat="Server" />
	</asp:Panel>
	
	<SplendidCRM:SearchView ID="ctlSearchViewHistory" Module="Activities" SearchMode="SearchSubpanel" IsSubpanelSearch="true" ShowSearchTabs="false" ShowDuplicateSearch="false" ShowSearchViews="false" Visible="false" Runat="Server" />
	
	<SplendidCRM:SplendidGrid id="grdHistory" SkinID="grdSubPanelView" AllowPaging="<%# !PrintView %>" EnableViewState="true" runat="server">
		<Columns>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Center">
				<ItemTemplate>
					<SplendidCRM:DynamicImage ImageSkinID='<%# DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE") %>' runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn  HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
				<ItemTemplate>
					<asp:HyperLink Visible='<%# SplendidCRM.Security.GetUserAccess(Sql.ToString(DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE")), "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ACTIVITY_ASSIGNED_USER_ID"))) >= 0 %>' NavigateUrl='<%# "~/" + DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE") + "/edit.aspx?id=" + DataBinder.Eval(Container.DataItem, "ACTIVITY_ID") %>' CssClass="listViewTdToolsS1" Runat="server">
						<asp:Image SkinID="edit_inline" AlternateText='<%# L10n.Term(".LNK_EDIT") %>' Runat="server" />&nbsp;<%# L10n.Term(".LNK_EDIT") %>
					</asp:HyperLink>
					&nbsp;
					<span onclick="return confirm('<%= L10n.TermJavaScript(".NTC_DELETE_CONFIRMATION") %>')">
						<asp:ImageButton Visible='<%# SplendidCRM.Security.GetUserAccess(Sql.ToString(DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE")), "delete", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ACTIVITY_ASSIGNED_USER_ID"))) >= 0 %>' CommandName="Activities.Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ACTIVITY_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_DELETE") %>' SkinID="delete_inline" Runat="server" />
						<asp:LinkButton  Visible='<%# SplendidCRM.Security.GetUserAccess(Sql.ToString(DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE")), "delete", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ACTIVITY_ASSIGNED_USER_ID"))) >= 0 %>' CommandName="Activities.Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ACTIVITY_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_DELETE") %>' Runat="server" />
					</span>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
</div>

