<Serializable()> _
Public Class File
    Private _name As String
    Private _type As File.Types
    Private _path As String
    Private _size As Single
    Private _downloads As Integer
    Private _requiredUrl As String
    Private _renderedUrl As String


#Region " Enumerations "

    <Flags()> _
    Public Enum Types
        Image = &H1
        Video = &H2
        Acrobat = &H4
        Vegas = &H8
        DVDArchitect = &H10
        Cool3D = &H20
        Compressed = &H40
        Excel = &H80
        Script = &H100
        Text = &H200
        MediaStudioPro = &H400
        Flash = &H800
        Preset = &H1000
    End Enum

#End Region

#Region " Properties "

    Public Property Type() As AMP.File.Types
        Get
            Return _type
        End Get
        Set(ByVal Value As AMP.File.Types)
            _type = Value
        End Set
    End Property

    Public Property Path() As String
        Get
            Return _path
        End Get
        Set(ByVal Value As String)
            _path = Security.SafeString(Value, 100)
        End Set
    End Property

    Public Property Size() As Single
        Get
            Return _size
        End Get
        Set(ByVal Value As Single)
            _size = Value
        End Set
    End Property

    Public Property Downloads() As Integer
        Get
            Return _downloads
        End Get
        Set(ByVal Value As Integer)
            _downloads = Value
        End Set
    End Property

    Public Property RequiredUrl() As String
        Get
            Return _requiredUrl
        End Get
        Set(ByVal Value As String)
            _requiredUrl = Security.SafeString(Value, 150)
        End Set
    End Property

    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal Value As String)
            _name = Security.SafeString(Value, 100)
        End Set
    End Property

    Public ReadOnly Property Extension() As String
        Get
            Return Me.Name.Substring(Me.Name.LastIndexOf(".") + 1).ToLower
        End Get
    End Property

#End Region

    '---COMMENT---------------------------------------------------------------
    '	Infer file type from extension
    '
    '	Date:		Name:	Description:
    '	12/21/04    JEA		Creation
    '-------------------------------------------------------------------------
    Private Sub InferType(ByVal name As String)
        Dim extension As String = name.Substring(name.LastIndexOf(".") + 1).ToLower
        Select Case extension
            Case "pdf"
                Me.Type = File.Types.Acrobat
            Case "zip"
                Me.Type = File.Types.Compressed
            Case "c3d"
                Me.Type = File.Types.Cool3D
            Case "dar"
                Me.Type = File.Types.DVDArchitect
            Case "xls"
                Me.Type = File.Types.Excel
            Case "swf"
                Me.Type = File.Types.Flash
            Case "jpg", "jpeg", "png", "gif", "bmp"
                Me.Type = File.Types.Image
            Case "msp"
                Me.Type = File.Types.MediaStudioPro
            Case "js"
                Me.Type = File.Types.Script
            Case "txt"
                Me.Type = File.Types.Text
            Case "veg"
                Me.Type = File.Types.Vegas
            Case "avi", "wmv", "m2t", "mpg", "mpeg", "qt", "mov", "mvp"
                Me.Type = File.Types.Video
            Case "sfpreset"
                Me.Type = File.Types.Preset
        End Select
    End Sub
End Class
