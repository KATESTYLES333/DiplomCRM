<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.ChatChannels.ListView" %>
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
<div id="divListView">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlModuleHeader" Module="ChatChannels" Title=".moduleList.Home" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchView" Src="~/_controls/SearchView.ascx" %>
	<SplendidCRM:SearchView ID="ctlSearchView" Module="ChatChannels" ShowSearchTabs="false" Visible="<%# !PrintView %>" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Module="ChatChannels" Title="ChatChannels.LBL_LIST_FORM_TITLE" Runat="Server" />
	
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>
	
	<asp:HiddenField ID="LAYOUT_LIST_VIEW" Runat="server" />
	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdListView" AllowPaging="<%# !PrintView %>" EnableViewState="true" runat="server">
	</SplendidCRM:SplendidGrid>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	 
</div>

