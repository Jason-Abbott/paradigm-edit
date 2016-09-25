Imports System.IO
Imports System.Runtime.Serialization
Imports System.Security.Permissions
Imports System.Runtime.Serialization.Formatters.Binary
Imports AMP.Data
Imports AMP.Site

<Serializable()> _
Public Class Person
    Implements IComparable, ICloneable

    Private _id As Guid
    Private _firstName As String
    Private _lastName As String
    Private _nickName As String
    Private _imageFile As String
    Private _email As String
    Private _password As String
    Private _jobTitle As String
    Private _webSite As String
    Private _registeredOn As DateTime
    Private _description As String
    Private _permission As AMP.Site.Permission()
    Private _lastLogin As DateTime
    Private _status As AMP.Site.Status
    Private _confirmationCode As String
    Private _address As New AMP.AddressCollection
    Private _privateEmail As Boolean
    Private _employer As New AMP.Company
    Private _roles As New AMP.RoleCollection
    Private _section As Integer = AMP.Site.Section.All

#Region " Properties "

    Public Property ImageFile() As String
        Get
            Return _imageFile
        End Get
        Set(ByVal Value As String)
            _imageFile = Value
        End Set
    End Property

    Public Property Permissions() As AMP.Site.Permission()
        Get
            Return _permission
        End Get
        Set(ByVal Value As AMP.Site.Permission())
            _permission = Value
        End Set
    End Property

    Public Property Section() As Integer
        Get
            Return _section
        End Get
        Set(ByVal Value As Integer)
            _section = Value
        End Set
    End Property

    Public Property Roles() As AMP.RoleCollection
        Get
            Return _roles
        End Get
        Set(ByVal Value As AMP.RoleCollection)
            _roles = Value
        End Set
    End Property

    Public Property JobTitle() As String
        Get
            Return _jobTitle
        End Get
        Set(ByVal Value As String)
            _jobTitle = Security.SafeString(Value, 75)
        End Set
    End Property

    Public Property Employer() As AMP.Company
        Get
            Return _employer
        End Get
        Set(ByVal Value As AMP.Company)
            _employer = Value
        End Set
    End Property

    Public Property PrivateEmail() As Boolean
        Get
            Return _privateEmail
        End Get
        Set(ByVal Value As Boolean)
            _privateEmail = Value
        End Set
    End Property

    Public Property LastLogin() As DateTime
        Get
            Return _lastLogin
        End Get
        Set(ByVal Value As DateTime)
            _lastLogin = Value
        End Set
    End Property

    Public Property ConfirmationCode() As String
        Get
            Return _confirmationCode
        End Get
        Set(ByVal Value As String)
            _confirmationCode = Security.SafeString(Value, 20)
        End Set
    End Property

    Public Property Status() As AMP.Site.Status
        Get
            Return _status
        End Get
        Set(ByVal Value As AMP.Site.Status)
            _status = Value
        End Set
    End Property

    Public Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal Value As String)
            _description = Security.SafeString(Value, 1500)
        End Set
    End Property

    Public Property RegisteredOn() As DateTime
        Get
            Return _registeredOn
        End Get
        Set(ByVal Value As DateTime)
            _registeredOn = Value
        End Set
    End Property

    Public Property WebSite() As String
        Get
            Return _webSite
        End Get
        Set(ByVal Value As String)
            _webSite = Security.SafeString(Value, 150)
        End Set
    End Property

    Public Property Password() As String
        Get
            Return _password
        End Get
        Set(ByVal Value As String)
            _password = Security.SafeString(Value, 32)
        End Set
    End Property

    Public Property Email() As String
        Get
            Return _email
        End Get
        Set(ByVal Value As String)
            _email = Security.SafeString(Value, 50)
        End Set
    End Property

    Public Property Address() As AddressCollection
        Get
            Return _address
        End Get
        Set(ByVal Value As AddressCollection)
            _address = Value
        End Set
    End Property

    Public ReadOnly Property ID() As Guid
        Get
            Return _id
        End Get
    End Property

    Public Property FirstName() As String
        Get
            Return _firstName
        End Get
        Set(ByVal Value As String)
            _firstName = Security.SafeString(Value, 100)
        End Set
    End Property

    Public Property LastName() As String
        Get
            Return _lastName
        End Get
        Set(ByVal Value As String)
            _lastName = Security.SafeString(Value, 100)
        End Set
    End Property

    Public Property NickName() As String
        Get
            Return _nickName
        End Get
        Set(ByVal Value As String)
            _nickName = Security.SafeString(Value, 100)
        End Set
    End Property

    Public ReadOnly Property FullName() As String
        Get
            Return String.Format("{0} {1}", _firstName, _lastName).Trim
        End Get
    End Property

    Public ReadOnly Property DisplayName() As String
        Get
            Return IIf(_nickName = Nothing, Me.FullName, _nickName).ToString
        End Get
    End Property

    Public ReadOnly Property DetailLink() As String
        Get
            Return String.Format("<a href=""{0}/person.aspx?id={1}"">{2}</a>", _
                Global.BasePath, Me.ID, HttpUtility.HtmlEncode(Me.DisplayName))
        End Get
    End Property

    Public ReadOnly Property EmailLink() As String
        Get
            Return String.Format("<a href=""mailto:{0}"">{0}</a>", Me.Email)
        End Get
    End Property

    Public ReadOnly Property WebSiteLink() As String
        Get
            If Me.WebSite <> Nothing Then
                Return String.Format("<a href=""http://{0}"">{0}</a>", Me.WebSite)
            End If
        End Get
    End Property

#End Region

    Public Sub New()
        _id = Guid.NewGuid
    End Sub

    '---COMMENT---------------------------------------------------------------
    '	get all permissions for this person
    '
    '	Date:		Name:	Description:
    '	12/7/04	    JEA		Creation
    '-------------------------------------------------------------------------
    Public Function EffectivePermissions() As AMP.Site.Permission()
        Dim pc As New PermissionCollection

        ' get personal permissions
        If Not Me.Permissions Is Nothing Then
            For x As Integer = 0 To Me.Permissions.Length - 1
                pc.Add(Me.Permissions(x))
            Next
        End If

        ' get role permissions
        If Not Me.Roles.Permissions Is Nothing Then
            For x As Integer = 0 To Me.Roles.Permissions.Length - 1
                pc.Add(Me.Roles.Permissions(x))
            Next
        End If

        Return pc.All
    End Function

    '---COMMENT---------------------------------------------------------------
    '	does person have given permission either directly or from role
    '
    '	Date:		Name:	Description:
    '	12/7/04	    JEA		Creation
    '-------------------------------------------------------------------------
    Public Function HasPermission(ByVal permission As AMP.Site.Permission) As Boolean
        Dim permissions As AMP.Site.Permission() = Me.EffectivePermissions
        If Not permissions Is Nothing Then
            Return (Array.BinarySearch(permissions, permission) >= 0)
        Else
            Return False
        End If
    End Function

    '---COMMENT---------------------------------------------------------------
    '	does person have text in name or other string
    '
    '	Date:		Name:	Description:
    '	12/7/04	    JEA		Creation
    '-------------------------------------------------------------------------
    Public Function HasText(ByVal text As String) As Boolean
        text = text.ToLower
        If _firstName.ToLower.IndexOf(text) <> -1 _
            OrElse _lastName.ToLower.IndexOf(text) <> -1 _
            OrElse (_nickName <> Nothing AndAlso _nickName.ToLower.IndexOf(text) <> -1) Then

            Return True
        Else
            Return False
        End If
    End Function

    Public Function CompareTo(ByVal entity As Object) As Integer Implements System.IComparable.CompareTo
        Dim p As AMP.Person = DirectCast(entity, AMP.Person)
        Dim compare As Integer
        compare = String.Compare(Me.LastName, p.LastName)
        If compare = 0 Then compare = String.Compare(Me.FirstName, p.FirstName)
        Return compare
    End Function

#Region " Serialization "

    Public Function Clone() As Object Implements System.ICloneable.Clone
        Return Serialization.Clone(Me)
    End Function

#End Region

End Class
