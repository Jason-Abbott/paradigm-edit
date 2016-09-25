Imports System.Web.HttpContext
Imports System.Collections.Specialized

Public Class Profile

    Private Shared _useCookies As Boolean = True

#Region " Session and Cookie Keys "

    Private Shared _userKey As String = "User"
    Private Shared _personKey As String = "PersonID"
    Private Shared _messageKey As String = "Message"
    Private Shared _offsetKey As String = "TimeOffset"
    Private Shared _lastLoginKey As String = "LastLogin"
    Private Shared _authenticatedKey As String = "Authenticated"
    Private Shared _destinationKey As String = "DestinationPage"
    Private Shared _resultsKey As String = "SearchResults"
    Private Shared _contributeKey As String = "Contribution"
    Private Shared _formValuesKey As String = "FormValues"
    Private Shared _resumeDownload As String = "ResumeDownloadID"
    Private Shared _testCookie As String = "test"

#End Region

#Region " Properties "

    '---COMMENT---------------------------------------------------------------
    '	hold viewed asset ID when redirected to login
    '
    '	Date:		Name:	Description:
    '	1/23/05     JEA		Creation
    '-------------------------------------------------------------------------
    Public Shared Property ResumeDownload() As Guid
        Get
            If Current.Session(_resumeDownload) Is Nothing Then
                Return Nothing
            Else
                Return DirectCast(Current.Session(_resumeDownload), Guid)
            End If
        End Get
        Set(ByVal Value As Guid)
            If Value.Equals(Guid.Empty) Then
                Current.Session.Remove(_resumeDownload)
            Else
                Current.Session(_resumeDownload) = Value
            End If
        End Set
    End Property

    '---COMMENT---------------------------------------------------------------
    '	hold form post values when redirected to login
    '
    '	Date:		Name:	Description:
    '	1/19/05     JEA		Creation
    '-------------------------------------------------------------------------
    Public Shared Property FormValues() As NameValueCollection
        Get
            If Current.Session(_formValuesKey) Is Nothing Then
                Return Nothing
            Else
                Return DirectCast(Current.Session(_formValuesKey), NameValueCollection)
            End If
        End Get
        Set(ByVal Value As NameValueCollection)
            If Value Is Nothing Then
                Current.Session.Remove(_formValuesKey)
            Else
                Current.Session(_formValuesKey) = Value
            End If
        End Set
    End Property

    Public Shared Property SearchResults() As ArrayList
        Get
            If Current.Session(_resultsKey) Is Nothing Then
                Return Nothing
            Else
                Return DirectCast(Current.Session(_resultsKey), ArrayList)
            End If
        End Get
        Set(ByVal Value As ArrayList)
            If Value Is Nothing Then
                Current.Session.Remove(_resultsKey)
            Else
                Current.Session(_resultsKey) = Value
            End If
        End Set
    End Property

    Public Shared Property UseCookies() As Boolean
        Get
            Return _useCookies
        End Get
        Set(ByVal Value As Boolean)
            _useCookies = Value
        End Set
    End Property

    Public Shared Property Message() As String
        Get
            If Current.Session(_messageKey) Is Nothing Then
                Return Nothing
            Else
                Dim Value As String
                Value = CStr(Current.Session(_messageKey))
                Current.Session.Remove(_messageKey)
                Return Value
            End If
        End Get
        Set(ByVal Value As String)
            If Value = Nothing Then
                Current.Session.Remove(_messageKey)
            Else
                Current.Session(_messageKey) = Value
            End If
        End Set
    End Property

    '---COMMENT---------------------------------------------------------------
    '	Tracks time difference between client and server so times can be
    '   displayed relative to client locale
    '
    '	Date:		Name:	Description:
    '	12/3/04     JEA		Creation
    '-------------------------------------------------------------------------
    Public Shared Property TimeOffset() As TimeSpan
        Get
            If Current.Session(_offsetKey) Is Nothing Then
                If Current.Request.Cookies(_offsetKey) Is Nothing Then
                    Return New TimeSpan(0)
                Else
                    ' get offset from cookie and store in session
                    Dim offset As New TimeSpan
                    offset = DirectCast(Current.Request.Cookies(_offsetKey).Value, TimeSpan)
                    Current.Session(_offsetKey) = offset
                    Return offset
                End If
            Else
                Return DirectCast(Current.Session(_offsetKey), TimeSpan)
            End If
        End Get
        Set(ByVal Value As TimeSpan)
            Current.Session(_offsetKey) = Value
            SetCookie(_offsetKey, Value)
        End Set
    End Property

    '---COMMENT---------------------------------------------------------------
    '   Use last login to determine which assets are new to this user
    '
    '	Date:		Name:	Description:
    '	12/3/04     JEA		Creation
    '-------------------------------------------------------------------------
    Public Shared Property LastLogin() As DateTime
        Get
            If Current.Session(_offsetKey) Is Nothing Then
                Return Nothing
            Else
                Return DirectCast(Current.Session(_offsetKey), DateTime)
            End If
        End Get
        Set(ByVal Value As DateTime)
            Current.Session(_offsetKey) = Value
        End Set
    End Property

    Public Shared Property Authenticated() As Boolean
        Get
            If Current.Session(_authenticatedKey) Is Nothing Then
                Return False
            Else
                Return CBool(Current.Session(_authenticatedKey))
            End If
        End Get
        Set(ByVal Value As Boolean)
            Current.Session(_authenticatedKey) = Value
        End Set
    End Property

    Public Shared Property User() As AMP.Person
        Get
            If Current.Session(_userKey) Is Nothing Then
                Return Nothing
            Else
                Return DirectCast(Current.Session(_userKey), AMP.Person)
            End If
        End Get
        Set(ByVal Value As AMP.Person)
            If Value Is Nothing Then
                Current.Session.Remove(_userKey)
            Else
                Current.Session(_userKey) = Value
                PersonID = Value.ID
            End If
        End Set
    End Property

    '---COMMENT---------------------------------------------------------------
    '   Get person ID from session or cookie
    '
    '	Date:		Name:	Description:
    '	12/3/04     JEA		Creation
    '   12/7/04     JEA     Also persist to cookie
    '-------------------------------------------------------------------------
    Public Shared Property PersonID() As Guid
        Get
            If Current.Session(_personKey) Is Nothing Then
                If Current.Request.Cookies(_personKey) Is Nothing OrElse _
                    Current.Request.Cookies(_personKey).Value.Length < 32 Then

                    Return Nothing
                Else
                    ' looks like a guid
                    Return New Guid(Current.Request.Cookies(_personKey).Value)
                End If
            Else
                Return New Guid(Current.Session(_personKey).ToString)
            End If
        End Get
        Set(ByVal Value As Guid)
            If Value.Equals(Guid.Empty) Then
                Current.Session.Remove(_personKey)
                'Current.Response.Cookies.Remove(_personKey)
            Else
                Current.Session(_personKey) = Value
                SetCookie(_personKey, Value)
            End If
        End Set
    End Property

    '---COMMENT---------------------------------------------------------------
    '	Stores intended page when redirected to login for authentication
    '
    '	Date:		Name:	Description:
    '	9/22/04     JEA		Creation
    '-------------------------------------------------------------------------
    Public Shared Property DestinationPage() As String
        Get
            If Current.Session(_destinationKey) Is Nothing Then
                Return Nothing
            Else
                Return CStr(Current.Session(_destinationKey))
            End If
        End Get
        Set(ByVal Value As String)
            If Value = Nothing Then
                Current.Session.Remove(_destinationKey)
            Else
                Current.Session(_destinationKey) = Value
            End If
        End Set
    End Property

#End Region

    Public Sub Clear()
        Me.PersonID = Nothing
        Me.CreateGuest()
    End Sub

    '---COMMENT---------------------------------------------------------------
    '	Set a persistent cookie for given name-value
    '
    '	Date:		Name:	Description:
    '	12/10/04    JEA		Creation
    '-------------------------------------------------------------------------
    Private Shared Sub SetCookie(ByVal name As String, ByVal value As Object)
        If _useCookies Then
            Dim cookie As New HttpCookie(name, value.ToString)
            cookie.Expires = New Date(2010, 1, 1)
            Current.Response.Cookies.Add(cookie)
        End If
    End Sub

    '---COMMENT---------------------------------------------------------------
    '	write a cookie to be tested later
    '
    '	Date:		Name:	Description:
    '	1/24/05     JEA		Creation
    '-------------------------------------------------------------------------
    Public Shared Sub WriteTestCookie()
        Dim cookie As HttpCookie = New HttpCookie(_testCookie, HttpContext.Current.Session.SessionID)
        cookie.Expires = System.DateTime.Now.AddSeconds(1)
        Current.Response.Cookies.Add(cookie)
    End Sub

    Public Shared Function SupportsCookies() As Boolean
        Dim cookie As HttpCookie = Current.Request.Cookies(_testCookie)
        Dim supported As Boolean = ((Not cookie Is Nothing) AndAlso _
            cookie.Value = HttpContext.Current.Session.SessionID)
        If supported Then Current.Request.Cookies.Remove(_testCookie)
        Return supported
    End Function

    '---COMMENT---------------------------------------------------------------
    '	create a guest account
    '
    '	Date:		Name:	Description:
    '	12/10/04    JEA		Creation
    '   1/12/05     JEA     Handle empty web site
    '-------------------------------------------------------------------------
    Public Shared Sub CreateGuest()
        Dim useCookies As Boolean = Profile.UseCookies

        ' disable cookies for guest creation
        Profile.UseCookies = False
        Profile.User = New AMP.Person
        Profile.UseCookies = useCookies

        If Not WebSite Is Nothing Then
            Profile.User.Roles.Add(WebSite.Roles(AMP.Site.Role.Anonymous))
        End If
        Profile.User.Section = AMP.Site.Section.All
        Profile.Authenticated = False
    End Sub

End Class
