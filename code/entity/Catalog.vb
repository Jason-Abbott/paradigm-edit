<Serializable()> _
Public Class Catalog
    Inherits ProductCollection

    Private _promotions As JJI.PromotionCollection

#Region " Properties "

    Public Property Promotions() As JJI.PromotionCollection
        Get
            Return _promotions
        End Get
        Set(ByVal Value As JJI.PromotionCollection)
            _promotions = Value
        End Set
    End Property

#End Region

    ' show items on special
    ' perhaps contain promotion object

End Class
