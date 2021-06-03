// 09/20/2013 Paul.  Move EXTENSION to the main table. 
// http://www.asp.net/signalr/overview/hubs-api/hubs-api-guide-javascript-client

// 11/25/2014 Paul.  SignalR_Connection_Start() needs to be called outside of the ready event. 
function SignalR_Connection_Start()
{
	if ( sUSER_EXTENSION.length > 0 || (sUSER_PHONE_MOBILE.length > 0 && sUSER_SMS_OPT_IN == 'yes') || sUSER_TWITTER_TRACKS.length > 0 || sUSER_CHAT_CHANNELS.length > 0 )
	{
		// Start Connection
		// { transport: ['webSockets'] }
		$.connection.hub.logging = true;
		$.connection.hub.start().done(function()
		{
			try
			{
				if ( sUSER_PHONE_MOBILE.length > 0 && sUSER_SMS_OPT_IN == 'yes' && twilioManager !== undefined )
				{
					twilioManager.server.joinGroup($.connection.hub.id, sUSER_PHONE_MOBILE).done(function(result)
					{
						//TwilioStatusDialog('Twilio Join', result);
					})
					.fail(function(e)
					{
					});
				}
			}
			catch(e)
			{
			}
			try
			{
				if ( sUSER_EXTENSION.length > 0 && asteriskManager !== undefined )
				{
					asteriskManager.server.joinGroup($.connection.hub.id, sUSER_EXTENSION).done(function(result)
					{
						//AsteriskStatusDialog('Asterisk Join', result);
					})
					.fail(function(e)
					{
					});
				}
			}
			catch(e)
			{
			}
			try
			{
				if ( sUSER_EXTENSION.length > 0 && avayaManager !== undefined )
				{
					avayaManager.server.joinGroup($.connection.hub.id, sUSER_EXTENSION).done(function(result)
					{
						//AvayaStatusDialog('Avaya Join', result);
					})
					.fail(function(e)
					{
					});
				}
			}
			catch(e)
			{
			}
			try
			{
				if ( sUSER_TWITTER_TRACKS.length > 0 && twitterManager !== undefined )
				{
					twitterManager.server.joinGroup($.connection.hub.id, sUSER_TWITTER_TRACKS).done(function(result)
					{
						//var divMyTwitterTracks = document.getElementById('divMyTwitterTracks');
						//divMyTwitterTracks.innerHTML = result;
					})
					.fail(function(e)
					{
					});
				}
			}
			catch(e)
			{
			}
			try
			{
				if ( sUSER_CHAT_CHANNELS.length > 0 && chatManager !== undefined )
				{
					chatManager.server.joinGroup($.connection.hub.id, sUSER_CHAT_CHANNELS).done(function(result)
					{
						//var divMyChatChannels = document.getElementById('divMyChatChannels');
						//divMyChatChannels.innerHTML = result;
					})
					.fail(function(e)
					{
					});
				}
			}
			catch(e)
			{
			}
		});
	}
}

function SignalR_Connection_Stop()
{
	if ( sUSER_EXTENSION.length > 0 || (sUSER_PHONE_MOBILE.length > 0 && sUSER_SMS_OPT_IN == 'yes') || sUSER_TWITTER_TRACKS.length > 0 || sUSER_CHAT_CHANNELS.length > 0 )
	{
		$.connection.hub.stop();
	}
}

$(document).ready(function()
{
	SignalR_Connection_Start();
});


