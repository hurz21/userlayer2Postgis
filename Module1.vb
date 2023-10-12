Module Module1
    Property mycgi As clsCGI4VBNET
    Private username As String
    Private modus As String
    Private outfile As String
    Private mac As String
    Private vid As String
    Property zeitStart As Date
    Property zeitend As Date
    'http://w2gis02.kreis-of.local/cgi-bin/apps/paradigmaex/layer2shpfile/userlayer2postgis.cgi?user=Feinen_J&vid=23608&modus=einzeln
    'http://w2gis02.kreis-of.local/cgi-bin/apps/paradigmaex/layer2shpfile/userlayer2postgis/userlayer2postgis.cgi?user=feij&modus=liste&outfile=Feinen_J
    Sub Main()
        Dim DbTyp As String = "sqls"

#If DEBUG Then
        'isdebugmode = True
        DbTyp = "sqls"
#End If
        mycgi = New clsCGI4VBNET("dr.j.feinen@kreis-offenbach.de")
        protokoll()
        getCgiParams()
        mac = ""

        showSteuerParams()
        l("DbTyp:: " & DbTyp)
        modTools.enc = System.Text.Encoding.GetEncoding(("iso-8859-2"))
        If Not eingabeist_ok() Then
            mycgi.SendHeader("Eingaben unvollständig")
            mycgi.Send("Eingaben unvollständig")
            l("Eingaben unvollständig")
            End
        End If
        'modTools.main2()
        Dim returnstring As String = ""
        Dim result As String = modTools.main2(username, vid, modus, returnstring, DbTyp, mac)
        mycgi.SendHeaderAJAX()
        mycgi.Send("job ok" & "#" & returnstring)

        l(result)
        l("dauer ms: " & CStr(System.DateTime.Now.Subtract(zeitStart).TotalMilliseconds))
        l("----------------- finito")
    End Sub
    Public Sub protokoll()
        With My.Application.Log.DefaultFileLogWriter
#If DEBUG Then
            .CustomLocation = "c:\" & "protokoll"
#Else
            .CustomLocation = "d:\websys\" & "protokoll"
#End If

            .BaseFileName = "userlayer2postgis_" & mycgi.GetCgiValue("user") & "_" & mycgi.GetCgiValue("modus") & "_" & mycgi.GetCgiValue("vid")
            .AutoFlush = True
            .Append = False
        End With
        zeitStart = Now
        l("protokoll now: " & zeitStart)
    End Sub
    Public Sub showSteuerParams()
        l("-----------------showCgiParams ---------------------- ")
        l(mycgi.sFormData)
        l("username: " & username)
        l("modus: " & modus)
        l("vid: " & vid)
        'l("gemcode: " & gemcode)
        'l("fs: " & fs)
        l("outfile: " & outfile)
        l("mac: " & mac)
        l("---------------- showCgiParams ende ")
    End Sub

    Private Sub getCgiParams()
        l("getCgiParams -------------------------")
        Try
#If DEBUG Then
            username = "petersdorff_l"
            'username = "feinen_j"
            vid = "36677"
            vid = "41263                                                                                               "
            modus = "einzeln"
            'modus = "sachgebiet3307"

            'username = "feij"
            'modus = "einzeln"
            'vid = "27715"

            'modus = "liste"
            'outfile = "Feinen_J"
            'username = "feinen_j"
            'mac = (mycgi.GetCgiValue("mac"))
            'rid = "26929"
            'fs = "FS0607280020000100700" 'der dateiname kann nicht über cgi geleitet werden. funzt nicht
            'gemcode = "728"
            '        rbtyp   fst = 2
#Else
            username = mycgi.GetCgiValue("user")
            vid = (mycgi.GetCgiValue("vid"))
            modus = (mycgi.GetCgiValue("modus"))
            outfile = (mycgi.GetCgiValue("outfile"))
            mac = (mycgi.GetCgiValue("mac"))
            If username = String.Empty Then username = (mycgi.GetCgiValue("outfile"))
#End If
            'If isdebugmode Then

            'Else

            'End If
        Catch ex As Exception
            l("fehler in getCgiParams: " & ex.ToString)
        End Try

    End Sub

    Public Function eingabeist_ok() As Boolean
        Return True
        l("eingabeist_ok-------------------")
        Try
            If modus = "einzeln" And CInt(vid) < 1 Then
                l("Fehler :vid) < 1  ")
                Return False
            End If
            'If String.IsNullOrEmpty(username) And String.IsNullOrEmpty(outfile) Then
            '    l("Fehler :username " & username)
            '    Return False
            'End If

            Return True
        Catch ex As Exception
            l("Fehler ineingabeist_ok : " & ex.ToString)
            Return False
        End Try
    End Function
End Module
