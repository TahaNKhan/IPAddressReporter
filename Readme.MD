# IP Address Reporter

## What?

Reports external IP address to specified email addresses and (optionally) updates a godaddy host to point to the specfic IP.

## But Why?

Two primary reasons:
* I want to access my home server via RDP from anywhere remotely.
* COVID-19 is giving me extra time to work on some fun side projects.

## How do I use this?

2 things need to be done before its ready to work.
1. Fill out the appsettings.json file.
    1. RecipientEmails - Array of string emails ex: ["email1@example.com"]
        * Side note: you can send an email to a phone number and receive it as a text: https://email2sms.info/
    2. LogFileLocation - Optional, list the directory where you want to log, if not provided, it will log to the console.
    3. SendEmails - set this to true
    4. Set the email credentials, for EmailDisplayName, put a name in there, it'll show up as the sender in the email. Use [app password](https://support.google.com/accounts/answer/185833?hl=en) feature in gmail.
    5. WaitTimeSeconds - The wait time between IP checks, I set it to an hour.
    6. Host - The host name you own on GoDaddy
    7. GoDaddyAPISecrets - GoDaddy API Key and Secret, and the URL you'd like to point to, testing API https://api.ote-godaddy.com or the production API https://api.godaddy.com
2. Compile and run.

I have personally set this up as a service.

## Limitations/Notes

* Currently only supports gmail as the email provider and godaddy as the host provider.
* Uses http://checkip.amazonaws.com/ to find out external IP address.
* Uses a file logger/console logger.

## Troubleshooting

Check the log file for output/exceptions.