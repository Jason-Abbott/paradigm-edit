<Serializable()> _
Public Class PublisherCollection
    Inherits CollectionBase

#Region " Properties "

    Public Property Item(ByVal index As Integer) As AMP.Publisher
        Get
            Return DirectCast(MyBase.InnerList(index), AMP.Publisher)
        End Get
        Set(ByVal Value As AMP.Publisher)
            MyBase.InnerList(index) = Value
        End Set
    End Property

    Default Public ReadOnly Property WithID(ByVal id As Guid) As AMP.Publisher
        Get
            For Each item As AMP.Publisher In Me.InnerList
                If item.ID.Equals(id) Then
                    Return item
                    Exit For
                End If
            Next
            Return Nothing
        End Get
    End Property

    Default Public ReadOnly Property WithID(ByVal id As String) As AMP.Publisher
        Get
            Return Me.WithID(New Guid(id))
        End Get
    End Property

#End Region

    '---COMMENT---------------------------------------------------------------
    '	basic list methods
    '
    '	Date:		Name:	Description:
    '	12/26/04	JEA		Creation
    '-------------------------------------------------------------------------
    Public Function Add(ByVal entity As AMP.Publisher) As Integer
        Return MyBase.InnerList.Add(entity)
    End Function

    Public Sub Remove(ByVal entity As AMP.Publisher)
        MyBase.InnerList.Remove(entity)
    End Sub

    Public Sub Remove(ByVal id As Guid)
        Me.Remove(Me.WithID(id))
    End Sub
End Class
