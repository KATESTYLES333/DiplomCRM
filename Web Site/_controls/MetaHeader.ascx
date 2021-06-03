<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MetaHeader.ascx.cs" Inherits="SplendidCRM._controls.MetaHeader" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
	<meta name="keywords" content="<%= Application["CONFIG.header_keywords"] %>" />
	<link rel="SHORTCUT ICON" href="<%= Application["imageURL"] %>SplendidCRM_Icon.ico" />
	<!-- 01/24/2018 Paul.  Include version in url to ensure updates of combined files. -->
	<script type="text/javascript" src="<%= Application["rootURL"] %>HeaderScriptsCombined<%= "_" + Sql.ToString(Application["SplendidVersion"]) %>"></script>

<script type="text/javascript">
// 05/08/2010 Paul.  Move onkeypress to SplendidCRM.js. 
// 05/08/2010 Paul.  Create rootURL javascript variable for use by the CalendarPopup. 
// This will allow us to remove the CalendarPopup definitions in the Admin area. 
// 08/25/2013 Paul.  Move sREMOTE_SERVER definition to the master pages. 
// 09/07/2013 Paul.  Change rootURL to sREMOTE_SERVER to match Survey module. 
// 09/20/2013 Paul.  Move EXTENSION to the main table. 
// 09/27/2013 Paul.  SMS messages need to be opt-in. 
// 11/10/2014 Paul.  Add Chat Channels. 
// 06/29/2017 Paul.  AssemblyVersion is needed for HTML5 Dashboard. 
var sAssemblyVersion     = '<%# Sql.ToString(Application["SplendidVersion"]) %>';
var sREMOTE_SERVER       = '<%# Application["rootURL"] %>';
var sUSER_EXTENSION      = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["EXTENSION"   ])) %>';
var sUSER_FULL_NAME      = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["FULL_NAME"   ])) %>';
var sUSER_PHONE_WORK     = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["PHONE_WORK"  ])) %>';
var sUSER_SMS_OPT_IN     = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["SMS_OPT_IN"  ])) %>';
var sUSER_PHONE_MOBILE   = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["PHONE_MOBILE"])) %>';
var sUSER_TWITTER_TRACKS = '';
var sUSER_CHAT_CHANNELS  = '';

</script>


