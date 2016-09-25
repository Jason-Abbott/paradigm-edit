Imports System.Web
Imports System.Text
Imports System.Web.SessionState
Imports System.Configuration.ConfigurationSettings
Imports AMP.Site

Public Class Global
    Inherits System.Web.HttpApplication

    Private Shared _basePath As String
    Private Shared _rootEntity As AMP.Site
    Private Shared _fileName As String = String.Format("{0}{1}\ParadigmEdit.dat", _
        HttpRuntime.AppDomainAppPath, AppSettings("DataFolder"))

#Region " Component Designer Generated Code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Component Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        components = New System.ComponentModel.Container
    End Sub

#End Region

    '---COMMENT---------------------------------------------------------------
    '	retrieve or create site base entity
    '
    '	Date:		Name:	Description:
    '	12/3/04     JEA		Creation
    '-------------------------------------------------------------------------
    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        _basePath = HttpRuntime.AppDomainAppVirtualPath
        If BasePath = "/" Then _basePath = ""

        '' load data
        'Dim schemaChange As Boolean = False
        'Dim file As New AMP.Data.File

        'BugOut("Starting application")

        '_rootEntity = DirectCast(file.Load(_fileName, Type.GetType("AMP.Site"), schemaChange), AMP.Site)
        'If _rootEntity Is Nothing OrElse _rootEntity.Persons Is Nothing Then
        '    BugOut("Unable to retrieve valid root entity")
        '    _rootEntity = New AMP.Site
        'ElseIf schemaChange Then
        '    BugOut("Saving root entity to persist new schema")
        '    _rootEntity.Save()
        'End If
    End Sub

    '---COMMENT---------------------------------------------------------------
    '	Auto-login from cookie, if cookie found
    '
    '	Date:		Name:	Description:
    '	12/3/04     JEA		Creation
    '   1/24/05     JEA     Write test cookie
    '-------------------------------------------------------------------------
    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        'Dim personID As Guid = Profile.PersonID
        'If Not personID.Equals(Guid.Empty) Then
        '    ' populated person ID would have to come from cookie, attempt auto-login
        '    BugOut("Starting session for Person ID {0}", personID)
        '    Dim security As New AMP.Security
        '    If security.Authenticate(personID) Then Return
        '    BugOut("Failed to auto-login")
        'End If
        '' create empty person for profile
        'BugOut("Starting session with new user")
        'Profile.CreateGuest()
    End Sub

    '---COMMENT---------------------------------------------------------------
    '	cleanup persisted session data
    '
    '	Date:		Name:	Description:
    '	1/15/05     JEA		Creation
    '-------------------------------------------------------------------------
    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        
    End Sub

    '---COMMENT---------------------------------------------------------------
    '	not sure when this fires but if it does then let's save the root entity
    '
    '	Date:		Name:	Description:
    '	12/3/04     JEA		Creation
    '-------------------------------------------------------------------------
    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        '_rootEntity.Save()
    End Sub

#Region " Unhandled events "

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires at the beginning of each request
    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires upon attempting to authenticate the use
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when an error occurs
    End Sub

#End Region

    '---COMMENT---------------------------------------------------------------
    '	strings used to differentiate output caching
    '
    '	Date:		Name:	Description:
    '	12/3/04     JEA		Creation
    '   1/18/05     JEA     Add "message"
    '-------------------------------------------------------------------------
    Public Overrides Function GetVaryByCustomString(ByVal context As System.Web.HttpContext, _
        ByVal custom As String) As String

        Dim keys As String() = custom.Split(",".ToCharArray)
        Dim pattern As New StringBuilder

        For Each key As String In keys
            pattern.Append("_")
            Select Case key.Trim.ToLower
                Case "section"
                    pattern.Append(Profile.User.Section)
                Case "message"
                    pattern.Append(Profile.Message)
                Case "role"
                    pattern.Append(Profile.User.Roles.ToString)
                Case "browser"
                    pattern.Append(Request.Browser.Type.ToLower)
            End Select
        Next

        Return pattern.ToString
    End Function

#Region " Global Properties "

    Public Shared ReadOnly Property DataFilePath() As String
        Get
            Return _fileName
        End Get
    End Property

    Public Shared Property WebSite() As AMP.Site
        Get
            Return _rootEntity
        End Get
        Set(ByVal Value As AMP.Site)
            _rootEntity = Value
        End Set
    End Property

    Public Shared ReadOnly Property BasePath() As String
        Get
            Return _basePath
        End Get
    End Property

#End Region

End Class
