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

function Resources_RESOURCE_LAST_NAME_Changed(fldRESOURCE_LAST_NAME)
{
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userResource field. 
	// 08/24/2009 Paul.  One of the base controls can contain LAST_NAME in the text, so just get the length minus 4. 
	var userContext = fldRESOURCE_LAST_NAME.id.substring(0, fldRESOURCE_LAST_NAME.id.length - 'RESOURCE_LAST_NAME'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'RESOURCE_LAST_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_RESOURCE_LAST_NAME = document.getElementById(userContext + 'PREV_RESOURCE_LAST_NAME');
	if ( fldPREV_RESOURCE_LAST_NAME == null )
	{
		//alert('Could not find ' + userContext + 'PREV_RESOURCE_LAST_NAME');
	}
	else if ( fldPREV_RESOURCE_LAST_NAME.value != fldRESOURCE_LAST_NAME.value )
	{
		if ( fldRESOURCE_LAST_NAME.value.length > 0 )
		{
			try
			{
				SplendidCRM.Resources.AutoComplete.Resources_RESOURCE_LAST_NAME_Get(fldRESOURCE_LAST_NAME.value, Resources_RESOURCE_LAST_NAME_Changed_OnSucceededWithContext, Resources_RESOURCE_LAST_NAME_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('Resources_RESOURCE_LAST_NAME_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'NAME' : '' };
			Resources_RESOURCE_LAST_NAME_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function Resources_RESOURCE_LAST_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID        = result.ID  ;
		var sLAST_NAME = result.LAST_NAME;
		
		var fldAjaxErrors             = document.getElementById(userContext + 'RESOURCE_LAST_NAME_AjaxErrors');
		var fldRESOURCE_ID             = document.getElementById(userContext + 'RESOURCE_ID'            );
		var fldRESOURCE_LAST_NAME      = document.getElementById(userContext + 'RESOURCE_LAST_NAME'     );
		var fldPREV_RESOURCE_LAST_NAME = document.getElementById(userContext + 'PREV_RESOURCE_LAST_NAME');
		if ( fldRESOURCE_ID             != null ) fldRESOURCE_ID.value             = sID       ;
		if ( fldRESOURCE_LAST_NAME      != null ) fldRESOURCE_LAST_NAME.value      = sLAST_NAME;
		if ( fldPREV_RESOURCE_LAST_NAME != null ) fldPREV_RESOURCE_LAST_NAME.value = sLAST_NAME;
	}
	else
	{
		alert('result from Resources.AutoComplete service is null');
	}
}

function Resources_RESOURCE_LAST_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'RESOURCE_LAST_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldRESOURCE_ID             = document.getElementById(userContext + 'RESOURCE_ID'            );
	var fldRESOURCE_LAST_NAME      = document.getElementById(userContext + 'RESOURCE_LAST_NAME'     );
	var fldPREV_RESOURCE_LAST_NAME = document.getElementById(userContext + 'PREV_RESOURCE_LAST_NAME');
	if ( fldRESOURCE_ID             != null ) fldRESOURCE_ID.value             = '';
	if ( fldRESOURCE_LAST_NAME      != null ) fldRESOURCE_LAST_NAME.value      = '';
	if ( fldPREV_RESOURCE_LAST_NAME != null ) fldPREV_RESOURCE_LAST_NAME.value = '';
}

function Resources_RESOURCE_NAME_ItemSelected(sender, e)
{
	Resources_RESOURCE_NAME_Changed(sender.get_element());
}

function Resources_RESOURCE_NAME_Changed(fldRESOURCE_NAME)
{
	if ( fldRESOURCE_NAME != null )
	{
		var userContext = fldRESOURCE_NAME.id.substring(0, fldRESOURCE_NAME.id.length - 'RESOURCE_NAME'.length)
		var fldAjaxErrors = document.getElementById(userContext + 'RESOURCE_NAME_AjaxErrors');
		if ( fldAjaxErrors != null )
			fldAjaxErrors.innerHTML = '';
		
		var fldPREV_RESOURCE_NAME = document.getElementById(userContext + 'PREV_RESOURCE_NAME');
		if ( fldPREV_RESOURCE_NAME == null )
		{
			//alert('Could not find ' + userContext + 'PREV_RESOURCE_NAME');
		}
		else if ( fldPREV_RESOURCE_NAME.value != fldRESOURCE_NAME.value )
		{
			if ( fldRESOURCE_NAME.value.length > 0 )
			{
				try
				{
					SplendidCRM.Resources.AutoComplete.Resources_RESOURCE_NAME_Get(fldRESOURCE_NAME.value, Resources_RESOURCE_NAME_Changed_OnSucceededWithContext, Resources_RESOURCE_NAME_Changed_OnFailed, userContext);
				}
				catch(e)
				{
					alert('Resources_RESOURCE_NAME_Changed: ' + e.message);
				}
			}
			else
			{
				// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
				var result = { 'ID' : '', 'NAME' : '' };
				Resources_RESOURCE_NAME_Changed_OnSucceededWithContext(result, userContext, null);
			}
		}
	}
}

function Resources_RESOURCE_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		var fldAjaxErrors        = document.getElementById(userContext + 'RESOURCE_NAME_AjaxErrors');
		var fldRESOURCE_ID        = document.getElementById(userContext + 'RESOURCE_ID'  );
		var fldRESOURCE_NAME      = document.getElementById(userContext + 'RESOURCE_NAME');
		var fldPREV_RESOURCE_NAME = document.getElementById(userContext + 'PREV_RESOURCE_NAME');
		if ( fldRESOURCE_ID        != null ) fldRESOURCE_ID.value        = sID  ;
		if ( fldRESOURCE_NAME      != null ) fldRESOURCE_NAME.value      = sNAME;
		if ( fldPREV_RESOURCE_NAME != null ) fldPREV_RESOURCE_NAME.value = sNAME;
		// 08/21/2010 Paul.  We typically submit the form when the partner changes so that we can load the address. 
		// 08/21/2010 Paul.  If an Update button is available, then click it. 
		var fldRESOURCE_UPDATE = document.getElementById(userContext + 'RESOURCE_UPDATE');
		if ( fldRESOURCE_UPDATE != null )
			fldRESOURCE_UPDATE.click();
		// 09/16/2010 Paul.  We want to automatically click the update button in the RelatedSelect control. 
		// In order for this to work, we must define our own command buttons in the GridView. 
		var btnUpdate = document.getElementById(userContext + 'btnUpdate');
		if ( btnUpdate != null )
		{
			btnUpdate.click();
		}
	}
	else
	{
		alert('result from Resources.AutoComplete service is null');
	}
}

function Resources_RESOURCE_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'RESOURCE_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldRESOURCE_ID        = document.getElementById(userContext + 'RESOURCE_ID'       );
	var fldRESOURCE_NAME      = document.getElementById(userContext + 'RESOURCE_NAME'     );
	var fldPREV_RESOURCE_NAME = document.getElementById(userContext + 'PREV_RESOURCE_NAME');
	if ( fldRESOURCE_ID        != null ) fldRESOURCE_ID.value        = '';
	if ( fldRESOURCE_NAME      != null ) fldRESOURCE_NAME.value      = '';
	if ( fldPREV_RESOURCE_NAME != null ) fldPREV_RESOURCE_NAME.value = '';
}

// 07/27/2010 Paul.  Allow Resource lookup from Quotes, Orders or Invoices. 
// 07/27/2010 Paul.  Since we are using the ContextKey for the AutoComplete, we need to also use it with the Get. 
function Resources_BILLING_RESOURCE_NAME_Changed(fldRESOURCE_NAME)
{
	var userContext = fldRESOURCE_NAME.id.substring(0, fldRESOURCE_NAME.id.length - 'BILLING_RESOURCE_NAME'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'BILLING_RESOURCE_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var spartner_NAME = '';
	var fldBILLING_partner_NAME = document.getElementById(userContext + 'BILLING_partner_NAME');
	if ( fldBILLING_partner_NAME != null )
	{
		spartner_NAME = fldBILLING_partner_NAME.value;
	}
	var fldPREV_RESOURCE_NAME = document.getElementById(userContext + 'PREV_BILLING_RESOURCE_NAME');
	if ( fldPREV_RESOURCE_NAME == null )
	{
		//alert('Could not find ' + userContext + 'PREV_RESOURCE_NAME');
	}
	else if ( fldPREV_RESOURCE_NAME.value != fldRESOURCE_NAME.value )
	{
		if ( fldRESOURCE_NAME.value.length > 0 )
		{
			try
			{
				SplendidCRM.Resources.AutoComplete.Resources_BILLING_RESOURCE_NAME_Get(fldRESOURCE_NAME.value, spartner_NAME, Resources_BILLING_RESOURCE_NAME_Changed_OnSucceededWithContext, Resources_BILLING_RESOURCE_NAME_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('Resources_BILLING_RESOURCE_NAME_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'NAME' : '' };
			Resources_BILLING_RESOURCE_NAME_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function Resources_BILLING_RESOURCE_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		var fldAjaxErrors        = document.getElementById(userContext + 'BILLING_RESOURCE_NAME_AjaxErrors');
		var fldRESOURCE_ID        = document.getElementById(userContext + 'BILLING_RESOURCE_ID'  );
		var fldRESOURCE_NAME      = document.getElementById(userContext + 'BILLING_RESOURCE_NAME');
		var fldPREV_RESOURCE_NAME = document.getElementById(userContext + 'PREV_BILLING_CONTACT');
		if ( fldRESOURCE_ID        != null ) fldRESOURCE_ID.value        = sID  ;
		if ( fldRESOURCE_NAME      != null ) fldRESOURCE_NAME.value      = sNAME;
		if ( fldPREV_RESOURCE_NAME != null ) fldPREV_RESOURCE_NAME.value = sNAME;
		// 08/21/2010 Paul.  We typically submit the form when the partner changes so that we can load the address. 
		// 08/21/2010 Paul.  If an Update button is available, then click it. 
		var fldBILLING_RESOURCE_UPDATE = document.getElementById(userContext + 'BILLING_RESOURCE_UPDATE');
		if ( fldBILLING_RESOURCE_UPDATE != null )
			fldBILLING_RESOURCE_UPDATE.click();
	}
	else
	{
		alert('result from Resources.AutoComplete service is null');
	}
}

function Resources_BILLING_RESOURCE_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'BILLING_RESOURCE_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldRESOURCE_ID        = document.getElementById(userContext + 'BILLING_RESOURCE_ID'       );
	var fldRESOURCE_NAME      = document.getElementById(userContext + 'BILLING_RESOURCE_NAME'     );
	var fldPREV_RESOURCE_NAME = document.getElementById(userContext + 'PREV_BILLING_RESOURCE_NAME');
	if ( fldRESOURCE_ID        != null ) fldRESOURCE_ID.value        = '';
	if ( fldRESOURCE_NAME      != null ) fldRESOURCE_NAME.value      = '';
	if ( fldPREV_RESOURCE_NAME != null ) fldPREV_RESOURCE_NAME.value = '';
}

function Resources_SHIPPING_RESOURCE_NAME_Changed(fldRESOURCE_NAME)
{
	var userContext = fldRESOURCE_NAME.id.substring(0, fldRESOURCE_NAME.id.length - 'SHIPPING_RESOURCE_NAME'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'SHIPPING_RESOURCE_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var spartner_NAME = '';
	var fldSHIPPING_partner_NAME = document.getElementById(userContext + 'SHIPPING_partner_NAME');
	if ( fldSHIPPING_partner_NAME != null )
	{
		spartner_NAME = fldSHIPPING_partner_NAME.value;
	}
	var fldPREV_RESOURCE_NAME = document.getElementById(userContext + 'PREV_SHIPPING_RESOURCE_NAME');
	if ( fldPREV_RESOURCE_NAME == null )
	{
		//alert('Could not find ' + userContext + 'PREV_RESOURCE_NAME');
	}
	else if ( fldPREV_RESOURCE_NAME.value != fldRESOURCE_NAME.value )
	{
		if ( fldRESOURCE_NAME.value.length > 0 )
		{
			try
			{
				SplendidCRM.Resources.AutoComplete.Resources_BILLING_RESOURCE_NAME_Get(fldRESOURCE_NAME.value, spartner_NAME, Resources_SHIPPING_RESOURCE_NAME_Changed_OnSucceededWithContext, Resources_SHIPPING_RESOURCE_NAME_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('Resources_SHIPPING_RESOURCE_NAME_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'NAME' : '' };
			Resources_SHIPPING_RESOURCE_NAME_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function Resources_SHIPPING_RESOURCE_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		var fldAjaxErrors        = document.getElementById(userContext + 'SHIPPING_RESOURCE_NAME_AjaxErrors');
		var fldRESOURCE_ID        = document.getElementById(userContext + 'SHIPPING_RESOURCE_ID'  );
		var fldRESOURCE_NAME      = document.getElementById(userContext + 'SHIPPING_RESOURCE_NAME');
		var fldPREV_RESOURCE_NAME = document.getElementById(userContext + 'PREV_SHIPPING_CONTACT');
		if ( fldRESOURCE_ID        != null ) fldRESOURCE_ID.value        = sID  ;
		if ( fldRESOURCE_NAME      != null ) fldRESOURCE_NAME.value      = sNAME;
		if ( fldPREV_RESOURCE_NAME != null ) fldPREV_RESOURCE_NAME.value = sNAME;
		// 08/21/2010 Paul.  We typically submit the form when the partner changes so that we can load the address. 
		// 08/21/2010 Paul.  If an Update button is available, then click it. 
		var fldSHIPPING_RESOURCE_UPDATE = document.getElementById(userContext + 'SHIPPING_RESOURCE_UPDATE');
		if ( fldSHIPPING_RESOURCE_UPDATE != null )
			fldSHIPPING_RESOURCE_UPDATE.click();
	}
	else
	{
		alert('result from Resources.AutoComplete service is null');
	}
}

function Resources_SHIPPING_RESOURCE_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'SHIPPING_RESOURCE_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldRESOURCE_ID        = document.getElementById(userContext + 'SHIPPING_RESOURCE_ID'       );
	var fldRESOURCE_NAME      = document.getElementById(userContext + 'SHIPPING_RESOURCE_NAME'     );
	var fldPREV_RESOURCE_NAME = document.getElementById(userContext + 'PREV_SHIPPING_RESOURCE_NAME');
	if ( fldRESOURCE_ID        != null ) fldRESOURCE_ID.value        = '';
	if ( fldRESOURCE_NAME      != null ) fldRESOURCE_NAME.value      = '';
	if ( fldPREV_RESOURCE_NAME != null ) fldPREV_RESOURCE_NAME.value = '';
}

		// 03/10/2016 Paul.  Missing lookup for Reports To Name. 
function Resources_REPORTS_TO_NAME_Changed(fldRESOURCE_NAME)
{
	if ( fldRESOURCE_NAME != null )
	{
		var userContext = fldRESOURCE_NAME.id.substring(0, fldRESOURCE_NAME.id.length - 'RESOURCE_NAME'.length)
		var fldAjaxErrors = document.getElementById(userContext + 'RESOURCE_NAME_AjaxErrors');
		if ( fldAjaxErrors != null )
			fldAjaxErrors.innerHTML = '';
		
		var fldPREV_RESOURCE_NAME = document.getElementById(userContext + 'PREV_RESOURCE_NAME');
		if ( fldPREV_RESOURCE_NAME == null )
		{
			//alert('Could not find ' + userContext + 'PREV_RESOURCE_NAME');
		}
		else if ( fldPREV_RESOURCE_NAME.value != fldRESOURCE_NAME.value )
		{
			if ( fldRESOURCE_NAME.value.length > 0 )
			{
				try
				{
					SplendidCRM.Resources.AutoComplete.Resources_REPORTS_TO_NAME_Get(fldRESOURCE_NAME.value, Resources_REPORTS_TO_NAME_Changed_OnSucceededWithContext, Resources_REPORTS_TO_NAME_Changed_OnFailed, userContext);
				}
				catch(e)
				{
					alert('Resources_REPORTS_TO_NAME_Changed: ' + e.message);
				}
			}
			else
			{
				// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
				var result = { 'ID' : '', 'NAME' : '' };
				Resources_REPORTS_TO_NAME_Changed_OnSucceededWithContext(result, userContext, null);
			}
		}
	}
}

function Resources_REPORTS_TO_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		var fldAjaxErrors        = document.getElementById(userContext + 'RESOURCE_NAME_AjaxErrors');
		var fldRESOURCE_ID        = document.getElementById(userContext + 'RESOURCE_ID'  );
		var fldRESOURCE_NAME      = document.getElementById(userContext + 'RESOURCE_NAME');
		var fldPREV_RESOURCE_NAME = document.getElementById(userContext + 'PREV_RESOURCE_NAME');
		if ( fldRESOURCE_ID        != null ) fldRESOURCE_ID.value        = sID  ;
		if ( fldRESOURCE_NAME      != null ) fldRESOURCE_NAME.value      = sNAME;
		if ( fldPREV_RESOURCE_NAME != null ) fldPREV_RESOURCE_NAME.value = sNAME;
		// 08/21/2010 Paul.  We typically submit the form when the partner changes so that we can load the address. 
		// 08/21/2010 Paul.  If an Update button is available, then click it. 
		var fldRESOURCE_UPDATE = document.getElementById(userContext + 'RESOURCE_UPDATE');
		if ( fldRESOURCE_UPDATE != null )
			fldRESOURCE_UPDATE.click();
		// 09/16/2010 Paul.  We want to automatically click the update button in the RelatedSelect control. 
		// In order for this to work, we must define our own command buttons in the GridView. 
		var btnUpdate = document.getElementById(userContext + 'btnUpdate');
		if ( btnUpdate != null )
		{
			btnUpdate.click();
		}
	}
	else
	{
		alert('result from Resources.AutoComplete service is null');
	}
}

function Resources_REPORTS_TO_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'RESOURCE_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldRESOURCE_ID        = document.getElementById(userContext + 'RESOURCE_ID'       );
	var fldRESOURCE_NAME      = document.getElementById(userContext + 'RESOURCE_NAME'     );
	var fldPREV_RESOURCE_NAME = document.getElementById(userContext + 'PREV_RESOURCE_NAME');
	if ( fldRESOURCE_ID        != null ) fldRESOURCE_ID.value        = '';
	if ( fldRESOURCE_NAME      != null ) fldRESOURCE_NAME.value      = '';
	if ( fldPREV_RESOURCE_NAME != null ) fldPREV_RESOURCE_NAME.value = '';
}

if ( typeof(Sys) !== 'undefined' )
	Sys.Application.notifyScriptLoaded();



