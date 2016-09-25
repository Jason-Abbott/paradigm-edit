Imports System.Threading
Imports System.Web.HttpContext
Imports AMP.Data.Serialization
Imports System.Runtime.Serialization
Imports System.Configuration.ConfigurationSettings

<Serializable()> _
Public Class Site
    Implements ICloneable, IDeserializationCallback

    ' don't serialize fields that could be in an odd state
    <NonSerialized()> Private _saveDelay As TimeSpan
    <NonSerialized()> Private _bufferSave As Boolean = True
    <NonSerialized()> Private _saveState As SaveState = SaveState.None

    Private _persons As New AMP.PersonCollection
    Private _roles As New AMP.RoleCollection
    Private _publishers As New AMP.PublisherCollection
    Private _categories As New AMP.CategoryCollection

#Region " Enumerations "

    <Flags()> _
    Private Enum SaveState
        None = &H0
        Sleeping = &H1
        Writing = &H2
    End Enum

    <Flags()> _
    Public Enum Entity
        Asset = &H1
        Person = &H2
        Tour = &H4
        Contest = &H8
        Product = &H10
        Software = &H20
        Publisher = &H40
    End Enum

    <Flags()> _
    Public Enum Section
        All = Adobe Or Sony Or Ulead
        Sony = Vegas Or DvdArchitect
        Ulead = Cool3D Or MediaStudio
        None = &H0
        Vegas = &H1
        DvdArchitect = &H2
        Acid = &H4
        SoundForge = &H8
        Cool3D = &H10
        MediaStudio = &H20
        Adobe = &H100
        HDV = &H200
    End Enum

    Public Enum Status
        Pending = 1
        Approved = 2
        Rejected = 3
        Disabled = 4
    End Enum

    Public Enum Role
        Anonymous
        UnverifiedGuest
        VerifiedGuest
        Editor
        Manager
        Administrator
    End Enum

    Public Enum Activity
        ' login
        Login
        Register
        FailedLogin
        EmailedPassword
        AutoLoginFromCookie
        ResentValidationCode
        ValidateRegistration
        EnteredBadValidationCode
        UnauthorizedAccessAttempt
        TriedRegisteringWithExistingEmail
        ' asset
        RankAsset
        EditAsset
        DenySubmittedAsset
        ApproveSubmittedAsset
        ' file
        FileUpload
        DeleteFile
        FileDownload
        AttemptedLargeFileUpload
        ' link
        ViewLink
        ' contest
        Vote
        EnterAsset
        SaveContest
        CreateContest
        ' database
        BackupDatabase
        CompactDatabase
        ' accounts
        EditUserAccount
        CreateUserAccount
        DisableUserAccount
    End Enum

    Public Enum Permission
        ' user
        AddUser
        EditAnyUser
        DeleteUser
        EditMyself
        ViewUserDetails
        ' product
        AddProduct
        EditProduct
        DeleteProduct
        PurchaseProduct
        ' assets
        AddAsset
        EditMyAsset
        EditAnyAsset
        DownloadAsset
        LinkToAsset
        ApproveAsset
        RejectAsset
        ChangeAssetSection
        ' contests
        AddContest
        EditContest
        EnterContest
        VoteInContest
        ' menu
        AddMenuItem
        EditMenuItem
        DeleteMenuItem
        ' feature
        AddFeaturedItem
        EditFeaturedItem
        DeleteFeaturedItem
        ' quote
        AddQuote
        EditQuote
        DeleteQuote
        ' general content
        AddContent
        EditContent
        DeleteContent
    End Enum

#End Region

#Region " Properties "

    Public Property Categories() As AMP.CategoryCollection
        Get
            _categories.Sort()
            Return _categories
        End Get
        Set(ByVal Value As AMP.CategoryCollection)
            _categories = Value
        End Set
    End Property

    Public Property Publishers() As AMP.PublisherCollection
        Get
            Return _publishers
        End Get
        Set(ByVal Value As AMP.PublisherCollection)
            _publishers = Value
        End Set
    End Property

    Public Property UseSaveBuffer() As Boolean
        Get
            Return _bufferSave
        End Get
        Set(ByVal Value As Boolean)
            _bufferSave = Value
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

    Public Property Persons() As AMP.PersonCollection
        Get
            Return _persons
        End Get
        Set(ByVal Value As AMP.PersonCollection)
            _persons = Value
        End Set
    End Property

#End Region

    Public Sub New()
        Me.Initialize()
    End Sub

    Private Sub Initialize()
        _saveDelay = TimeSpan.FromMinutes(CDbl(AppSettings("SaveDelayMinutes")))
        _bufferSave = True
    End Sub

#Region " Serialization "

    '---COMMENT---------------------------------------------------------------
    '	serialize collections and persist to disk
    '
    '	Date:		Name:	Description:
    '	12/18/04	JEA		Creation
    '-------------------------------------------------------------------------
    Public Sub Save()
        BugOut("WebSite.Save() called")
        BugTab()

        If _saveState = SaveState.None Then
            BugOut("Queueing new thread to save")
            ThreadPool.QueueUserWorkItem(AddressOf Me.Flush)
        ElseIf Not (_bufferSave OrElse _saveState = SaveState.Writing) Then
            ' if immediate save requested and existing thread not already writing
            BugOut("Queuing another thread to save immediately")
            ThreadPool.QueueUserWorkItem(AddressOf Me.Flush)
        Else
            BugOut("Existing thread will satisfy save request")
        End If

        BugUntab()
    End Sub

    '---COMMENT---------------------------------------------------------------
    '	flush save calls to disk
    '
    '	Date:		Name:	Description:
    '	12/18/04	JEA		Creation
    '-------------------------------------------------------------------------
    Private Sub Flush(ByVal state As Object)
        If _bufferSave Then
            BugOut("Putting WebSite.Flush() thread to sleep")
            _saveState = SaveState.Sleeping
            Thread.Sleep(_saveDelay)
            BugOut("WebSite.Flush() is awake")
        Else
            ' always default back to buffered saves
            BugOut("Buffer disabled so flushing immediately after call to Flush()")
            _bufferSave = False
        End If

        Dim file As New AMP.Data.File
        _saveState = SaveState.Writing
        file.Save(Global.DataFilePath, Me)
        _saveState = SaveState.None
    End Sub

    Public Function Clone() As Object Implements System.ICloneable.Clone
        Return AMP.Data.Serialization.Clone(Me)
    End Function

    '---COMMENT---------------------------------------------------------------
    '	properly initialize certain values on deserialization
    '
    '	Date:		Name:	Description:
    '	12/18/04	JEA		Creation
    '-------------------------------------------------------------------------
    Public Sub OnDeserialization(ByVal sender As Object) Implements IDeserializationCallback.OnDeserialization
        BugOut("Executing AMP.WebSite.OnDeserialization")
        Me.Initialize()
    End Sub

#End Region

End Class
