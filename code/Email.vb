Imports System.Text
Imports System.Web.Mail
Imports System.Configuration.ConfigurationSettings

Public Class Email
    Private _from As String
    Private Const _schema As String = "http://schemas.microsoft.com/cdo/configuration/"

    Public Sub New()
        _from = String.Format("AMP ({0})", AppSettings("MailFrom"))
    End Sub

#Region " Properties "

    Public Property From() As String
        Get
            Return _from
        End Get
        Set(ByVal Value As String)
            _from = Value
        End Set
    End Property

#End Region

#Region " Enumerations "

    Private Enum SendUsing
        Pickup = 1
        Port = 2
    End Enum

    Private Enum Authentication
        Anonymous
        Basic
        Ntlm
    End Enum

#End Region

    '---COMMENT---------------------------------------------------------------
    '	send email about critical error
    '
    '	Date:		Name:	Description:
    '	1/18/05	    JEA 	Creation
    '-------------------------------------------------------------------------
    Public Sub [Error](ByVal ex As Exception, ByVal user As AMP.Person, ByVal clientIP As String, _
        ByVal machine As String, ByVal process As String, ByVal sendTo As String())

        Dim mail As New MailMessage
        Dim file As New AMP.data.File
        Dim template As String = AppSettings("ErrorTemplate")
        Dim recipients As New StringBuilder
        Dim body As String = file.Load(template)

        body = body.Replace("<userName>", user.FullName)
        body = body.Replace("<dateTime>", DateTime.Now.ToString)
        body = body.Replace("<userIP>", clientIP)
        body = body.Replace("<server>", machine)
        body = body.Replace("<process>", process)
        body = body.Replace("<message>", ex.Message)
        body = body.Replace("<stack>", ex.StackTrace.Replace(vbCrLf, "<br/>"))

        With recipients
            For Each address As String In sendTo
                If .Length > 0 Then .Append(",")
                .Append(address)
            Next
        End With

        With mail
            .BodyFormat = DirectCast(IIf(template.EndsWith("htm"), MailFormat.Html, MailFormat.Text), MailFormat)
            .Priority = MailPriority.High
            .From = _from
            .To = recipients.ToString
            .Subject = "Critical AMP.com error"
            .Body = body.ToString
        End With

        Me.Send(mail)
    End Sub


    '---COMMENT---------------------------------------------------------------
    '	send confirmation code to user
    '
    '	Date:		Name:	Description:
    '	1/7/05	    JEA 	Creation
    '-------------------------------------------------------------------------
    Public Sub Confirmation(ByVal name As String, ByVal email As String, ByVal code As String)
        Dim mail As New MailMessage
        Dim file As New AMP.data.File
        Dim body As String = file.Load(AppSettings("ConfirmationTemplate"))

        body = body.Replace("<name>", name)
        body = body.Replace("<code>", code)

        With mail
            .BodyFormat = MailFormat.Text
            .From = _from
            .To = String.Format("{0} ({1})", name, email)
            .Subject = "Your AMP Confirmation Code"
            .Body = body
        End With

        Me.Send(mail)
    End Sub

    Public Sub Confirmation(ByVal person As AMP.Person)
        Me.Confirmation(person.FullName, person.Email, person.ConfirmationCode)
    End Sub

    '---COMMENT---------------------------------------------------------------
    '	send password to user
    '
    '	Date:		Name:	Description:
    '	1/7/05	    JEA 	Creation
    '-------------------------------------------------------------------------
    Public Sub Password(ByVal name As String, ByVal email As String, ByVal password As String)
        Dim mail As New MailMessage
        Dim file As New AMP.data.File
        Dim body As String = file.Load(AppSettings("PasswordTemplate"))

        body = body.Replace("<name>", name)
        body = body.Replace("<password>", password)

        With mail
            .BodyFormat = MailFormat.Text
            .From = _from
            .To = String.Format("{0} ({1})", name, email)
            .Subject = "Your AMP Password"
            .Body = body
        End With

        Me.Send(mail)
    End Sub

    Public Sub Password(ByVal person As AMP.Person, ByVal password As String)
        Me.Password(person.FullName, person.Email, password)
    End Sub

    '---COMMENT---------------------------------------------------------------
    '	Send e-mail through specified mail server
    '   use smtp login when needed
    '
    '	Date:		Name:	Description:
    '	10/1/04	    JEA 	Creation
    '-------------------------------------------------------------------------
    Private Sub Send(ByVal mail As MailMessage)
        Dim userName As String = AppSettings("MailUser")
        If userName <> Nothing Then
            ' server must require authentication
            With mail.Fields
                .Add(_schema & "sendusername", userName)
                .Add(_schema & "sendpassword", AppSettings("MailPassword"))
                .Add(_schema & "smtpauthenticate", Authentication.Basic)
                '.Add(_schema & "sendusing", SendUsing.Port)
                '.Add(_schema & "smtpserverport", 25)
                '.Add(_schema & "smtpusessl", False)
                '.Add(_schema & "smtpconnectiontimeout", 60)
            End With
        End If
        SmtpMail.SmtpServer = AppSettings("MailServer")
        SmtpMail.Send(mail)
    End Sub
End Class