#Twilio Relay

## Forward

To forward a message, send a message from one of the allowed phone numbers to the relay number in the format below.

    TO_NUMBER MESSAGE

    +15555555555 Message of any length

### Confirmation

    FROM_NUMBER:FORWARDED
	
	+15555555151:FORWARDED

## Receive

If the relay number receives a message from an unknown number, the message is sent to the catchall number.

    DEFAULTED FROM RELAY_NUMBER: Message of any length
	
	DEFAULTED FROM+15555555555: Message of any length
