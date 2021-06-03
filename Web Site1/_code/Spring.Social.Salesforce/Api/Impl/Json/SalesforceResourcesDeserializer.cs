#region License

/*
 * Copyright (C) 2012 SplendidCRM Software, Inc. All Rights Reserved. 
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System;
using System.Globalization;

using Spring.Json;

namespace Spring.Social.Salesforce.Api.Impl.Json
{
	/// <summary>
	/// JSON deserializer for Resources. 
	/// </summary>
	/// <author>SplendidCRM (.NET)</author>
	class SalesforceResourcesDeserializer : IJsonDeserializer
	{
		public object Deserialize(JsonValue json, JsonMapper mapper)
		{
			SalesforceResources resource = null;
			if ( json != null && !json.IsNull )
			{
				resource = new SalesforceResources();
				resource.SObjectsUrl = json.ContainsName("sobjects") ? json.GetValue<string>("sobjects") : String.Empty;
				resource.SearchUrl   = json.ContainsName("search"  ) ? json.GetValue<string>("search"  ) : String.Empty;
				resource.QueryUrl    = json.ContainsName("query"   ) ? json.GetValue<string>("query"   ) : String.Empty;
				resource.RecentUrl   = json.ContainsName("recent"  ) ? json.GetValue<string>("recent"  ) : String.Empty;
			}
			return resource;
		}
	}
}

