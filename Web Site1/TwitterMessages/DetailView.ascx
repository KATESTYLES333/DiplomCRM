<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DetailView.ascx.cs" Inherits="SplendidCRM.TwitterMessages.DetailView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divDetailView" runat="server">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="TwitterMessages" EnablePrint="true" HelpName="DetailView" EnableHelp="true" EnableFavorites="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlDynamicButtons" Visible="<%# !PrintView %>" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="DetailNavigation" Src="~/_controls/DetailNavigation.ascx" %>
	<SplendidCRM:DetailNavigation ID="ctlDetailNavigation" Module="<%# m_sMODULE %>" Visible="<%# !PrintView %>" Runat="Server" />

	<script type="text/javascript">
	function OAuthTokenUpdate(oauth_token, oauth_verifier)
	{
		document.getElementById('<%= txtOAUTH_TOKEN.ClientID    %>').value = oauth_token   ;
		document.getElementById('<%= txtOAUTH_VERIFIER.ClientID %>').value = oauth_verifier;
		document.getElementById('<%= btnOAuthChanged.ClientID   %>').click();
	}
	</script>

	<asp:HiddenField ID="txtOAUTH_TOKEN"         runat="server" />
	<asp:HiddenField ID="txtOAUTH_SECRET"        runat="server" />
	<asp:HiddenField ID="txtOAUTH_VERIFIER"      runat="server" />
	<asp:HiddenField ID="txtOAUTH_ACCESS_TOKEN"  runat="server" />
	<asp:HiddenField ID="txtOAUTH_ACCESS_SECRET" runat="server" />
	<asp:Button ID="btnOAuthChanged" CommandName="OAuthToken" OnCommand="Page_Command" style="display: none" Runat="server" />
	<asp:HiddenField ID="LAYOUT_DETAIL_VIEW" Runat="server" />
	<table ID="tblMain" class="tabDetailView" runat="server">
	</table>

	<div id="divDetailSubPanel">
		<asp:PlaceHolder ID="plcSubPanel" Runat="server" />
	</div>
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />


