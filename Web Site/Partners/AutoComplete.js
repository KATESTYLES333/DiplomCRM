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

function Partners_partner_NAME_Changed(fldpartner_NAME)
{
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userResource field. 
	// 08/24/2009 Paul.  One of the base controls can contain NAME in the text, so just get the length minus 4. 
	var userContext = fldpartner_NAME.id.substring(0, fldpartner_NAME.id.length - 'partner_NAME'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'partner_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_partner_NAME = document.getElementById(userContext + 'PREV_partner_NAME');
	if ( fldPREV_partner_NAME == null )
	{
		//alert('Could not find ' + userContext + 'PREV_partner_NAME');
	}
	else if ( fldPREV_partner_NAME.value != fldpartner_NAME.value )
	{
		if ( fldpartner_NAME.value.length > 0 )
		{
			try
			{
				SplendidCRM.Partners.AutoComplete.Partners_partner_NAME_Get(fldpartner_NAME.value, Partners_partner_NAME_Changed_OnSucceededWithContext, Partners_partner_NAME_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('Partners_partner_NAME_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'NAME' : '' };
			Partners_partner_NAME_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function Partners_partner_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		var fldAjaxErrors        = document.getElementById(userContext + 'partner_NAME_AjaxErrors');
		var fldpartner_ID        = document.getElementById(userContext + 'partner_ID'       );
		var fldpartner_NAME      = document.getElementById(userContext + 'partner_NAME'     );
		var fldPREV_partner_NAME = document.getElementById(userContext + 'PREV_partner_NAME');
		if ( fldpartner_ID        != null ) fldpartner_ID.value        = sID  ;
		if ( fldpartner_NAME      != null ) fldpartner_NAME.value      = sNAME;
		if ( fldPREV_partner_NAME != null ) fldPREV_partner_NAME.value = sNAME;
		// 03/24/2011 Paul.  If an Update button is available, then click it. 
		var fldpartner_UPDATE = document.getElementById(userContext + 'partner_UPDATE');
		if ( fldpartner_UPDATE != null )
			fldpartner_UPDATE.click();
	}
	else
	{
		alert('result from Partners.AutoComplete service is null');
	}
}

function Partners_partner_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'partner_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldpartner_ID        = document.getElementById(userContext + 'partner_ID'       );
	var fldpartner_NAME      = document.getElementById(userContext + 'partner_NAME'     );
	var fldPREV_partner_NAME = document.getElementById(userContext + 'PREV_partner_NAME');
	if ( fldpartner_ID        != null ) fldpartner_ID.value        = '';
	if ( fldpartner_NAME      != null ) fldpartner_NAME.value      = '';
	if ( fldPREV_partner_NAME != null ) fldPREV_partner_NAME.value = '';
}

// 07/27/2010 Paul.  Allow Partner lookup from Quotes, Orders or Invoices. 
function Partners_BILLING_partner_NAME_Changed(fldpartner_NAME)
{
	var userContext = fldpartner_NAME.id.substring(0, fldpartner_NAME.id.length - 'BILLING_partner_NAME'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'BILLING_partner_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_partner_NAME = document.getElementById(userContext + 'PREV_BILLING_partner_NAME');
	if ( fldPREV_partner_NAME == null )
	{
		//alert('Could not find ' + userContext + 'PREV_partner_NAME');
	}
	else if ( fldPREV_partner_NAME.value != fldpartner_NAME.value )
	{
		if ( fldpartner_NAME.value.length > 0 )
		{
			try
			{
				SplendidCRM.Partners.AutoComplete.Partners_partner_NAME_Get(fldpartner_NAME.value, Partners_BILLING_partner_NAME_Changed_OnSucceededWithContext, Partners_BILLING_partner_NAME_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('Partners_BILLING_partner_NAME_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'NAME' : '' };
			Partners_BILLING_partner_NAME_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function Partners_BILLING_partner_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		var fldAjaxErrors        = document.getElementById(userContext + 'BILLING_partner_NAME_AjaxErrors');
		var fldpartner_ID        = document.getElementById(userContext + 'BILLING_partner_ID'  );
		var fldpartner_NAME      = document.getElementById(userContext + 'BILLING_partner_NAME');
		var fldPREV_partner_NAME = document.getElementById(userContext + 'PREV_BILLING_ACCOUNT');
		if ( fldpartner_ID        != null ) fldpartner_ID.value        = sID  ;
		if ( fldpartner_NAME      != null ) fldpartner_NAME.value      = sNAME;
		if ( fldPREV_partner_NAME != null ) fldPREV_partner_NAME.value = sNAME;
		// 07/27/2010 Paul.  We typically submit the form when the partner changes so that we can load the address. 
		// 08/21/2010 Paul.  If an Update button is available, then click it. 
		var fldBILLING_partner_UPDATE = document.getElementById(userContext + 'BILLING_partner_UPDATE');
		if ( fldBILLING_partner_UPDATE != null )
			fldBILLING_partner_UPDATE.click();
	}
	else
	{
		alert('result from Partners.AutoComplete service is null');
	}
}

function Partners_BILLING_partner_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'BILLING_partner_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldpartner_ID        = document.getElementById(userContext + 'BILLING_partner_ID'       );
	var fldpartner_NAME      = document.getElementById(userContext + 'BILLING_partner_NAME'     );
	var fldPREV_partner_NAME = document.getElementById(userContext + 'PREV_BILLING_partner_NAME');
	if ( fldpartner_ID        != null ) fldpartner_ID.value        = '';
	if ( fldpartner_NAME      != null ) fldpartner_NAME.value      = '';
	if ( fldPREV_partner_NAME != null ) fldPREV_partner_NAME.value = '';
}

function Partners_SHIPPING_partner_NAME_Changed(fldpartner_NAME)
{
	var userContext = fldpartner_NAME.id.substring(0, fldpartner_NAME.id.length - 'SHIPPING_partner_NAME'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'SHIPPING_partner_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_partner_NAME = document.getElementById(userContext + 'PREV_SHIPPING_partner_NAME');
	if ( fldPREV_partner_NAME == null )
	{
		//alert('Could not find ' + userContext + 'PREV_partner_NAME');
	}
	else if ( fldPREV_partner_NAME.value != fldpartner_NAME.value )
	{
		if ( fldpartner_NAME.value.length > 0 )
		{
			try
			{
				SplendidCRM.Partners.AutoComplete.Partners_partner_NAME_Get(fldpartner_NAME.value, Partners_SHIPPING_partner_NAME_Changed_OnSucceededWithContext, Partners_SHIPPING_partner_NAME_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('Partners_SHIPPING_partner_NAME_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'NAME' : '' };
			Partners_SHIPPING_partner_NAME_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function Partners_SHIPPING_partner_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		var fldAjaxErrors        = document.getElementById(userContext + 'SHIPPING_partner_NAME_AjaxErrors');
		var fldpartner_ID        = document.getElementById(userContext + 'SHIPPING_partner_ID'  );
		var fldpartner_NAME      = document.getElementById(userContext + 'SHIPPING_partner_NAME');
		var fldPREV_partner_NAME = document.getElementById(userContext + 'PREV_SHIPPING_ACCOUNT');
		if ( fldpartner_ID        != null ) fldpartner_ID.value        = sID  ;
		if ( fldpartner_NAME      != null ) fldpartner_NAME.value      = sNAME;
		if ( fldPREV_partner_NAME != null ) fldPREV_partner_NAME.value = sNAME;
		// 07/27/2010 Paul.  We typically submit the form when the partner changes so that we can load the address. 
		// 08/21/2010 Paul.  If an Update button is available, then click it. 
		var fldSHIPPING_partner_UPDATE = document.getElementById(userContext + 'SHIPPING_partner_UPDATE');
		if ( fldSHIPPING_partner_UPDATE != null )
			fldSHIPPING_partner_UPDATE.click();
	}
	else
	{
		alert('result from Partners.AutoComplete service is null');
	}
}

function Partners_SHIPPING_partner_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'SHIPPING_partner_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldpartner_ID        = document.getElementById(userContext + 'SHIPPING_partner_ID'       );
	var fldpartner_NAME      = document.getElementById(userContext + 'SHIPPING_partner_NAME'     );
	var fldPREV_partner_NAME = document.getElementById(userContext + 'PREV_SHIPPING_partner_NAME');
	if ( fldpartner_ID        != null ) fldpartner_ID.value        = '';
	if ( fldpartner_NAME      != null ) fldpartner_NAME.value      = '';
	if ( fldPREV_partner_NAME != null ) fldPREV_partner_NAME.value = '';
}

// 07/28/2010 Paul.  We need this function in order to allow AutoComplete in the Partners Parent field. 
function Partners_PARENT_NAME_Changed(fldpartner_NAME)
{
	var userContext = fldpartner_NAME.id.substring(0, fldpartner_NAME.id.length - 'PARENT_NAME'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'PARENT_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_partner_NAME = document.getElementById(userContext + 'PREV_PARENT_NAME');
	if ( fldPREV_partner_NAME == null )
	{
		//alert('Could not find ' + userContext + 'PREV_partner_NAME');
	}
	else if ( fldPREV_partner_NAME.value != fldpartner_NAME.value )
	{
		if ( fldpartner_NAME.value.length > 0 )
		{
			try
			{
				SplendidCRM.Partners.AutoComplete.Partners_partner_NAME_Get(fldpartner_NAME.value, Partners_PARENT_NAME_Changed_OnSucceededWithContext, Partners_PARENT_NAME_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('Partners_PARENT_NAME_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'NAME' : '' };
			Partners_PARENT_NAME_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function Partners_PARENT_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		var fldAjaxErrors        = document.getElementById(userContext + 'PARENT_NAME_AjaxErrors');
		var fldpartner_ID        = document.getElementById(userContext + 'PARENT_ID'  );
		var fldpartner_NAME      = document.getElementById(userContext + 'PARENT_NAME');
		var fldPREV_partner_NAME = document.getElementById(userContext + 'PREV_PARENT');
		if ( fldpartner_ID        != null ) fldpartner_ID.value        = sID  ;
		if ( fldpartner_NAME      != null ) fldpartner_NAME.value      = sNAME;
		if ( fldPREV_partner_NAME != null ) fldPREV_partner_NAME.value = sNAME;
	}
	else
	{
		alert('result from Partners.AutoComplete service is null');
	}
}

function Partners_PARENT_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'PARENT_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldpartner_ID        = document.getElementById(userContext + 'PARENT_ID'       );
	var fldpartner_NAME      = document.getElementById(userContext + 'PARENT_NAME'     );
	var fldPREV_partner_NAME = document.getElementById(userContext + 'PREV_PARENT_NAME');
	if ( fldpartner_ID        != null ) fldpartner_ID.value        = '';
	if ( fldpartner_NAME      != null ) fldpartner_NAME.value      = '';
	if ( fldPREV_partner_NAME != null ) fldPREV_partner_NAME.value = '';
}

if ( typeof(Sys) !== 'undefined' )
	Sys.Application.notifyScriptLoaded();



