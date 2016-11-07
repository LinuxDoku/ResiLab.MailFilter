# ResiLab MailFilter
This program is used to filter imap mail boxes based on various rules.

Rules are auto generated by learning from your already qualified spam messages,
the algorithim learns itself over time and there is also a configuration
to add custom rules. The custom rules are very handy to auto move newsletters, etc.
out of your inbox to keep it clean.

## Install
Download the source code and compile it with your preferred .NET compiler.

Install the windows service with 

```
ResiLab.MailFilter.exe install
```

on linux you have to do this yourself.

## Configure
The `config.json` can contain multiple mail boxes. Every mailbox needs server, username
and password to connect. At the moment only imap servers are supported. Pop3 is not planned.

To enctypt the password the .NET data protection with local machine scope is used, so you 
have to run the command to encrypt your password on the target machine.

```
ResiLab.MailFilter.exe crypt <YOUR PASSWORD>
```

copy the output string (without the spaces) to the config file.


### Spam Protection
To enable the spam protection simply add this to your configuration:

```json
"Spam": {
    "EnableSpamProtection": true,
    "Target": "Spam"
}
```
The target folder is used as the source folder for the learning process and also as the target for
detected spam messages.

### Custom Rules
Custom rules are configured like this in the `Rules` array of the configuration file:

```json
{
    "Type": "SubjectStartsWith",
    "Value": "Hey, I am spam",
    "Destination": "Spam"
}
```

Following rule types are implemented:

- SenderEquals
- SenderContains
- SenderEndsWith

- SubjectEquals
- SubjectContains
- SubjectBeginsWith
- SubjectEndsWith

## How the spam protection feature works
An analyzer scans the spam folder in your mail box periodically and uses this learning
data to generate new rules at runtime in the mailbox processor.
At the moment the sender address and subject of these mails are used. Some other data 
is also gathered but not used yet.

This mechanism is self learning, cause when the analyzer matches an message with an already
known subject it registeres all the other metadata of the mail and generates rules based 
on them - so for example later mails from the same address or with the same fishing urls 
are also detected as spam.

To ensure that this mechanism is not removing mails from your inbox which are by persons
you trust it will later (not yet implemented yet) generate a whitelist based on the already
read and send mails of your mail box. So persons you stay in contact with are not
blocked by the spam filter.
