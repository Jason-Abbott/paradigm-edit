Imports System.Text
Imports System.Web.Caching
Imports System.IO
Imports System.Configuration.ConfigurationSettings

Public Class Page
    Inherits System.Web.UI.Page

    Private _profile As New AMP.Profile
    Private _say As New AMP.Locale
    Private _security As AMP.Security
    Private _requireAuthentication As Boolean = False
    Private _title As String
    Private _styleSheet As New ArrayList
    Private _scriptFile As New ArrayList
    Private _scriptBlock As New StringBuilder
    Private _script As String
    Private _url As Hashtable
    Private _templateFile As String = "~/template/NewsPaper.ascx"
    Private _contents As New ArrayList
    Private _template As Templates.NewsPaper

#Region " Controls "

    Protected pnlHeadline As Panel
    Protected pnlLeftColumn As Panel
    Protected pnlRightColumn As Panel

#End Region

#Region " Properties "

    Public WriteOnly Property ScriptBlock() As String
        Set(ByVal Value As String)
            _scriptBlock.Append(vbCrLf)
            _scriptBlock.Append(Value)
            _scriptBlock.Append(vbCrLf)
        End Set
    End Property

    Public Property Security() As AMP.Security
        Get
            If _security Is Nothing Then _security = New AMP.Security
            Return _security
        End Get
        Set(ByVal Value As AMP.Security)
            _security = Value
        End Set
    End Property

    Public Property Profile() As AMP.Profile
        Get
            Return _profile
        End Get
        Set(ByVal Value As AMP.Profile)
            _profile = Value
        End Set
    End Property

    Public Property Say() As AMP.Locale
        Get
            Return _say
        End Get
        Set(ByVal Value As AMP.Locale)
            _say = Value
        End Set
    End Property

    Public Property StyleSheet() As ArrayList
        Get
            Return _styleSheet
        End Get
        Set(ByVal Value As ArrayList)
            _styleSheet = Value
        End Set
    End Property

    Public Property ScriptFile() As ArrayList
        Get
            Return _scriptFile
        End Get
        Set(ByVal Value As ArrayList)
            _scriptFile = Value
        End Set
    End Property

    Public Property Template() As String
        Get
            Return _templateFile
        End Get
        Set(ByVal Value As String)
            _templateFile = Value
        End Set
    End Property

    Public Property Script() As String
        Get
            Return _script
        End Get
        Set(ByVal Value As String)
            _script = Value
        End Set
    End Property

    Public WriteOnly Property Title() As String
        Set(ByVal Value As String)
            _title = Value
        End Set
    End Property

    '---COMMENT---------------------------------------------------------------
    '	return cleaned copy of referring page
    '
    '	Date:		Name:	Description:
    '	10/4/04		JEA		Creation
    '-------------------------------------------------------------------------
    Public ReadOnly Property ReferringPage() As String
        Get
            If Not Request.UrlReferrer Is Nothing Then
                Dim name As String = Request.UrlReferrer.ToString
                Return name.Substring(name.LastIndexOf("/") + 1)
            Else
                Return Nothing
            End If
        End Get
    End Property

    '---COMMENT---------------------------------------------------------------
    '	Causes derived page to redirect to login if user isn't authenticated
    '   Set this property in derived Page_Init() in "Web Form Designer Generated Code" region
    '
    '	Date:		Name:	Description:
    '	9/21/04		JEA		Creation
    '-------------------------------------------------------------------------
    Public WriteOnly Property RequireAuthentication() As Boolean
        Set(ByVal Value As Boolean)
            _requireAuthentication = Value
        End Set
    End Property

    '---COMMENT---------------------------------------------------------------
    '	return name of derived .aspx page
    '
    '	Date:		Name:	Description:
    '	9/22/04		JEA		Creation
    '-------------------------------------------------------------------------
    Public ReadOnly Property PageName() As String
        Get
            'Dim name As String = HttpContext.Current.Request.ServerVariables("SCRIPT_NAME")
            Dim name As String = Request.Path
            Return name.Substring(name.LastIndexOf("/") + 1)
        End Get
    End Property

#End Region

    Public Sub SendBack(ByVal defaultPage As String)
        Dim sendTo As String = Me.ReferringPage
        If sendTo = Nothing Or sendTo = Me.PageName Then sendTo = defaultPage
        Response.Redirect(sendTo, True)
    End Sub

    Public Sub SendBack()
        SendBack("default.aspx")
    End Sub

    Public Sub SendToLogin()
        Profile.DestinationPage = Request.Url.PathAndQuery
        'Server.Transfer(AppSettings("LoginPage"))
        Profile.WriteTestCookie()
        Response.Redirect(AppSettings("LoginPage"), False)
    End Sub

    '---COMMENT---------------------------------------------------------------
    '	if the page is set to require authentication, then send to login page
    '   when not authenticated
    '
    '	Date:		Name:	Description:
    '	9/22/04		JEA		Creation
    '   11/15/04    JEA     Rewrote to use templates
    '   11/20/04    JEA     Cache template
    '-------------------------------------------------------------------------
    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If _requireAuthentication And Not Profile.Authenticated Then
            Me.SendToLogin()
        Else
            _styleSheet.Add("menu")
            _scriptFile.Add("common")
            _scriptFile.Add("menu")
            _scriptFile.Add("cookies")

            _template = DirectCast(Me.Page.LoadControl(_templateFile), Templates.NewsPaper)

            With _template
                .Headline = pnlHeadline
                .RightColumn = pnlRightColumn
                .LeftColumn = pnlLeftColumn
                .ScriptFile = _scriptFile
                .StyleSheet = _styleSheet
                If _scriptBlock.Length > 0 Then .ScriptBlock = _scriptBlock.ToString
            End With

            Me.Controls.Clear()
            Me.Controls.AddAt(0, _template)
        End If
    End Sub

    '---COMMENT---------------------------------------------------------------
    '	allow time for control loads to update message
    '
    '	Date:		Name:	Description:
    '	1/11/05		JEA		Creation
    '-------------------------------------------------------------------------
    Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
        _template.Message = Profile.Message
        If _title <> Nothing Then _template.Title = _title
    End Sub

#Region " Custom Form Support "

    '---COMMENT---------------------------------------------------------------
    '	override state methods to support XHTML strict elements
    '
    '	Date:		Name:	Description:
    '   11/15/04    JEA     Creation
    '-------------------------------------------------------------------------
    Protected Overrides Function LoadPageStateFromPersistenceMedium() As Object
        Dim format As LosFormatter = New LosFormatter
        Dim viewState As String = Request.Form("__VIEWSTATE").ToString()
        Return format.Deserialize(viewState)
    End Function

    Protected Overrides Sub SavePageStateToPersistenceMedium(ByVal viewState As Object)
        Dim format As LosFormatter = New LosFormatter
        Dim writer As New StringWriter
        Dim stateField As New Literal
        format.Serialize(writer, viewState)
        stateField.Text = "<input type=""hidden"" name=""__VIEWSTATE"" value=""" & writer.ToString & """ />"
        _template.StateValue = stateField
    End Sub

    Public Overrides Sub VerifyRenderingInServerForm(ByVal control As System.Web.UI.Control)
        ' do nothing
    End Sub

#End Region

End Class
